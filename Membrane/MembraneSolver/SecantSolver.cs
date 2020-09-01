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
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 1000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
        private static void SecantSolver(Membrane membrane, StressState appliedStresses, int numLoadSteps = 100, int maxIterations = 1000, double tolerance = 1E-3)
        {
            // Get initial stresses
            var f0 = (double)1 / numLoadSteps * appliedStresses;

            // Calculate initial stiffness
            var D = membrane.InitialStiffness();

            // Calculate e0
            var ei = StrainState.FromStresses(f0, D);

			// Initiate solution values
			_lastStiffness = D;
			_lastStrains   = StrainState.Zero;
			_lastResidual  = StressState.Zero;

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
                    _currentResidual = membrane.AverageStresses - fi;

					// Get current strains
                    _currentStrains  = membrane.AverageStrains;

                    // Calculate convergence
                    double conv = Convergence(_currentResidual, fi);

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

					// Calculate increment

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
        /// Calculate the secant stiffness <see cref="Matrix"/> of current iteration.
        /// </summary>
        private static Matrix<double> SecantStiffnessUpdate()
        {
			// Calculate the variation of strains and residual as vectors
			Vector<double>
				dStrain = (_currentStrains  - _lastStrains).AsVector(),
				dRes    = (_currentResidual - _lastResidual).AsVector();

			// Calculate 2-norm of strains
			double norm = dStrain.Norm(2);

			// Calculate K increment
			var dK = ((dRes - _lastStiffness * dStrain) / norm).ToColumnMatrix() * dStrain.ToRowMatrix();

			return
				_lastStiffness + dK;
        }
    }
}
