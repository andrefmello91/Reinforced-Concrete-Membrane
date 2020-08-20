using System;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.RootFinding;
using Material.Concrete;
using Reinforcement   = Material.Reinforcement.BiaxialReinforcement;
using Parameters      = Material.Concrete.Parameters;


namespace RCMembrane
{
	/// <summary>
	/// DSFM class, based on formulation by Vecchio (2000).
	/// </summary>
	public class DSFMMembrane : Membrane
	{
		// Properties
		public Vector<double>                 CrackSlipStrains           { get; set; }
		public Vector<double>                 PseudoPrestress            { get; set; }
		public (double e1, double e2)         ApparentPrincipalStrains   { get; set; }
		public (double theta1, double theta2) ApparentAngles             { get; set; }

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
		public DSFMMembrane(BiaxialConcrete concrete, Reinforcement reinforcement, double width, bool considerCrackSlip = true) : base(concrete, reinforcement, width)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new BiaxialConcrete(fc, phiAg, ParameterModel.DSFM, ConstitutiveModel.DSFM);

			// Initiate crack slip strains
			ConsiderCrackSlip = considerCrackSlip;
			CrackSlipStrains  = Vector<double>.Build.Dense(3);
		}

		///<inheritdoc/>
		/// <summary>
		/// Membrane element for DSFM analysis.
		/// </summary>
		/// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
		public DSFMMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, Reinforcement reinforcement, double width,
			bool considerCrackSlip = true) : base(concreteParameters, concreteConstitutive, reinforcement, width)
		{
			// Initiate new concrete
			Concrete = new BiaxialConcrete(concreteParameters, concreteConstitutive);

			// Initiate crack slip strains
			ConsiderCrackSlip = considerCrackSlip;
			CrackSlipStrains  = Vector<double>.Build.Dense(3);
		}

        /// <summary>
        /// Get current concrete strains.
        /// </summary>
        private Vector<double> ConcreteStrains => Strains - CrackSlipStrains;

        /// <summary>
        /// Calculate stresses and the membrane stiffness by DSFM, given strains.
        /// </summary>
        /// <param name="appliedStrains">Current strains.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="iteration">Current iteration.</param>
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

		/// <summary>
        /// Calculate and set crack slip strains and pseudo-prestress.
        /// </summary>
		public void CalculateCrackSlip()
		{
			// Calculate crack local stresses
			var (_, _, vci) = CrackLocalStresses();

			// Calculate crack slip strains
			CrackSlipStrains = CrackSlip(vci);

			// Calculate pseudo-prestress
			PseudoPrestress = PseudoPStresses();
		}

        /// <summary>
        /// Calculate crack local stresses.
        /// <para>fscrx and fscry are reinforcement local stresses on the crack surface.</para>
        /// <para>vci is the shear stress on the crack surface.</para>
        /// </summary>
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        private (double fscrx, double fscry, double vci) CrackLocalStresses(double? thetaC1 = null)
		{
			double theta1 = thetaC1 ?? Concrete.PrincipalAngles.theta1;

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
				var (thetaNx, thetaNy) = Reinforcement.Angles(theta1);
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

        /// <summary>
        /// Calculate crack slip strains.
        /// </summary>
        /// <param name="vci">Shear stress on crack surface, in MPa.</param>
        /// <param name="thetaE1">Apparent principal tensile strain angle, in radians.</param>
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        private Vector<double> CrackSlip(double vci = 0, double? thetaE1 = null, double? thetaC1 = null)
		{
			double
				thetaC = thetaC1 ?? Concrete.PrincipalAngles.theta1,
				thetaE = thetaE1 ?? ApparentAngles.theta1;

			// Calculate shear slip strains
			var (cos2ThetaC, sin2ThetaC) = DirectionCosines(2 * thetaC);

			// Get shear slip strains
			double
				ysa = StressCrackSlip(thetaC, vci),
				ysb = RotationLagCrackSlip(thetaE),
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

        /// <summary>
        /// Calculate crack slip strains by stress-based approach (Walraven, 1980).
        /// </summary>
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        /// <param name="vci">Shear stress on crack surface, in MPa.</param>
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

        /// <summary>
        /// Calculate crack slip strains by stress-based approach (Okamura and Maekawa)
        /// </summary>
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        /// <param name="vci">Shear stress on crack surface, in MPa.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculate shear slip strain by rotation lag approach.
        /// </summary>
        /// <param name="thetaE1">Apparent principal tensile strain angle, in radians.</param>
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

        /// <summary>
        /// Calculate current pseudo-prestresses, in MPa.
        /// </summary>
        /// <param name="concreteStiffness">Current concrete stiffness.</param>
        /// <param name="crackSlipStrains">Current crack slip strains.</param>
        /// <returns></returns>
        private Vector<double> PseudoPStresses(Matrix<double> concreteStiffness = null, Vector<double> crackSlipStrains = null)
		{
			var Dc = concreteStiffness ?? Concrete.Stiffness;
			var es = crackSlipStrains  ?? CrackSlipStrains;

			return
				Dc * es;
		}
	}
}