using System;
using System.IO;
using Material.Concrete;
using MathNet.Numerics;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using OnPlaneComponents;

namespace RCMembrane
{
    public static partial class MembraneSolver
	{
        ///// <summary>
        ///// Membrane solver for known <see cref="StressState"/>.
        ///// </summary>
        ///// <param name="membrane">The membrane object.</param>
        ///// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        ///// <param name="fileName">The name of saved result file. <para>See: <seealso cref="ResultFileName"/>, <seealso cref="OutputResults"/>.</para></param>
        ///// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        ///// <param name="maxIterations">Maximum number of iterations (default: 1000).</param>
        ///// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        //private static void Solver(Membrane membrane, StressState appliedStresses, out string fileName, int numLoadSteps = 100, int maxIterations = 1000, double tolerance = 1E-3)
        //{
        //    AnalysisStart(membrane);

        //    // Get initial stresses
        //    var f0 = (double)1 / numLoadSteps * appliedStresses;

        //    // Calculate initial stiffness
        //    var D = membrane.InitialStiffness;

        //    // Calculate e0
        //    var ei = StrainState.FromStresses(f0, D);

        //    // Initiate output matrices
        //    _strainOutput = new StrainState[numLoadSteps];
        //    _stressOutput = new StressState[numLoadSteps];
        //    _principalStressOutput = new PrincipalStressState[numLoadSteps];

        //    _concretePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];
        //    _averagePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];

        //    // Auxiliary verifiers
        //    membrane.Stop = (false, string.Empty);
        //    bool cracked = false;

        //    // Initiate load steps
        //    int ls;
        //    for (ls = 1; ls <= numLoadSteps; ls++)
        //    {
        //        // Calculate stresses
        //        var fi = ls * f0;

        //        for (int it = 1; it <= maxIterations; it++)
        //        {
        //            // Do analysis
        //            membrane.Calculate(ei, ls, it);

        //            if (!cracked && membrane.Concrete.Cracked)
        //            {
        //                cracked = true;
        //                Console.WriteLine("Concrete cracked at step {0}", ls);
        //            }

        //            // Calculate residual
        //            var fr = ResidualStresses(membrane, fi);

        //            // Calculate convergence
        //            double conv = Convergence(fr, fi);

        //            if (ConvergenceReached(conv, tolerance, it))
        //            {
        //                if (membrane is DSFMMembrane dsfm && membrane.Concrete.Cracked)
        //                {
        //                    Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", ls, it, dsfm.SlipApproach);

        //                }
        //                else
        //                    Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

        //                // Update stiffness
        //                D = membrane.Stiffness;

        //                // Set results on output matrices
        //                SaveResults(membrane, fi, ls);

        //                break;
        //            }

        //            // Increment strains
        //            ei += StrainIncrement(D, fr);

        //            if (it == maxIterations)
        //            {
        //                membrane.Stop = (true, "CONVERGENCE NOT REACHED");
        //                Console.WriteLine("LS = {0}, {1}", ls, membrane.Stop.Message);
        //            }
        //        }

        //        if (membrane.Stop.S)
        //        {
        //            // Decrement ls to correct output file
        //            ls--;
        //            break;
        //        }
        //    }

        //    // Output results
        //    OutputResults(membrane, out fileName, ls);
        //    AnalysisDone(membrane);
        //}
		
        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 2).</param>
        private static bool ConvergenceReached(StressState residualStresses, StressState appliedStresses, double tolerance,
            int iteration, int minIterations = 2) => ConvergenceReached(Convergence(residualStresses, appliedStresses), tolerance, iteration, minIterations);

        /// <summary>
        /// Calculate residual <see cref="StressState"/>, in MPa.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        /// <returns></returns>
        private static StressState ResidualStresses(Membrane membrane, StressState appliedStresses)
        {
            //if (membrane is DSFMMembrane dsfm)
            //    appliedStresses += dsfm.PseudoPStresses();

            return
				appliedStresses - membrane.AverageStresses;
        }

        /// <summary>
        /// Calculate the strain increment for next iteration.
        /// </summary>
        /// <param name="stiffness">Current stiffness <see cref="Matrix"/>.</param>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        private static StrainState StrainIncrement(Matrix<double> stiffness, StressState residualStresses) => StrainState.FromStresses(residualStresses, stiffness);
	}
}
