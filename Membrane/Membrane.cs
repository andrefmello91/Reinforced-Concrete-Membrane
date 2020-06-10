using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using Material;
using MathNet.Numerics.Data.Text;

namespace Membrane
{
    public abstract partial class Membrane
    {
        // Properties
        public Concrete.Biaxial               Concrete               { get; set; }
        public Reinforcement.Biaxial          Reinforcement          { get; }
        public (bool S, string Message)       Stop                   { get; set; }
        public Vector<double>                 Strains                { get; set; }
        public (double theta1, double theta2) PrincipalAngles        { get; set; }
        public Matrix<double>                 ConcreteStiffness      { get; set; }
        public Matrix<double>                 ReinforcementStiffness { get; set; }
        public Vector<double>                 ConcreteStresses       { get; set; }
        public Vector<double>                 ReinforcementStresses  { get; set; }

		// Abstract properties
		public abstract Vector<double> ConcreteStrains { get; }

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

        // Solver settings
        private int    NumLoadSteps  = 100;
        private int    MaxIterations = 1000;
        private double Tolerance     = 1E-3;

        // Get steel parameters
        private double fyx  => Reinforcement.Steel.X.YieldStress;
        private double Esxi => Reinforcement.Steel.X.ElasticModule;
        private double fyy  => Reinforcement.Steel.Y.YieldStress;
        private double Esyi => Reinforcement.Steel.Y.ElasticModule;

        // Get reinforcement
        private double phiX => Reinforcement.BarDiameter.X;
        private double phiY => Reinforcement.BarDiameter.Y;
        private double psx  => Reinforcement.Ratio.X;
        private double psy  => Reinforcement.Ratio.Y;

        // Calculate crack spacings
        private double smx => phiX / (5.4 * psx);
        private double smy => phiY / (5.4 * psy);

        // Get current Stiffness
        public Matrix<double> Stiffness => ConcreteStiffness + ReinforcementStiffness;

        // Get current stresses
        public Vector<double> Stresses  => ConcreteStresses + ReinforcementStresses;

        public abstract void Analysis(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0);

