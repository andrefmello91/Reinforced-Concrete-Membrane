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
	public class MembraneSolver
	{
        /// <summary>
        /// Default save location for output file.
        /// </summary>
		private const string DefaultLocation = "D:/";

        /// <summary>
        /// The applied <see cref="StressState"/>.
        /// </summary>
        private StressState _appliedStresses;

		/// <summary>
		/// The applied <see cref="StressState"/> at the current load step.
		/// </summary>
		private StressState _stepStresses;

        /// <summary>
        /// <see cref="Array"/> of <see cref="StrainState"/> results for each load step.
        /// </summary>
        private StrainState[] _strainOutput;

	    /// <summary>
	    /// <see cref="Array"/> of concrete <see cref="PrincipalStrainState"/> results for each load step.
	    /// </summary>
	    private PrincipalStrainState[] _concretePrincipalStrainOutput;

	    /// <summary>
	    /// <see cref="Array"/> of average <see cref="PrincipalStrainState"/> results for each load step.
	    /// </summary>
	    private PrincipalStrainState[] _averagePrincipalStrainOutput;

	    /// <summary>
	    /// <see cref="Array"/> of <see cref="StressState"/> results for each load step.
	    /// </summary>
	    private StressState[] _stressOutput;

	    /// <summary>
	    /// <see cref="Array"/> of <see cref="PrincipalStressState"/> results for each load step.
	    /// </summary>
	    private PrincipalStressState[] _principalStressOutput;

        /// <summary>
        /// The secant stiffness <see cref="Matrix"/> of current iteration
        /// </summary>
        private Matrix<double> _currentStiffness;

	    /// <summary>
	    /// The secant stiffness <see cref="Matrix"/> of last iteration
	    /// </summary>
	    private Matrix<double> _lastStiffness;

	    /// <summary>
	    /// The <see cref="StrainState"/> of current iteration
	    /// </summary>
	    private StrainState _currentStrains;

	    /// <summary>
	    /// The <see cref="StrainState"/> of last iteration
	    /// </summary>
	    private StrainState _lastStrains;

	    /// <summary>
	    /// The residual <see cref="StressState"/> of current iteration
	    /// </summary>
	    private StressState _currentResidual;

	    /// <summary>
	    /// The residual <see cref="StressState"/> of last iteration
	    /// </summary>
	    private StressState _lastResidual;

		/// <summary>
        /// The step of concrete cracking.
        /// </summary>
	    private int? _crackStep;

	    /// <summary>
	    /// Stop parameter for analysis.
	    /// </summary>
	    private bool _stop;

        /// <summary>
        /// Current load step.
        /// </summary>
	    private int _loadStep;

        /// <summary>
        /// Number of successfully calculated load steps.
        /// </summary>
	    private int _calculatedLoadSteps;

        /// <summary>
        /// Number of load steps.
        /// </summary>
	    private int _numLoadSteps;

        /// <summary>
        /// Current iteration.
        /// </summary
        private int _iteration;

        /// <summary>
        /// Maximum number of iterations.
        /// </summary
        private int _maxIterations;

        /// <summary>
        /// Convergence tolerance.
        /// </summary
        private double _tolerance;

        /// <summary>
        /// The <see cref="Membrane"/> element.
        /// </summary>
        public Membrane Element { get; }

        /// <summary>
        /// Get current load factor.
        /// </summary>
        private double LoadFactor => (double)_loadStep / _numLoadSteps;

        /// <summary>
        /// Get the analysis type.
        /// </summary>
        private string AnalysisType => Element is MCFTMembrane ? "MCFT" : "DSFM";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="element">The <see cref="Membrane"/> element to analyze.</param>
        public MembraneSolver(Membrane element)
        {
	        Element = element;
        }

        /// <summary>
        /// Simple console solver, with example from <see cref="PanelExamples"/>.
        /// </summary>
        public static void SolverExample()
	    {
		    // Initiate the membrane
		    var membraneMCFT = PanelExamples.PL6();
		    var membraneDSFM = PanelExamples.PL6(ConstitutiveModel.DSFM);

		    // Initiate stresses
		    var sigma = new StressState(2.98 * 3, 0, 3);

            // Initiate solvers
            var mcftSolver = new MembraneSolver(membraneMCFT);
            var dsfmSolver = new MembraneSolver(membraneDSFM);

		    // Solve
		    mcftSolver.Solve(sigma);
		    dsfmSolver.Solve(sigma);

            // Output results
            mcftSolver.OutputResults(out var mcftFile);
            dsfmSolver.OutputResults(out var dsfmFile);
            
		    Console.WriteLine("Done! Press any key to exit.");
            System.Diagnostics.Process.Start(mcftFile);
            System.Diagnostics.Process.Start(dsfmFile);
            Console.ReadKey();
	    }

        /// <summary>
        /// Solve this element.
        /// </summary>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps (default: 100).</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 10000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 4E-4).</param>
        public void Solve(StressState appliedStresses, int numLoadSteps = 100, int maxIterations = 10000, double tolerance = 4E-4)
	    {
		    // Initialize fields and write a starting message
		    Initiate(appliedStresses, numLoadSteps, maxIterations, tolerance);

		    // Analyze by steps
		    StepAnalysis();
            AnalysisDone();
	    }

        /// <summary>
        /// Initialize auxiliary fields and write a starting message in <see cref="Console"/>.
        /// </summary>
        /// <param name="appliedStresses">Applied <see cref="StressState"/>, in MPa.</param>
        /// <param name="numLoadSteps">The number of load steps.</param>
        /// <param name="maxIterations">Maximum number of iterations (default: 10000).</param>
        /// <param name="tolerance">Stress convergence tolerance (default: 4E-4).</param>
        private void Initiate(StressState appliedStresses, int numLoadSteps, int maxIterations, double tolerance)
        {
            // Set values
            _appliedStresses = appliedStresses;
            _numLoadSteps    = numLoadSteps;
            _maxIterations   = maxIterations;
            _tolerance       = tolerance;
            _loadStep        = 1;

	        // Get initial stresses
	        _stepStresses = LoadFactor * _appliedStresses;

	        // Calculate initial stiffness
	        _currentStiffness = Element.InitialStiffness;

	        // Calculate initial strains
	        _currentStrains = StrainState.FromStresses(_stepStresses, _currentStiffness);

	        // Initiate solution values
	        _lastStiffness   = _currentStiffness.Clone();
	        _lastStrains     = StrainState.Zero;
	        _lastResidual    = StressState.Zero;
	        _currentResidual = StressState.Zero;

	        // Initiate output matrices
	        _strainOutput          = new StrainState[_numLoadSteps];
	        _stressOutput          = new StressState[_numLoadSteps];
	        _principalStressOutput = new PrincipalStressState[_numLoadSteps];

	        _concretePrincipalStrainOutput = new PrincipalStrainState[_numLoadSteps];
	        _averagePrincipalStrainOutput  = new PrincipalStrainState[_numLoadSteps];

	        // Auxiliary verifiers
	        _stop      = false;
	        _crackStep = null;

	        Console.WriteLine("\nStarting {0} analysis...\n", Element is MCFTMembrane ? "MCFT" : "DSFM");
        }

        /// <summary>
        /// Do analysis by load steps.
        /// </summary>
        private void StepAnalysis()
        {
	        // Initiate load steps
	        for (_loadStep = 1; _loadStep <= _numLoadSteps; _loadStep++)
	        {
		        // Calculate stresses
		        _stepStresses = LoadFactor * _appliedStresses;

				// Iterate to find solution
				Iterate();

				// Solution reached:
				if (_stop)
				{
					// Solution not reached
					// Decrement ls to correct output file
					_loadStep--;
					break;
				}
	        }

	        _calculatedLoadSteps = _loadStep;
        }

        /// <summary>
        /// Iterate to find solution.
        /// </summary>
        private void Iterate()
        {
	        for (_iteration = 1; _iteration <= _maxIterations; _iteration++)
	        {
		        // Do analysis
		        Element.Calculate(_currentStrains);

		        // Verify if concrete cracks in this load step and write a message
		        ConcreteCrackedMessage();

		        // Calculate and update residual
		        ResidualUpdate();

		        // If convergence is reached
		        if (ConvergenceReached())
		        {
			        // Write message
			        ConvergenceMessage();

			        // Set results on output matrices
			        SaveResults();

			        break;
		        }

		        // If reached max iterations
		        if (_iteration == _maxIterations)
		        {
			        // Write message
			        NoConvergenceMessage();
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
        private double Convergence()
        {
	        Vector<double>
		        res = _currentResidual.AsVector(),
		        app = _stepStresses.AsVector();

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
        /// <param name="minIterations">Minimum number of iterations (default: 2).</param>
        private bool ConvergenceReached(int minIterations = 2) => ConvergenceReached(Convergence(), minIterations);

        /// <summary>
        /// Verify if convergence is reached.
        /// </summary>
        /// <param name="convergence">Calculated convergence.</param>
        /// <param name="minIterations">Minimum number of iterations (default: 2).</param>
        private bool ConvergenceReached(double convergence, int minIterations = 2) => convergence <= _tolerance && _iteration >= minIterations;

        /// <summary>
        /// Calculate the secant stiffness <see cref="Matrix"/> of current iteration.
        /// </summary>
        private void SecantStiffnessUpdate()
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
		private void ResidualUpdate()
		{
			// Set new values
            _lastResidual    = _currentResidual.Copy();
			_currentResidual = Element.AverageStresses - _stepStresses;
        }

        /// <summary>
        /// Update strains.
        /// </summary>
        private void StrainUpdate()
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
        private void SaveResults()
        {
            // Get row
            int i = _loadStep - 1;

            _strainOutput[i]          = Element.AverageStrains;
            _stressOutput[i]          = _stepStresses;
            _principalStressOutput[i] = Element.Concrete.PrincipalStresses;

            _concretePrincipalStrainOutput[i] = Element.Concrete.PrincipalStrains;
            _averagePrincipalStrainOutput[i]  = Element.AveragePrincipalStrains;
        }

        /// <summary>
        /// Output results in a CSV file.
        /// </summary>
        /// <param name="filePath">The full path of output file produced.</param>
        /// <param name="location">The location to save output file. <para>Default: D:/ </para></param>
        public void OutputResults(out string filePath, string location = DefaultLocation)
        {
            // Get readers for result
            string[]
                outputReaders = { "ex", "ey", "exy", "", "fx", "fy", "fxy", "", "ec1", "ec2", "theta1e", "", "fc1", "fc2", "theta1f" };

            // Result matrices
            var result = Matrix.Build.Dense(_calculatedLoadSteps, outputReaders.Length, double.NaN);

            for (int i = 0; i < _calculatedLoadSteps; i++)
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

            filePath = ResultFileName(location);

            // Save results
            DelimitedWriter.Write(filePath, result, ";", outputReaders, null, null, double.NaN);
        }

        /// <summary>
        /// Write a ending message in <see cref="Console"/>.
        /// </summary>
        private void AnalysisDone() => Console.WriteLine($"\n{AnalysisType} analysis done.\n");

        /// <summary>
        /// Write a message in the load step that concrete cracks.
        /// </summary>
        private void ConcreteCrackedMessage()
        {
	        if (_crackStep.HasValue || !Element.Concrete.Cracked)
		        return;

			// Set and write message
	        _crackStep = _loadStep;

	        Console.WriteLine("Concrete cracked at step {0}", _crackStep.Value);
        }

        /// <summary>
        /// Write a message after achieving convergence.
        /// </summary>
        private void ConvergenceMessage()
        {
	        if (Element is DSFMMembrane dsfm && Element.Concrete.Cracked)
		        Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", _loadStep, _iteration, dsfm.SlipApproach);

	        else
		        Console.WriteLine("LS = {0}, Iterations = {1}", _loadStep, _iteration);
        }

        /// <summary>
        /// Write a message if convergence is not reached.
        /// </summary>
        private void NoConvergenceMessage()
		{
			// Set stop
			_stop = true;
			Console.WriteLine("LS = {0}, {1}", _loadStep, "CONVERGENCE NOT REACHED");
		}

        /// <summary>
        /// Returns the output file name and save location.
        /// </summary>
        /// <param name="location">The location to save output file.</param>
        private string ResultFileName(string location)
        {
            // Set filename
            var fileName = $"{location}membrane_result_";

            fileName += Element is MCFTMembrane ? "MCFT" : "DSFM";

            // Set file extension
            return
	            $"{fileName}.csv";
        }
    }
}
