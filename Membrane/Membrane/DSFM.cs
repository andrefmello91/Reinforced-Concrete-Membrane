using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using Material.Concrete;
using OnPlaneComponents;
using Reinforcement   = Material.Reinforcement.BiaxialReinforcement;
using Parameters      = Material.Concrete.Parameters;
using UnitsNet;


namespace RCMembrane
{
	/// <summary>
	/// DSFM class, based on formulation by Vecchio (2000).
	/// </summary>
	public class DSFMMembrane : Membrane
	{
		/// <summary>
		/// Get/set the average <see cref="PrincipalStrainState"/>.
		/// </summary>
		public PrincipalStrainState AveragePrincipalStrains { get; set; }

		///<inheritdoc/>
		public override StrainState ConcreteStrains => AverageStrains - CrackSlipStrains;

        /// <summary>
        /// Get/set the crack slip <see cref="StrainState"/>.
        /// </summary>
        public StrainState CrackSlipStrains { get; set; }

		/// <summary>
		/// Get/set the pseudo <see cref="StressState"/>.
		/// </summary>
		public StressState PseudoPrestress { get; set; }

		/// <summary>
		/// Get/set crack slip consideration.
		/// </summary>
		public bool ConsiderCrackSlip
		{
			get => Concrete.Constitutive.ConsiderCrackSlip;
			set => Concrete.Constitutive.ConsiderCrackSlip = value;
		}

		///<inheritdoc/>
		/// <summary>
		/// Membrane element for DSFM analysis.
		/// </summary>
		/// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
		public DSFMMembrane(BiaxialConcrete concrete, Reinforcement reinforcement, double width,
			bool considerCrackSlip = true) : base(concrete, reinforcement, width)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new BiaxialConcrete(fc, phiAg, ParameterModel.DSFM, ConstitutiveModel.DSFM);

			// Initiate crack slip strains
			ConsiderCrackSlip = considerCrackSlip;
			CrackSlipStrains  = StrainState.Zero;
		}

		///<inheritdoc/>
		/// <summary>
		/// Membrane element for DSFM analysis.
		/// </summary>
		/// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
		public DSFMMembrane(Parameters concreteParameters, Constitutive concreteConstitutive,
			Reinforcement reinforcement, double width,
			bool considerCrackSlip = true) : base(concreteParameters, concreteConstitutive, reinforcement, width)
		{
			// Initiate new concrete
			Concrete = new BiaxialConcrete(concreteParameters, concreteConstitutive);

			// Initiate crack slip strains
			ConsiderCrackSlip = considerCrackSlip;
			CrackSlipStrains  = StrainState.Zero;
		}

		/// <summary>
		/// Calculate stresses and the membrane stiffness by DSFM, given strains.
		/// </summary>
		/// <param name="appliedStrains">Current strains.</param>
		/// <param name="loadStep">Current load step.</param>
		/// <param name="iteration">Current iteration.</param>
		public override void Calculate(StrainState appliedStrains, int loadStep = 0, int iteration = 0)
		{
			// Set strains
			AverageStrains = appliedStrains;

			// Calculate reference length
			double lr = ReferenceLength();

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(ConcreteStrains, lr, Reinforcement);
			Reinforcement.CalculateStresses(AverageStrains);

			// Calculate apparent principal strains
			AveragePrincipalStrains  = PrincipalStrainState.FromStrain(AverageStrains);

			// Check stresses on crack
			if (ConsiderCrackSlip)
				CalculateCrackSlip();
			else
				CrackCheck();

			// Calculate stiffness
			Concrete.CalculateStiffness();
			Reinforcement.CalculateStiffness();
		}

		/// <summary>
		/// Calculate and set crack slip strains and pseudo-prestress.
		/// </summary>
		public void CalculateCrackSlip()
		{
			// Calculate crack local stresses
			var vci = ShearAtCrack();

			// Calculate crack slip strains
			CrackSlipStrains = CrackSlip(vci);

			// Calculate pseudo-prestress
			PseudoPrestress = PseudoPStresses();
		}

