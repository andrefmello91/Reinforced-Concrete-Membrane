using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
using Material;

namespace Membrane
{
    public abstract class Membrane
    {
        // Properties
        public Concrete Concrete { get; }
        public Reinforcement.Panel Reinforcement { get; }
        public (bool S, string Message) Stop { get; set; }
        public int LSCrack { get; set; }
        public (int X, int Y) LSYield { get; set; }
        public int LSPeak { get; set; }
        public Vector<double> Strains { get; set; }
        public (double theta1, double theta2) PrincipalAngles { get; set; }
        public Matrix<double> ConcreteStiffness { get; set; }
        public Matrix<double> ReinforcementStiffness { get; set; }
        public Matrix<double> TransformationMatrix { get; set; }
		public Vector<double> ConcreteStresses { get; set; }
		public Vector<double> ReinforcementStresses { get; set; }
        private int LoadStep { get; set; }
        public int Iteration { get; set; }

        // Constructor
        public Membrane(Concrete concrete, Reinforcement.Panel reinforcement)
        { 
            // Get materials
            Concrete = concrete;
            Reinforcement = reinforcement;

			// Set initial strains
			Strains = Vector<double>.Build.Dense(3);
        }

        // Get steel parameters
        private double fyx => Reinforcement.Steel.X.YieldStress;
        private double Esxi => Reinforcement.Steel.X.ElasticModule;
        private double fyy => Reinforcement.Steel.Y.YieldStress;
        private double Esyi => Reinforcement.Steel.Y.ElasticModule;

        // Get reinforcement
        private double phiX => Reinforcement.BarDiameter.X;
        private double phiY => Reinforcement.BarDiameter.Y;
        private double psx  => Reinforcement.Ratio.X;
        private double psy  => Reinforcement.Ratio.Y;

        // Calculate crack spacings
        private double smx => phiX / (5.4 * psx);
        private double smy => phiY / (5.4 * psy);

        public abstract void Analysis(Vector<double> appliedStresses, int loadStep = 0);
        public abstract void Analysis2(Vector<double> appliedStrains, int loadStep = 0);

        // Calculate tensile strain angle
        public (double theta1, double theta2) StrainAngles(Vector<double> strains, (double ec1, double ec2) principalStrains)
        {
	        double theta1 = Constants.PiOver4;

	        // Get the strains
	        var e = strains;
	        var ec2 = principalStrains.ec2;

	        // Verify the strains
	        if (e.Exists(Auxiliary.NotZero))
	        {
		        // Calculate the strain slope
		        if (e[2] == 0)
			        theta1 = 0;

		        else if (Math.Abs(e[0] - e[1]) <= 1E-9 && e[2] < 0)
				        theta1 = -Constants.PiOver4;

		        else
			        //theta1 = 0.5 * Trig.Atan(e[2] / (e[0] - e[1]));
			        theta1 = Constants.PiOver2 - Trig.Atan(2 * (e[0] - ec2) / e[2]);
            }

			// Calculate theta2
			double theta2 = Constants.PiOver2 - theta1;

			//if (theta2 > Constants.PiOver2)
			//	theta2 -= Constants.Pi;

	        return 
		        (theta1, theta2);
        }

        // Calculate principal strains
        public (double ec1, double ec2) PrincipalStrains(Vector<double> strains)
        {
	        // Get the strains
	        var e = strains;

	        // Calculate radius and center of Mohr's Circle
	        double
		        cen = 0.5 * (e[0] + e[1]),
		        rad = 0.5 * Math.Sqrt((e[1] - e[0]) * (e[1] - e[0]) + e[2] * e[2]);

	        // Calculate principal strains in concrete
	        double
		        ec1 = cen + rad,
		        ec2 = cen - rad;

	        return
		        (ec1, ec2);
        }

