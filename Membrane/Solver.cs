using System;
using Material.Concrete;
using Material.Reinforcement;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;
using MathNet.Numerics.LinearAlgebra;

namespace RCMembrane
{
	/// <summary>
    /// Simple solver class.
    /// </summary>
    public static class Solver
    {
		/// <summary>
        /// Simple console solver, with example panel element.
        /// </summary>
	    public static void Solve()
	    {
		    // Initiate concrete
		    var concrete = new BiaxialConcrete(14.5, 6);

		    // Initiate steel for each direction
		    var steelX = new Steel(276, 200000);
		    var steelY = new Steel(276, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement((6.35, 4.7), (50.55, 49.57), (steelX, steelY), 70);

		    // Initiate the membrane
		    var membrane = new DSFMMembrane(concrete, reinforcement, 70);

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
