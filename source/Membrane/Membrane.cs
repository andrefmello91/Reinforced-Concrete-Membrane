using System;
using System.Linq;
using Extensions;
using Material.Concrete;
using Material.Concrete.Biaxial;
using Material.Reinforcement.Biaxial;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using OnPlaneComponents;
using UnitsNet;
using UnitsNet.Units;
using static Extensions.UnitExtensions;

#nullable enable

namespace RCMembrane
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
		public StressState AverageStresses  => Reinforcement is null ? Concrete.Stresses : Concrete.Stresses + Reinforcement.Stresses;

		/// <summary>
		///     Get <see cref="BiaxialConcrete" /> of the membrane element.
		/// </summary>
		public BiaxialConcrete Concrete { get; protected set; }

		/// <summary>
		///     Get the <see cref="StrainState" /> in concrete.
		/// </summary>
		public abstract StrainState ConcreteStrains { get; }

		/// <summary>
		///     Get initial <see cref="Membrane" /> stiffness <see cref="Matrix" />.
		/// </summary>
		/// <inheritdoc cref="BiaxialConcrete.InitialStiffness"/>
		public Matrix<double> InitialStiffness => Reinforcement is null ? Concrete.InitialStiffness : Concrete.InitialStiffness + Reinforcement.InitialStiffness;

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
			: this (concreteParameters, reinforcement, Length.From(width, unit), model)
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
			Concrete      = new BiaxialConcrete(concreteParameters, model);
			Reinforcement = reinforcement;

			Width = width;

			// Set initial strains
			AverageStrains = StrainState.Zero;
		}

		#endregion

		#region  Methods

		/// <summary>
		///     Read membrane element based on concrete constitutive model.
		/// </summary>
		/// <param name="concreteParameters">Concrete <see cref="IParameters" /> object.</param>
		/// <param name="reinforcement"><see cref="WebReinforcement" /> object .</param>
		/// <param name="width">The width of cross-section, in mm.</param>
		/// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane" /> (default: true)</param>
		public static Membrane Read(IParameters concreteParameters, WebReinforcement? reinforcement, double width, ConstitutiveModel model = ConstitutiveModel.MCFT, bool considerCrackSlip = true) =>
			Read(concreteParameters, reinforcement, Length.FromMillimeters(width), model, considerCrackSlip);

		/// <summary>
		///     Read membrane element based on concrete constitutive model.
		/// </summary>
		/// <param name="concreteParameters">Concrete <see cref="IParameters" /> object.</param>
		/// <param name="reinforcement"><see cref="WebReinforcement" /> object .</param>
		/// <param name="width">The width of cross-section.</param>
		/// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane" /> (default: true)</param>
		public static Membrane Read(IParameters concreteParameters, WebReinforcement? reinforcement, Length width, ConstitutiveModel model = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
		{
			if (model is ConstitutiveModel.MCFT)
				return new MCFTMembrane(concreteParameters, reinforcement, width);

			return new DSFMMembrane(concreteParameters, reinforcement, width, considerCrackSlip);
		}

		/// <summary>
		///     Calculate the average crack opening.
		/// </summary>
		/// <param name="membrane">The <see cref="Membrane" /> object.</param>
		public static Length CrackOpening(Membrane membrane) =>
			membrane.Concrete.PrincipalStrains.Epsilon1 <= 0 || membrane.Concrete.PrincipalStrains.Epsilon1.ApproxZero(1E-9)
				? Length.Zero
				: membrane.Concrete.PrincipalStrains.Epsilon1 * CrackSpacing(membrane);

		/// <summary>
		///     Calculate the crack spacing in principal strain direction.
		/// </summary>
		/// <inheritdoc cref="CrackOpening" />
		public static Length CrackSpacing(Membrane membrane)
		{
			// Get the angles
			var (cosThetaC, sinThetaC) = membrane.Concrete.PrincipalStrains.Theta1.DirectionCosines(true);

			// Calculate crack spacings in X and Y
			double
				smx = membrane.Reinforcement?.DirectionX?.CrackSpacing().Millimeters ?? 21,
				smy = membrane.Reinforcement?.DirectionY?.CrackSpacing().Millimeters ?? 21,
				sm = 1.00 / (sinThetaC / smx + cosThetaC / smy);

			// Calculate crack spacing
			return
				Length.FromMillimeters(sm);
		}

		/// <summary>
		///     Calculate <see cref="AverageStresses" /> and <see cref="Stiffness" />, given a known <see cref="StrainState" />.
		/// </summary>
		/// <param name="appliedStrains">Current applied <see cref="StrainState" />.</param>
		public abstract void Calculate(StrainState appliedStrains);

		/// <summary>
		///     Limit tensile principal stress by crack check procedure, by Bentz (2000).
		/// </summary>
		public void CrackCheck()
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
		///     Calculate maximum shear stress on crack, in MPa.
		/// </summary>
		public Pressure MaximumShearOnCrack() => MaximumShearOnCrack(CrackOpening());

		/// <summary>
		///     Calculate maximum shear stress on crack, in MPa.
		/// </summary>
		/// <param name="crackOpening">Average crack opening, in mm.</param>
		public Pressure MaximumShearOnCrack(Length crackOpening)
		{
			var vcimax = 0.18 * Concrete.Parameters.Strength.Megapascals.Sqrt()
			             / (0.31 + 24 * crackOpening.Millimeters / (Concrete.Parameters.AggregateDiameter.Millimeters + 16));

			return Pressure.FromMegapascals(vcimax);
		}

		public abstract Membrane Clone();

		/// <inheritdoc cref="CrackSpacing" />
		protected Length CrackSpacing() => CrackSpacing(this);

		/// <inheritdoc cref="CrackOpening" />
		protected Length CrackOpening() => CrackOpening(this);

		/// <summary>
		///     Compare two <see cref="Membrane" /> objects.
		///     <para>Returns true if <see cref="Concrete" /> and <see cref="Reinforcement" /> are equal.</para>
		/// </summary>
		/// <param name="other">The other <see cref="Membrane" /> object.</param>
		public virtual bool Equals(Membrane? other) => !(other is null) && Concrete == other.Concrete && Reinforcement == other.Reinforcement;

		public override string ToString() =>
			"Membrane\n" +
			$"Width = {Width}\n" +
			$"{Concrete}\n" +
			$"{Reinforcement}\n";

		public override bool Equals(object? obj) => obj is Membrane other && Equals(other);

		public override int GetHashCode() => Concrete.GetHashCode() + Reinforcement.GetHashCode();

		#endregion

		#region Operators

		/// <summary>
		///     Returns true if parameters and constitutive model are equal.
		/// </summary>
		public static bool operator == (Membrane left, Membrane right) => !(left is null) && left.Equals(right);

		/// <summary>
		///     Returns true if parameters and constitutive model are different.
		/// </summary>
		public static bool operator != (Membrane left, Membrane right) => !(left is null) && !left.Equals(right);

		#endregion
	}
}