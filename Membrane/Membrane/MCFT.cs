using MathNet.Numerics.LinearAlgebra;
using Material.Concrete;
using Material.Concrete.Biaxial;
using Material.Reinforcement;
using Material.Reinforcement.Biaxial;
using OnPlaneComponents;
using UnitsNet;
using Parameters    = Material.Concrete.Parameters;

namespace RCMembrane
{
	/// <summary>
    /// MCFT class, based on formulation by Vecchio and Collins (1986).
    /// </summary>
	public class MCFTMembrane : Membrane
	{
		///<inheritdoc/>
		public override StrainState ConcreteStrains => AverageStrains;

		/// <inheritdoc/>
		public override PrincipalStrainState AveragePrincipalStrains => Concrete.PrincipalStrains;

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        ///<inheritdoc/>
        public MCFTMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, double width)
	        : this(concrete.Parameters, reinforcement, width)
        {
        }

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        ///<inheritdoc/>
        public MCFTMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, Length width) 
	        : this (concrete.Parameters, reinforcement, width)
        {
        }

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        ///<inheritdoc/>
		public MCFTMembrane(Parameters concreteParameters, WebReinforcement reinforcement, double width)
	        : this(concreteParameters, reinforcement, Length.FromMillimeters(width))
		{
		}

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section.</param>
        public MCFTMembrane(Parameters concreteParameters, WebReinforcement reinforcement, Length width)
	        : base(concreteParameters, reinforcement, width, ConstitutiveModel.MCFT)
        {
        }

        /// <summary>
        /// Calculate <see cref="StressState"/> and <see cref="Membrane.Stiffness"/> by MCFT, given a known <see cref="StrainState"/>.
        /// </summary>
        /// <param name="appliedStrains">Current <see cref="StrainState"/>.</param>
        public override void Calculate(StrainState appliedStrains)
		{
			AverageStrains = appliedStrains.Copy();

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(ConcreteStrains, Reinforcement);
			Reinforcement.CalculateStresses(AverageStrains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			CrackCheck();
		}

        /// <summary>
        /// Compare two <see cref="MCFTMembrane"/> objects.
        /// <para>Returns true if <see cref="Membrane.Concrete"/> and <see cref="Membrane.Reinforcement"/> are equal.</para>
        /// </summary>
        /// <param name="other">The other <see cref="MCFTMembrane"/> object.</param>
        public virtual bool Equals(MCFTMembrane other) => !(other is null) && base.Equals(other);

        public override bool Equals(object obj) => obj is MCFTMembrane other && base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
	}
}