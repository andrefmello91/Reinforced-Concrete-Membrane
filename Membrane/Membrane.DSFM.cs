using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using Material;
using Relations;
using Parameters = Material.Concrete.ModelParameters;
using Behavior = Material.Concrete.ModelBehavior;

namespace Membrane
{
	public class DSFM : Membrane
	{
		// Properties
		public Vector<double>                 CrackSlipStrains           { get; set; }
		public Vector<double>                 PseudoPrestress            { get; set; }
		public (double e1, double e2)         ApparentPrincipalStrains   { get; set; }
		public (double theta1, double theta2) ApparentAngles             { get; set; }

		public bool ConsiderCrackSlip
		{
			get => Concrete.ConcreteBehavior.ConsiderCrackSlip;
			set => Concrete.ConcreteBehavior.ConsiderCrackSlip = value;
		}

		// Constructor
		public DSFM(Concrete.Biaxial concrete, Reinforcement.Biaxial reinforcement, double panelWidth,
			bool considerCrackSlip = true) : base(concrete, reinforcement, panelWidth)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new Concrete.Biaxial(fc, phiAg, Parameters.DSFM, Behavior.DSFM);

			// Initiate crack slip strains
			ConsiderCrackSlip = considerCrackSlip;
			CrackSlipStrains  = Vector<double>.Build.Dense(3);
		}

        // Get concrete strains
        private Vector<double> ConcreteStrains => Strains - CrackSlipStrains;

		// Do Analysis
		public override void Calculate(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0)
		{
			// Set strains
			Strains = appliedStrains;

			// Calculate reference length
			double lr = ReferenceLength();

            // Calculate and set concrete and steel stresses
            Concrete.CalculatePrincipalStresses(ConcreteStrains, lr, Reinforcement);
			Reinforcement.CalculateStresses(Strains);

            // Calculate apparent principal strains
            ApparentPrincipalStrains = Strain.PrincipalStrains(Strains);
			ApparentAngles           = Strain.PrincipalAngles(Strains, ApparentPrincipalStrains);

			// Check stresses on crack
			if (ConsiderCrackSlip)
				CalculateCrackSlip();
			else
				CrackCheck();

			// Calculate stiffness
			Concrete.CalculateStiffness();
			Reinforcement.CalculateStiffness();
        }

		// Calculate residual stresses
		public override Vector<double> ResidualStresses(Vector<double> appliedStresses)
		{
			if (ConsiderCrackSlip)
				return
					base.ResidualStresses(appliedStresses + PseudoPrestress);

			return
				base.ResidualStresses(appliedStresses);
		}

		// Calculate crack slip
		public void CalculateCrackSlip()
		{
			// Calculate crack local stresses
			var (_, _, vci) = CrackLocalStresses();

			// Calculate crack slip strains
			CrackSlipStrains = CrackSlip(vci);

			// Calculate pseudo-prestress
			PseudoPrestress = PseudoPStress();
		}


		// Calculate crack local stresses
		private (double fscrx, double fscry, double vci) CrackLocalStresses(double? thetaC1 = null)
		{
			if (!thetaC1.HasValue)
				thetaC1 = Concrete.PrincipalAngles.theta1;

			// Initiate stresses
			double
				fscrx = 0,
				fscry = 0,
				vci = 0;

			// Verify if concrete is cracked
			if (Concrete.Cracked)
			{
				// Get the strains
				double
					ex = Strains[0],
					ey = Strains[1];

				// Get concrete tensile stress
				var fc1 = Concrete.PrincipalStresses.fc1;

				// Get reinforcement angles and stresses
				var (thetaNx, thetaNy) = Reinforcement.Angles(thetaC1.Value);
				var (fsx, fsy) = Reinforcement.SteelStresses;

				// Calculate cosines and sines
				//var (cosTheta, sinTheta) = Auxiliary.DirectionCosines(thetaC1);
				var (cosNx, sinNx) = DirectionCosines(thetaNx);
				var (cosNy, sinNy) = DirectionCosines(thetaNy);
				double
					//cosTheta2 = cosTheta * cosTheta,
					//sinTheta2 = sinTheta * sinTheta,
					cosNx2 = cosNx * cosNx,
					cosNy2 = cosNy * cosNy;

				// Solve the nonlinear equation by Brent Method
				bool solution = false;
				double de1Cr = 0;
				try
				{
					solution = Brent.TryFindRoot(CrackEquilibrium, 0, 0.005, 1E-9, 100, out de1Cr);

					// Function to check equilibrium
					double CrackEquilibrium(double de1CrIt)
					{
						// Calculate local strains
						double
							esCrxIt = ex + de1CrIt * cosNx2,
							esCryIt = ey + de1CrIt * cosNy2;

						// Calculate reinforcement stresses
						double
							fscrxIt = Math.Min(esCrxIt * Esxi, fyx),
							fscryIt = Math.Min(esCryIt * Esyi, fyy);

						// Check equilibrium (must be zero)
						return
							psx * (fscrxIt - fsx) * cosNx2 + psy * (fscryIt - fsy) * cosNy2 - fc1;
					}
				}
				catch
				{
					solution = false;
				}
				finally
				{
					// Verify if it reached convergence
					if (solution)
					{
						// Calculate local strains
						double
							esCrx = ex + de1Cr * cosNx2,
							esCry = ey + de1Cr * cosNy2;

						// Calculate reinforcement stresses
						fscrx = Math.Min(esCrx * Esxi, fyx);
						fscry = Math.Min(esCry * Esyi, fyy);

						// Calculate shear stress
						vci = psx * (fscrx - fsx) * cosNx * sinNx + psy * (fscry - fsy) * cosNy * sinNy;
					}
				}
			}

			return
				(fscrx, fscry, vci);
		}

