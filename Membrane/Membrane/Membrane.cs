using System;
using System.Linq;
using Extensions.Number;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Material.Concrete;
using Material.Concrete.Biaxial;
using Material.Reinforcement;
using Material.Reinforcement.Biaxial;
using MathNet.Numerics;
using OnPlaneComponents;
using UnitsNet;

namespace RCMembrane
{
	/// <summary>
    /// Membrane element base class.
    /// </summary>
    public abstract class Membrane : IEquatable<Membrane>
    {
		// Auxiliary fields
		private Length _width;

		/// <summary>
        /// Get <see cref="BiaxialConcrete"/> of the membrane element.
        /// </summary>
        public BiaxialConcrete Concrete { get; protected set; }

        /// <summary>
        /// Get/set <see cref="WebReinforcement"/> of the membrane element.
		/// </summary>
        public WebReinforcement Reinforcement { get; }

		/// <summary>
        /// Get/set the average <see cref="StrainState"/> in the membrane element.
        /// </summary>
        public StrainState AverageStrains { get; protected set; }

		/// <summary>
		/// Get/set the average <see cref="PrincipalStrainState"/>.
		/// </summary>
		public virtual PrincipalStrainState AveragePrincipalStrains { get; protected set; }

        /// <summary>
        /// Get the <see cref="StrainState"/> in concrete.
        /// </summary>
        public abstract StrainState ConcreteStrains { get; }