        // Get current Stiffness
        public Matrix<double> Stiffness
        {
	        get
	        {
				// Check if strains are set
				if (Strains != null || Strains.Exists(Auxiliary.NotZero))
					return
						ConcreteStiffness + ReinforcementStiffness;

				// Calculate initial stiffness
				return
					InitialStiffness;
	        }
        }

		// Get current stresses
		public Vector<double> Stresses
		{
			get
			{
				if (Strains != null)
					return
						ConcreteStresses + ReinforcementStresses;

				return
					CreateVector.DenseOfArray(new double[] {0, 0, 0});
			}
		}

		// Calculate initial stiffness
		public Matrix<double> InitialConcreteStiffness
		{
			get
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
        }

        // Initial reinforcement stiffness
        public Matrix<double> InitialReinforcementStiffness
        {
	        get
	        {
		        // Steel matrix
		        var Ds = Matrix<double>.Build.Dense(3, 3);
		        Ds[0, 0] = psx * Esxi;
		        Ds[1, 1] = psy * Esyi;

		        return Ds;
	        }
        }

        // Calculate initial stiffness
        public Matrix<double> InitialStiffness => InitialConcreteStiffness + InitialReinforcementStiffness;

		// Calculate stiffness
        public Matrix<double> Stiffness_(double theta)
        {
	        return
		        Concrete_Stiffness(theta) + Reinforcement_Stiffness();
        }

        // Calculate steel stiffness matrix
        public Matrix<double> Reinforcement_Stiffness()
        {
			// Calculate secant module
			var (Esx, Esy) = Reinforcement.SecantModule;

	        // Steel matrix
	        var Ds = Matrix<double>.Build.Dense(3, 3);
	        Ds[0, 0] = psx * Esx;
	        Ds[1, 1] = psy * Esy;

	        return Ds;
        }

        // Calculate concrete stiffness matrix
        public abstract Matrix<double> Concrete_Stiffness(double theta);

        // Calculate stresses/strains transformation matrix
        // This matrix transforms from x-y to 1-2 coordinates
        public abstract Matrix<double> Transformation_Matrix(double theta);

        // Calculate concrete stresses
        public abstract Vector<double> Concrete_Stresses(double theta);

        // Get reinforcement stresses as a vector multiplied by reinforcement ratio
        public Vector<double> Reinforcement_Stresses()
        {
	        var (fsx, fsy) = Reinforcement.Stresses;

	        return
		        CreateVector.DenseOfArray(new[] { psx * fsx, psy * fsy, 0 });
        }

        // Calculate stresses
        public Vector<double> Stresses_(double theta)
        {
	        return
		        Concrete_Stresses(theta) + Reinforcement_Stresses();
        }

        // Set results
        public abstract void Results();

        // Calculate slopes related to reinforcement
        private (double X, double Y) ReinforcementAngles(double theta1)
        {
	        // Calculate angles
	        double
		        thetaNx = theta1,
		        thetaNy = theta1 - Constants.PiOver2;

	        return
		        (thetaNx, thetaNy);
        }

        // Crack check
        // Crack check procedure
        public double CrackCheck(double theta2)
        {
            // Get the values
            double ec1 = Concrete.PrincipalStrains.ec1;
            var (fsx, fsy) = Reinforcement.Stresses;
            double fc = Concrete.fc;
            double f1a = Concrete.PrincipalStresses.fc1;
            double phiAg = Concrete.AggregateDiameter;

            // Calculate thetaC sine and cosine
            var (cosTheta, sinTheta) = Auxiliary.DirectionCosines(theta2);
            double tanTheta = Auxiliary.Tangent(theta2);

            // Average crack spacing and opening
            double
                smTheta = 1 / (sinTheta / smx + cosTheta / smy),
                w = smTheta * ec1;

            // Reinforcement capacity reserve
            double
                f1cx = psx * (fyx - fsx),
                f1cy = psy * (fyy - fsy);

            // Maximum possible shear on crack interface
            double vcimaxA = 0.18 * Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));

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
            StressesOnCrack();
            void StressesOnCrack()
            {
                // Initiate vci = 0 (for most common cases)
                double vci = 0;

                if (f1cx > f1cy && f1cy < fc1) // Y dominant
                    vci = (fc1 - f1cy) / tanTheta;

                if (f1cx < f1cy && f1cx < fc1) // X dominant
                    vci = (f1cx - fc1) * tanTheta;

                // Reinforcement stresses
                double
                    fsxcr = (fc1 + vci / tanTheta) / psx + fsx,
                    fsycr = (fc1 + vci * tanTheta) / psy + fsy;

                // Check if reinforcement yielded at crack
                int
                    lsYieldX = 0,
                    lsYieldY = 0;

                if (LSYield.X == 0 && fsxcr >= fyx)
                    lsYieldX = LoadStep;

                if (LSYield.Y == 0 && fsycr >= fyy)
                    lsYieldY = LoadStep;

                LSYield = (lsYieldX, lsYieldY);
            }

