﻿using System;
using Extensions.Number;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.RootFinding;
using Material.Concrete;
using Material.Concrete.Biaxial;
using Material.Reinforcement;
using Material.Reinforcement.Biaxial;
using OnPlaneComponents;
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
        /// Initial cracking angle.
        /// </summary>
		private double? _thetaIc;

		public string SlipApproach;

		///<inheritdoc/>
		public override StrainState ConcreteStrains => AverageStrains - CrackSlipStrains;

        /// <summary>
        /// Get/set the crack slip <see cref="StrainState"/>.
        /// </summary>
        public StrainState CrackSlipStrains { get; private set; }

        /// <summary>
        /// Get/set crack slip consideration.
        /// <para>See: <seealso cref="Constitutive.ConsiderCrackSlip"/>.</para>
        /// </summary>
        public bool ConsiderCrackSlip
		{
			get => Concrete.Constitutive.ConsiderCrackSlip;
			set => Concrete.Constitutive.ConsiderCrackSlip = value;
		}

        /// <summary>
        /// Get current pseudo-stresses, in MPa.
        /// </summary>
        public StressState PseudoStresses => CrackSlipStrains.IsZero ? StressState.Zero : StressState.FromStrains(CrackSlipStrains, Concrete.Stiffness);

        /// <summary>
        /// Membrane element for DSFM analysis.
        /// </summary>
        /// <inheritdoc/>
        /// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
        public DSFMMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, double width, bool considerCrackSlip = true) 
	        : this(concrete.Parameters, reinforcement, width, considerCrackSlip)
		{
		}

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        /// <inheritdoc/>
        /// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
        public DSFMMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, Length width, bool considerCrackSlip = true) 
	        : this(concrete.Parameters, reinforcement, width, considerCrackSlip)
        {
        }

        ///<inheritdoc/>
        /// <summary>
        /// Membrane element for DSFM analysis.
        /// </summary>
        /// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
        public DSFMMembrane(Parameters concreteParameters, WebReinforcement reinforcement, double width, bool considerCrackSlip = true) 
	        : this(concreteParameters, reinforcement, Length.FromMillimeters(width), considerCrackSlip)
		{
		}

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <inheritdoc/>
        /// <param name="considerCrackSlip">Consider crack slip? (default: true)</param>
        public DSFMMembrane(Parameters concreteParameters, WebReinforcement reinforcement, Length width, bool considerCrackSlip = true) 
	        : base(concreteParameters, reinforcement, width, ConstitutiveModel.DSFM)
        {
	        // Initiate crack slip strains
	        ConsiderCrackSlip = considerCrackSlip;
	        CrackSlipStrains  = StrainState.Zero;
        }

        /// <summary>
        /// Calculate <see cref="StressState"/> and <see cref="Membrane.Stiffness"/> by DSFM, given a known <see cref="StrainState"/>.
        /// </summary>
        /// <param name="appliedStrains">Current <see cref="StrainState"/>.</param>
        public override void Calculate(StrainState appliedStrains)
		{
			// Set strains
			AverageStrains = appliedStrains.Copy();

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(ConcreteStrains, Reinforcement, ReferenceLength());
			Reinforcement.CalculateStresses(AverageStrains);

			// Calculate apparent principal strains
			AveragePrincipalStrains  = PrincipalStrainState.FromStrain(AverageStrains);

			// Check stresses on crack
			if (ConsiderCrackSlip)
				CalculateCrackSlip();
			else
				CrackCheck();
		}

		/// <summary>
		/// Calculate and set <see cref="CrackSlipStrains"/>.
		/// </summary>
		private	void CalculateCrackSlip()
		{
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

            // Verify the initial crack angle
            InitialCrackAngle();

			// Calculate crack local stresses
			var vci = ShearAtCrack();

			// Calculate crack slip strains
			CrackSlipStrains = CrackSlip(vci);
		}

		/// <summary>
        /// Set the angle of initial cracking (<see cref="_thetaIc"/>).
        /// </summary>
		private void InitialCrackAngle()
		{
			if (!_thetaIc.HasValue && Concrete.Cracked)
				_thetaIc = Concrete.PrincipalStrains.Theta1;
		}

		/// <summary>
		/// Calculate shear stress at the crack surface.
		/// </summary>
		private double ShearAtCrack()
		{
			double theta1 = Concrete.PrincipalStrains.Theta1;

			// Initiate stresses
			double vci = 0;

            // Get the average strains
            double
                ex = AverageStrains.EpsilonX,
				ey = AverageStrains.EpsilonY;

			// Get concrete tensile stress
			var fc1 = Concrete.PrincipalStresses.Sigma1;

			// Get reinforcement angles
			var (thetaNx, thetaNy) = Reinforcement?.Angles(theta1) ?? (theta1, theta1 - Constants.PiOver2);

			// Get reinforcement stresses
			double
				fsx = Reinforcement?.Stresses.SigmaX ?? 0,
				fsy = Reinforcement?.Stresses.SigmaY ?? 0;

			// Calculate cosines and sines
			var (cosNx, sinNx) = thetaNx.DirectionCosines(true);
			var (cosNy, sinNy) = thetaNy.DirectionCosines(true);
			double
				cosNx2 = cosNx * cosNx,
				cosNy2 = cosNy * cosNy;

			// Solve the nonlinear equation by Brent Method
			if (Brent.TryFindRoot(CrackEquilibrium, 0, 0.005, 1E-9, 1000, out var de1Cr))
			{
				// Calculate local strains
				double
					esCrx = ex + de1Cr * cosNx2,
					esCry = ey + de1Cr * cosNy2;

				// Calculate reinforcement stresses
				double
					fscrx = Reinforcement?.DirectionX?.CalculateStress(esCrx) ?? 0,
					fscry = Reinforcement?.DirectionY?.CalculateStress(esCry) ?? 0;

				// Calculate shear stress
				vci = (fscrx - fsx) * cosNx * sinNx + (fscry - fsy) * cosNy * sinNy;
			}

			return vci;

			// Function to check equilibrium
			double CrackEquilibrium(double de1CrIt)
			{
				// Calculate local strains
				double
					esCrxIt = ex + de1CrIt * cosNx2,
					esCryIt = ey + de1CrIt * cosNy2;

				// Calculate reinforcement stresses
				double
					fscrxIt = Reinforcement?.DirectionX?.CalculateStress(esCrxIt) ?? 0,
					fscryIt = Reinforcement?.DirectionY?.CalculateStress(esCryIt) ?? 0;

				// Check equilibrium (must be zero)
				return
					(fscrxIt - fsx) * cosNx2 + (fscryIt - fsy) * cosNy2 - fc1;
			}
		}

		/// <summary>
		/// Calculate crack slip <see cref="StrainState"/>.
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private StrainState CrackSlip(double vci)
		{
			//Console.WriteLine(vci);
			// Get shear slip strains
			double
				ysa = StressCrackSlip(vci),
                ysb = RotationLagCrackSlip(),
                ys  = Math.Max(ysa, ysb);

			SlipApproach = ys == ysa ? "Stress" : "Rotation lag";

			// Get concrete principal angle
			double thetaC1 = Concrete.PrincipalStrains.Theta1;

			// Calculate direction cosines
			var (cos2ThetaC, sin2ThetaC) = (2 * thetaC1).DirectionCosines();

            // Calculate the shear slip strains
            double
                eslx  = -0.5 * ys * sin2ThetaC,
				esly  =  0.5 * ys * sin2ThetaC,
				eslxy = ys * cos2ThetaC;

            // Correct strains if gamma is negative
            if (AverageStrains.GammaXY < 0)
            {
                eslx  = -eslx;
                esly  = -esly;
                eslxy = -eslxy;
            }

            return new StrainState(eslx, esly, eslxy);
		}

		/// <summary>
		/// Calculate crack slip strains by stress-based approach (Walraven, 1980).
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private double StressCrackSlip(double vci)
		{
			if (vci.ApproxZero())
				return 0;

			// Get concrete principal tensile strain and strength
			double fc = Concrete.fc;

			// Calculate crack spacings and width
			double s = CrackSpacing();

			// Calculate crack width
			double w = CrackOpening();

			// Calculate shear slip strain by stress-based approach
			double
				a  = Math.Max(0.234 * w.Pow(-0.707) - 0.2, 0),
				ds = vci / (1.8 * w.Pow(-0.8) + a * fc);

            //Console.Write("\n" + (ds/s) + "\n");

            return
				ds / s;
		}

		/// <summary>
		/// Calculate crack slip strains by stress-based approach (Okamura and Maekawa)
		/// </summary>
		/// <param name="vci">Shear stress on crack surface, in MPa.</param>
		private double StressCrackSlip2(double vci)
		{
			if (vci.ApproxZero())
				return 0;

			// Get concrete principal tensile strain and strength
			double ec1 = Concrete.PrincipalStrains.Epsilon1;

			// Calculate crack spacings and width
			double s = CrackSpacing();

            // Calculate crack width
            double w = ec1 * s;

			// Calculate vciMax and psi ratio
			double
				vciMax = MaximumShearOnCrack(w),
				psi    = vci / vciMax;

			// Calculate shear slip strain by stress-based approach
			double ds = 0.75 * w * (psi / (psi - 1)).Sqrt();

			return
				ds / s;
		}

		/// <summary>
		/// Calculate shear slip strain by rotation lag approach.
		/// </summary>
		private double RotationLagCrackSlip()
		{
			// Get the strains
			double
				ex  = AverageStrains.EpsilonX,
				ey  = AverageStrains.EpsilonY,
				yxy = AverageStrains.GammaXY;

			// Calculate shear slip strain by rotation lag approach
			double
				thetaIc = _thetaIc ?? Constants.PiOver4,
				dThetaE = AveragePrincipalStrains.Theta1 - thetaIc;

			// Get theta L
			double thetaL = Reinforcement is null || !Reinforcement.XYReinforced
				? 10.ToRadian()
				: Reinforcement.XYReinforced
                    ? 5.ToRadian()
					: 7.5.ToRadian();

			// Correct thetaL if dThetaE < 0
			if (dThetaE < 0)
				thetaL = -thetaL;

			// Get dTheta s
			double dThetaS = dThetaE.Abs() > thetaL.Abs()
				? dThetaE - thetaL 
				: dThetaE;

			double
				thetaS = thetaIc + dThetaS;

			var (cos2ThetaS, sin2ThetaS) = (2 * thetaS).DirectionCosines();

			return
				(yxy * cos2ThetaS + (ey - ex) * sin2ThetaS).Abs();
		}

        /// <summary>
        /// Compare two <see cref="DSFMMembrane"/> objects.
        /// <para>Returns true if <see cref="Membrane.Concrete"/> and <see cref="Membrane.Concrete"/> are equal.</para>
        /// </summary>
        /// <param name="other">The other <see cref="DSFMMembrane"/> object.</param>
        public virtual bool Equals(DSFMMembrane other) => !(other is null) && base.Equals(other);

		public override bool Equals(object obj) => obj is DSFMMembrane other && base.Equals(other);

		public override int GetHashCode() => base.GetHashCode();

    }
}