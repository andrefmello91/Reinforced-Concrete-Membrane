using Material.Concrete;
using Material.Reinforcement;
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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 6.35, 53.86, steelY, 70);

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
            var reinforcement = new WebReinforcement(2.03, 51.37, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(3.3, 50.91, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(3.45, 25.2, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(5.79, 101.66, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(5.44, 25.37, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 4.7, 49.57, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 5.44, 50.7, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 3.18, 50.43, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 0, 0, null, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(4.09, 50.73, steelXY, 70);

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
	        var reinforcement = new WebReinforcement(4.09, 50.73, steelXY, 70);

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
	        var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 2.67, 50, steelY, 70);

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
	        var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.01, 50.82, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 5.41, 50.52, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 5.87, 50.87, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelXY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.47, 50.38, steelY, 70);

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
		    var reinforcement = new WebReinforcement(6.35, 50.55, steelX, 4.7, 49.08, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(72.2, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 0, 0, null, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(66.1, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(58.4, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(68.5, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(52.1, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(49.7, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(53.6, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 89.54, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(55.9, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 59.21, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(56, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 179.07, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(51.4, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(606, 200000);
	        var steelY = new Steel(521, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8, 44.46, steelX, 5.72, 59.21, steelY, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(49.9, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(522, 200000);
	        var steelY = steelX.Copy();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89.54, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(43, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(522, 200000);
	        var steelY = steelX.Copy();

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(5.72, 44.5, steelX, 5.72, 89.54, steelY, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(16.4, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(17.7, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(20.2, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(20.4, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(425, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(24, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(25.9, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(23.1, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(433, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 74.11, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(41.1, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(489, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(41.7, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(502, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(41.6, 9.5, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steel = new Steel(502, 200000);

	        // Get reinforcement
	        var reinforcement = WebReinforcement.DirectionXOnly(6, 40, steel, 70);

            return
                Membrane.ReadMembrane(concrete, reinforcement, 70, considerCrackSlip);
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
	        var concrete = new Concrete(38.5, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
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
	        var concrete = new Concrete(38.2, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
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
	        var concrete = new Concrete(42, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
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
	        var concrete = new Concrete(43.1, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
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
	        var concrete = new Concrete(38.1, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
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
	        var concrete = new Concrete(43.5, 10, parModel, constitutiveModel);

	        // Initiate steel for each direction
	        var steelX = new Steel(604, 200000);
	        var steelY = new Steel(529, 200000);

	        // Get reinforcement
	        var reinforcement = new WebReinforcement(8.1, 92.7, steelX, 3.85, 178.8, steelY, 73);

	        return
		        Membrane.ReadMembrane(concrete, reinforcement, 73, considerCrackSlip);
        }
		
        #endregion
    }
}