        // Solver for known stresses
        public virtual void Solver(Vector<double> stresses)
        {
            // Get initial stresses
            var sig0 = (double)1 / NumLoadSteps * stresses;

            // Calculate initial stiffness
            var D = InitialStiffness();

            // Calculate e0
            var ei = D.Solve(sig0);

            // Initiate matrices
            var epsCMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 3);
            var epsMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 3);
            var e1Matrix = Matrix<double>.Build.Dense(NumLoadSteps, 2);
            var sigMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 3);
            var sig1Matrix = Matrix<double>.Build.Dense(NumLoadSteps, 2);
            var sigCMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 3);
            var sigSMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 3);
            var thetaMatrix = Matrix<double>.Build.Dense(NumLoadSteps, 1);

            //int[] itUpdate = new int[200];
            //for (int i = 0; i < itUpdate.Length; i++)
            //	itUpdate[i] = 50 * i;
            Stop = (false, string.Empty);

            // Initiate load steps
            for (int ls = 1; ls <= NumLoadSteps; ls++)
            {
                // Calculate stresses
                var sig = ls * sig0;

                for (int it = 1; it <= MaxIterations; it++)
                {
                    // Do analysis
                    Analysis(ei, ls, it);

                    // Calculate residual
                    var sigR = sig - Stresses;

                    // Calculate convergence
                    double conv = Convergence(sigR, sig);

                    if (ConvergenceReached(conv, it))
                    {
                        Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

                        // Update stiffness
                        D = Stiffness;

                        // Get stresses
                        var (fc1, fc2) = Concrete.PrincipalStresses;
                        var (ec1, ec2) = Concrete.PrincipalStrains;

                        // Set results
                        epsMatrix.SetRow(ls - 1, Strains);
                        epsCMatrix.SetRow(ls - 1, ConcreteStrains);
                        sigMatrix.SetRow(ls - 1, Stresses);
                        sigCMatrix.SetRow(ls - 1, ConcreteStresses);
                        sigSMatrix.SetRow(ls - 1, ReinforcementStresses);
                        e1Matrix.SetRow(ls - 1, new[] { ec1, ec2 });
                        sig1Matrix.SetRow(ls - 1, new[] { fc1, fc2 });
                        thetaMatrix[ls - 1, 0] = PrincipalAngles.theta2;

                        break;
                    }

                    // Increment strains
                    ei += D.Solve(sigR);

                    if (it == MaxIterations)
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
                epsMatrix.Column(2), sigMatrix.Column(2), Vector<double>.Build.Dense(NumLoadSteps),
                e1Matrix.Column(0), sig1Matrix.Column(0), Vector<double>.Build.Dense(NumLoadSteps),
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
        private bool ConvergenceReached(double convergence, int iteration) => convergence <= Tolerance && iteration > 9;
		
        // Calculate tensile strain angle
        public (double theta1, double theta2) StrainAngles(Vector<double> strains, (double ec1, double ec2) principalStrains) => Relations.Strain.PrincipalAngles(strains, principalStrains);

        // Calculate principal strains
        public (double ec1, double ec2) PrincipalStrains(Vector<double> strains) => Relations.Strain.PrincipalStrains(strains);

        // Calculate initial stiffness
        public Matrix<double> InitialConcreteStiffness()
        {
	        // Concrete matrix
	        double Ec = Concrete.Ec;
	        var Dc1 = Matrix<double>.Build.Dense(3, 3);
	        Dc1[0, 0] = Ec;
	        Dc1[1, 1] = Ec;
	        Dc1[2, 2] = 0.5 * Ec;

	        // Get transformation matrix
	        var T = Transformation_Matrix(Constants.PiOver4);

	        // Calculate Dc
	        return
		        T.Transpose() * Dc1 * T;
        }

        // Initial reinforcement stiffness
        public Matrix<double> InitialReinforcementStiffness()
        {
	        // Steel matrix
	        var Ds = Matrix<double>.Build.Dense(3, 3);
	        Ds[0, 0] = psx * Esxi;
	        Ds[1, 1] = psy * Esyi;

	        return Ds;
        }

        // Calculate initial stiffness
        public Matrix<double> InitialStiffness() => InitialConcreteStiffness() + InitialReinforcementStiffness();

        // Calculate steel stiffness matrix
        public Matrix<double> Reinforcement_Stiffness((double Esx, double Esy)? steelSecantModule = null)
        {
	        double Esx, Esy;

	        if (steelSecantModule.HasValue)
		        (Esx, Esy) = steelSecantModule.Value;
	        else
		        (Esx, Esy) = Reinforcement.SecantModule;

	        // Steel matrix
	        var Ds = Matrix<double>.Build.Dense(3, 3);
	        Ds[0, 0] = psx * Esx;
	        Ds[1, 1] = psy * Esy;

	        return Ds;
        }

        // Calculate concrete stiffness matrix
        public Matrix<double> Concrete_Stiffness(double? thetaC1 = null, (double Ec1, double Ec2)? concreteSecantModule = null)
        {
	        double Ec1, Ec2;

	        if (concreteSecantModule.HasValue)
		        (Ec1, Ec2) = concreteSecantModule.Value;
	        else
		        (Ec1, Ec2) = Concrete.SecantModule;

	        double Gc = Ec1 * Ec2 / (Ec1 + Ec2);

	        // Concrete matrix
	        var Dc1 = Matrix<double>.Build.Dense(3, 3);
	        Dc1[0, 0] = Ec1;
	        Dc1[1, 1] = Ec2;
	        Dc1[2, 2] = Gc;

	        // Get transformation matrix
	        var T = Transformation_Matrix(thetaC1);

	        // Calculate Dc
	        return
		        T.Transpose() * Dc1 * T;
        }

        // Calculate stresses/strains transformation matrix
        // This matrix transforms from x-y to 1-2 coordinates
        public Matrix<double> Transformation_Matrix(double? theta1 = null)
        {
	        if (!theta1.HasValue)
		        theta1 = PrincipalAngles.theta1;

	        return
		        Relations.Strain.TransformationMatrix(theta1.Value);
        }

        // Calculate concrete stresses
        public Vector<double> Concrete_Stresses(Matrix<double> concreteStiffness = null, Vector<double> concreteStrains = null)
        {
	        if (concreteStiffness == null)
		        concreteStiffness = ConcreteStiffness;

	        if (concreteStrains == null)
		        concreteStrains = ConcreteStrains;

	        return
		        concreteStiffness * concreteStrains;
        }

        // Get reinforcement stresses as a vector multiplied by reinforcement ratio
        public Vector<double> Reinforcement_Stresses()
        {
            var (fsx, fsy) = Reinforcement.Stresses;

            return
                CreateVector.DenseOfArray(new[] { psx * fsx, psy * fsy, 0 });
        }

        // Calculate slopes related to reinforcement
        private (double X, double Y) ReinforcementAngles(double theta1) => Reinforcement.Angles(theta1);

        // Crack check procedure
        public double CrackCheck(double? theta2 = null)
        {
	        if (!theta2.HasValue)
		        theta2 = PrincipalAngles.theta2;

            // Get the values
            double ec1 = Concrete.PrincipalStrains.ec1;
            (double fsx, double fsy) = Reinforcement.Stresses;
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

            // Calculate critical stresses on crack
            return fc1;
        }

        // Calculate maximum shear on crack
        private double MaximumShearOnCrack(double theta2, double ec1)
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
        private double MaximumShearOnCrack(double w)
        {
	        double
		        fc    = Concrete.fc,
		        phiAg = Concrete.AggregateDiameter;

	        // Maximum possible shear on crack interface
	        return
		        0.18 * Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));
        }

        // Calculate reference length
        private double ReferenceLength(double? thetaC1 = null)
        {
	        if (!thetaC1.HasValue)
		        thetaC1 = PrincipalAngles.theta1;

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