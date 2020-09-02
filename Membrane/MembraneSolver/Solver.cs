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
	/// <summary>
    /// Simple solver class.
    /// </summary>
    public static partial class MembraneSolver
	{
		/// <summary>
        /// <see cref="Array"/> of <see cref="StrainState"/> results for each load step.
        /// </summary>
		private static StrainState[] _strainOutput;

        /// <summary>
        /// <see cref="Array"/> of concrete <see cref="PrincipalStrainState"/> results for each load step.
        /// </summary>
        private static PrincipalStrainState[] _concretePrincipalStrainOutput;

        /// <summary>
        /// <see cref="Array"/> of average <see cref="PrincipalStrainState"/> results for each load step.
        /// </summary>
        private static PrincipalStrainState[] _averagePrincipalStrainOutput;

        /// <summary>
        /// <see cref="Array"/> of <see cref="StressState"/> results for each load step.
        /// </summary>
        private static StressState[] _stressOutput;

        /// <summary>
        /// <see cref="Array"/> of <see cref="PrincipalStressState"/> results for each load step.
        /// </summary>
        private static PrincipalStressState[] _principalStressOutput;

		/// <summary>
        /// The output file name and save location.
        /// </summary>
        private const string ResultFileName = "D:/membrane_result.csv";

        /// <summary>
        /// Simple console solver, with example from <see cref="PanelExamples"/>.
        /// </summary>
        public static void Solve()
	    {
		    // Initiate the membrane
		    var membrane = PanelExamples.PV10(ConstitutiveModel.DSFM);

		    // Initiate stresses
		    var sigma = new StressState(0, 0, 5);

		    // Solve
		    SecantSolver(membrane, sigma);

		    Console.WriteLine("Done! Press any key to exit.");
		    System.Diagnostics.Process.Start(ResultFileName);
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
            var D = membrane.InitialStiffness;

            // Calculate e0
            var ei = StrainState.FromStresses(f0, D);

            // Initiate output matrices
            _strainOutput          = new StrainState[numLoadSteps];
            _stressOutput          = new StressState[numLoadSteps];
            _principalStressOutput = new PrincipalStressState[numLoadSteps];

            _concretePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];
            _averagePrincipalStrainOutput  = new PrincipalStrainState[numLoadSteps];

            // Auxiliary verifiers
            membrane.Stop = (false, string.Empty);
			bool cracked  = false;

			// Initiate load steps
            for (int ls = 1; ls <= numLoadSteps; ls++)
            {
                // Calculate stresses
                var fi = ls * f0;

                for (int it = 1; it <= maxIterations; it++)
                {
                    // Do analysis
                    membrane.Calculate(ei, ls, it);

                    if (!cracked && membrane.Concrete.Cracked)
                    {
	                    cracked = true;
	                    Console.WriteLine("Concrete cracked at step {0}", ls);
                    }

                    // Calculate residual
                    var fr = ResidualStresses(membrane, fi);

                    // Calculate convergence
                    double conv = Convergence(fr, fi);

                    if (ConvergenceReached(conv, tolerance, it))
                    {
                        Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

                        if (membrane is DSFMMembrane dsfm && membrane.Concrete.Cracked)
                        {
                            Console.WriteLine(dsfm.SlipApproach);
                        }

                        // Update stiffness
                        D = membrane.Stiffness;

						// Set results on output matrices
						SaveResults(membrane, ls);

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

			// Output results
			OutputResults(numLoadSteps);
        }

        /// <summary>
        /// Calculate stress convergence.
        /// </summary>
        /// <param name="residualStresses">Residual <see cref="StressState"/>, in MPa.</param>
        /// <param name="appliedStresses">Known applied <see cref="StressState"/>, in MPa.</param>
        private static double Convergence(StressState residualStresses, StressState appliedStresses)
        {
	        Vector<double>
		        res = residualStresses.AsVector(),
		        app = appliedStresses.AsVector();

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
        /// <param name="minIterations">Minimum number of iterations (default: 2).</param>
        private static bool ConvergenceReached(double convergence, double tolerance, int iteration, int minIterations = 2) => convergence <= tolerance && iteration >= minIterations;

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

        /// <summary>
        /// Save results in output matrices.
        /// <para>See: <see cref="_strainOutput"/>, <see cref="_stressOutput"/>, <see cref="_concretePrincipalStrainOutput"/> and <see cref="_principalStressOutput"/>.</para>
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> element analyzed.</param>
        /// <param name="loadStep">The current load step.</param>
        private static void SaveResults(Membrane membrane, int loadStep)
        {
			// Get row
			int i = loadStep - 1;

			_strainOutput[i]          = membrane.AverageStrains;
			_stressOutput[i]          = membrane.AverageStresses;
			_principalStressOutput[i] = membrane.Concrete.PrincipalStresses;

			_concretePrincipalStrainOutput[i] = membrane.Concrete.PrincipalStrains;
			_averagePrincipalStrainOutput[i]  = membrane.AveragePrincipalStrains;
        }

        /// <summary>
        /// Output results in a CSV file.
        /// </summary>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        private static void OutputResults(int numLoadSteps = 100)
        {
	        // Get readers for result
	        string[]
		        outputReaders = { "ex", "ey", "exy", "", "fx", "fy", "fxy", "", "ec1", "ec2", "theta1e", "", "fc1", "fc2", "theta1f" };

            // Result matrices
            var result = Matrix.Build.Dense(numLoadSteps, outputReaders.Length);

            for (int i = 0; i < numLoadSteps; i++)
            {
				// Set stress and strain states
				result.SetSubMatrix(i,  0, _strainOutput[i].AsVector().ToRowMatrix());
				result.SetSubMatrix(i,  4, _stressOutput[i].AsVector().ToRowMatrix());
				result.SetSubMatrix(i,  8, _concretePrincipalStrainOutput[i].AsVector().ToRowMatrix());
				result.SetSubMatrix(i, 12, _principalStressOutput[i].AsVector().ToRowMatrix());

				// Set angles in degrees (average angle in strains)
				result[i, 10] = Trig.RadianToDegree(_averagePrincipalStrainOutput[i].Theta1);
				result[i, 14] = Trig.RadianToDegree(_principalStressOutput[i].Theta1);
            }

	        // Save results
	        DelimitedWriter.Write(ResultFileName, result, ";", outputReaders, null, null, 0);
        }
    }
}
