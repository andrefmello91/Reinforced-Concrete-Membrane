using MathNet.Numerics.LinearAlgebra;
using Material;
using Parameters    = Material.Concrete.Parameters;
using Behavior      = Material.Concrete.Behavior;
using Concrete      = Material.Concrete.Biaxial;
using Reinforcement = Material.Reinforcement.Biaxial;

namespace RCMembrane
{
	/// <summary>
    /// MCFT class, based on formulation by Vecchio and Collins (1986).
    /// </summary>
	public class MCFT : Membrane
	{
		///<inheritdoc/>
		/// <summary>
        /// Membrane element for MCFT analysis.
        /// </summary>
		public MCFT(Concrete concrete, Reinforcement reinforcement, double sectionWidth) : base(concrete, reinforcement, sectionWidth)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new Concrete(fc, phiAg);
		}

		///<inheritdoc/>
		/// <summary>
		/// Membrane element for MCFT analysis.
		/// </summary>
		public MCFT(Parameters concreteParameters, Behavior concreteBehavior, Reinforcement reinforcement, double sectionWidth) : base(concreteParameters, concreteBehavior, reinforcement, sectionWidth)
		{
			// Initiate new concrete
			Concrete = new Concrete(concreteParameters, concreteBehavior);
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