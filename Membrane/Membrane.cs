using System;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Material;
using MathNet.Numerics.Data.Text;

namespace RCMembrane
{
    public abstract class Membrane
    {
        // Properties
        public Concrete.Biaxial               Concrete               { get; set; }
        public Reinforcement.Biaxial          Reinforcement          { get; }
        public (bool S, string Message)       Stop                   { get; set; }
        public Vector<double>                 Strains                { get; set; }

        // Constructor
        public Membrane(Concrete.Biaxial concrete, Reinforcement.Biaxial reinforcement, double panelWidth)
        {
            // Get reinforcement
            var diams = reinforcement.BarDiameter;
            var spcs  = reinforcement.BarSpacing;
            var steel = reinforcement.Steel;

            // Initiate new materials
            Reinforcement = new Reinforcement.Biaxial(diams, spcs, steel, panelWidth);

            // Set initial strains
            Strains = Vector<double>.Build.Dense(3);
        }

        // Get steel parameters
        public double fyx  => Reinforcement.Steel.X.YieldStress;
        public double Esxi => Reinforcement.Steel.X.ElasticModule;
        public double fyy  => Reinforcement.Steel.Y.YieldStress;
        public double Esyi => Reinforcement.Steel.Y.ElasticModule;

        // Get reinforcement
        public double phiX => Reinforcement.BarDiameter.X;
        public double phiY => Reinforcement.BarDiameter.Y;
        public double psx  => Reinforcement.Ratio.X;
        public double psy  => Reinforcement.Ratio.Y;

        // Calculate crack spacings
        public double smx => phiX / (5.4 * psx);
        public double smy => phiY / (5.4 * psy);

        // Get current Stiffness
        public Matrix<double> Stiffness => Concrete.Stiffness + Reinforcement.Stiffness;

        // Get current stresses
        public Vector<double> Stresses  => Concrete.Stresses + Reinforcement.Stresses;

        public abstract void Calculate(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0);

        // Solver for known stresses
        public virtual void Solver(Vector<double> stresses, int numLoadSteps = 100, int maxIterations = 1000, double tolerance = 1E-3)
        {
            // Get initial stresses
            var f0 = (double)1 / numLoadSteps * stresses;

            // Calculate initial stiffness
            var D = InitialStiffness();

            // Calculate e0
            var ei = D.Solve(f0);

            // Initiate matrices
            var epsMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var e1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);
            var sigMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var sig1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);

            //int[] itUpdate = new int[200];
            //for (int i = 0; i < itUpdate.Length; i++)
            //	itUpdate[i] = 50 * i;
            Stop = (false, string.Empty);

            // Initiate load steps
            for (int ls = 1; ls <= numLoadSteps; ls++)
            {
                // Calculate stresses
                var fi = ls * f0;

                for (int it = 1; it <= maxIterations; it++)
                {
                    // Do analysis
                    Calculate(ei, ls, it);

                    // Calculate residual
                    var fr = ResidualStresses(fi);

                    // Calculate convergence
                    double conv = Convergence(fr, fi);

                    if (ConvergenceReached(conv, tolerance, it))
                    {
                        Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

                        // Update stiffness
                        D = Stiffness;

                        // Get stresses
                        var (fc1, fc2) = Concrete.PrincipalStresses;
                        var (ec1, ec2) = Concrete.PrincipalStrains;

                        // Set results
                        epsMatrix.SetRow(ls - 1, Strains);
                        sigMatrix.SetRow(ls - 1, Stresses);
                        e1Matrix.SetRow(ls - 1, new[] { ec1, ec2 });
                        sig1Matrix.SetRow(ls - 1, new[] { fc1, fc2 });

                        break;
                    }

                    // Increment strains
                    ei += StrainIncrement(D, fr);

                    if (it == maxIterations)
                    {
                        Stop = (true, "CONVERGENCE NOT REACHED");
                        Console.WriteLine("LS = {0}, {1}", ls, Stop.Message);
                    }
                }

                if (Stop.S)
                    break;
            }

            // Result matrices
            var sigXeps = Matrix<double>.Build.DenseOfColumnVectors(
                epsMatrix.Column(2), sigMatrix.Column(2), Vector<double>.Build.Dense(numLoadSteps),
                e1Matrix.Column(0), sig1Matrix.Column(0), Vector<double>.Build.Dense(numLoadSteps),
                e1Matrix.Column(1), sig1Matrix.Column(1));

            string[]
                res = { "Gamma", "Tau", "", "ec1", "fc1", "", "ec2", "fc2" };

