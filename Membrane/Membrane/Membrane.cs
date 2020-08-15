using System;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Material.Concrete;
using MathNet.Numerics.Data.Text;
using Parameters    = Material.Concrete.Parameters;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;

namespace RCMembrane
{
	/// <summary>
    /// Membrane element base class.
    /// </summary>
    public abstract class Membrane
    {
        // Properties
        public BiaxialConcrete          Concrete               { get; set; }
        public Reinforcement            Reinforcement          { get; }
        public (bool S, string Message) Stop                   { get; set; }
        public Vector<double>           Strains                { get; set; }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concrete">Biaxial concrete object <see cref="BiaxialConcrete"/>.</param>
        /// <param name="reinforcement">Biaxial reinforcement object <see cref="Material.Reinforcement.BiaxialReinforcement"/>.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        public Membrane(BiaxialConcrete concrete, Reinforcement reinforcement, double sectionWidth)
        {
            // Get reinforcement
            var diams = reinforcement.BarDiameter;
            var spcs  = reinforcement.BarSpacing;
            var steel = reinforcement.Steel;

            // Initiate new materials
            Reinforcement = new Reinforcement(diams, spcs, steel, sectionWidth);

            // Set initial strains
            Strains = Vector<double>.Build.Dense(3);
        }

        /// <summary>
        /// Base membrane element constructor.
        /// </summary>
        /// <param name="concreteParameters">Concrete parameters object <see cref="Parameters"/>.</param>
        /// <param name="concreteConstitutive">Concrete constitutive object <see cref="Constitutive"/>.</param>
        /// <param name="reinforcement">Biaxial reinforcement object <see cref="Material.Reinforcement.BiaxialReinforcement"/>.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        public Membrane(Parameters concreteParameters, Constitutive concreteConstitutive, Reinforcement reinforcement, double sectionWidth)
        {
            // Get reinforcement
            var diams = reinforcement.BarDiameter;
            var spcs  = reinforcement.BarSpacing;
            var steel = reinforcement.Steel;

            // Initiate new materials
            Reinforcement = new Reinforcement(diams, spcs, steel, sectionWidth);

            // Set initial strains
            Strains = Vector<double>.Build.Dense(3);
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concrete">Biaxial concrete object <see cref="BiaxialConcrete"/>.</param>
        /// <param name="reinforcement">Biaxial reinforcement object <see cref="Material.Reinforcement.BiaxialReinforcement"/>.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for DSFM (default: true)</param>
        public static Membrane ReadMembrane(BiaxialConcrete concrete, Reinforcement reinforcement, double sectionWidth, bool considerCrackSlip = true)
        {
			if (concrete.Constitutive is MCFTConstitutive)
				return new MCFTMembrane(concrete, reinforcement, sectionWidth);

			return new DSFMMembrane(concrete, reinforcement, sectionWidth, considerCrackSlip);
        }

        /// <summary>
        /// Read membrane element based on concrete constitutive model.
        /// </summary>
        /// <param name="concreteParameters">Concrete parameters object <see cref="Parameters"/>.</param>
        /// <param name="concreteConstitutive">Concrete constitutive object <see cref="Constitutive"/>.</param>
        /// <param name="reinforcement">Biaxial reinforcement object <see cref="Material.Reinforcement.BiaxialReinforcement"/>.</param>
        /// <param name="sectionWidth">The width of cross-section, in mm.</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for DSFM (default: true)</param>
        public static Membrane ReadMembrane(Parameters concreteParameters, Constitutive concreteConstitutive, Reinforcement reinforcement, double sectionWidth, bool considerCrackSlip = true)
        {
			if (concreteConstitutive is MCFTConstitutive)
				return new MCFTMembrane(concreteParameters, concreteConstitutive, reinforcement, sectionWidth);

			return new DSFMMembrane(concreteParameters, concreteConstitutive, reinforcement, sectionWidth, considerCrackSlip);
        }

        // Get steel parameters
        public double fyx  => Reinforcement.Steel.X.YieldStress;
        public double Esxi => Reinforcement.Steel.X.ElasticModule;
        public double fyy  => Reinforcement.Steel.Y.YieldStress;
        public double Esyi => Reinforcement.Steel.Y.ElasticModule;

        // Get reinforcement
        public double phiX => Reinforcement.BarDiameter.X;
        public double phiY => Reinforcement.BarDiameter.Y;
        public double psx  => Reinforcement.Ratio.X;
        public double psy  => Reinforcement.Ratio.Y;

        // Calculate crack spacings
        public double smx => phiX / (5.4 * psx);
        public double smy => phiY / (5.4 * psy);

        /// <summary>
        /// Get current membrane stiffness.
        /// </summary>
        public Matrix<double> Stiffness => Concrete.Stiffness + Reinforcement.Stiffness;

        /// <summary>
        /// Get current stresses, in MPa.
        /// </summary>
        public Vector<double> Stresses  => Concrete.Stresses + Reinforcement.Stresses;

		/// <summary>
        /// Calculate stresses and the membrane stiffness, given strains.
        /// </summary>
        /// <param name="appliedStrains">Current strains.</param>
        /// <param name="loadStep">Current load step.</param>
        /// <param name="iteration">Current iteration.</param>
        public abstract void Calculate(Vector<double> appliedStrains, int loadStep = 0, int iteration = 0);

        /// <summary>
        /// Calculate initial membrane stiffness.
        /// </summary>
        public Matrix<double> InitialStiffness() => Concrete.InitialStiffness() + Reinforcement.InitialStiffness();

		/// <summary>
        /// Limit tensile principal stress by crack check procedure, by Bentz (2000).
        /// </summary>
        /// <param name="theta2">Principal compressive strain angle, in radians.</param>
        public void CrackCheck(double? theta2 = null)
        {
			// Verify if concrete is cracked
			if (!Concrete.Cracked)
				return;

            // Get the values
            double theta = theta2 ?? Concrete.PrincipalAngles.theta2;
            double ec1 = Concrete.PrincipalStrains.ec1;
            (double fsx, double fsy) = Reinforcement.SteelStresses;
            double f1a = Concrete.PrincipalStresses.fc1;

            // Calculate thetaC sine and cosine
            var (cosTheta, sinTheta) = DirectionCosines(theta);
            double tanTheta = Tangent(theta);

            // Reinforcement capacity reserve
            double
                f1cx = psx * (fyx - fsx),
                f1cy = psy * (fyy - fsy);

            // Maximum possible shear on crack interface
            double vcimaxA = MaximumShearOnCrack(theta, ec1);

            // Maximum possible shear for biaxial yielding
            double vcimaxB = Math.Abs(f1cx - f1cy) / (tanTheta + 1 / tanTheta);

            // Maximum shear on crack
            double vcimax = Math.Min(vcimaxA, vcimaxB);

            // Biaxial yielding condition
            double f1b = f1cx * sinTheta * sinTheta + f1cy * cosTheta * cosTheta;

            // Maximum tensile stress for equilibrium in X and Y
            double
                f1c = f1cx + vcimax / tanTheta,
                f1d = f1cy + vcimax * tanTheta;

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
        /// <param name="theta2">Principal compressive strain angle, in radians.</param>
        /// <param name="ec1">Principal tensile strain.</param>
        public double MaximumShearOnCrack(double theta2, double ec1)
        {
	        // Calculate thetaC sine and cosine
	        var (cosTheta, sinTheta) = DirectionCosines(theta2);

	        // Average crack spacing and opening
	        double
		        smTheta = 1 / (sinTheta / smx + cosTheta / smy),
		        w = smTheta * ec1;

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
        /// <param name="thetaC1">Concrete principal tensile strain angle, in radians.</param>
        public double ReferenceLength(double? thetaC1 = null)
        {
			double theta = thetaC1 ?? Concrete.PrincipalAngles.theta1;

	        var (cosTheta, sinTheta) = DirectionCosines(theta);

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
        /// <returns></returns>
        public (double cos, double sin) DirectionCosines(double angle)
        {
	        double
		        cos = Trig.Cos(angle).CoerceZero(1E-6),
		        sin = Trig.Sin(angle).CoerceZero(1E-6);

	        return (cos, sin);
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