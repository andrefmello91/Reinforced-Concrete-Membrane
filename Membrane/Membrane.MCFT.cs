using MathNet.Numerics.LinearAlgebra;
using Material;
using Parameters    = Material.Concrete.Parameters;
using Behavior      = Material.Concrete.Behavior;
using Concrete      = Material.Concrete.Biaxial;
using Reinforcement = Material.Reinforcement.Biaxial;

namespace RCMembrane
{
	public class MCFT : Membrane
	{
		// Constructor
		public MCFT(Concrete concrete, Reinforcement reinforcement, double panelWidth) : base(concrete,
			reinforcement, panelWidth)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new Concrete.Biaxial(fc, phiAg);
		}

		public MCFT(Parameters concreteParameters, Behavior concreteBehavior, Reinforcement reinforcement, double panelWidth) : base(concreteParameters, concreteBehavior, reinforcement, panelWidth)
		{
			// Initiate new concrete
			Concrete = new Concrete(concreteParameters, concreteBehavior);
		}

		// Do analysis by MCFT with applied strains
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