            return fc1;
        }

        public class MCFT : Membrane
        {
            // Constructor
            public MCFT(Concrete concrete, Reinforcement.Panel reinforcement) : base(concrete, reinforcement)
            {
            }

			// Tolerances
			private double fTol = 1E-3;
			private double eTol = 1E-9;

			// Do analysis by MCFT with applied stress
	        public override void Analysis(Vector<double> appliedStresses, int loadStep = 0)
			{
				// Initiate values
				Iteration = 0;

				// Get initial values
				var e0 = Strains;
				var (ec1, ec2) = Concrete.PrincipalStrains;
				double theta2 = PrincipalAngles.theta2;

				// Iteration procedure
				for (int it = 0; it < 1000; it++)
				{
                    //if (loadStep == 30)
                    //{
                        Console.WriteLine("\n\n----------------------------------------------------");
                        Console.WriteLine("LS = {0}, Iteration = {1}", loadStep, it);
                        Console.WriteLine("----------------------------------------------------");
                        Console.WriteLine("ec1 = {0}, ec2 = {1}, theta = {2}", ec1, ec2, theta2);
                    //}

                    // Set concrete and steel strains (stresses are not updated yet)
                    Concrete.SetStrains((ec1, ec2));
					Reinforcement.SetStrains(e0);

                    // Calculate stiffness
                    var D = Stiffness_(theta2);

					// Calculate new strains
					var ei = D.Solve(appliedStresses);

                    //if (loadStep == 30)
                        Console.WriteLine("D = {0} \nei ={1}", D, ei);

					// Calculate new principal strains
                    (ec1, ec2) = PrincipalStrains(ei);
                    theta2 = StrainAngles(ei, (ec1, ec2)).theta2;

                    if (loadStep == 30)
                        Console.WriteLine("ec1 = {0}, ec2 = {1}, theta = {2}", ec1, ec2, theta2);

                    // Calculate and set concrete and steel stresses
                    Concrete.SetStresses((ec1, ec2));
                    Reinforcement.SetStresses(ei);

					// Verify if concrete is cracked and check crack stresses to limit fc1
					if (Concrete.Cracked)
						CrackCheck(theta2);

                    //if (loadStep == 30)
                    //{
                        var (fc1, fc2) = Concrete.PrincipalStresses;
						var (fsx, fsy) = Reinforcement.Stresses;
						Console.WriteLine("fc1 = {0}, fc2 = {1} \n fsx = {2}, fsy = {3}", fc1, fc2, fsx, fsy);
                    //}

                    // Calculate final stresses (do crack check if needed)
                    var f = Stresses_(theta2);

                    if (loadStep == 30)
                        Console.WriteLine("f = {0}", f);

					// Check convergence
                    if (CheckConvergence(ei - e0, f - appliedStresses))
					{
						// Convergence reached
						// Set results
						Iteration = it;
						Strains = ei;
						PrincipalAngles = (Constants.PiOver2 - theta2, theta2);
						ConcreteStiffness = Concrete_Stiffness(theta2);
						ReinforcementStiffness = Reinforcement_Stiffness();
						ConcreteStresses = Concrete_Stresses(theta2);
						ReinforcementStresses = Reinforcement_Stresses();
						TransformationMatrix = Transformation_Matrix(PrincipalAngles.theta2);

						break;
					}

					// Update strains
					e0 = ei;
				}
            }

			// Do analysis by MCFT with applied strains
            public override void Analysis2(Vector<double> appliedStrains, int loadStep = 0)
            {
	            // Calculate new principal strains
	            var (ec1, ec2) = PrincipalStrains(appliedStrains);
	            double theta2 = StrainAngles(appliedStrains, (ec1, ec2)).theta2;

	            // Calculate and set concrete and steel stresses
	            Concrete.SetStrainsAndStresses((ec1, ec2));
	            Reinforcement.SetStrainsAndStresses(appliedStrains);

	            // Verify if concrete is cracked and check crack stresses to limit fc1
	            if (Concrete.Cracked)
		            CrackCheck(theta2);

                // Set strain and stress states
                Strains               = appliedStrains;
                PrincipalAngles       = (Constants.PiOver2 - theta2, theta2);
                ConcreteStresses      = Concrete_Stresses(theta2);
                ReinforcementStresses = Reinforcement_Stresses();
            }

            // Check convergence
            private bool CheckConvergence(Vector<double> residualStrain, Vector<double> residualStress)
			{
				// Calculate maximum residuals
				double
					erMax = residualStrain.AbsoluteMaximum(),
					frMax = residualStress.AbsoluteMaximum();

				if (erMax <= eTol && frMax <= fTol)
					return true;

				return false;
			}

            // Calculate concrete stiffness matrix
            public override Matrix<double> Concrete_Stiffness(double theta2)
			{
				var (Ec1, Ec2) = Concrete.SecantModule;
				double Gc = Ec1 * Ec2 / (Ec1 + Ec2);

				// Concrete matrix
				var Dc1 = Matrix<double>.Build.Dense(3, 3);
				Dc1[0, 0] = Ec2;
				Dc1[1, 1] = Ec1;
				Dc1[2, 2] = Gc;

				// Get transformation matrix
				var T = Transformation_Matrix(theta2);

				// Calculate Dc
				return
					T.Transpose() * Dc1 * T;
			}

			// Calculate stresses/strains transformation matrix
			// This matrix transforms from x-y to 1-2 coordinates
			public override Matrix<double> Transformation_Matrix(double theta2)
			{
				double psi = Constants.Pi - theta2;
				var (cos, sin) = Auxiliary.DirectionCosines(psi);
				double
					cos2 = cos * cos,
					sin2 = sin * sin,
					cosSin = cos * sin;

				return
					Matrix<double>.Build.DenseOfArray(new[,]
					{
						{        cos2,       sin2,      cosSin },
						{        sin2,       cos2,     -cosSin },
						{ -2 * cosSin, 2 * cosSin, cos2 - sin2 }
					});
			}

			// Calculate concrete stresses
			public override Vector<double> Concrete_Stresses(double theta2)
			{
				// Get principal stresses
				var (fc1, fc2) = Concrete.PrincipalStresses;

				// Calculate theta2 (fc2 angle)
				var (cos, sin) = Auxiliary.DirectionCosines(2 * theta2);

				// Calculate stresses by Mohr's Circle
				double
					cen = 0.5 * (fc1 + fc2),
					rad = 0.5 * (fc1 - fc2),
					fcx = cen - rad * cos,
					fcy = cen + rad * cos,
					vcxy = rad * sin;

				return
					CreateVector.DenseOfArray(new[] { fcx, fcy, vcxy });
			}

			// Set results
			public override void Results()
			{
				// Set results for stiffness
				TransformationMatrix = Transformation_Matrix(PrincipalAngles.theta2);
				ConcreteStiffness = Concrete_Stiffness(PrincipalAngles.theta2);
				ReinforcementStiffness = Reinforcement_Stiffness();
			}

            // Calculate tensile principal strain by equilibrium in a crack
            private double CrackEquilibrium()
			{
                // Get the values
                double ec1 = Concrete.PrincipalStrains.ec1;
                double theta = PrincipalAngles.theta2;
                var (fsx, fsy) = Reinforcement.Stresses;
                double fc = Concrete.fc;
                double fcr = Concrete.fcr;
                double phiAg = Concrete.AggregateDiameter;

                // Constitutive relation
                double f1a = fcr / (1 + Math.Sqrt(500 * ec1));

				// Calculate thetaC sine and cosine
				var (cosTheta, sinTheta) = Auxiliary.DirectionCosines(theta);
				double
					tanTheta  = Auxiliary.Tangent(theta),
					cosTheta2 = cosTheta * cosTheta,
					sinTheta2 = sinTheta * sinTheta;

				// Average crack spacing and opening
				double
					smTheta = 1 / (sinTheta / smx + cosTheta / smy),
					w = smTheta * ec1;

				// Calculate maximum shear stress on crack
				double vcimax = Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));

				// Equilibrium Systems
				double EquilibriumSystem1()
				{
					double vci = (psx * (fyx - fsx) - psy * (fyy - fsy)) * sinTheta * cosTheta;

					// Calculate fci
					double fci = 0;

					if (Math.Abs(vci) >= 0.18 * vcimax)
						fci = vcimax * (1 - Math.Sqrt(1.22 * (1 - Math.Abs(vci) / vcimax)));

                    return
                        psx * (fyx - fsx) * sinTheta2 + psy * (fyy - fsy) * cosTheta2 - fci;
				}

				double EquilibriumSystem2()
				{
					return
						psx * (fyx - fsx) - vcimax * (1 / tanTheta + 1);
				}

				double EquilibriumSystem3()
				{
					return
						psx * (fyx - fsx) + vcimax * (1 / tanTheta - 1);
				}

				double EquilibriumSystem4()
				{
					return
						psy * (fyy - fsy) + vcimax * (tanTheta - 1);
				}

				double EquilibriumSystem5()
				{
					return
						psy * (fyy - fsy) - vcimax * (tanTheta + 1);
				}

				// Create a list and add all the equilibrium systems
				var f1List = new List<double>();
				f1List.Add(EquilibriumSystem1());
				f1List.Add(EquilibriumSystem2());
				f1List.Add(EquilibriumSystem3());
				f1List.Add(EquilibriumSystem4());
				f1List.Add(EquilibriumSystem5());

				// Get maximum value
				return
					f1List.Max();
			}
        }

        public class DSFM : Membrane
        {
	        // Properties
	        public Vector<double> ConcreteStrains  { get; set; }
	        public Vector<double> CrackSlipStrains { get; set; }

	        // Constructor
	        public DSFM(Concrete concrete, Reinforcement.Panel reinforcement, double referenceLength) : base(concrete,
		        reinforcement)
	        {
		        concrete.ReferenceLength = referenceLength;
	        }

	        // Do analysis
	        public override void Analysis(Vector<double> appliedStresses, int loadStep = 0)
	        {
		        throw new NotImplementedException();
	        }

	        public override void Analysis2(Vector<double> appliedStrains, int loadStep = 0)
	        {
		        // Get strains
		        var e = appliedStrains;

		        // Get concrete strains from last iteration
		        Vector<double> ec;
		        if (CrackSlipStrains != null)
			        ec = appliedStrains - CrackSlipStrains;
		        else
			        ec = appliedStrains;

		        // Calculate principal strains
		        var (e1, e2) = PrincipalStrains(e);
		        var (ec1, ec2) = PrincipalStrains(ec);

		        // Calculate thetaC and thetaE
		        var (thetaE1,_) = StrainAngles(e, (e1, e2));
		        var (thetaC1, thetaC2) = StrainAngles(ec, (ec1, ec2));

				// Calculate reinforcement angles
				var (thetaNx, thetaNy) = ReinforcementAngles(thetaC1);

		        // Calculate and set concrete and steel stresses
		        Concrete.SetStrainsAndStresses((ec1, ec2), Reinforcement, (thetaNx, thetaNy));
		        Reinforcement.SetStrainsAndStresses(appliedStrains);

				// Calculate concrete stiffness
				var Dc = Concrete_Stiffness(thetaC1);

				// Initiate crack slip strains and pseudo-prestress
				var es   = Vector<double>.Build.Dense(3);
				var sig0 = Vector<double>.Build.Dense(3);

		        // Verify if concrete is cracked
		        if (Concrete.Cracked)
		        {
			        // Calculate crack local stresses
			        var (_, _, vci) = CrackLocalStresses(thetaC1);

			        // Calculate crack slip strains
			        es = Crack_Slip_Strains(e, thetaE1, thetaC1, vci);

					// Calculate pseudo-prestress
					sig0 = PseudoPrestress(Dc, es);
		        }

		        // Set strain and stress states
		        Strains = appliedStrains;
		        CrackSlipStrains = es;
		        ConcreteStrains = appliedStrains - es;
		        PrincipalAngles = (thetaC1, thetaC2);
		        ConcreteStresses = Concrete_Stresses(Dc, sig0, e);
		        ReinforcementStresses = Reinforcement_Stresses();
	        }

            // Calculate concrete stiffness matrix
            public override Matrix<double> Concrete_Stiffness(double thetaC1)
	        {
		        var (Ec1, Ec2) = Concrete.SecantModule;
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
	        public override Matrix<double> Transformation_Matrix(double thetaC1)
	        {
		        var (cos, sin) = Auxiliary.DirectionCosines(thetaC1);
		        double
			        cos2 = cos * cos,
			        sin2 = sin * sin,
			        cosSin = cos * sin;

		        return
			        Matrix<double>.Build.DenseOfArray(new[,]
			        {
				        {        cos2,       sin2,      cosSin },
				        {        sin2,       cos2,     -cosSin },
				        { -2 * cosSin, 2 * cosSin, cos2 - sin2 }
			        });
	        }

	        public override Vector<double> Concrete_Stresses(double theta)
	        {
		        throw new NotImplementedException();
	        }

			// Calculate concrete stresses
			public Vector<double> Concrete_Stresses(Matrix<double> Dc, Vector<double> sig0, Vector<double> apparentStrains)
			{
				return
					Dc * apparentStrains - sig0;
			}

	        // Calculate crack local stresses
            private (double fscrx, double fscry, double vci) CrackLocalStresses(double thetaC1)
	        {
		        // Initiate stresses
		        double
			        fscrx = 0,
			        fscry = 0,
			        vci = 0;

		        // Get the strains
		        double
			        ex = Strains[0],
			        ey = Strains[1];

		        // Get concrete tensile stress
		        double fc1 = Concrete.PrincipalStresses.fc1;

		        // Get reinforcement angles and stresses
		        var (thetaNx, thetaNy) = ReinforcementAngles(thetaC1);
		        var (fsx, fsy) = Reinforcement.Stresses;

		        // Calculate cosines and sines
		        var (cosNx, sinNx) = Auxiliary.DirectionCosines(thetaNx);
		        var (cosNy, sinNy) = Auxiliary.DirectionCosines(thetaNy);
		        double
			        cosNx2 = cosNx * cosNx,
			        cosNy2 = cosNy * cosNy;

		        // Function to check equilibrium
		        Func<double, double> crackEquilibrium = de1crIt =>
		        {
			        // Calculate local strains
			        double
				        escrx = ex + de1crIt * cosNx2,
				        escry = ey + de1crIt * cosNy2;

			        // Calculate reinforcement stresses
			        fscrx = Math.Min(escrx * Esxi, fyx);
			        fscry = Math.Min(escry * Esyi, fyy);

			        // Check equilibrium (must be zero)
			        double equil = psx * (fscrx - fsx) * cosNx2 + psy * (fscry - fsy) * cosNy2 - fc1;

			        return equil;
		        };

		        // Solve the nonlinear equation by Brent Method
		        double de1cr;
		        bool solution = Brent.TryFindRoot(crackEquilibrium, 1E-9, 0.01, 1E-6, 1000, out de1cr);

		        // Verify if it reached convergence
		        if (solution)
		        {
			        // Calculate local strains
			        double
				        escrx = ex + de1cr * cosNx2,
				        escry = ey + de1cr * cosNy2;

			        // Calculate reinforcement stresses
			        fscrx = Math.Min(escrx * Esxi, fyx);
			        fscry = Math.Min(escry * Esyi, fyy);

			        // Calculate shear stress
			        vci = psx * (fscrx - fsx) * cosNx * sinNx + psy * (fscry - fsy) * cosNy * sinNy;
		        }

		        // Analysis must stop
		        else
			        Stop = (true, "Equilibrium on crack not reached at step ");

		        return (fscrx, fscry, vci);
	        }

	        // Calculate crack slip
	        private Vector<double> Crack_Slip_Strains(Vector<double> apparentStrains, double thetaE1, double thetaC1, double vci)
	        {
		        // Get concrete principal tensile strain
		        double ec1 = Concrete.PrincipalStrains.ec1;
		        double fc = Concrete.fc;

		        // Get the strains
		        double
			        ex  = apparentStrains[0],
			        ey  = apparentStrains[1],
			        yxy = apparentStrains[2];

		        // Get the angles
		        var (cosThetaC, sinThetaC) = Auxiliary.DirectionCosines(thetaC1);

		        // Calculate crack spacings and width
		        double s = 1 / (sinThetaC / smx + cosThetaC / smy);

		        // Calculate crack width
		        double w = ec1 * s;

		        // Calculate shear slip strain by stress-based approach
		        double
			        ds = vci / (1.8 * Math.Pow(w, -0.8) + (0.234 * Math.Pow(w, -0.707) - 0.2) * fc),
			        ysa = ds / s;

		        // Calculate shear slip strain by rotation lag approach
		        double
			        thetaIc = Constants.PiOver4,
			        dThetaE = thetaE1 - thetaIc,
			        thetaL = Trig.DegreeToRadian(5),
			        dThetaS;

		        if (Math.Abs(dThetaE) > thetaL)
			        dThetaS = dThetaE - thetaL;

		        else
			        dThetaS = dThetaE;

		        double
			        thetaS = thetaIc + dThetaS;

		        var (cos2ThetaS, sin2ThetaS) = Auxiliary.DirectionCosines(2 * thetaS);

		        double ysb = yxy * cos2ThetaS + (ey - ex) * sin2ThetaS;

		        // Calculate shear slip strains
		        var (cos2ThetaC, sin2ThetaC) = Auxiliary.DirectionCosines(2 * thetaC1);

		        double
			        ys = Math.Max(ysa, ysb),
			        exs = -ys / 2 * sin2ThetaC,
			        eys = ys / 2 * sin2ThetaC,
			        yxys = ys * cos2ThetaC;

		        // Calculate the vector of shear slip strains
		        return
			        Vector<double>.Build.DenseOfArray(new[] {exs, eys, yxys});
	        }

	        // Calculate the pseudo-prestress
	        private Vector<double> PseudoPrestress(Matrix<double> Dc, Vector<double> es)
	        {
		        return
			        Dc * es;
	        }

	        // Set results
	        public override void Results()
	        {
		        // Set results for stiffness
		        TransformationMatrix = Transformation_Matrix(PrincipalAngles.theta1);
		        ConcreteStiffness = Concrete_Stiffness(PrincipalAngles.theta1);
		        ReinforcementStiffness = Reinforcement_Stiffness();
	        }
        }
    }

}