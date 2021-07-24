using System;
using System.Diagnostics.CodeAnalysis;
using andrefmello91.Extensions;
using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents;
using MathNet.Numerics.RootFinding;
using UnitsNet;
using UnitsNet.Units;
#nullable enable

namespace andrefmello91.ReinforcedConcreteMembrane
{
	/// <summary>
	///     SMM class, based on formulation by Hsu and Zhu (2002).
	/// </summary>
	internal class SMMMembrane : Membrane
	{
		#region Constructors

		/// <inheritdoc cref="MCFTMembrane(IParameters, WebReinforcement, Length)" />
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="width" /></param>
		internal SMMMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, double width, LengthUnit unit = LengthUnit.Millimeter)
			: this(concreteParameters, reinforcement, (Length) width.As(unit))
		{
		}

		/// <summary>
		///     Membrane element for SMM analysis.
		/// </summary>
		/// <inheritdoc cref="Membrane(IParameters, WebReinforcement?, Length, ConstitutiveModel)" />
		internal SMMMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, Length width)
			: base(concreteParameters, reinforcement, width, ConstitutiveModel.SMM)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		///     Calculate <see cref="StressState" /> and <see cref="Membrane.Stiffness" /> by MCFT, given a known
		///     <see cref="StrainState" />.
		/// </summary>
		/// <inheritdoc />
		public override void Calculate(StrainState appliedStrains)
		{
			AverageStrains          = appliedStrains.Clone();
			AveragePrincipalStrains = appliedStrains.ToPrincipal();
			
			// Remove Poisson effect
			var noPoissonStrain = RemovePoissonEffect(AveragePrincipalStrains.Transform(Concrete.DeviationAngle), Reinforcement, Concrete.Cracked);
					
			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(noPoissonStrain, Reinforcement);
			Reinforcement?.Calculate(noPoissonStrain.ToHorizontal());
		}

		// private void Solve(StressState appliedStresses)
		// {
		// 	var strains = AveragePrincipalStrains.Transform(Concrete.DeviationAngle);
		//
		// 	if (!Concrete.Cracked)
		// 	{
		// 		// Remove Poisson effect
		// 		var noPoissonStrain = RemovePoissonEffect(strains, Reinforcement, Concrete.Cracked);
		// 			
		// 		// Calculate and set concrete and steel stresses
		// 		Concrete.CalculatePrincipalStresses(noPoissonStrain, Reinforcement);
		// 		Reinforcement?.CalculateStresses(noPoissonStrain.ToHorizontal());
		// 		
		// 		return;
		// 	}
		// 	
		// 	// Initiate strains
		// 	var y12 = 0D;
		// 	var e1  = 0D;
		// 	
		// 	// Solve the nonlinear equation by Brent Method
		// 	if (Brent.TryFindRoot(SolveGamma, -0.1, 0.1, 0.1, 10000, out y12))
		// 	{
		// 		
		// 	}
		//
		// 	double SolveGamma(double gamma)
		// 	{
		// 		// Solve the nonlinear equation by Brent Method
		// 		if (!Brent.TryFindRoot(SolveEpsilon1, 0, 0.1, 0.1, 10000, out e1))
		// 			return 1;
		//
		// 		double SolveEpsilon1(double epsilon1)
		// 		{
		// 			strains = new StrainState(strains.EpsilonX, epsilon1, gamma, strains.ThetaX);
		// 			
		// 			// Remove Poisson effect
		// 			var noPoissonStrain = RemovePoissonEffect(strains, Reinforcement, Concrete.Cracked);
		// 			
		// 			// Calculate and set concrete and steel stresses
		// 			Concrete.CalculatePrincipalStresses(noPoissonStrain, Reinforcement);
		// 			Reinforcement?.CalculateStresses(noPoissonStrain.ToHorizontal());
		// 			
		// 			// Calculate summed reinforcement stresses
		// 			// From constitutive model
		// 			var eq11 = (Reinforcement?.Stresses.SigmaX + Reinforcement?.Stresses.SigmaY) ?? Pressure.Zero;
		// 			
		// 			// From equilibrium
		// 			var eq12 = appliedStresses.SigmaX + appliedStresses.SigmaY - Concrete.Stresses.SigmaX - Concrete.Stresses.SigmaY;
		// 			
		// 			// Subtraction must be zero
		// 			return
		// 				(eq12 - eq11).Megapascals;
		// 		}
		// 		
		// 		// Calculate subtracted reinforcement stresses
		// 		// From constitutive model
		// 		var eq21 = (Reinforcement?.Stresses.SigmaX - Reinforcement?.Stresses.SigmaY) ?? Pressure.Zero;
		// 			
		// 		// From equilibrium
		// 		var (cos, sin) = (2 * Concrete.Stresses.ThetaX).DirectionCosines();
		// 		var eq22       = appliedStresses.SigmaX - appliedStresses.SigmaY - (Concrete.Stresses.SigmaX - Concrete.Stresses.SigmaY) * cos + 2 * Concrete.Stresses.TauXY * sin;
		//
		// 		return
		// 			(eq22 - eq21).Megapascals;
		// 	}
		// }
		
		/// <summary>
		///		Calculate the strain state affected by Poisson ratios.
		/// </summary>
		/// <param name="strainsAtAvgPrincipal">The strain state in concrete, at the average principal strain direction of the membrane element.</param>
		/// <param name="reinforcement">The reinforcement.</param>
		/// <param name="cracked">The cracked state of concrete. True if cracked.</param>
		/// <returns>
		///		The <see cref="StrainState"/> without Poisson effect.
		/// </returns>
		private static StrainState RemovePoissonEffect(StrainState strainsAtAvgPrincipal, WebReinforcement? reinforcement, bool cracked)
		{
			// Get initial strains
			var e1i = strainsAtAvgPrincipal.EpsilonX;
			var e2i = strainsAtAvgPrincipal.EpsilonY;
			
			// Get coefficients
			var (v12, v21) = PoissonCoefficients(reinforcement, cracked);
			
			// Calculate strains
			var v1 = 1D / (1D - v12 * v21);
			var v2 = v21 * v1;

			var e1 = v1 * e1i + v2 * e2i;
			var e2 = v2 * e1i + v1 * e2i;

			return new StrainState(e1, e2, strainsAtAvgPrincipal.GammaXY, strainsAtAvgPrincipal.ThetaX);
		}

		/// <summary>
		///		Calculate the Poisson coefficients for SMM.
		/// </summary>
		/// <param name="reinforcement">The reinforcement.</param>
		/// <param name="cracked">The cracked state of concrete. True if cracked.</param>
		private static (double v12, double v21) PoissonCoefficients(WebReinforcement? reinforcement, bool cracked)
		{
			var v21 = cracked
				? 0
				: 0.2;

			if (reinforcement is null)
				return (0.2, v21);

			var strains = reinforcement.Strains;
				
			var esf = Math.Max(strains.EpsilonX, strains.EpsilonY);
				
			var ey = strains.EpsilonX >= strains.EpsilonY
				? reinforcement.DirectionX?.Steel.YieldStrain
				: reinforcement.DirectionY?.Steel.YieldStrain;

			var v12 = esf <= 0 || !ey.HasValue
				? 0.2
				: 0.2 + 850 * esf;

			return (v12, v21);
		}

		/// <inheritdoc />
		public override Membrane Clone() => new SMMMembrane(Concrete.Parameters, Reinforcement?.Clone(), Width);

		#region Object override

		/// <inheritdoc />
		public override bool Equals(object? obj) => obj is SMMMembrane other && base.Equals(other);

		/// <inheritdoc />
		public override int GetHashCode() => base.GetHashCode();

		#endregion

		#endregion

	}
}