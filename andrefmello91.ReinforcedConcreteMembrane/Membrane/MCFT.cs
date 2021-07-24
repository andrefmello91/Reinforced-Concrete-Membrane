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
	///     MCFT class, based on formulation by Vecchio and Collins (1986).
	/// </summary>
	internal class MCFTMembrane : Membrane
	{

		#region Properties

		/// <inheritdoc />
		public override PrincipalStrainState AveragePrincipalStrains => Concrete.PrincipalStrains;

		#endregion

		#region Constructors

		/// <inheritdoc cref="MCFTMembrane(IParameters, WebReinforcement, Length)" />
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="width" /></param>
		internal MCFTMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, double width, LengthUnit unit = LengthUnit.Millimeter)
			: this(concreteParameters, reinforcement, (Length) width.As(unit))
		{
		}

		/// <summary>
		///     Membrane element for MCFT analysis.
		/// </summary>
		/// <inheritdoc cref="Membrane(IParameters, WebReinforcement?, Length, ConstitutiveModel)" />
		internal MCFTMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, Length width)
			: base(concreteParameters, reinforcement, width, ConstitutiveModel.MCFT)
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
			AverageStrains = appliedStrains.Clone();

			// Calculate and set concrete and steel stresses
			Concrete.Calculate(AverageStrains, Reinforcement);
			Reinforcement?.Calculate(AverageStrains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			CrackCheck();
		}

		/// <inheritdoc />
		public override Membrane Clone() => new MCFTMembrane(Concrete.Parameters, Reinforcement?.Clone(), Width);

		#region Object override

		/// <inheritdoc />
		public override bool Equals(object? obj) => obj is MCFTMembrane other && base.Equals(other);

		/// <inheritdoc />
		public override int GetHashCode() => base.GetHashCode();

		#endregion

		#endregion

	}
}