		// Calculate crack slip
		private Vector<double> CrackSlip(double vci = 0, double? thetaE1 = null, double? thetaC1 = null)
		{
			if (!thetaC1.HasValue)
				thetaC1 = Concrete.PrincipalAngles.theta1;

			if (!thetaE1.HasValue)
				thetaE1 = ApparentAngles.theta1;

			// Calculate shear slip strains
			var (cos2ThetaC, sin2ThetaC) = DirectionCosines(2 * thetaC1.Value);

			// Get shear slip strains
			double
				ysa = StressCrackSlip(thetaC1.Value, vci),
				ysb = RotationLagCrackSlip(thetaE1.Value),
				ys = Math.Max(ysa, ysb);

			// Calculate the vector of shear slip strains
			return
				Vector<double>.Build.DenseOfArray(new[]
				{
					-ys / 2 * sin2ThetaC,
					 ys / 2 * sin2ThetaC,
					 ys * cos2ThetaC
				});
		}

		// Calculate shear slip strain by stress-based approach (Walraven)
		private double StressCrackSlip(double thetaC1, double vci)
		{
			if (vci == 0)
				return 0;

			// Get concrete principal tensile strain
			double ec1 = Concrete.PrincipalStrains.ec1;
			double fc = Concrete.fc;

			// Get the angles
			var (cosThetaC, sinThetaC) = DirectionCosines(thetaC1);
			cosThetaC = Math.Abs(cosThetaC);
			sinThetaC = Math.Abs(sinThetaC);

			// Calculate crack spacings and width
			double s = 1 / (sinThetaC / smx + cosThetaC / smy);

			// Calculate crack width
			double w = ec1 * s;

			// Calculate shear slip strain by stress-based approach
			double
				a = Math.Max(0.234 * Math.Pow(w, -0.707) - 0.2, 0),
				ds = vci / (1.8 * Math.Pow(w, -0.8) + a * fc);

			return
				ds / s;
		}

		// Calculate shear slip strain by stress-based approach (Okamura and Maekawa)
		private double StressCrackSlip2(double thetaC1, double vci)
		{
			if (vci == 0)
				return 0;

			// Get concrete principal tensile strain
			double ec1 = Concrete.PrincipalStrains.ec1;

			// Get the angles
			var (cosThetaC, sinThetaC) = DirectionCosines(thetaC1);
			cosThetaC = Math.Abs(cosThetaC);
			sinThetaC = Math.Abs(sinThetaC);

			// Calculate crack spacings and width
			double s = 1 / (sinThetaC / smx + cosThetaC / smy);

			// Calculate crack width
			double w = ec1 * s;

			// Calculate vciMax and psi ratio
			double
				vciMax = MaximumShearOnCrack(w),
				psi = vci / vciMax;

			// Calculate shear slip strain by stress-based approach
			double ds = 0.75 * w * Math.Sqrt(psi / (psi - 1));

			return
				ds / s;
		}

		// Calculate shear slip strain by rotation lag approach
		private double RotationLagCrackSlip(double thetaE1)
		{
			// Get the strains
			double
				ex  = Strains[0],
				ey  = Strains[1],
				yxy = Strains[2];

			// Calculate shear slip strain by rotation lag approach
			double
				thetaIc = Constants.PiOver4,
				dThetaE = thetaE1 - thetaIc,
				thetaL,
				dThetaS;

			if (psx > 0 && psy > 0)
				thetaL = Trig.DegreeToRadian(5);
			else
				thetaL = Trig.DegreeToRadian(7.5);


			if (Math.Abs(dThetaE) > thetaL)
				dThetaS = dThetaE - thetaL;

			else
				dThetaS = dThetaE;

			double
				thetaS = thetaIc + dThetaS;

			var (cos2ThetaS, sin2ThetaS) = DirectionCosines(2 * thetaS);

			return
				yxy * cos2ThetaS + (ey - ex) * sin2ThetaS;
		}

		// Calculate the pseudo-prestress
		private Vector<double> PseudoPStress(Matrix<double> Dc = null, Vector<double> es = null)
		{
			if (Dc == null)
				Dc = Concrete.Stiffness;

			if (es == null)
				es = CrackSlipStrains;

			return
				Dc * es;
		}
	}
}