using MathNet.Numerics.LinearAlgebra;
using Material.Concrete;
using Parameters    = Material.Concrete.Parameters;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;

namespace RCMembrane
{
	/// <summary>
    /// MCFT class, based on formulation by Vecchio and Collins (1986).
    /// </summary>
	public class MCFTMembrane : Membrane
	{
		///<inheritdoc/>
		/// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
		public MCFTMembrane(BiaxialConcrete concrete, Reinforcement reinforcement, double width) : base(concrete, reinforcement, width)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new BiaxialConcrete(fc, phiAg);
		}

		///<inheritdoc/>
		/// <summary>
		/// Membrane element for MCFT analysis.
		/// </summary>
		public MCFTMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, Reinforcement reinforcement, double width) : base(concreteParameters, concreteConstitutive, reinforcement, width)
		{
			// Initiate new concrete
			Concrete = new BiaxialConcrete(concreteParameters, concreteConstitutive);
		}

		/// <summary>
		/// Calculate stresses and the membrane stiffness by MCFT, given strains.
		/// </summary>
		/// <param name="appliedStrains">Current strains.</param>
		/// <param name="loadStep">Current load step.</param>
		/// <param name="iteration">Current iteration.</param>
		public override void Calculate(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0)
		{
			Strains = appliedStrains;

			// Calculate and set concrete and steel stresses
			Concrete.CalculatePrincipalStresses(Strains);
			Reinforcement.CalculateStresses(Strains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			CrackCheck();

			// Calculate stiffness
			Concrete.CalculateStiffness();
			Reinforcement.CalculateStiffness();
		}
	}
}