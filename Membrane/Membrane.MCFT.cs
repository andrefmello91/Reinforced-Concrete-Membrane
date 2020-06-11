using MathNet.Numerics.LinearAlgebra;
using Material;
using Parameters = Material.Concrete.ModelParameters;
using Behavior   = Material.Concrete.ModelBehavior;

namespace Membrane
{
	public class MCFT : Membrane
	{
		// Constructor
		public MCFT(Concrete.Biaxial concrete, Reinforcement.Biaxial reinforcement, double panelWidth) : base(concrete,
			reinforcement, panelWidth)
		{
			// Get concrete parameters
			double
				fc    = concrete.fc,
				phiAg = concrete.AggregateDiameter;

			// Initiate new concrete
			Concrete = new Concrete.Biaxial(fc, phiAg, Parameters.MCFT, Behavior.MCFT);
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