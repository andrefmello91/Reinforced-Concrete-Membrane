using System;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Material.Concrete;
using Material.Reinforcement;
using MathNet.Numerics.Data.Text;
using OnPlaneComponents;
using Parameters    = Material.Concrete.Parameters;

namespace RCMembrane
{
	/// <summary>
    /// Membrane element base class.
    /// </summary>
    public abstract class Membrane
    {
		/// <summary>
        /// Get/set <see cref="BiaxialConcrete"/> of the membrane element.
        /// </summary>
        public BiaxialConcrete          Concrete               { get; set; }

        /// <summary>
        /// Get/set <see cref="BiaxialReinforcement"/> of the membrane element.
		/// </summary>
        public BiaxialReinforcement     Reinforcement          { get; }

        /// <summary>
        /// Get/set stop parameters of the membrane element.
        /// </summary>
        public (bool S, string Message) Stop                   { get; set; }

		/// <summary>
        /// Get/set the average <see cref="StrainState"/> in the membrane element.
        /// </summary>
        public StrainState           AverageStrains                { get; set; }

		/// <summary>
        /// Get the <see cref="StrainState"/> in concrete.
        /// </summary>
        public abstract StrainState           ConcreteStrains                { get; }

		/// <summary>
        /// Get the width of the membrane element.
        /// </summary>
		public double Width { get; }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="BiaxialReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        public Membrane(BiaxialConcrete concrete, BiaxialReinforcement reinforcement, double width)
        {
            // Initiate new materials
            Concrete      = BiaxialConcrete.Copy(concrete);
            Reinforcement = BiaxialReinforcement.Copy(reinforcement);

            Width = width;

            // Set initial strains
            AverageStrains = StrainState.Zero;
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="concreteConstitutive">Concrete <see cref="Constitutive"/> object.</param>
        /// <param name="reinforcement"><see cref="BiaxialReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        public Membrane(Parameters concreteParameters, Constitutive concreteConstitutive, BiaxialReinforcement reinforcement, double width)
        {
            // Initiate new materials
			Concrete      = new BiaxialConcrete(concreteParameters, concreteConstitutive);
            Reinforcement = BiaxialReinforcement.Copy(reinforcement);

            Width = width;

            // Set initial strains
            AverageStrains = StrainState.Zero;
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concrete"><see cref="BiaxialConcrete"/> object .</param>
        /// <param name="reinforcement"><see cref="BiaxialReinforcement"/> object.</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true)</param>
        public static Membrane ReadMembrane(BiaxialConcrete concrete, BiaxialReinforcement reinforcement, double width, bool considerCrackSlip = true)
        {
			if (concrete.Constitutive is MCFTConstitutive)
				return new MCFTMembrane(concrete, reinforcement, width);

			return new DSFMMembrane(concrete, reinforcement, width, considerCrackSlip);
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concreteParameters">Concrete <see cref="Parameters"/> object.</param>
        /// <param name="concreteConstitutive">Concrete <see cref="Constitutive"/> object.</param>
        /// <param name="reinforcement"><see cref="BiaxialReinforcement"/> object .</param>
        /// <param name="width">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for DSFM (default: true)</param>
        public static Membrane ReadMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, BiaxialReinforcement reinforcement, double width, bool considerCrackSlip = true)
        {
			if (concreteConstitutive is MCFTConstitutive)
				return new MCFTMembrane(concreteParameters, concreteConstitutive, reinforcement, width);

			return new DSFMMembrane(concreteParameters, concreteConstitutive, reinforcement, width, considerCrackSlip);
        }

        /// <summary>
        /// Get crack spacing in X direction.
        /// </summary>
        protected double smx => CrackSpacing(Reinforcement.DirectionX);

        /// <summary>
        /// Get crack spacing in Y direction.
        /// </summary>
        protected double smy => CrackSpacing(Reinforcement.DirectionY);

        /// <summary>
        /// Get current membrane stiffness <see cref="Matrix"/>.
        /// </summary>
        public Matrix<double> Stiffness => Concrete.Stiffness + Reinforcement.Stiffness;

        /// <summary>
        /// Get average <see cref="StressState"/>, in MPa.
        /// </summary>
        public StressState AverageStresses  => Concrete.Stresses + Reinforcement.Stresses;

		/// <summary>
        /// Calculate <see cref="AverageStresses"/> and <see cref="Stiffness"/>, given a known <see cref="StrainState"/>.
        /// </summary>
        /// <param name="appliedStrains">Current applied <see cref="StrainState"/>.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="iteration">Current iteration.</param>
        public abstract void Calculate(StrainState appliedStrains, int loadStep = 0, int iteration = 0);

        /// <summary>
        /// Calculate initial membrane stiffness <see cref="Matrix"/>.
        /// </summary>
        public Matrix<double> InitialStiffness() => Concrete.InitialStiffness() + Reinforcement.InitialStiffness();

		/// <summary>
        /// Calculate the crack spacing.
        /// </summary>
        /// <param name="direction">The <see cref="WebReinforcementDirection"/>.</param>
        /// <returns></returns>
        private double CrackSpacing(WebReinforcementDirection direction)
        {
	        if (direction is null)
		        return 0;

	        double
		        phi = direction.BarDiameter,
		        ps  = direction.Ratio,
		        sm  = phi / (5.4 * ps);

	        if (double.IsNaN(sm))
		        sm = 0;

	        return sm;
        }

        /// <summary>
        /// Limit tensile principal stress by crack check procedure, by Bentz (2000).
        /// </summary>
        public void CrackCheck()
        {
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

            // Get the values
            double
	            theta1 = Concrete.PrincipalStrains.Theta1,
				ec1    = Concrete.PrincipalStrains.Epsilon1,
				f1a    = Concrete.PrincipalStresses.Sigma1;

            // Calculate thetaC sine and cosine
            var (cosTheta, sinTheta) = DirectionCosines(theta1);
            double tanTheta = Tangent(theta1);

            // Reinforcement capacity reserve
            double
	            f1cx = Reinforcement?.DirectionX?.CapacityReserve ?? 0,
	            f1cy = Reinforcement?.DirectionY?.CapacityReserve ?? 0;

            // Maximum possible shear on crack interface
            double vcimaxA = MaximumShearOnCrack(theta1, ec1);

            // Maximum possible shear for biaxial yielding
            double vcimaxB = Math.Abs(f1cx - f1cy) / (tanTheta + 1 / tanTheta);

            // Maximum shear on crack
            double vcimax = Math.Min(vcimaxA, vcimaxB);

            // Biaxial yielding condition
            double f1b = f1cx * cosTheta * cosTheta + f1cy * sinTheta * sinTheta;

            // Maximum tensile stress for equilibrium in X and Y
            double
                f1c = f1cx + vcimax * tanTheta,
                f1d = f1cy + vcimax / tanTheta;

            // Calculate the minimum tensile stress
            var f1List = new[] { f1a, f1b, f1c, f1d };
            var fc1 = f1List.Min();

            // Set to concrete
            if (fc1 < f1a)
                Concrete.SetTensileStress(fc1);
        }

        /// <summary>
        /// Calculate maximum shear stress on crack, in MPa.
        /// </summary>
        /// <param name="theta1">Principal compressive strain angle, in radians.</param>
        /// <param name="ec1">Principal tensile strain.</param>
        public double MaximumShearOnCrack(double theta1, double ec1)
        {
	        // Calculate thetaC sine and cosine
	        var (cosTheta, sinTheta) = DirectionCosines(theta1);

	        // Average crack spacing and opening
	        double
		        smTheta = 1 / (Math.Abs(sinTheta) / smx + Math.Abs(cosTheta) / smy),
		        w       = smTheta * ec1;

	        // Maximum possible shear on crack interface
	        return
		        MaximumShearOnCrack(w);
        }

        /// <summary>
        /// Calculate maximum shear stress on crack, in MPa.
        /// </summary>
        /// <param name="w">Average crack opening, in mm.</param>
        /// <returns></returns>
        public double MaximumShearOnCrack(double w)
        {
	        double
		        fc    = Concrete.fc,
		        phiAg = Concrete.AggregateDiameter;

	        // Maximum possible shear on crack interface
	        return
		        0.18 * Math.Sqrt(fc) / (0.31 + 24 * w / (phiAg + 16));
        }

        /// <summary>
        /// Calculate reference length, in mm.
        /// </summary>
        public double ReferenceLength()
        {
			double theta1 = Concrete.PrincipalStrains.Theta1;

	        var (cosTheta, sinTheta) = DirectionCosines(theta1);

	        return
		        0.5 / (Math.Abs(sinTheta) / smx + Math.Abs(cosTheta) / smy);
        }

        /// <summary>
        /// Verify if a number is zero (true if is not zero).
        /// </summary>
        /// <param name="number">The number.</param>
        public bool NotZero(double number) => number != 0;

        /// <summary>
        /// Calculate the direction cosines (cos, sin) of an angle.
        /// </summary>
        /// <param name="angle">Angle, in radians.</param>
        /// <param name="absoluteValue">Return cosine and sine in absolute values? (default: false).</param>
        public (double cos, double sin) DirectionCosines(double angle, bool absoluteValue = false)
        {
	        double
		        cos = Trig.Cos(angle).CoerceZero(1E-6),
		        sin = Trig.Sin(angle).CoerceZero(1E-6);

			if (!absoluteValue)
				return (cos, sin);

			return (Math.Abs(cos), Math.Abs(sin));
        }

        /// <summary>
        /// Calculate tangent of an angle.
        /// </summary>
        /// <param name="angle">Angle, in radians.</param>
        public static double Tangent(double angle)
        {
	        double tan;

	        // Calculate the tangent, return 0 if 90 or 270 degrees
	        if (angle == Constants.PiOver2 || angle == Constants.Pi3Over2)
		        tan = 1.633e16;

	        else
		        tan = Trig.Cos(angle).CoerceZero(1E-6);

	        return tan;
        }
    }
}