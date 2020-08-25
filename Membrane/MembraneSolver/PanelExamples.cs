using Material.Concrete;
using Material.Reinforcement;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;
using Concrete      = Material.Concrete.BiaxialConcrete;

namespace RCMembrane
{
    /// <summary>
    /// Class with some panel examples.
    /// </summary>
    public static class PanelExamples
    {
		// Panels from Collins et al. (1985).
	    #region Collins1985

        /// <summary>
        /// PV1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PV1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(34.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(483, 200000);
		    var steelY = new Steel(283, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 6.35, 53.86, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(23.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(428, 200000);

            // Get reinforcement
            var reinforcement = new Reinforcement(2.03, 51.37, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(26.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(662, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(3.3, 50.91, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(26.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(242, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(3.45, 25.2, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV5 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV5(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(28.3, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(621, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(5.79, 101.66, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV6 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV6(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(29.8, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(266, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV7 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV7(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(31, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(453, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV8 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV8(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(29.8, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(462, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(5.44, 25.37, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV9 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV9(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(11.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(455, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV10 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV10(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(14.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(276, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 4.7, 49.57, steelXY, 70);

			return
				Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV11 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV11(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(15.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(235, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 5.44, 50.7, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV12 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV12(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(16, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(468, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 3.18, 50.43, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV13 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV13(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(18.2, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(248, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 0, 0, null, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV14 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV14(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(20.4, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(455, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV15 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV15(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(21.7, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(255, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(4.09, 50.73, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV16 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PV16(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT,
	        bool considerCrackSlip = true) => PV15(constitutiveModel, considerCrackSlip);

        /// <summary>
        /// PV17 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PV17(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var concrete = new Concrete(18.6, 6, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelXY = new Steel(255, 200000);

	        // Get reinforcement
	        var reinforcement = new Reinforcement(4.09, 50.73, steelXY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV18 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PV18(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var concrete = new Concrete(19.5, 6, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(431, 200000);
	        var steelY = new Steel(412, 200000);

	        // Get reinforcement
	        var reinforcement = new Reinforcement(6.35, 50.55, steelX, 2.67, 50, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV19 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PV19(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var concrete = new Concrete(19, 6, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(458, 200000);
	        var steelY = new Steel(299, 200000);

	        // Get reinforcement
	        var reinforcement = new Reinforcement(6.35, 50.55, steelX, 4.01, 50.82, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV20 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV20(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(460, 200000);
		    var steelY = new Steel(297, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV21 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV21(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(458, 200000);
		    var steelY = new Steel(302, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 5.41, 50.52, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV22 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV22(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19.6, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(458, 200000);
		    var steelY = new Steel(420, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 5.87, 50.87, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV23 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV23(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(20.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(518, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV24 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV24(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(23.8, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(492, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV25 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV25(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19.2, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(466, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV26 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV26(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(21.3, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(456, 200000);
		    var steelY = new Steel(463, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV27 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV27(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(20.5, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(442, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV28 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV28(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(483, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV29 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV29(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(21.7, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(441, 200000);
		    var steelY = new Steel(324, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        /// <summary>
        /// PV30 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
	    public static Membrane PV30(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
	    {
			// Get concrete parameter model
			var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

			// Initiate concrete
		    var concrete = new Concrete(19.1, 6, parModel, constitutiveModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(437, 200000);
		    var steelY = new Steel(472, 200000);

		    // Get reinforcement
		    var reinforcement = new Reinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
        }

        #endregion


    }
}
