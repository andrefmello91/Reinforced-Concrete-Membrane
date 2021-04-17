using System;
using System.Collections.Generic;
using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents;
using andrefmello91.Extensions;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace andrefmello91.ReinforcedConcreteMembrane
{
	/// <summary>
	///     Simple solver class.
	/// </summary>
	public class MembraneSolver
	{
		#region Fields

		/// <summary>
		///     Default save location for output file.
		/// </summary>
		private const string DefaultLocation = "D:/";

		/// <summary>
		///     The applied <see cref="StressState" />.
		/// </summary>
		private StressState _appliedStresses;

		/// <summary>
		///     <see cref="Array" /> of average <see cref="PrincipalStrainState" /> results for each load step.
		/// </summary>
		private List<PrincipalStrainState> _averagePrincipalStrainOutput = new();

		/// <summary>
		///     Number of successfully calculated load steps.
		/// </summary>
		private int _calculatedLoadSteps;

		/// <summary>
		///     <see cref="Array" /> of concrete <see cref="PrincipalStrainState" /> results for each load step.
		/// </summary>
		private List<PrincipalStrainState> _concretePrincipalStrainOutput = new();

		/// <summary>
		///     The step of concrete cracking.
		/// </summary>
		private int? _crackStep;

		/// <summary>
		///     The residual <see cref="StressState" /> of current iteration
		/// </summary>
		private StressState _currentResidual = StressState.Zero;

		/// <summary>
		///     The secant stiffness <see cref="Matrix" /> of current iteration
		/// </summary>
		private Matrix<double> _currentStiffness = null!;

		/// <summary>
		///     The <see cref="StrainState" /> of current iteration
		/// </summary>
		private StrainState _currentStrains;

		/// <summary>
		///     Current iteration.
		/// </summary>
		private int _iteration;

		/// <summary>
		///     The residual <see cref="StressState" /> of last iteration
		/// </summary>
		private StressState _lastResidual = StressState.Zero;

		/// <summary>
		///     The secant stiffness <see cref="Matrix" /> of last iteration
		/// </summary>
		private Matrix<double> _lastStiffness = null!;

		/// <summary>
		///     The <see cref="StrainState" /> of last iteration
		/// </summary>
		private StrainState _lastStrains = StrainState.Zero;

		/// <summary>
		///     Current load step.
		/// </summary>
		private int _loadStep = 1;

		/// <summary>
		///     Maximum number of iterations.
		/// </summary>
		private int _maxIterations;

		/// <summary>
		///     Number of load steps.
		/// </summary>
		private int _numLoadSteps;

		/// <summary>
		///     <see cref="Array" /> of <see cref="PrincipalStressState" /> results for each load step.
		/// </summary>
		private List<PrincipalStressState> _principalStressOutput = new ();

		/// <summary>
		///     The applied <see cref="StressState" /> at the current load step.
		/// </summary>
		private StressState _stepStresses;

		/// <summary>
		///     Stop parameter for analysis.
		/// </summary>
		private bool _stop;

		/// <summary>
		///     <see cref="Array" /> of <see cref="StrainState" /> results for each load step.
		/// </summary>
		private List<StrainState> _strainOutput = new ();

		/// <summary>
		///     <see cref="Array" /> of <see cref="StressState" /> results for each load step.
		/// </summary>
		private List<StressState> _stressOutput = new ();

		/// <summary>
		///     Convergence tolerance.
		/// </summary>
		private double _tolerance;

		/// <summary>
		///		Write analysis results in console?
		/// </summary>
		private bool _writeInConsole = true;
		
		/// <summary>
		///		Get the cracking <see cref="StressState"/>.
		/// </summary>
		public StressState CrackingStresses { get; private set; }
		
		/// <summary>
		///		Get the ultimate <see cref="StressState"/>.
		/// </summary>
		public StressState UltimateStresses { get; private set; }
		
		#endregion

		#region Properties

		/// <summary>
		///     Get the analysis type.
		/// </summary>
		private string AnalysisType => Element is MCFTMembrane ? "MCFT" : "DSFM";

		/// <summary>
		///     The <see cref="Membrane" /> element.
		/// </summary>
		public Membrane Element { get; }

		/// <summary>
		///     Get current load factor.
		/// </summary>
		private double LoadFactor => (double) _loadStep / _numLoadSteps;

		#endregion

		#region Constructors

		/// <summary>
		///     Default constructor.
		/// </summary>
		/// <param name="element">The <see cref="Membrane" /> element to analyze.</param>
		public MembraneSolver(Membrane element) => Element = element;

		#endregion

		#region  Methods

		/// <summary>
		///     Simple console solver example.
		/// </summary>
		public static void SolverExample()
		{
			// Initiate steel for each direction
			var steelXY = new Steel(276, 200000);

			// Get reinforcement
			var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 4.7, 49.57, steelXY.Clone(), 70);

			// Initiate the membrane
			var membraneMCFT = new MCFTMembrane(new Parameters(14.5, 6), reinforcement, 70);
			var membraneDSFM = new DSFMMembrane(new Parameters(14.5, 6), reinforcement.Clone(), 70);

			// Initiate stresses
			var sigma = new StressState(0, 0, 5);

			// Initiate solvers
			var mcftSolver = new MembraneSolver(membraneMCFT);
			var dsfmSolver = new MembraneSolver(membraneDSFM);

			// Solve
			mcftSolver.Solve(sigma);
			dsfmSolver.Solve(sigma);

			// Output results
			mcftSolver.OutputResults(out _);
			dsfmSolver.OutputResults(out _);

			Console.WriteLine("Done! Press any key to exit.");
			Console.ReadKey();
		}

		/// <summary>
		///     Solve this element.
		/// </summary>
		/// <param name="appliedStresses">Applied <see cref="StressState" />.</param>
		/// <param name="numLoadSteps">The number of load steps for <paramref name="appliedStresses"/> (default: 100).</param>
		/// <param name="maxIterations">Maximum number of iterations (default: 10000).</param>
		/// <param name="tolerance">Stress convergence tolerance (default: 1E-3).</param>
		/// <param name="simulate">Simulate until convergence is not reached (failure).</param>
		/// <param name="writeInConsole">Write analysis results in console?</param>
		public void Solve(StressState appliedStresses, int numLoadSteps = 100, int maxIterations = 10000, double tolerance = 1E-3, bool simulate = false, bool writeInConsole = true)
		{
			// Initialize fields and write a starting message
			Initiate(appliedStresses, numLoadSteps, maxIterations, tolerance, writeInConsole);

			// Analyze by steps
			StepAnalysis(simulate);
			AnalysisDone();
		}

		/// <summary>
		///     Output results in a CSV file.
		/// </summary>
		/// <param name="filePath">The full path of output file produced.</param>
		/// <param name="location">The location to save output file.
		///     <para>Default: D:/ </para>
		/// </param>
		public void OutputResults(out string filePath, string location = DefaultLocation)
		{
			// Get readers for result
			string[]
				outputReaders = { "ex", "ey", "exy", "", "fx", "fy", "fxy", "", "ec1", "ec2", "theta1e", "", "fc1", "fc2", "theta1f" };

			// Result matrices
			var result = Matrix<double>.Build.Dense(_calculatedLoadSteps, outputReaders.Length, double.NaN);

			for (var i = 0; i < _calculatedLoadSteps; i++)
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
		///     Initialize auxiliary fields and write a starting message in <see cref="Console" />.
		/// </summary>
		/// <param name="appliedStresses">Applied <see cref="StressState" />.</param>
		/// <param name="numLoadSteps">The number of load steps.</param>
		/// <param name="maxIterations">Maximum number of iterations.</param>
		/// <param name="tolerance">Stress convergence tolerance.</param>
		/// <param name="writeInConsole">Write analysis results in console?</param>
		private void Initiate(StressState appliedStresses, int numLoadSteps, int maxIterations, double tolerance, bool writeInConsole)
		{
			// Set values
			_appliedStresses = appliedStresses;
			_numLoadSteps    = numLoadSteps;
			_maxIterations   = maxIterations;
			_tolerance       = tolerance;
			_writeInConsole  = writeInConsole;

			// Get initial stresses
			_stepStresses = LoadFactor * _appliedStresses;

			// Calculate initial stiffness
			_currentStiffness = Element.InitialStiffness;

			// Calculate initial strains
			_currentStrains = StrainState.FromStresses(_stepStresses, _currentStiffness);

			// Initiate solution values
			_lastStiffness   = _currentStiffness.Clone();
			
			// Set Cs
			if (Element is DSFMMembrane membrane)
				membrane.SetCs(appliedStresses);
			
			if (_writeInConsole)
				Console.WriteLine("\nStarting {0} analysis...\n", Element is MCFTMembrane ? "MCFT" : "DSFM");
		}

		/// <summary>
		///     Do analysis by load steps.
		/// </summary>
		private void StepAnalysis(bool simulate = false)
		{
			// Initiate load steps
			_loadStep = 1;
			while (simulate || _loadStep <= _numLoadSteps)
			{
				// Calculate stresses
				_stepStresses = LoadFactor * _appliedStresses;

				// Iterate to find solution
				Iterate();

				// Solution reached:
				if (!_stop)
				{
					_loadStep++;
					continue;
				}

				// Solution not reached
				// Decrement ls to correct output file
				_loadStep--;
				break;
			}

			// Set last stresses
			_calculatedLoadSteps = _loadStep;
			UltimateStresses     = _stepStresses;
		}

		/// <summary>
		///     Iterate to find solution.
		/// </summary>
		private void Iterate()
		{
			_iteration = 1;
			
			while (_iteration <= _maxIterations)
			{
				// Do analysis
				Element.Calculate(_currentStrains);

				// Verify if concrete cracks in this load step and write a message
				ConcreteCrackedCheck();

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
					StopAnalysis();
					break;
				}

				// Update stiffness and strains
				SecantStiffnessUpdate();
				StrainUpdate();
				
				_iteration++;
			}
		}

		/// <summary>
		///     Calculate stress convergence.
		/// </summary>
		private double Convergence()
		{
			Vector<double>
				res = _currentResidual.AsVector(),
				app = _stepStresses.AsVector();

			double
				num = 0,
				den = 1;

			for (var i = 0; i < res.Count; i++)
			{
				num += res[i] * res[i];
				den += app[i] * app[i];
			}

			return
				num / den;
		}

		/// <summary>
		///     Verify if convergence is reached.
		/// </summary>
		/// <param name="minIterations">Minimum number of iterations (default: 2).</param>
		private bool ConvergenceReached(int minIterations = 2) => ConvergenceReached(Convergence(), minIterations);

		/// <summary>
		///     Verify if convergence is reached.
		/// </summary>
		/// <param name="convergence">Calculated convergence.</param>
		/// <param name="minIterations">Minimum number of iterations (default: 2).</param>
		private bool ConvergenceReached(double convergence, int minIterations = 2) => convergence <= _tolerance && _iteration >= minIterations;

		/// <summary>
		///     Calculate the secant stiffness <see cref="Matrix" /> of current iteration.
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
		///     Update residual <see cref="StressState" />.
		/// </summary>
		private void ResidualUpdate()
		{
			// Set new values
			_lastResidual    = _currentResidual.Clone();
			_currentResidual = Element.AverageStresses - _stepStresses;
		}

		/// <summary>
		///     Update strains.
		/// </summary>
		private void StrainUpdate()
		{
			// Get current strains
			var eCur = _currentStrains.Clone();

			// Increment strains
			_currentStrains += StrainState.FromStresses(-_currentResidual, _currentStiffness);

			// Set last strains
			_lastStrains = eCur;
		}

		/// <summary>
		///     Save results in output matrices.
		///     <para>
		///         See: <see cref="_strainOutput" />, <see cref="_stressOutput" />,
		///         <see cref="_concretePrincipalStrainOutput" /> and <see cref="_principalStressOutput" />.
		///     </para>
		/// </summary>
		private void SaveResults()
		{
			_strainOutput.Add(Element.AverageStrains);
			_stressOutput.Add(_stepStresses);
			_principalStressOutput.Add(Element.Concrete.PrincipalStresses);

			_concretePrincipalStrainOutput.Add(Element.Concrete.PrincipalStrains);
			_averagePrincipalStrainOutput.Add(Element.AveragePrincipalStrains);
		}

		/// <summary>
		///     Write a ending message in <see cref="Console" />.
		/// </summary>
		private void AnalysisDone()
		{
			if (!_writeInConsole)
				return;
			
			Console.WriteLine($"\n{AnalysisType} analysis done.\n");
		}

		/// <summary>
		///     Write a message in the load step that concrete cracks.
		/// </summary>
		private void ConcreteCrackedCheck()
		{
			if (_crackStep.HasValue || !Element.Concrete.Cracked)
				return;

			// Set and write message
			_crackStep = _loadStep;

			// Set cracking stresses
			CrackingStresses = _stepStresses.Clone();
			
			if (_writeInConsole)
				Console.WriteLine("Concrete cracked at step {0}", _crackStep.Value);
		}

		/// <summary>
		///     Write a message after achieving convergence.
		/// </summary>
		private void ConvergenceMessage()
		{
			if (!_writeInConsole)
				return;

			switch (Element)
			{
				case DSFMMembrane dsfm when Element.Concrete.Cracked:
					Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", _loadStep, _iteration, dsfm.SlipApproach);
					break;

				default:
					Console.WriteLine("LS = {0}, Iterations = {1}", _loadStep, _iteration);
					break;
			}
		}

		/// <summary>
		///     Write a message if convergence is not reached.
		/// </summary>
		private void StopAnalysis()
		{
			// Set stop
			_stop = true;
			
			if (_writeInConsole)
				Console.WriteLine("LS = {0}, {1}", _loadStep, "CONVERGENCE NOT REACHED");
		}

		/// <summary>
		///     Returns the output file name and save location.
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

		#endregion
	}
}