		/// <summary>
		/// Calculate shear stress at the crack surface.
		/// </summary>
		private double ShearAtCrack()
		{
			double theta1 = Concrete.PrincipalStrains.Theta1;

			// Initiate stresses
			double vci   = 0;

			// Verify if concrete is cracked
			if (Concrete.Cracked)
			{
				// Get the average strains
				double
					ex = AverageStrains.EpsilonX,
					ey = AverageStrains.EpsilonY;

				// Get concrete tensile stress
				var fc1 = Concrete.PrincipalStresses.Sigma1;

				// Get reinforcement ratio
				double
					psx = Reinforcement?.DirectionX?.Ratio ?? 0,
					psy = Reinforcement?.DirectionY?.Ratio ?? 0;

				// Get reinforcement angles
				var (thetaNx, thetaNy) = Reinforcement?.Angles(theta1) ??
				                         (Concrete.PrincipalStrains.Theta1,
					                         Concrete.PrincipalStrains.Theta1 - Constants.PiOver2);

				// Get reinforcement stresses
				var rSt = Reinforcement?.Stresses;
				double
					fsx = rSt?.SigmaX ?? 0,
					fsy = rSt?.SigmaY ?? 0;

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
							fscrxIt = Reinforcement?.DirectionX?.Steel.CalculateStress(esCrxIt) ?? 0,
							fscryIt = Reinforcement?.DirectionY?.Steel.CalculateStress(esCryIt) ?? 0;

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
						double
							fscrx = Reinforcement?.DirectionX?.Steel.CalculateStress(esCrx) ?? 0,
							fscry = Reinforcement?.DirectionY?.Steel.CalculateStress(esCry) ?? 0;

						// Calculate shear stress
						vci = psx * (fscrx - fsx) * cosNx * sinNx + psy * (fscry - fsy) * cosNy * sinNy;
					}
				}
			}

			return vci;
		}

		/// <summary>
		/// Calculate crack slip strains.
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private StrainState CrackSlip(double vci)
		{
			// Get shear slip strains
			double
				ysa = StressCrackSlip(vci),
				ysb = RotationLagCrackSlip(),
				ys = Math.Max(ysa, ysb);

			// Get concrete principal angle
			double thetaC1 = Concrete.PrincipalStrains.Theta1;

			// Calculate direction cosines
			var (cos2ThetaC, sin2ThetaC) = DirectionCosines(2 * thetaC1);

            // Calculate the shear slip strains
            double
                eslx  = -0.5 * ys * sin2ThetaC,
				esly  =  0.5 * ys * sin2ThetaC,
				eslxy = ys * cos2ThetaC;

			return new StrainState(eslx, esly, eslxy);
		}

		/// <summary>
		/// Calculate crack slip strains by stress-based approach (Walraven, 1980).
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private double StressCrackSlip(double vci)
		{
			if (vci == 0)
				return 0;

			// Get concrete principal angle
			double thetaC1 = Concrete.PrincipalStrains.Theta1;

			// Get concrete principal tensile strain and strength
			double ec1 = Concrete.PrincipalStrains.Epsilon1;
			double fc  = Concrete.fc;

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

		/// <summary>
		/// Calculate crack slip strains by stress-based approach (Okamura and Maekawa)
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private double StressCrackSlip2(double vci)
		{
			if (vci == 0)
				return 0;

			// Get concrete principal angle
			double thetaC1 = Concrete.PrincipalStrains.Theta1;

			// Get concrete principal tensile strain and strength
			double ec1 = Concrete.PrincipalStrains.Epsilon1;

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

		/// <summary>
		/// Calculate shear slip strain by rotation lag approach.
		/// </summary>
		private double RotationLagCrackSlip()
		{
			// Get reinforcement ratio
			double
				psx = Reinforcement?.DirectionX?.Ratio ?? 0,
				psy = Reinforcement?.DirectionY?.Ratio ?? 0;

			// Get principal angle
			double thetaE1 = AveragePrincipalStrains.Theta1;

			// Get the strains
			double
				ex  = AverageStrains.EpsilonX,
				ey  = AverageStrains.EpsilonY,
				yxy = AverageStrains.GammaXY;

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

		/// <summary>
		/// Calculate current pseudo-prestresses, in MPa.
		/// </summary>
		private StressState PseudoPStresses()
		{
			var Dc = Concrete.Stiffness;
			var es = CrackSlipStrains.Vector;

			// Calculate stress vector
			var ps = Dc * es;

			return StressState.FromVector(ps);
		}
	}
}