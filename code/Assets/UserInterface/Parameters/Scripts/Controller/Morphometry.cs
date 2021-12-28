using UserInterface.Elements;
using UserInterface.Parameters.Data;

namespace UserInterface.Parameters.Controller
{
    /// <summary>
    /// This class controls the user configuration of model parameters related to the morphology of the alveolus. 
    /// </summary>
    /// <remarks>
    /// Input from the user is passed to <see cref="ParametersData"/> of the corresponding instance.
    /// It takes into account which pressure unit is currently selected and converts the values accordingly if necessary 
    /// (the model calculations are all performed in mmHg). If changes are made in ParametersData or another instance is selected, 
    /// the parameter sliders are updated immediately.
    /// </remarks>
    public class Morphometry : ParametersDataConsumer
    {
        public ParameterSlider surfaceAreaSlider;
        public ParameterSlider tissueBarrierThicknessSlider;
        public LabeledValue dmO2;

        private void OnEnable()
        {
            surfaceAreaSlider.onValueChanged.AddListener(SurfaceAreaSliderChangedHandler);
            tissueBarrierThicknessSlider.onValueChanged.AddListener(TissueBarrierThicknessSliderChangedHandler);
        }

        private void OnDisable()
        {
            surfaceAreaSlider.onValueChanged.RemoveListener(SurfaceAreaSliderChangedHandler);
            tissueBarrierThicknessSlider.onValueChanged.RemoveListener(TissueBarrierThicknessSliderChangedHandler);
        }

        protected override void ParametersDataChangedHandler()
        {
            surfaceAreaSlider.SetValueWithoutNotify(ParametersDataInstance.surfaceArea);
            tissueBarrierThicknessSlider.SetValueWithoutNotify(ParametersDataInstance.barrierThickness);
            dmO2.Value = ParametersDataInstance.dmO2;
        }

        private void TissueBarrierThicknessSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.barrierThickness = v;
        }

        private void SurfaceAreaSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.surfaceArea = v;
        }
    }
}