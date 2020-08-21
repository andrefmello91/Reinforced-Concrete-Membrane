using System;
using Material.Concrete;
using Material.Reinforcement;
using MathNet.Numerics.Data.Text;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;
using MathNet.Numerics.LinearAlgebra;

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
		    var membrane = PanelExamples.PV10(ConstitutiveModel.DSFM);

		    // Initiate stresses
		    var sigma = 5 * Vector<double>.Build.DenseOfArray(new double[]
		    {
			    0, 0, 1
		    });

		    // Solve
		    Solver(membrane, sigma);

		    Console.WriteLine("Done! Press any key to exit.");
		    Console.ReadKey();
	    }

        /// <summary>
        /// Membrane solver for known stresses.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="stresses">Applied stresses, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 1000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        private static void Solver(Membrane membrane, Vector<double> stresses, int numLoadSteps = 100, int maxIterations = 1000, double tolerance = 1E-3)
        {
            // Get initial stresses
            var f0 = (double)1 / numLoadSteps * stresses;

            // Calculate initial stiffness
            var D = membrane.InitialStiffness();

            // Calculate e0
            var ei = D.Solve(f0);

            // Initiate matrices
            var epsMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var e1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);
            var sigMatrix = Matrix<double>.Build.Dense(numLoadSteps, 3);
            var sig1Matrix = Matrix<double>.Build.Dense(numLoadSteps, 2);

            //int[] itUpdate = new int[200];
            //for (int i = 0; i < itUpdate.Length; i++)
            //	itUpdate[i] = 50 * i;
            membrane.Stop = (false, string.Empty);

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
                        var (fc1, fc2) = membrane.Concrete.PrincipalStresses;
                        var (ec1, ec2) = membrane.Concrete.PrincipalStrains;

                        // Set results
                        epsMatrix.SetRow(ls - 1, membrane.AverageStrains);
                        sigMatrix.SetRow(ls - 1, membrane.StressesState);
                        e1Matrix.SetRow(ls - 1, new[] { ec1, ec2 });
                        sig1Matrix.SetRow(ls - 1, new[] { fc1, fc2 });

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

            // Result matrices
            var sigXeps = Matrix<double>.Build.DenseOfColumnVectors(
                epsMatrix.Column(2), sigMatrix.Column(2), Vector<double>.Build.Dense(numLoadSteps),
                e1Matrix.Column(0), sig1Matrix.Column(0), Vector<double>.Build.Dense(numLoadSteps),
                e1Matrix.Column(1), sig1Matrix.Column(1));

            string[]
                res = { "Gamma", "Tau", "", "ec1", "fc1", "", "ec2", "fc2" };

            // Save results
            DelimitedWriter.Write("D:/sig x eps.csv", sigXeps, ";", res, null, null, 0);
        }

        /// <summary>
        /// Calculate stress convergence.
        /// </summary>
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        private static double Convergence(Vector<double> residualStresses, Vector<double> appliedStresses)
        {
            double
                num = 0,
                den = 1;

            for (int i = 0; i < residualStresses.Count; i++)
            {
                num += residualStresses[i] * residualStresses[i];
                den += appliedStresses[i] * appliedStresses[i];
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
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 10).</param>
        private static bool ConvergenceReached(Vector<double> residualStresses, Vector<double> appliedStresses, double tolerance,
            int iteration, int minIterations = 10) => ConvergenceReached(Convergence(residualStresses, appliedStresses), tolerance, iteration, minIterations);

        /// <summary>
        /// Calculate residual stresses, in MPa.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Known applied stresses, in MPa.</param>
        /// <returns></returns>
        private static Vector<double> ResidualStresses(Membrane membrane, Vector<double> appliedStresses)
        {
	        if (membrane is DSFMMembrane dsfmMembrane && dsfmMembrane.ConsiderCrackSlip)
		        return
			        appliedStresses + dsfmMembrane.PseudoPrestress - dsfmMembrane.StressesState;

	        return
		        appliedStresses - membrane.StressesState;
        }

        /// <summary>
        /// Calculate the strain increment for next iteration.
        /// </summary>
        /// <param name="stiffness">Current stiffness.</param>
        /// <param name="residualStresses">Residual stresses, in MPa.</param>
        private static Vector<double> StrainIncrement(Matrix<double> stiffness, Vector<double> residualStresses)
        {
            return
                stiffness.Solve(residualStresses);
        }
    }
}