            // Save results
            DelimitedWriter.Write("D:/sig x eps.csv", sigXeps, ";", res, null, null, 0);
        }

        // Calculate convergence
        private double Convergence(Vector<double> residualStress, Vector<double> appliedStress)
        {
	        double
		        num = 0,
		        den = 1;

	        for (int i = 0; i < residualStress.Count; i++)
	        {
		        num += residualStress[i] * residualStress[i];
		        den += appliedStress[i] * appliedStress[i];
	        }

	        return
		        num / den;
        }

        // Verify if convergence is reached
        private bool ConvergenceReached(double convergence, double tolerance, int iteration) => convergence <= tolerance && iteration > 9;
		
		// Calculate residual stresses
		public virtual Vector<double> ResidualStresses(Vector<double> appliedStresses)
		{
			return
				appliedStresses - Stresses;
		}

		// Calculate strain increment
		private Vector<double> StrainIncrement(Matrix<double> stiffness, Vector<double> residualStresses)
		{
			return
				stiffness.Solve(residualStresses);
		}

        // Calculate initial stiffness
        public Matrix<double> InitialStiffness() => Concrete.InitialStiffness() + Reinforcement.InitialStiffness();

        // Crack check procedure
        public void CrackCheck(double? theta2 = null)
        {
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

	        if (!theta2.HasValue)
		        theta2 = Concrete.PrincipalAngles.theta2;

            // Get the values
            double ec1 = Concrete.PrincipalStrains.ec1;
            (double fsx, double fsy) = Reinforcement.SteelStresses;
            double f1a = Concrete.PrincipalStresses.fc1;

            // Calculate thetaC sine and cosine
            var (cosTheta, sinTheta) = DirectionCosines(theta2.Value);
            double tanTheta = Tangent(theta2.Value);

            // Reinforcement capacity reserve
            double
                f1cx = psx * (fyx - fsx),
                f1cy = psy * (fyy - fsy);

            // Maximum possible shear on crack interface
            double vcimaxA = MaximumShearOnCrack(theta2.Value, ec1);

            // Maximum possible shear for biaxial yielding
            double vcimaxB = Math.Abs(f1cx - f1cy) / (tanTheta + 1 / tanTheta);

            // Maximum shear on crack
            double vcimax = Math.Min(vcimaxA, vcimaxB);

            // Biaxial yielding condition
            double f1b = f1cx * sinTheta * sinTheta + f1cy * cosTheta * cosTheta;

            // Maximum tensile stress for equilibrium in X and Y
            double
                f1c = f1cx + vcimax / tanTheta,
                f1d = f1cy + vcimax * tanTheta;

            // Calculate the minimum tensile stress
            var f1List = new[] { f1a, f1b, f1c, f1d };
            var fc1 = f1List.Min();

            // Set to concrete
            if (fc1 < f1a)
                Concrete.SetTensileStress(fc1);
        }

        // Calculate maximum shear on crack
        public double MaximumShearOnCrack(double theta2, double ec1)
        {
	        // Calculate thetaC sine and cosine
	        var (cosTheta, sinTheta) = DirectionCosines(theta2);

	        // Average crack spacing and opening
	        double
		        smTheta = 1 / (sinTheta / smx + cosTheta / smy),
		        w = smTheta * ec1;

	        // Maximum possible shear on crack interface
	        return
		        MaximumShearOnCrack(w);
        }

        // Calculate maximum shear on crack
        public double MaximumShearOnCrack(double w)
        {
	        double
		        fc    = Concrete.fc,
		        phiAg = Concrete.AggregateDiameter;

	        // Maximum possible shear on crack interface
	        return
		        0.18 * Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));
        }

        // Calculate reference length
        public double ReferenceLength(double? thetaC1 = null)
        {
	        if (!thetaC1.HasValue)
		        thetaC1 = Concrete.PrincipalAngles.theta1;

	        var (cosThetaC, sinThetaC) = DirectionCosines(thetaC1.Value);

	        return
		        0.5 / (Math.Abs(sinThetaC) / smx + Math.Abs(cosThetaC) / smy);
        }

        // Verify if a number is zero
        public bool NotZero(double num) => num != 0;

        // Get the direction cosines of an angle
        public (double cos, double sin) DirectionCosines(double angle)
        {
	        double
		        cos = Trig.Cos(angle).CoerceZero(1E-6),
		        sin = Trig.Sin(angle).CoerceZero(1E-6);

	        return (cos, sin);
        }

        public static double Tangent(double angle)
        {
	        double tan;

	        // Calculate the tangent, return 0 if 90 or 270 degrees
	        if (angle == Constants.PiOver2 || angle == Constants.Pi3Over2)
		        tan = 1.633e16;

	        else
		        tan = Trig.Cos(angle).CoerceZero(1E-6);

	        return tan;
        }
    }
}