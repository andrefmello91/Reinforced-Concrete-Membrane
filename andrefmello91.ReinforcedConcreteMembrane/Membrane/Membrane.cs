using System;
using System.Linq;
using andrefmello91.Extensions;
using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;
using andrefmello91.OnPlaneComponents;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;
using static UnitsNet.UnitMath;

#nullable enable

namespace andrefmello91.ReinforcedConcreteMembrane
{
	/// <summary>
	///     Membrane element base class.
	/// </summary>
	public abstract class Membrane : IEquatable<Membrane>, ICloneable<Membrane>
	{

		#region Properties

		/// <summary>
		///     Get/set the average <see cref="PrincipalStrainState" />.
		/// </summary>
		public virtual PrincipalStrainState AveragePrincipalStrains { get; protected set; }

		/// <summary>
		///     Get/set the average <see cref="StrainState" /> in the membrane element.
		/// </summary>
		public StrainState AverageStrains { get; protected set; }

		/// <summary>
		///     Get average <see cref="StressState" />.
		/// </summary>
		public StressState AverageStresses => Reinforcement is null
			? Concrete.Stresses
			: Concrete.Stresses + Reinforcement.Stresses;

		/// <summary>
		///     Get <see cref="BiaxialConcrete" /> of the membrane element.
		/// </summary>
		public BiaxialConcrete Concrete { get; protected set; }

		/// <summary>
		///     Get initial <see cref="Membrane" /> stiffness <see cref="Matrix" />.
		/// </summary>
		/// <inheritdoc cref="BiaxialConcrete.InitialStiffness" />
		public Matrix<double> InitialStiffness => Reinforcement is null
			? Concrete.InitialStiffness
			: Concrete.InitialStiffness + Reinforcement.InitialStiffness;

		/// <summary>
		///     Get reference length.
		/// </summary>
		public Length ReferenceLength => 0.5 * CrackSpacing();

		/// <summary>
		///     Get/set <see cref="WebReinforcement" /> of the membrane element.
		/// </summary>
		public WebReinforcement? Reinforcement { get; }

		/// <summary>
		///     Get current <see cref="Membrane" /> stiffness <see cref="Matrix" />.
		/// </summary>
		public Matrix<double> Stiffness => Reinforcement is null ? Concrete.Stiffness : Concrete.Stiffness + Reinforcement.Stiffness;

		/// <summary>
		///     Get the width of the membrane element.
		/// </summary>
		public Length Width { get; }

		#endregion

		#region Constructors

		/// <inheritdoc cref="Membrane(IParameters, WebReinforcement?, Length, ConstitutiveModel)" />
		/// <param name="unit">The <see cref="LengthUnit" /> of <paramref name="width" /></param>
		protected Membrane(IParameters concreteParameters, WebReinforcement? reinforcement, double width, ConstitutiveModel model, LengthUnit unit = LengthUnit.Millimeter)
			: this(concreteParameters, reinforcement, (Length) width.As(unit), model)
		{
		}

		/// <summary>
		///     Base membrane element constructor.
		/// </summary>
		/// <param name="concreteParameters">Concrete <see cref="Parameters" /> object.</param>
		/// <param name="reinforcement"><see cref="WebReinforcement" /> object .</param>
		/// <param name="width">The width of cross-section.</param>
		/// <param name="model">Concrete <see cref="ConstitutiveModel" />.</param>
		protected Membrane(IParameters concreteParameters, WebReinforcement? reinforcement, Length width, ConstitutiveModel model)
		{
			// Initiate new materials
			Concrete      = BiaxialConcrete.From(concreteParameters, model);
			Reinforcement = reinforcement;

			Width = width;

			// Set initial strains
			AverageStrains = StrainState.Zero;
		}

		#endregion

		#region Methods

