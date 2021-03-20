using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents.Strain;
using andrefmello91.OnPlaneComponents.Stress;
using UnitsNet;
using UnitsNet.Units;

#nullable enable

namespace andrefmello91.ReinforcedConcreteMembrane
{
	/// <summary>
	///     MCFT class, based on formulation by Vecchio and Collins (1986).
	/// </summary>
	public class MCFTMembrane : Membrane
	{
		#region Properties

		/// <inheritdoc />
		public override PrincipalStrainState AveragePrincipalStrains => Concrete.PrincipalStrains;

		/// <inheritdoc />
		public override StrainState ConcreteStrains => AverageStrains;

		#endregion

		#region Constructors

		/// <inheritdoc cref="MCFTMembrane(IParameters, WebReinforcement, Length)" />
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="width" /></param>
		public MCFTMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, double width, LengthUnit unit = LengthUnit.Millimeter)
			: this(concreteParameters, reinforcement, Length.From(width, unit))
		{
		}

		/// <summary>
		///     Membrane element for MCFT analysis.
		/// </summary>
		/// <inheritdoc cref="Membrane(IParameters, WebReinforcement?, Length, ConstitutiveModel)" />
		public MCFTMembrane(IParameters concreteParameters, WebReinforcement? reinforcement, Length width)
			: base(concreteParameters, reinforcement, width, ConstitutiveModel.MCFT)
		{
		}

		#endregion

		#region  Methods

		/// <summary>
		///     Calculate <see cref="StressState" /> and <see cref="Membrane.Stiffness" /> by MCFT, given a known
		///     <see cref="StrainState" />.
		/// </summary>
		/// <param name="appliedStrains">Current <see cref="StrainState" />.</param>
		public override void Calculate(StrainState appliedStrains)
		{
			AverageStrains = appliedStrains.Clone();

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(ConcreteStrains, Reinforcement);
			Reinforcement?.CalculateStresses(AverageStrains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			CrackCheck();
		}

		public override Membrane Clone() => new MCFTMembrane(Concrete.Parameters, Reinforcement?.Clone(), Width);

		/// <summary>
		///     Compare two <see cref="MCFTMembrane" /> objects.
		///     <para>Returns true if <see cref="Membrane.Concrete" /> and <see cref="Membrane.Reinforcement" /> are equal.</para>
		/// </summary>
		/// <param name="other">The other <see cref="MCFTMembrane" /> object.</param>
		public virtual bool Equals(MCFTMembrane other) => !(other is null) && base.Equals(other);

		public override bool Equals(object obj) => obj is MCFTMembrane other && base.Equals(other);

		public override int GetHashCode() => base.GetHashCode();

		#endregion
	}
}