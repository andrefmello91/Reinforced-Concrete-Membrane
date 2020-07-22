using System;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Material;
using MathNet.Numerics.Data.Text;
using Parameters    = Material.Concrete.Parameters;
using Behavior      = Material.Concrete.Behavior;
using Concrete      = Material.Concrete.Biaxial;
using Reinforcement = Material.Reinforcement.Biaxial;

namespace RCMembrane
{
	/// <summary>
    /// Membrane element base class.
    /// </summary>
    public abstract class Membrane
    {
        // Properties
        public Concrete                 Concrete               { get; set; }
        public Reinforcement            Reinforcement          { get; }
        public (bool S, string Message) Stop                   { get; set; }
        public Vector<double>           Strains                { get; set; }

		/// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concrete">Biaxial concrete object.</param>
        /// <param name="reinforcement">Biaxial reinforcement object.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        public Membrane(Concrete concrete, Reinforcement reinforcement, double sectionWidth)
        {
            // Get reinforcement
            var diams = reinforcement.BarDiameter;
            var spcs  = reinforcement.BarSpacing;
            var steel = reinforcement.Steel;

            // Initiate new materials
            Reinforcement = new Reinforcement(diams, spcs, steel, sectionWidth);

            // Set initial strains
            Strains = Vector<double>.Build.Dense(3);
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete parameters object.</param>
        /// <param name="concreteBehavior">Concrete behavior object.</param>
        /// <param name="reinforcement">Biaxial reinforcement object.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        public Membrane(Parameters concreteParameters, Behavior concreteBehavior, Reinforcement reinforcement, double sectionWidth)
        {
            // Get reinforcement
            var diams = reinforcement.BarDiameter;
            var spcs  = reinforcement.BarSpacing;
            var steel = reinforcement.Steel;

            // Initiate new materials
            Reinforcement = new Reinforcement(diams, spcs, steel, sectionWidth);

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

        /// <summary>
        /// Get current membrane stiffness.
        /// </summary>
        public Matrix<double> Stiffness => Concrete.Stiffness + Reinforcement.Stiffness;

        /// <summary>
        /// Get current stresses, in MPa.
        /// </summary>
        public Vector<double> Stresses  => Concrete.Stresses + Reinforcement.Stresses;

		/// <summary>
        /// Calculate stresses and the membrane stiffness, given strains.
        /// </summary>
        /// <param name="appliedStrains">Current strains.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="iteration">Current iteration.</param>
        public abstract void Calculate(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0);

        /// <summary>
        /// Membrane solver for known stresses.
        /// </summary>
        /// <param name="stresses">Applied stresses, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 1000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
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

        /// <summary>
        /// Calculate stress convergence.
        /// </summary>
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        private double Convergence(Vector<double> residualStresses, Vector<double> appliedStresses)
        {
	        double
		        num = 0,
		        den = 1;

	        for (int i = 0; i < residualStresses.Count; i++)
	        {
		        num += residualStresses[i] * residualStresses[i];
		        den += appliedStresses[i] * appliedStresses[i];
	        }

	        return
		        num / den;
        }

        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="convergence">Calculated convergence.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 10).</param>
        private bool ConvergenceReached(double convergence, double tolerance, int iteration, int minIterations = 10) => convergence <= tolerance && iteration >= minIterations;

        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 10).</param>
        private bool ConvergenceReached(Vector<double> residualStresses, Vector<double> appliedStresses, double tolerance,
	        int iteration, int minIterations = 10) => ConvergenceReached(Convergence(residualStresses, appliedStresses), tolerance, iteration, minIterations);

        /// <summary>
        /// Calculate residual stresses, in MPa.
        /// </summary>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        /// <returns></returns>
        public virtual Vector<double> ResidualStresses(Vector<double> appliedStresses)
		{
			return
				appliedStresses - Stresses;
		}

        /// <summary>
        /// Calculate the strain increment for next iteration.
        /// </summary>
        /// <param name="stiffness">Current stiffness.</param>
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        private Vector<double> StrainIncrement(Matrix<double> stiffness, Vector<double> residualStresses)
		{
			return
				stiffness.Solve(residualStresses);
		}

        /// <summary>
        /// Calculate initial membrane stiffness.
        /// </summary>
        public Matrix<double> InitialStiffness() => Concrete.InitialStiffness() + Reinforcement.InitialStiffness();

		/// <summary>
        /// Limit tensile principal stress by crack check procedure, by Bentz (2000).
        /// </summary>
        /// <param name="theta2">Principal compressive strain angle, in radians.</param>
        public void CrackCheck(double? theta2 = null)
        {
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

            // Get the values
            double theta = theta2 ?? Concrete.PrincipalAngles.theta2;
            double ec1 = Concrete.PrincipalStrains.ec1;
            (double fsx, double fsy) = Reinforcement.SteelStresses;
            double f1a = Concrete.PrincipalStresses.fc1;

            // Calculate thetaC sine and cosine
            var (cosTheta, sinTheta) = DirectionCosines(theta);
            double tanTheta = Tangent(theta);

            // Reinforcement capacity reserve
            double
                f1cx = psx * (fyx - fsx),
                f1cy = psy * (fyy - fsy);

            // Maximum possible shear on crack interface
            double vcimaxA = MaximumShearOnCrack(theta, ec1);

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

        /// <summary>
        /// Calculate maximum shear stress on crack, in MPa.
        /// </summary>
        /// <param name="theta2">Principal compressive strain angle, in radians.</param>
        /// <param name="ec1">Principal tensile strain.</param>
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

        /// <summary>
        /// Calculate maximum shear stress on crack, in MPa.
        /// </summary>
        /// <param name="w">Average crack opening, in mm.</param>
        /// <returns></returns>
        public double MaximumShearOnCrack(double w)
        {
	        double
		        fc    = Concrete.fc,
		        phiAg = Concrete.AggregateDiameter;

	        // Maximum possible shear on crack interface
	        return
		        0.18 * Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));
        }

        /// <summary>
        /// Calculate reference length, in mm.
        /// </summary>
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        public double ReferenceLength(double? thetaC1 = null)
        {
			double theta = thetaC1 ?? Concrete.PrincipalAngles.theta1;

	        var (cosTheta, sinTheta) = DirectionCosines(theta);

	        return
		        0.5 / (Math.Abs(sinTheta) / smx + Math.Abs(cosTheta) / smy);
        }

        /// <summary>
        /// Verify if a number is zero (true if is not zero).
        /// </summary>
        /// <param name="number">The number.</param>
        public bool NotZero(double number) => number != 0;

        /// <summary>
        /// Calculate the direction cosines (cos, sin) of an angle.
        /// </summary>
        /// <param name="angle">Angle, in radians.</param>
        /// <returns></returns>
        public (double cos, double sin) DirectionCosines(double angle)
        {
	        double
		        cos = Trig.Cos(angle).CoerceZero(1E-6),
		        sin = Trig.Sin(angle).CoerceZero(1E-6);

	        return (cos, sin);
        }

        /// <summary>
        /// Calculate tangent of an angle.
        /// </summary>
        /// <param name="angle">Angle, in radians.</param>
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