        /// <summary>
        /// Get the width of the membrane element, in mm.
        /// </summary>
        public double Width => _width.Millimeters;

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        protected Membrane(BiaxialConcrete concrete, WebReinforcement reinforcement, double width)
			: this (concrete.Parameters, reinforcement, width, concrete.Model)
        {
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section.</param>
        protected Membrane(BiaxialConcrete concrete, WebReinforcement reinforcement, Length width)
			: this (concrete.Parameters, reinforcement, width, concrete.Model)
        {
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        /// <param name="model">Concrete <see cref="ConstitutiveModel"/>.</param>
        protected Membrane(Parameters concreteParameters, WebReinforcement reinforcement, double width, ConstitutiveModel model)
			: this (concreteParameters, reinforcement, Length.FromMillimeters(width), model)
        {
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section.</param>
        /// <param name="model">Concrete <see cref="ConstitutiveModel"/>.</param>
        protected Membrane(Parameters concreteParameters, WebReinforcement reinforcement, Length width, ConstitutiveModel model)
        {
            // Initiate new materials
			Concrete      = new BiaxialConcrete(concreteParameters, model);
            Reinforcement = reinforcement.Copy();

            _width = width;

            // Set initial strains
            AverageStrains = StrainState.Zero;
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true)</param>
        public static Membrane Read(BiaxialConcrete concrete, WebReinforcement reinforcement, double width, bool considerCrackSlip = true) =>
	        Read(concrete, reinforcement, Length.FromMillimeters(width), considerCrackSlip);

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true)</param>
        public static Membrane Read(BiaxialConcrete concrete, WebReinforcement reinforcement, Length width, bool considerCrackSlip = true)
        {
			if (concrete.Model is ConstitutiveModel.MCFT)
				return new MCFTMembrane(concrete, reinforcement, width);

			return new DSFMMembrane(concrete, reinforcement, width, considerCrackSlip);
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true)</param>
        public static Membrane Read(Parameters concreteParameters, WebReinforcement reinforcement, double width, ConstitutiveModel model = ConstitutiveModel.MCFT, bool considerCrackSlip = true) =>
	        Read(concreteParameters, reinforcement, Length.FromMillimeters(width), model, considerCrackSlip);

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="reinforcement"><see cref="WebReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true)</param>
        public static Membrane Read(Parameters concreteParameters, WebReinforcement reinforcement, Length width, ConstitutiveModel model = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        if (model is ConstitutiveModel.MCFT)
		        return new MCFTMembrane(concreteParameters, reinforcement, width);

	        return new DSFMMembrane(concreteParameters, reinforcement, width, considerCrackSlip);
        }

        /// <summary>
        /// Get average <see cref="StressState"/>, in MPa.
        /// </summary>
        public StressState AverageStresses  => Reinforcement is null ? Concrete.Stresses : Concrete.Stresses + Reinforcement.Stresses;

        /// <summary>
        /// Get current <see cref="Membrane"/> stiffness <see cref="Matrix"/>.
        /// </summary>
        public Matrix<double> Stiffness => Reinforcement is null ? Concrete.Stiffness : Concrete.Stiffness + Reinforcement.Stiffness;

        /// <summary>
        /// Get initial <see cref="Membrane"/> stiffness <see cref="Matrix"/>.
        /// </summary>
        public Matrix<double> InitialStiffness => Reinforcement is null ? Concrete.InitialStiffness : Concrete.InitialStiffness + Reinforcement.InitialStiffness;

        /// <summary>
        /// Calculate <see cref="AverageStresses"/> and <see cref="Stiffness"/>, given a known <see cref="StrainState"/>.
        /// </summary>
        /// <param name="appliedStrains">Current applied <see cref="StrainState"/>.</param>
        public abstract void Calculate(StrainState appliedStrains);

		/// <summary>
        /// Calculate the crack spacing at <paramref name="direction"/> (in mm), according to Kaklauskas (2019) expression.
        /// <para>sm = 21 mm + 0.155 phi / rho</para>
        /// </summary>
        /// <param name="direction">The <see cref="WebReinforcementDirection"/>.</param>
        private double CrackSpacing(WebReinforcementDirection direction) =>
			direction is null || direction.BarDiameter.ApproxZero() || direction.Ratio.ApproxZero()
				? 21
				: 21 + 0.155 * direction.BarDiameter / direction.Ratio;

		/// <summary>
        /// Calculate the crack spacing in principal strain direction.
        /// </summary>
        protected double CrackSpacing()
        {
	        // Get the angles
	        var (cosThetaC, sinThetaC) = Concrete.PrincipalStrains.Theta1.DirectionCosines(true);

			// Calculate crack spacings in X and Y
			double
				smx = CrackSpacing(Reinforcement.DirectionX),
				smy = CrackSpacing(Reinforcement.DirectionY);

	        // Calculate crack spacing
	        return
		        1 / (sinThetaC / smx + cosThetaC / smy);
        }

		/// <summary>
        /// Calculate the average crack opening, in mm.
        /// </summary>
		protected double CrackOpening() => Concrete.PrincipalStrains.Epsilon1 * CrackSpacing();

        /// <summary>
        /// Limit tensile principal stress by crack check procedure, by Bentz (2000).
        /// </summary>
        public void CrackCheck()
        {
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

            // Get concrete tensile stress
            double f1a = Concrete.PrincipalStresses.Sigma1;

			// Get reinforcement angles related to crack
			var (thetaNx, thetaNy) = Reinforcement?.Angles(Concrete.PrincipalStrains.Theta1) ?? (Concrete.PrincipalStrains.Theta1, Concrete.PrincipalStrains.Theta1 - Constants.PiOver2);
			double
				cosNx = thetaNx.Cos(true),
				cosNy = thetaNy.Cos(true),
				tanNx = thetaNx.Tan(true),
				tanNy = thetaNy.Tan(true);

            // Reinforcement capacity reserve
            double
	            f1cx = Reinforcement?.DirectionX?.CapacityReserve ?? 0,
	            f1cy = Reinforcement?.DirectionY?.CapacityReserve ?? 0;

            // Maximum possible shear on crack interface
            double vcimaxA = MaximumShearOnCrack();

            // Maximum possible shear for biaxial yielding
            double vcimaxB = (f1cx - f1cy).Abs() / (tanNx + tanNy);

            // Maximum shear on crack
            double vcimax = Math.Min(vcimaxA, vcimaxB);

            // Biaxial yielding condition
            double f1b = f1cx * cosNx * cosNx + f1cy * cosNy * cosNy;

            // Maximum tensile stress for equilibrium in X and Y
            double
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
		/// Calculate maximum shear stress on crack, in MPa.
		/// </summary>
		public double MaximumShearOnCrack() => MaximumShearOnCrack(Concrete.PrincipalStrains.Epsilon1 * CrackSpacing());

        /// <summary>
        /// Calculate maximum shear stress on crack, in MPa.
        /// </summary>
        /// <param name="crackOpening">Average crack opening, in mm.</param>
        public double MaximumShearOnCrack(double crackOpening) => 0.18 * Concrete.fc.Sqrt() / (0.31 + 24 * crackOpening / (Concrete.AggregateDiameter + 16));

        /// <summary>
        /// Calculate reference length, in mm.
        /// </summary>
        public double ReferenceLength() => 0.5 * CrackSpacing();

        public override string ToString()
        {
	        return
		        "Membrane\n" +
		        $"Width = {_width}\n" +
		        $"{Concrete}\n" +
		        $"{Reinforcement}\n";
        }

        /// <summary>
        /// Compare two <see cref="Membrane"/> objects.
        /// <para>Returns true if <see cref="Concrete"/> and <see cref="Reinforcement"/> are equal.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Membrane"/> object.</param>
        public virtual bool Equals(Membrane other) => !(other is null) && Concrete == other.Concrete && Reinforcement == other.Reinforcement;

        public override bool Equals(object obj) => obj is Membrane other && Equals(other);

        public override int GetHashCode() => Concrete.GetHashCode() + Reinforcement.GetHashCode();

        /// <summary>
        /// Returns true if parameters and constitutive model are equal.
        /// </summary>
        public static bool operator == (Membrane left, Membrane right) => !(left is null) && left.Equals(right);

        /// <summary>
        /// Returns true if parameters and constitutive model are different.
        /// </summary>
        public static bool operator != (Membrane left, Membrane right) => !(left is null) && !left.Equals(right);

    }
}