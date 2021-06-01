using andrefmello91.Extensions;
using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents;
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
		/// <param name="appliedStrains">Current <see cref="StrainState" />.</param>
		public override void Calculate(StrainState appliedStrains)
		{
			AverageStrains          = appliedStrains.Clone();
			AveragePrincipalStrains = appliedStrains.ToPrincipal();
			
			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(AverageStrains, Reinforcement);
			Reinforcement?.CalculateStresses(AverageStrains);
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