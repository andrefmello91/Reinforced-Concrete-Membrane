using System;
using System.Collections.Generic;
using System.Linq;
using andrefmello91.Extensions;
using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents;
using MathNet.Numerics.Data.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace andrefmello91.ReinforcedConcreteMembrane.Solver
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
		///     <see cref="Array" /> of average <see cref="PrincipalStrainState" /> results for each step.
		/// </summary>
		private readonly List<PrincipalStrainState> _averagePrincipalStrainOutput = new()
		{
			PrincipalStrainState.Zero
		};

		/// <summary>
		///     <see cref="Array" /> of concrete <see cref="PrincipalStrainState" /> results for each step.
		/// </summary>
		private readonly List<PrincipalStrainState> _concretePrincipalStrainOutput = new()
		{
			PrincipalStrainState.Zero
		};

		/// <summary>
		///     <see cref="Array" /> of <see cref="PrincipalStressState" /> results for each step.
		/// </summary>
		private readonly List<PrincipalStressState> _principalStressOutput = new()
		{
			PrincipalStressState.Zero
		};

		/// <summary>
		///     <see cref="Array" /> of <see cref="StrainState" /> results for each step.
		/// </summary>
		private readonly List<StrainState> _strainOutput = new()
		{
			StrainState.Zero
		};

		/// <summary>
		///     <see cref="Array" /> of <see cref="StressState" /> results for each step.
		/// </summary>
		private readonly List<StressState> _stressOutput = new()
		{
			StressState.Zero
		};

		/// <summary>
		///     The applied <see cref="StressState" />.
		/// </summary>
		private StressState _appliedStresses;

		/// <summary>
		///     Number of successfully calculated steps.
		/// </summary>
		private int _calculatedSteps;

		/// <summary>
		///     The step of concrete cracking.
		/// </summary>
		private int? _crackStep;

		/// <summary>
		///     The strain increment of the current step.
		/// </summary>
		private StrainState _currentIncrement;

		/// <summary>
		///     The residual <see cref="StressState" /> of current iteration
		/// </summary>
		private StressState _currentResidual = StressState.Zero;

		/// <summary>
		///     The secant stiffness <see cref="Matrix" /> of current iteration
		/// </summary>
		private MaterialMatrix _currentStiffness = null!;

		/// <summary>
		///     The <see cref="StrainState" /> of current iteration
		/// </summary>
		private StrainState _currentStrains;

		/// <summary>
		///     The internal <see cref="StressState" /> of current iteration.
		/// </summary>
		private StressState _currentStresses;

		/// <summary>
		///     The initial strain increment of the current step.
		/// </summary>
		private StrainState _initialIncrement;

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
		private MaterialMatrix _lastStiffness = null!;

		/// <summary>
		///     The <see cref="StrainState" /> of last iteration
		/// </summary>
		private StrainState _lastStrains = StrainState.Zero;

		/// <summary>
		///     The internal <see cref="StressState" /> of last iteration.
		/// </summary>
		private StressState _lastStresses;

		/// <summary>
		///     Current step.
		/// </summary>
		private int _step;

		/// <summary>
		///     The applied <see cref="StressState" /> at the current step.
		/// </summary>
		private StressState _stepStresses;

		/// <summary>
		///     Stop parameter for analysis.
		/// </summary>
		private bool _stop;

		/// <summary>
		///     The strain increment for each step.
		/// </summary>
		/// <remarks>
		///     Used only in <see cref="AnalysisControl.Strain" />.
		/// </remarks>
		private StrainState _strainIncrement;

		/// <summary>
		///     Write analysis results in console?
		/// </summary>
		private bool _writeInConsole = true;

		#endregion

		#region Properties

		/// <summary>
		///     The type of analysis control.
		/// </summary>
		public AnalysisControl Control { get; set; }

		/// <summary>
		///     Get the cracking <see cref="StressState" />.
		/// </summary>
		public StressState CrackingStresses { get; private set; }

		/// <summary>
		///     The <see cref="Membrane" /> element.
		/// </summary>
		public Membrane Element { get; }

		/// <summary>
		///     Maximum number of iterations to perform at each step.
		/// </summary>
		/// <remarks>
		///     Default: 10000
		/// </remarks>
		public int MaximumIterations { get; set; } = 10000;

		/// <summary>
		///     The number of steps to perform.
		/// </summary>
		/// <remarks>
		///     Default: 100
		/// </remarks>
		public int NumberOfSteps { get; set; } = 100;

		/// <summary>
		///     Solver method.
		/// </summary>
		public NonlinearSolver Solver { get; set; }

		/// <summary>
		///     Convergence tolerance for strains.
		/// </summary>
		/// <remarks>
		///     Default: 1E-8
		/// </remarks>
		public double StrainTolerance { get; set; } = 1E-8;

		/// <summary>
		///     Convergence tolerance for stresses.
		/// </summary>
		/// <remarks>
		///     Default: 1E-3
		/// </remarks>
		public double StressTolerance { get; set; } = 1E-3;

		/// <summary>
		///     Get the ultimate <see cref="StressState" />.
		/// </summary>
		public StressState UltimateStresses { get; private set; }

		/// <summary>
		///     Get the analysis type.
		/// </summary>
		private string AnalysisType => $"{Element.Concrete.Model}";

		/// <summary>
		///     Get current load factor.
		/// </summary>
		private double LoadFactor => (double) _step / NumberOfSteps;

		/// <summary>
		///     Calculate strain convergence.
		/// </summary>
		private double StrainConvergence => CalculateConvergence(_currentIncrement.AsArray(), _initialIncrement.AsArray());

		/// <summary>
		///     Calculate stress convergence.
		/// </summary>
		private double StressConvergence => CalculateConvergence(_currentResidual.AsVector(), _stepStresses.AsVector());

		#endregion

		#region Constructors

		/// <summary>
		///     Default constructor.
		/// </summary>
		/// <param name="element">The <see cref="Membrane" /> element to analyze.</param>
		/// <param name="solver">The solver method.</param>
		/// <param name="control">The type of analysis control.</param>
		public MembraneSolver(Membrane element, NonlinearSolver solver = NonlinearSolver.NewtonRaphson, AnalysisControl control = AnalysisControl.Stress)
		{
			Element = element;
			Solver  = solver;
			Control = control;
		}

		#endregion

		#region Methods

		/// <summary>
		///     Simple console solver example.
		/// </summary>
		public static void SolverExample()
		{
			// Initiate steel for each direction
			var steelXY = new SteelParameters(276, 200000);

			// Get reinforcement
			var xRef          = WebReinforcementDirection.From(6.35, 50.55, steelXY, 70, 0);
			var yRef          = WebReinforcementDirection.From(4.7, 49.57, steelXY.Clone(), 70, 0);
			var reinforcement = new WebReinforcement(xRef, yRef, (Length) 70.As(LengthUnit.Millimeter));

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

		private static double CalculateConvergence(IEnumerable<double> numerator, IEnumerable<double> denominator) =>
			numerator.Sum(n => n * n) / (1 + denominator.Sum(d => d * d));

		/// <summary>
		///     Output results in a CSV file.
		/// </summary>
		/// <param name="filePath">The full path of output file produced.</param>
		/// <param name="location">
		///     The location to save output file.
		///     <para>Default: D:/ </para>
		/// </param>
		public void OutputResults(out string filePath, string location = DefaultLocation)
		{
			// Get readers for result
			string[]
				outputReaders = { "ex", "ey", "exy", "", "fx", "fy", "fxy", "", "ec1", "ec2", "theta1e", "", "fc1", "fc2", "theta1f" };

			// Result matrices
			var result = Matrix<double>.Build.Dense(_calculatedSteps, outputReaders.Length, double.NaN);

			for (var i = 0; i < _calculatedSteps; i++)
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
		///     Solve this element to applied stresses.
		/// </summary>
		/// <param name="appliedStresses">Applied <see cref="StressState" />.</param>
		/// <param name="simulate">Simulate until convergence is not reached (failure).</param>
		/// <param name="writeInConsole">Write analysis results in console?</param>
		public void Solve(StressState appliedStresses, bool simulate = false, bool writeInConsole = true)
		{
			// Initialize fields and write a starting message
			Initiate(appliedStresses, writeInConsole);

			// Analyze by steps
			StepAnalysis(simulate);
			AnalysisDone();
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
		///     Calculate and update stresses.
		/// </summary>
		private void CalculateStresses()
		{
			Element.Calculate(_currentStrains);

			// Update stresses
			_lastStresses    = _currentStresses.Clone();
			_currentStresses = Element.AverageStresses;
		}

		/// <summary>
		///     Write a message in the step that concrete cracks.
		/// </summary>
		private void ConcreteCrackedCheck()
		{
			if (_crackStep.HasValue || !Element.Concrete.Cracked)
				return;

			// Set and write message
			_crackStep = _step;

			// Set cracking stresses
			CrackingStresses = _currentStresses.Clone();

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
					Console.WriteLine("LS = {0}, Iterations = {1}, Slip Approach: {2}", _step, _iteration, dsfm.SlipApproach);
					break;

				default:
					Console.WriteLine("LS = {0}, Iterations = {1}", _step, _iteration);
					break;
			}
		}

		/// <summary>
		///     Verify if convergence is reached.
		/// </summary>
		/// <param name="convergence">Calculated convergence.</param>
		/// <param name="tolerance">The convergence tolerance.</param>
		/// <param name="minIterations">Minimum number of iterations (default: 2).</param>
		private bool ConvergenceReached(double convergence, double tolerance, int minIterations = 2) =>
			_iteration >= minIterations && convergence <= tolerance;

		/// <summary>
		///     Get the stress state for the current step, basing in the analysis control.
		/// </summary>
		private StressState GetStepStresses() =>
			Control switch
			{
				AnalysisControl.Stress => LoadFactor * _appliedStresses,
				_                      => _currentStiffness * (_currentStrains + _strainIncrement)
			};

		/// <summary>
		///     Initialize auxiliary fields and write a starting message in <see cref="Console" />.
		/// </summary>
		/// <param name="appliedStresses">Applied <see cref="StressState" />.</param>
		/// <param name="writeInConsole">Write analysis results in console?</param>
		private void Initiate(StressState appliedStresses, bool writeInConsole)
		{
			// Set values
			_appliedStresses = appliedStresses;
			_writeInConsole  = writeInConsole;

			// Get initial stresses
			_step             = 1;
			_stepStresses     = LoadFactor * _appliedStresses;
			_currentStresses  = _lastStresses     = StressState.Zero;
			_currentIncrement = _initialIncrement = StrainState.Zero;

			// Calculate initial stiffness
			_currentStiffness = Element.InitialStiffness;

			// Calculate initial strains
			_currentStrains = StrainState.Zero;

			// Get the strain increment
			_strainIncrement = _stepStresses / _currentStiffness;

			// Initiate solution values
			_lastStiffness = (MaterialMatrix) _currentStiffness.Clone();

			if (_writeInConsole)
				Console.WriteLine("\nStarting {0} analysis...\n", AnalysisType);
		}

		/// <summary>
		///     Iterate to find solution.
		/// </summary>
		private void Iterate()
		{
			_iteration = 1;

			while (_iteration <= MaximumIterations)
			{
				// Calculate stresses
				CalculateStresses();

				// Verify if concrete cracks in this step and write a message
				ConcreteCrackedCheck();

				// Calculate and update residual
				UpdateResidual();

				// Check stress convergence
				if (ConvergenceReached(StressConvergence, StressTolerance))
					goto ConvergenceReached;

				// If reached max iterations
				if (_iteration == MaximumIterations)
					goto Stop;

				// Update strains
				UpdateStrains();

				// Check strain convergence
				if (ConvergenceReached(StrainConvergence, StrainTolerance))
					goto ConvergenceReached;

				// Update stiffness
				UpdateStiffness();

				_iteration++;
			}

			ConvergenceReached:
			{
				// Write message
				ConvergenceMessage();

				// Set results on output matrices
				SaveResults();

				return;
			}

			// Write message
			Stop:
			StopAnalysis();
		}

		/// <summary>
		///     Returns the output file name and save location.
		/// </summary>
		/// <param name="location">The location to save output file.</param>
		private string ResultFileName(string location) =>
			$"{location}membrane_result_{AnalysisType}.csv";

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
			_stressOutput.Add(Element.AverageStresses);
			_principalStressOutput.Add(Element.Concrete.PrincipalStresses);

			_concretePrincipalStrainOutput.Add(Element.Concrete.PrincipalStrains);
			_averagePrincipalStrainOutput.Add(Element.AveragePrincipalStrains);
		}

		/// <summary>
		///     Do analysis by steps.
		/// </summary>
		private void StepAnalysis(bool simulate)
		{
			// Initiate steps
			_step = 1;
			while (simulate || _step <= NumberOfSteps)
			{
				// Calculate stresses
				_stepStresses = GetStepStresses();

				// Iterate to find solution
				Iterate();

				switch (_stop)
				{
					// Solution not reached
					case true:

						// Decrement ls to correct output file
						_step--;

						// Set last step
						_calculatedSteps = _step;
						return;

					// Solution reached
					default:

						// Set provisional stresses
						UltimateStresses = _currentStresses;

						if (_step > 930)
						{
						}

						_step++;
						continue;
				}
			}

			_step = 1;
		}

		/// <summary>
		///     Write a message if convergence is not reached.
		/// </summary>
		private void StopAnalysis()
		{
			// Set stop
			_stop = true;

			if (_writeInConsole)
				Console.WriteLine("LS = {0}, {1}", _step, "CONVERGENCE NOT REACHED");
		}

		/// <summary>
		///     Update residual <see cref="StressState" />.
		/// </summary>
		private void UpdateResidual()
		{
			// Set new values
			_lastResidual    = _currentResidual.Clone();
			_currentResidual = _currentStresses - _stepStresses;
		}

		/// <summary>
		///     Calculate the secant stiffness <see cref="Matrix" /> of current iteration.
		/// </summary>
		private void UpdateStiffness()
		{
			Matrix<double> dK;

			switch (Solver)
			{
				// Newton-Raphson update
				case NonlinearSolver.NewtonRaphson:
					// Get variations
					var de = (_currentStrains - _lastStrains).AsVector();
					var ds = (_currentStresses - _lastStresses).AsVector();

					// Calculate increment
					dK = ds.ToColumnMatrix() * de.ToRowMatrix();

					break;

				// Secant update
				case NonlinearSolver.Secant:
					// Calculate the variation of strains and residual as vectors
					Vector<double>
						dStrain = (_currentStrains - _lastStrains).AsVector(),
						dRes    = (_currentResidual - _lastResidual).AsVector();

					// Calculate increment
					dK = ((dRes - _lastStiffness * dStrain) / dStrain.Norm(2)).ToColumnMatrix() * dStrain.ToRowMatrix();

					break;

				default:
					return;
			}

			// Set new values
			_lastStiffness    = (MaterialMatrix) _currentStiffness.Clone();
			_currentStiffness = (MaterialMatrix) (_currentStiffness + new MaterialMatrix(dK));
		}

		/// <summary>
		///     Update strains.
		/// </summary>
		private void UpdateStrains()
		{
			// Calculate increment
			_currentIncrement = -(_currentResidual / _currentStiffness);

			// Calculate increment and set initial increment
			if (_iteration == 1)
				_initialIncrement = _currentIncrement.Clone();

			// Increment strains
			_lastStrains    =  _currentStrains.Clone();
			_currentStrains += _currentIncrement;
		}

		#endregion

	}

	/// <summary>
	///     Solver enumeration.
	/// </summary>
	public enum NonlinearSolver
	{
		/// <summary>
		///     Solve by Newton-Raphson method.
		/// </summary>
		NewtonRaphson,

		/// <summary>
		///     Solve by secant method.
		/// </summary>
		Secant
	}

	/// <summary>
	///     Types of analysis control.
	/// </summary>
	public enum AnalysisControl
	{
		/// <summary>
		///     Control the analysis by increasing the applied stress state.
		/// </summary>
		/// <remarks>
		///     This cannot predict the full response of the element.
		/// </remarks>
		Stress,

		/// <summary>
		///     Control the analysis by increasing the shear strain.
		/// </summary>
		/// <remarks>
		///     This can predict the full response of the element.
		/// </remarks>
		Strain
	}
}