		/// <summary>
		///     Calculate the average crack opening.
		/// </summary>
		/// <param name="reinforcement">The <see cref="WebReinforcement" /> object.</param>
		/// <param name="concreteStrains">The <see cref="PrincipalStrainState" /> in concrete.</param>
		public static Length CrackOpening(WebReinforcement? reinforcement, PrincipalStrainState concreteStrains) =>
			concreteStrains.Epsilon1 <= 0 || concreteStrains.Epsilon1.ApproxZero(1E-9)
				? Length.Zero
				: concreteStrains.Epsilon1 * CrackSpacing(reinforcement, concreteStrains);

		/// <summary>
		///     Create a membrane element based on concrete constitutive model.
		/// </summary>
		/// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane" /> (default: true)</param>
		/// <inheritdoc cref="Membrane(IParameters, WebReinforcement, double, ConstitutiveModel, LengthUnit)" />
		public static Membrane From(IParameters concreteParameters, WebReinforcement? reinforcement, double width, ConstitutiveModel model = ConstitutiveModel.MCFT, LengthUnit unit = LengthUnit.Millimeter, bool considerCrackSlip = true) =>
			From(concreteParameters, reinforcement, (Length) width.As(unit), model, considerCrackSlip);

		/// <inheritdoc cref="From(IParameters, WebReinforcement?, double, ConstitutiveModel, LengthUnit, bool)" />
		public static Membrane From(IParameters concreteParameters, WebReinforcement? reinforcement, Length width, ConstitutiveModel model = ConstitutiveModel.MCFT, bool considerCrackSlip = true) =>
			model switch
			{
				ConstitutiveModel.MCFT => new MCFTMembrane(concreteParameters, reinforcement, width),
				ConstitutiveModel.DSFM => new DSFMMembrane(concreteParameters, reinforcement, width, considerCrackSlip),
				_                      => new SMMMembrane(concreteParameters, reinforcement, width)
			};

		/// <summary>
		///     Calculate the crack spacing in principal strain direction.
		/// </summary>
		/// <inheritdoc cref="CrackOpening(WebReinforcement, PrincipalStrainState)" />
		private static Length CrackSpacing(WebReinforcement? reinforcement, PrincipalStrainState concreteStrains)
		{
			// Get the angles
			var (cosThetaC, sinThetaC) = concreteStrains.Theta1.DirectionCosines(true);

			// Calculate crack spacings in X and Y
			double
				smx = reinforcement?.DirectionX?.CrackSpacing().Millimeters ?? 21,
				smy = reinforcement?.DirectionY?.CrackSpacing().Millimeters ?? 21,
				sm  = 1.00 / (sinThetaC / smx + cosThetaC / smy);

			// Calculate crack spacing
			return
				(Length) sm.As(LengthUnit.Millimeter);
		}

		/// <summary>
		///     Calculate maximum shear stress on crack, in MPa.
		/// </summary>
		/// <param name="crackOpening">Average crack opening, in mm.</param>
		/// <param name="parameters">Concrete parameters.</param>
		private static Pressure MaximumShearOnCrack(Length crackOpening, IParameters parameters)
		{
			var vcimax = 0.18 * parameters.Strength.Megapascals.Sqrt()
			             / (0.31 + 24 * crackOpening.Millimeters / (parameters.AggregateDiameter.Millimeters + 16));

			return Pressure.FromMegapascals(vcimax);
		}

		/// <summary>
		///     Calculate <see cref="AverageStresses" /> and <see cref="Stiffness" />, given a known <see cref="StrainState" />.
		/// </summary>
		/// <param name="appliedStrains">Current applied <see cref="StrainState" />.</param>
		/// <param name="appliedStresses">Current applied <see cref="StressState"/> (only for <see cref="ConstitutiveModel.SMM"/>)</param>
		public abstract void Calculate(StrainState appliedStrains, StressState? appliedStresses = null);

