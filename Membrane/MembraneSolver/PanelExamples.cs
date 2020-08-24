using Material.Concrete;
using Material.Reinforcement;
using Reinforcement = Material.Reinforcement.BiaxialReinforcement;
using Concrete      = Material.Concrete.BiaxialConcrete;

namespace RCMembrane
{
    /// <summary>
    /// Class with some panel examples from Vecchio(1985).
    /// </summary>
    public static class PanelExamples
    {
        /// <summary>
        /// PV1 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// PV10 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// PV14 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
        /// PV20 Panel.
        /// </summary>
        /// <param name="constitutiveModel">The constitutive model for concrete</param>
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
    }
}
