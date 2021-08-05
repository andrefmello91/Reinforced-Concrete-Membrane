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

		/// <inheritdoc cref="MCFTMembrane(IConcreteParameters, WebReinforcement, Length)" />
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="width" /></param>
		internal SMMMembrane(IConcreteParameters concreteParameters, WebReinforcement? reinforcement, double width, LengthUnit unit = LengthUnit.Millimeter)
			: this(concreteParameters, reinforcement, (Length) width.As(unit))
		{
		}

		/// <summary>
		///     Membrane element for SMM analysis.
		/// </summary>
		/// <inheritdoc cref="Membrane(IConcreteParameters, WebReinforcement?, Length, ConstitutiveModel)" />
		internal SMMMembrane(IConcreteParameters concreteParameters, WebReinforcement? reinforcement, Length width)
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
			var noPoissonStrain = SMMConcrete.RemovePoissonEffect(AveragePrincipalStrains.Transform(Concrete.DeviationAngle), Reinforcement, Concrete.Cracked);
					
			// Calculate and set concrete and steel stresses
			Reinforcement?.Calculate(noPoissonStrain.ToHorizontal());
			Concrete.Calculate(AverageStrains, Reinforcement);
			
			CrackCheck();
			
			// ((SMMConcrete) Concrete).UpdateShearStress(AverageStresses, Reinforcement?.Stresses ?? StressState.Zero);
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