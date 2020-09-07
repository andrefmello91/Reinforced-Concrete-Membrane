using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using OnPlaneComponents;

namespace RCMembrane
{
    public partial class MembraneSolver
    {
	    /// <summary>
	    /// The secant stiffness <see cref="Matrix"/> of current iteration
	    /// </summary>
	    private static Matrix<double> _currentStiffness;

	    /// <summary>
	    /// The secant stiffness <see cref="Matrix"/> of last iteration
	    /// </summary>
	    private static Matrix<double> _lastStiffness;

	    /// <summary>
	    /// The <see cref="StrainState"/> of current iteration
	    /// </summary>
	    private static StrainState _currentStrains;

	    /// <summary>
	    /// The <see cref="StrainState"/> of last iteration
	    /// </summary>
	    private static StrainState _lastStrains;

	    /// <summary>
	    /// The residual <see cref="StressState"/> of current iteration
	    /// </summary>
	    private static StressState _currentResidual;

	    /// <summary>
	    /// The residual <see cref="StressState"/> of last iteration
	    /// </summary>
	    private static StressState _lastResidual;

        /// <summary>
        /// Membrane solver for known <see cref="StressState"/>.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="fileName">The name of saved result file. <para>See: <seealso cref="ResultFileName"/>, <seealso cref="OutputResults"/>.</para></param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 10000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-6).</param>
        private static void SecantSolver(Membrane membrane, StressState appliedStresses, out string fileName, int numLoadSteps = 100, int maxIterations = 10000, double tolerance = 1E-6)
        {
			AnalysisStart(membrane);

            // Get initial stresses
            var f0 = (double)1 / numLoadSteps * appliedStresses;

            // Calculate initial stiffness
            _currentStiffness = membrane.InitialStiffness;

            // Calculate initial strains
            _currentStrains = StrainState.FromStresses(f0, _currentStiffness);

			// Initiate solution values
			_lastStiffness    = _currentStiffness;
			_lastStrains      = StrainState.Zero;
			_lastResidual     = StressState.Zero;
			_currentResidual  = StressState.Zero;

            // Initiate output matrices
            _strainOutput = new StrainState[numLoadSteps];
            _stressOutput = new StressState[numLoadSteps];
            _principalStressOutput = new PrincipalStressState[numLoadSteps];

            _concretePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];
            _averagePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];

            // Auxiliary verifiers
            membrane.Stop = (false, string.Empty);
            bool cracked = false;

            // Initiate load steps
            int ls;
            for (ls = 1; ls <= numLoadSteps; ls++)
            {
                // Calculate stresses
                var fi = ls * f0;

                for (int it = 1; it <= maxIterations; it++)
                {
                    // Do analysis
                    membrane.Calculate(_currentStrains, ls, it);

                    if (!cracked && membrane.Concrete.Cracked)
                    {
                        cracked = true;
                        Console.WriteLine("Concrete cracked at step {0}", ls);
                    }

					// Calculate and update residual
					ResidualUpdate(membrane, fi);

                    // Calculate convergence
                    double conv = Convergence(_currentResidual, fi);

                    if (ConvergenceReached(conv, tolerance, it))
                    {
	                    if (membrane is DSFMMembrane dsfm && membrane.Concrete.Cracked)
	                    {
		                    Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", ls, it, dsfm.SlipApproach);

	                    }
	                    else
		                    Console.WriteLine("LS = {0}, Iterations = {1}", ls, it);

                        // Set results on output matrices
                        SaveResults(membrane, fi, ls);

                        break;
                    }
					
                    if (it == maxIterations)
                    {
                        membrane.Stop = (true, "CONVERGENCE NOT REACHED");
                        Console.WriteLine("LS = {0}, {1}", ls, membrane.Stop.Message);
                    }

                    else
                    {
	                    // Update stiffness and strains
	                    SecantStiffnessUpdate();
	                    StrainUpdate(membrane);
                    }
                }

                if (membrane.Stop.S)
                {
	                // Decrement ls to correct output file
	                ls--;
	                break;
                }
            }

            // Output results
            OutputResults(membrane, out fileName, ls);
			AnalysisDone(membrane);
        }

		/// <summary>
        /// Calculate the secant stiffness <see cref="Matrix"/> of current iteration.
        /// </summary>
        private static void SecantStiffnessUpdate()
        {
			// Get current stiffness
			Matrix<double> kCur = _currentStiffness;

			// Calculate the variation of strains and residual as vectors
			Vector<double>
				dStrain = (_currentStrains  - _lastStrains).AsVector(),
				dRes    = (_currentResidual - _lastResidual).AsVector();

			// Increment current stiffness
			var dK = ((dRes - _lastStiffness * dStrain) / dStrain.Norm(2)).ToColumnMatrix() * dStrain.ToRowMatrix();

			// Set new values
			_currentStiffness = _lastStiffness + dK;
			_lastStiffness    = kCur;
        }

        /// <summary>
        /// Update residual <see cref="StressState"/>.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
		private static void ResidualUpdate(Membrane membrane, StressState appliedStresses)
		{
			// Set new values
            _lastResidual = _currentResidual;
			_currentResidual = membrane.AverageStresses - appliedStresses;
        }

        /// <summary>
        /// Update strains.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        private static void StrainUpdate(Membrane membrane)
        {
			// Get current strains
			var eCur = _currentStrains;

			// Get residual
			var res = _currentResidual;

            // Correct residual if DSFM
            //if (membrane is DSFMMembrane dsfm)
            //    res -= dsfm.PseudoStresses;

            // Increment strains
            var inc = StrainState.FromStresses(-res, _currentStiffness);
			_currentStrains  += inc;

			// Set last strains
			_lastStrains = eCur;
        }
    }
}
