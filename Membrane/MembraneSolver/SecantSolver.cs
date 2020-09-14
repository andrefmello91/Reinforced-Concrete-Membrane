using System;
using Extensions.Number;
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
	public partial class MembraneSolver
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
        /// The step of concrete cracking.
        /// </summary>
	    private static int? _crackStep;

	    /// <summary>
	    /// Stop parameter for analysis.
	    /// </summary>
	    private static bool _stop;

        /// <summary>
        /// Simple console solver, with example from <see cref="PanelExamples"/>.
        /// </summary>
        public static void Solve()
	    {
		    // Initiate the membrane
		    var membraneMCFT = PanelExamples.PA2();
		    var membraneDSFM = PanelExamples.PA2(ConstitutiveModel.DSFM);

		    // Initiate stresses
		    var sigma = new StressState(0, 0, 8);

		    // Solve
		    SecantSolver(membraneMCFT, sigma, out string mcftFile);
		    SecantSolver(membraneDSFM, sigma, out string dsfmFile);

		    Console.WriteLine("Done! Press any key to exit.");
            System.Diagnostics.Process.Start(mcftFile);
            System.Diagnostics.Process.Start(dsfmFile);
            Console.ReadKey();
	    }

        /// <summary>
        /// Membrane solver for known <see cref="StressState"/>.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="fileName">The name of saved result file. <para>See: <seealso cref="ResultFileName"/>, <seealso cref="OutputResults"/>.</para></param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 10000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 6E-4).</param>
        private static void SecantSolver(Membrane membrane, StressState appliedStresses, out string fileName, int numLoadSteps = 100, int maxIterations = 10000, double tolerance = 4E-4)
        {
			// Initialize fields and write a starting message
            AnalysisStart(membrane, appliedStresses, numLoadSteps);

			// Analyze by steps
			StepAnalysis(membrane, appliedStresses, numLoadSteps, tolerance, maxIterations, out int finalLoadStep);

            // Output results
            OutputResults(membrane, finalLoadStep, out fileName);
			AnalysisDone(membrane);
        }

        /// <summary>
        /// Initialize auxiliary fields and write a starting message in <see cref="Console"/>.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object in analysis.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps.</param>
        private static void AnalysisStart(Membrane membrane, StressState appliedStresses, int numLoadSteps)
        {
	        // Get initial stresses
	        var f0 = (double)1 / numLoadSteps * appliedStresses;

	        // Calculate initial stiffness
	        _currentStiffness = membrane.InitialStiffness;

	        // Calculate initial strains
	        _currentStrains = StrainState.FromStresses(f0, _currentStiffness);

	        // Initiate solution values
	        _lastStiffness   = _currentStiffness.Clone();
	        _lastStrains     = StrainState.Zero;
	        _lastResidual    = StressState.Zero;
	        _currentResidual = StressState.Zero;

	        // Initiate output matrices
	        _strainOutput          = new StrainState[numLoadSteps];
	        _stressOutput          = new StressState[numLoadSteps];
	        _principalStressOutput = new PrincipalStressState[numLoadSteps];

	        _concretePrincipalStrainOutput = new PrincipalStrainState[numLoadSteps];
	        _averagePrincipalStrainOutput  = new PrincipalStrainState[numLoadSteps];

	        // Auxiliary verifiers
	        _stop      = false;
	        _crackStep = null;

	        Console.WriteLine("\nStarting {0} analysis...\n", membrane is MCFTMembrane ? "MCFT" : "DSFM");
        }

        /// <summary>
        /// Do analysis by load steps.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps to perform.</param>
        /// <param name="tolerance">The convergence tolerance.</param>
        /// <param name="maxIterations">Maximum number of iterations for each load step.</param>
        /// <param name="finalLoadStep">The last calculated load step.</param>
        private static void StepAnalysis(Membrane membrane, StressState appliedStresses, int numLoadSteps, double tolerance, int maxIterations, out int finalLoadStep)
        {
	        // Initiate load steps
	        int ls;
	        for (ls = 1; ls <= numLoadSteps; ls++)
	        {
		        // Calculate stresses
		        var fi = ls * appliedStresses;

				// Iterate to find solution
				Iterate(membrane, fi, ls, tolerance, maxIterations);

				// Solution reached:
				if (!_stop)
					continue;

				// Solution not reached
				// Decrement ls to correct output file
		        ls--;
		        break;
	        }

	        finalLoadStep = ls;
        }

        /// <summary>
        /// Iterate to find solution.
        /// </summary>
        /// <param name="membrane">The membrane object.</param>
        /// <param name="stepStresses">Applied <see cref="StressState"/> for this load step, in MPa.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="tolerance">The convergence tolerance (default: 1E-3).</param>
        /// <param name="maxIterations">Maximum number of iterations for each load step (default: 1000).</param>
        private static void Iterate(Membrane membrane, StressState stepStresses, int loadStep, double tolerance, int maxIterations)
        {
	        for (int it = 1; it <= maxIterations; it++)
	        {
		        // Do analysis
		        membrane.Calculate(_currentStrains);

		        // Verify if concrete cracks in this load step and write a message
		        ConcreteCrackedMessage(membrane, loadStep);

		        // Calculate and update residual
		        ResidualUpdate(membrane, stepStresses);

		        // Calculate convergence
		        var conv = Convergence(_currentResidual, stepStresses);

		        // If convergence is reached
		        if (ConvergenceReached(conv, tolerance, it))
		        {
			        // Write message
			        ConvergenceMessage(membrane, loadStep, it);

			        // Set results on output matrices
			        SaveResults(membrane, stepStresses, loadStep);

			        break;
		        }

		        // If reached max iterations
		        if (it == maxIterations)
		        {
			        // Write message
			        NoConvergenceMessage(loadStep);
			        break;
		        }

		        // Update stiffness and strains
		        SecantStiffnessUpdate();
		        StrainUpdate();
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
        /// <param name="tolerance">Stress convergence tolerance..</param>
        /// <param name="iteration">Current iteration.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 2).</param>
        private static bool ConvergenceReached(double convergence, double tolerance, int iteration, int minIterations = 2) => convergence <= tolerance && iteration >= minIterations;

        /// <summary>
        /// Calculate the secant stiffness <see cref="Matrix"/> of current iteration.
        /// </summary>
        private static void SecantStiffnessUpdate()
        {
			// Clone current stiffness
			var kCur = _currentStiffness.Clone();

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
            _lastResidual    = _currentResidual.Copy();
			_currentResidual = membrane.AverageStresses - appliedStresses;
        }

        /// <summary>
        /// Update strains.
        /// </summary>
        private static void StrainUpdate()
        {
			// Get current strains
			var eCur = _currentStrains.Copy();

            // Increment strains
			_currentStrains += StrainState.FromStresses(-_currentResidual, _currentStiffness);

			// Set last strains
			_lastStrains = eCur;
        }

        /// <summary>
        /// Save results in output matrices.
        /// <para>See: <see cref="_strainOutput"/>, <see cref="_stressOutput"/>, <see cref="_concretePrincipalStrainOutput"/> and <see cref="_principalStressOutput"/>.</para>
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> element analyzed.</param>
        /// <param name="appliedStresses">Current applied <see cref="StressState"/>.</param>
        /// <param name="loadStep">The current load step.</param>
        private static void SaveResults(Membrane membrane, StressState appliedStresses, int loadStep)
        {
            // Get row
            int i = loadStep - 1;

            _strainOutput[i]          = membrane.AverageStrains;
            _stressOutput[i]          = appliedStresses;
            _principalStressOutput[i] = membrane.Concrete.PrincipalStresses;

            _concretePrincipalStrainOutput[i] = membrane.Concrete.PrincipalStrains;
            _averagePrincipalStrainOutput[i]  = membrane.AveragePrincipalStrains;
        }

        /// <summary>
        /// Output results in a CSV file.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> element analyzed.</param>
        /// <param name="fileName">The name of saved result file. <para>See: <seealso cref="ResultFileName"/>.</para></param>
        /// <param name="calculatedLoadSteps">The number of calculated load steps.</param>
        private static void OutputResults(Membrane membrane, int calculatedLoadSteps, out string fileName)
        {
            // Get readers for result
            string[]
                outputReaders = { "ex", "ey", "exy", "", "fx", "fy", "fxy", "", "ec1", "ec2", "theta1e", "", "fc1", "fc2", "theta1f" };

            // Result matrices
            var result = Matrix.Build.Dense(calculatedLoadSteps, outputReaders.Length, double.NaN);

            for (int i = 0; i < calculatedLoadSteps; i++)
            {
                // Set stress and strain states
                result.SetSubMatrix(i, 0, _strainOutput[i].AsVector().ToRowMatrix());
                result.SetSubMatrix(i, 4, _stressOutput[i].AsVector().ToRowMatrix());
                result.SetSubMatrix(i, 8, _concretePrincipalStrainOutput[i].AsVector().ToRowMatrix());
                result.SetSubMatrix(i, 12, _principalStressOutput[i].AsVector().ToRowMatrix());

                // Set angles in degrees (average angle in strains)
                result[i, 10] = _averagePrincipalStrainOutput[i].Theta1.ToDegree();
                result[i, 14] = _principalStressOutput[i].Theta1.ToDegree();
            }

            fileName = ResultFileName(membrane);

            // Save results
            DelimitedWriter.Write(fileName, result, ";", outputReaders, null, null, double.NaN);
        }

        /// <summary>
        /// Write a ending message in <see cref="Console"/>.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object in analysis.</param>
        private static void AnalysisDone(Membrane membrane)
        {
            Console.WriteLine("\n{0} analysis done.\n", membrane is MCFTMembrane ? "MCFT" : "DSFM");
        }

        /// <summary>
        /// Write a message in the load step that concrete cracks.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object in analysis.</param>
        /// <param name="loadStep">The current load step.</param>
        private static void ConcreteCrackedMessage(Membrane membrane, int loadStep)
        {
	        if (_crackStep.HasValue || !membrane.Concrete.Cracked)
		        return;

			// Set and write message
	        _crackStep = loadStep;
	        Console.WriteLine("Concrete cracked at step {0}", _crackStep.Value);
        }

        /// <summary>
        /// Write a message after achieving convergence.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> object in analysis.</param>
        /// <param name="loadStep">The current load step.</param>
        /// <param name="iteration">The current iteration</param>
        private static void ConvergenceMessage(Membrane membrane, int loadStep, int iteration)
        {
	        if (membrane is DSFMMembrane dsfm && membrane.Concrete.Cracked)
		        Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", loadStep, iteration, dsfm.SlipApproach);
		        //Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}\nesl ={3}", loadStep, iteration, dsfm.SlipApproach, dsfm.CrackSlipStrains);

	        else
		        Console.WriteLine("LS = {0}, Iterations = {1}", loadStep, iteration);
        }

        /// <summary>
        /// Write a message if convergence is not reached.
        /// </summary>
        /// <param name="loadStep">The current load step.</param>
        private static void NoConvergenceMessage(int loadStep)
		{
			// Set stop
			_stop = true;
			Console.WriteLine("LS = {0}, {1}", loadStep, "CONVERGENCE NOT REACHED");
		}

        /// <summary>
        /// Returns the output file name and save location.
        /// </summary>
        /// <param name="membrane">The <see cref="Membrane"/> element analyzed.</param>
        private static string ResultFileName(Membrane membrane)
        {
            // Set filename
            var fileName = "D:/membrane_result_";

            fileName += membrane is MCFTMembrane ? "MCFT" : "DSFM";

            // Set file extension
            return
	            $"{fileName}.csv";
        }
    }
}
