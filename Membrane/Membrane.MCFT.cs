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

			// Initiate stiffness
			ConcreteStiffness      = InitialConcreteStiffness();
			ReinforcementStiffness = InitialReinforcementStiffness();
		}

		// Get concrete strains
		public override Vector<double> ConcreteStrains => Strains;

		// Do analysis by MCFT with applied strains
		public override void Analysis(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0)
		{
			Strains = appliedStrains;

			// Calculate new principal strains
			var (ec1, ec2)  = PrincipalStrains(Strains);
			PrincipalAngles = StrainAngles(Strains, (ec1, ec2));

			// Calculate and set concrete and steel stresses
			Concrete.SetStrainsAndStresses((ec1, ec2));
			Reinforcement.SetStrainsAndStresses(Strains);

			// Verify if concrete is cracked and check crack stresses to limit fc1
			if (Concrete.Cracked)
				CrackCheck();

			// Set results
			ConcreteStiffness      = Concrete_Stiffness();
			ReinforcementStiffness = Reinforcement_Stiffness();
			ConcreteStresses       = Concrete_Stresses();
			ReinforcementStresses  = Reinforcement_Stresses();
		}
	}
}