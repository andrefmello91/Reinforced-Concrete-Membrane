using System;
using System.IO;
using Material.Concrete;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using OnPlaneComponents;

namespace RCMembrane
{
	/// <summary>
    /// Simple solver class.
    /// </summary>
    public static class MembraneSolver
    {
		/// <summary>
        /// Simple console solver, with example panel element.
        /// </summary>
	    public static void Solve()
	    {
		    // Initiate the membrane
		    var membrane = PanelExamples.PV30(ConstitutiveModel.DSFM);

		    // Initiate stresses
		    var sigma = new StressState(10, 0, 0);

		    // Solve
		    Solver(membrane, sigma);

		    Console.WriteLine("Done! Press any key to exit.");
		    System.Diagnostics.Process.Start("D:/sig x eps.csv");
		    Console.ReadKey();
	    }

        /// <summary>
        /// Membrane solver for known <see cref="StressState"/>.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 1000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        private static void Solver(Membrane membrane, StressState appliedStresses, int numLoadSteps = 100, int maxIterations = 1000, double tolerance = 1E-3)
        {
            // Get initial stresses
            var f0 = (double)1 / numLoadSteps * appliedStresses;

            // Calculate initial stiffness
            var D = membrane.InitialStiffness();

            // Calculate e0
            var ei = StrainState.FromStresses(f0, D);

            // Initiate matrices
            var epsMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var e1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);
            var sigMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var sig1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);

            //int[] itUpdate = new int[200];
            //for (int i = 0; i < itUpdate.Length; i++)
            //	itUpdate[i] = 50 * i;
            membrane.Stop = (false, string.Empty);

			// Get readers for result
            string[]
	            res = { "Gamma", "Tau", "", "ec1", "fc1", "", "ec2", "fc2" };

            // Initiate load steps
            for (int ls = 1; ls <= numLoadSteps; ls++)
            {
                // Calculate stresses
                var fi = ls * f0;

                for (int it = 1; it <= maxIterations; it++)
                {
                    // Do analysis
                    membrane.Calculate(ei, ls, it);

                    // Calculate residual
                    var fr = ResidualStresses(membrane, fi);

                    // Calculate convergence
                    double conv = Convergence(fr, fi);

                    if (ConvergenceReached(conv, tolerance, it))
                    {
                        Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

                        // Update stiffness
                        D = membrane.Stiffness;

                        // Get stresses
                        double
	                        fc1 = membrane.Concrete.PrincipalStresses.Sigma1,
	                        fc2 = membrane.Concrete.PrincipalStresses.Sigma2,
	                        ec1 = membrane.Concrete.PrincipalStrains.Epsilon1,
	                        ec2 = membrane.Concrete.PrincipalStrains.Epsilon2;

                        // Set results
                        epsMatrix.SetRow(ls - 1, membrane.AverageStrains.Vector);
                        sigMatrix.SetRow(ls - 1, membrane.AverageStresses.Vector);
                        e1Matrix.SetRow(ls - 1, new[] { ec1, ec2 });
                        sig1Matrix.SetRow(ls - 1, new[] { fc1, fc2 });

                        // Result matrices
                        var sigXeps = Matrix<double>.Build.DenseOfColumnVectors(
	                        epsMatrix.Column(2), sigMatrix.Column(2), Vector<double>.Build.Dense(numLoadSteps),
	                        e1Matrix.Column(0), sig1Matrix.Column(0), Vector<double>.Build.Dense(numLoadSteps),
	                        e1Matrix.Column(1), sig1Matrix.Column(1));


                        // Save results
                        DelimitedWriter.Write("D:/sig x eps.csv", sigXeps, ";", res, null, null, 0);

                        break;
                    }

                    // Increment strains
                    ei += StrainIncrement(D, fr);

                    if (it == maxIterations)
                    {
	                    membrane.Stop = (true, "CONVERGENCE NOT REACHED");
                        Console.WriteLine("LS = {0}, {1}", ls, membrane.Stop.Message);
                    }
                }

                if (membrane.Stop.S)
                    break;
            }

        }

        /// <summary>
        /// Calculate stress convergence.
        /// </summary>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        private static double Convergence(StressState residualStresses, StressState appliedStresses)
        {
	        Vector<double>
		        res = residualStresses.Vector,
		        app = appliedStresses.Vector;

            double
                num = 0,
                den = 1;

            for (int i = 0; i < res.Count; i++)
            {
                num += res[i] * res[i];
                den += app[i] * app[i];
            }

            return
                num / den;
        }

        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="convergence">Calculated convergence.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 10).</param>
        private static bool ConvergenceReached(double convergence, double tolerance, int iteration, int minIterations = 10) => convergence <= tolerance && iteration >= minIterations;

        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 10).</param>
        private static bool ConvergenceReached(StressState residualStresses, StressState appliedStresses, double tolerance,
            int iteration, int minIterations = 10) => ConvergenceReached(Convergence(residualStresses, appliedStresses), tolerance, iteration, minIterations);

        /// <summary>
        /// Calculate residual <see cref="StressState"/>, in MPa.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        /// <returns></returns>
        private static StressState ResidualStresses(Membrane membrane, StressState appliedStresses) => appliedStresses - membrane.AverageStresses;

        /// <summary>
        /// Calculate the strain increment for next iteration.
        /// </summary>
        /// <param name="stiffness">Current stiffness <see cref="Matrix"/>.</param>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        private static StrainState StrainIncrement(Matrix<double> stiffness, StressState residualStresses) => StrainState.FromStresses(residualStresses, stiffness);
    }
}