		/// <summary>
		///     Limit tensile principal stress by crack check procedure, by Bentz (2000).
		/// </summary>
		protected void CrackCheck()
		{
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

			// Get concrete tensile stress
			var f1a = Concrete.PrincipalStresses.Sigma1;

			// Get reinforcement angles related to crack
			var (thetaNx, thetaNy) = Reinforcement?.Angles(Concrete.PrincipalStrains.Theta1) ?? (Concrete.PrincipalStrains.Theta1, Concrete.PrincipalStrains.Theta1 - Constants.PiOver2);
			double
				cosNx = thetaNx.Cos(true),
				cosNy = thetaNy.Cos(true),
				tanNx = thetaNx.Tan(true),
				tanNy = thetaNy.Tan(true);

			// Reinforcement capacity reserve
			Pressure
				f1cx = Reinforcement?.DirectionX?.CapacityReserve ?? Pressure.Zero,
				f1cy = Reinforcement?.DirectionY?.CapacityReserve ?? Pressure.Zero;

			// Maximum possible shear on crack interface
			var vcimaxA = MaximumShearOnCrack();

			// Maximum possible shear for biaxial yielding
			var vcimaxB = (f1cx - f1cy).Abs() / (tanNx + tanNy);

			// Maximum shear on crack
			var vcimax = Min(vcimaxA, vcimaxB);

			// Biaxial yielding condition
			var f1b = f1cx * cosNx * cosNx + f1cy * cosNy * cosNy;

			// Maximum tensile stress for equilibrium in X and Y
			Pressure
				f1c = f1cx + vcimax * tanNx,
				f1d = f1cy + vcimax * tanNy;

			// Calculate the minimum tensile stress
			var f1List = new[] { f1a, f1b, f1c, f1d };
			var fc1    = f1List.Min();

			// Set to concrete
			if (fc1 < f1a)
				Concrete.SetTensileStress(fc1);
		}

		/// <summary>
		///     Calculate the average crack opening.
		/// </summary>
		/// <param name="membrane">The <see cref="Membrane" /> object.</param>
		protected Length CrackOpening() => CrackOpening(Reinforcement, Concrete.PrincipalStrains);

		/// <summary>
		///     Calculate the crack spacing in principal strain direction.
		/// </summary>
		/// <inheritdoc cref="CrackOpening(Membrane)" />
		protected Length CrackSpacing() => CrackSpacing(Reinforcement, Concrete.PrincipalStrains);

		/// <summary>
		///     Calculate maximum shear stress on crack, in MPa.
		/// </summary>
		protected Pressure MaximumShearOnCrack() => MaximumShearOnCrack(CrackOpening(), Concrete.Parameters);

		#region Interface Implementations

		/// <inheritdoc />
		public abstract Membrane Clone();

		/// <summary>
		///     Compare two <see cref="Membrane" /> objects.
		///     <para>Returns true if <see cref="Concrete" /> and <see cref="Reinforcement" /> are equal.</para>
		/// </summary>
		/// <param name="other">The other <see cref="Membrane" /> object.</param>
		public bool Equals(Membrane? other) => other is not null && Concrete == other.Concrete && Reinforcement == other.Reinforcement;

		#endregion

		#region Object override

		/// <inheritdoc />
		public override bool Equals(object? obj) => obj is Membrane other && Equals(other);

		/// <inheritdoc />
		public override int GetHashCode() => Concrete.GetHashCode() + Reinforcement.GetHashCode();

		/// <inheritdoc />
		public override string ToString() =>
			"Membrane\n" +
			$"Width = {Width}\n" +
			$"{Concrete}\n" +
			$"{Reinforcement}\n";

		#endregion

		#endregion

		#region Operators

		/// <summary>
		///     Returns true if parameters and constitutive model are equal.
		/// </summary>
		public static bool operator ==(Membrane? left, Membrane? right) => left.IsEqualTo(right);

		/// <summary>
		///     Returns true if parameters and constitutive model are different.
		/// </summary>
		public static bool operator !=(Membrane? left, Membrane? right) => left.IsNotEqualTo(right);

		#endregion

	}
}