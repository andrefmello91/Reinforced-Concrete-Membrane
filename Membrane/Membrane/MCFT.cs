﻿using MathNet.Numerics.LinearAlgebra;
using Material.Concrete;
using Material.Reinforcement;
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
        public MCFTMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, double width) : base(concrete, reinforcement, width)
		{
		}

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        ///<inheritdoc/>
        public MCFTMembrane(BiaxialConcrete concrete, WebReinforcement reinforcement, Length width) : base (concrete.Parameters, concrete.Constitutive, reinforcement, width)
        {
        }

        /// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
        ///<inheritdoc/>
		public MCFTMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, WebReinforcement reinforcement, double width) : base(concreteParameters, concreteConstitutive, reinforcement, width)
		{
		}

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="concreteConstitutive">Concrete <see cref="Constitutive"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section.</param>
        public MCFTMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, WebReinforcement reinforcement, Length width) : base(concreteParameters, concreteConstitutive, reinforcement, width)
        {
        }

        /// <summary>
        /// Calculate <see cref="StressState"/> and <see cref="Membrane.Stiffness"/> by MCFT, given a known <see cref="StrainState"/>.
        /// </summary>
        /// <param name="appliedStrains">Current <see cref="StrainState"/>.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="iteration">Current iteration.</param>
        public override void Calculate(StrainState appliedStrains, int loadStep = 0, int iteration = 0)
		{
			AverageStrains = appliedStrains;

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(ConcreteStrains, Reinforcement);
			Reinforcement.CalculateStresses(AverageStrains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			CrackCheck();
		}
	}
}