using System;
using Material;
using MathNet.Numerics.LinearAlgebra;

namespace RCMembrane
{
    public static class Solver
    {
	    public static void Solve()
	    {
		    // Initiate concrete
		    var concrete = new Concrete.Biaxial(14.5, 6);

		    // Initiate steel for each direction
		    var steelX = new Steel(276, 200000);
		    var steelY = new Steel(276, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement.Biaxial((6.35, 4.7), (50.55, 49.57), (steelX, steelY), 70);

		    // Initiate the membrane
		    var membrane = new DSFM(concrete, reinforcement, 70);

		    // Initiate stresses
		    var sigma = 5 * Vector<double>.Build.DenseOfArray(new double[]
		    {
			    0, 0, 1
		    });

		    // Solve
		    membrane.Solver(sigma);

		    Console.WriteLine("Done! Press any key to exit.");
		    Console.ReadKey();
	    }
    }
}
