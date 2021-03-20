using andrefmello91.Material.Concrete;
using andrefmello91.Material.Reinforcement;

namespace andrefmello91.ReinforcedConcreteMembrane.Examples
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
			var parameters = new Parameters(34.5, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(483, 200000);
		    var steelY = new Steel(283, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 6.35, 53.86, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(23.5, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(428, 200000);

            // Get reinforcement
            var reinforcement = new WebReinforcement(2.03, 51.37, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(26.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(662, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(3.3, 50.91, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(26.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(242, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(3.45, 25.2, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(28.3, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(621, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(5.79, 101.66, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(29.8, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(266, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(31, 6, parModel);
            var steelXY = new Steel(453, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

            return
	            Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(29.8, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(462, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(5.44, 25.37, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(11.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(455, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(14.5, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(276, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 4.7, 49.57, steelXY, 70);

			return
				Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(15.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(235, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 5.44, 50.7, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(16, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(468, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 3.18, 50.43, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(18.2, 6, parModel);
            var steelXY = new Steel(248, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 0, 0, null, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(20.4, 6, parModel);
            var steelXY = new Steel(455, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(21.7, 6, parModel);

		    // Initiate steel for each direction
		    var steelXY = new Steel(255, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(4.09, 50.73, steelXY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
	        var parameters = new Parameters(18.6, 6, parModel);
            var steelXY = new Steel(255, 200000);
            var reinforcement = new WebReinforcement(4.09, 50.73, steelXY, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(19.5, 6, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(431, 200000);
	        var steelY = new Steel(412, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 2.67, 50, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
			var parameters = new Parameters(19, 6, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(458, 200000);
	        var steelY = new Steel(299, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.01, 50.82, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(460, 200000);
		    var steelY = new Steel(297, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19.5, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(458, 200000);
		    var steelY = new Steel(302, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 5.41, 50.52, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19.6, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(458, 200000);
		    var steelY = new Steel(420, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 5.87, 50.87, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(20.5, 6, parModel);
            var steelXY = new Steel(518, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(23.8, 6, parModel);
            var steelXY = new Steel(492, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19.2, 6, parModel);
            var steelXY = new Steel(466, 200000);
            var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);
            return Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(21.3, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(456, 200000);
		    var steelY = new Steel(463, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(20.5, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(442, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(483, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(21.7, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(441, 200000);
		    var steelY = new Steel(324, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
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
		    var parameters = new Parameters(19.1, 6, parModel);

		    // Initiate steel for each direction
		    var steelX = new Steel(437, 200000);
		    var steelY = new Steel(472, 200000);

		    // Get reinforcement
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        #endregion

        // Panels from Vecchio et. al. (1994)

        #region Vecchio1994

        /// <summary>
        /// PHS1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(72.2, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 0, 0, null, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(66.1, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(58.4, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(68.5, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS5 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS5(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(52.1, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS6 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS6(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(49.7, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS7 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS7(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(53.6, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }
		
        /// <summary>
        /// PHS8 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS8(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(55.9, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 59.21, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PHS9 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS9(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(56, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }
		
        /// <summary>
        /// PHS10 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PHS10(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(51.4, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 59.21, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PA1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PA1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(49.9, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(522, 200000);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89.54, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PA2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PA2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(43, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(522, 200000);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89.54, steelY, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }


        #endregion

        // Panels from Bhide and Collins (1989)
        #region Bhide1989

        /// <summary>
        /// PB4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(16.4, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB6 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB6(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(17.7, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB7 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB7(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(20.2, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB8 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB8(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(20.4, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB10 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB10(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(24, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB11 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB11(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(25.9, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB12 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB12(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(23.1, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB14 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB14(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.1, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(489, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB15 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB15(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(38.4, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(485, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB16 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB16(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.7, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(502, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB17 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB17(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.6, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(502, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB18 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB18(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(25.3, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(402, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB19 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB19(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(20, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(411, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB20 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB20(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(21.7, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(424, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB21 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB21(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(21.8, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(402, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB22 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB22(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(17.6, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB28 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB28(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(22.7, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(426, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 36.72, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB29 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB29(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.6, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(496, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PB30 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PB30(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(40.4, 9.5, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(496, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        #endregion

        // Panels from Xie et. al. (2011)
        #region Xie2011

        /// <summary>
        /// PL1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(38.5, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PL2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(38.2, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PL3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(42, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PL4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(43.1, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PL5 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL5(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(38.1, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PL6 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PL6(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(43.5, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.Read(parameters, reinforcement, 73, constitutiveModel, considerCrackSlip);
        }

        #endregion

        // Panels from Pang (1995)

        #region Pang1995

        /// <summary>
        /// A1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane A1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(42.2, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(444);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(10, 148.06, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// A2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane A2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.2, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(462);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(15, 166.43, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// A3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane A3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(41.6, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(446);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(20, 197.31, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// A4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane A4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(42.4, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(469);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25, 184.96, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(45.2, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(469);
	        var steelY = new Steel(444);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(15, 166.43, steelX, 10, 148.06, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(44, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(446);
	        var steelY = new Steel(462);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(20, 197.31, steelX, 15, 166.43, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(44.7, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(446);
	        var steelY = new Steel(444);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(20, 197.31, steelX, 10, 148.06, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(44.9, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(469);
	        var steelY = new Steel(444);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25, 184.96, steelX, 10, 148.06, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B5 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B5(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(42.8, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(469);
	        var steelY = new Steel(462);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25, 184.96, steelX, 15, 166.43, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// B6 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane B6(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(42.9, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(469);
	        var steelY = new Steel(446);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25, 184.96, steelX, 20, 197.31, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }


        #endregion

        // Panels from Zhang (1998)

        #region Zhang1998

        /// <summary>
        /// VA0 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VA0(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(98.8, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(11.3, 188, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VA1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VA1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(95.1, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(11.3, 94, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VA2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VA2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(98.2, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(409, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(16, 94, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VA3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VA3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(94.6, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(455, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(19.5, 94, steel, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VA4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VA4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(103.1, 19, parModel);

	        // Initiate steel for each direction
	        var steel = new Steel(470, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25.2, 94, steel, 203);

	        return
		        Membrane.Read(parameters, reinforcement, 203, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VB1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VB1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(98.2, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(409, 200000);
	        var steelY = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(16, 94, steelX, 11.3, 94, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VB2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VB2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(97.6, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(455, 200000);
	        var steelY = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(19.5, 94, steelX, 11.3, 94, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VB3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VB3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(102.3, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(470, 200000);
	        var steelY = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(25.2, 94, steelX, 11.3, 94, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// VB4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane VB4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(96.9, 19, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(455, 200000);
	        var steelY = new Steel(445, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(19.5, 188, steelX, 11.3, 188, steelY, 178);

	        return
		        Membrane.Read(parameters, reinforcement, 178, constitutiveModel, considerCrackSlip);
        }

        #endregion

        // Panels from Marti (1992)

        #region Marti1992

        /// <summary>
        /// PP1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PP1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(27, 13, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(479);
	        var steelY = new Steel(480);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(11.3, 108, steelX, 19.5, 108, steelY, 287);

	        return
		        Membrane.Read(parameters, reinforcement, 287, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PP2 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PP2(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(28.1, 13, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(486);
	        var steelY = new Steel(480);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(11.3, 108, steelX, 16, 108, steelY, 287);

	        return
		        Membrane.Read(parameters, reinforcement, 287, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PP3 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PP3(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(27.7, 13, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(480);
	        var steelY = new Steel(480);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(11.3, 108, steelX, 11.3, 108, steelY, 287);

	        return
		        Membrane.Read(parameters, reinforcement, 287, constitutiveModel, considerCrackSlip);
        }


        #endregion

        // Panels from Vecchio (1990)

        #region Vecchio1990

        /// <summary>
        /// PC1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PC1(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(25.1, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(500, 196800);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PC1A Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PC1A(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(27.9, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(500, 196800);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PC4 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PC4(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(24.9, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(260, 202700);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        /// <summary>
        /// PC7 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The <see cref="ConstitutiveModel"/> for concrete</param>
        /// <param name="considerCrackSlip">Consider crack slip? Only for <see cref="DSFMMembrane"/> (default: true).</param>
        public static Membrane PC7(ConstitutiveModel constitutiveModel = ConstitutiveModel.MCFT, bool considerCrackSlip = true)
        {
	        // Get concrete parameter model
	        var parModel = constitutiveModel == ConstitutiveModel.MCFT ? ParameterModel.MCFT : ParameterModel.DSFM;

	        // Initiate concrete
	        var parameters = new Parameters(28.7, 10, parModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(390, 195000);
	        var steelY = steelX.Clone();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89, steelY, 70);

	        return
		        Membrane.Read(parameters, reinforcement, 70, constitutiveModel, considerCrackSlip);
        }

        #endregion
    }
}
