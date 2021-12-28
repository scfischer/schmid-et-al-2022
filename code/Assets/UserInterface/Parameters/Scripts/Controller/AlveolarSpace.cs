using UserInterface.Elements;
using UserInterface.Parameters.Data;
using UnityEngine;
using Support;
using TMPro;
using System;

namespace UserInterface.Parameters.Controller
{
    /// <summary>
    /// This class controls the user configuration of model parameters related to alveolar space. 
    /// </summary>
    /// <remarks>
    /// Input from the user is passed to <see cref="ParametersData"/> of the corresponding instance.
    /// It takes into account which pressure unit is currently selected and converts the values accordingly if necessary 
    /// (the model calculations are all performed in mmHg). If changes are made in ParametersData or another instance is selected, 
    /// the parameter sliders are updated immediately.
    /// </remarks>
    public class AlveolarSpace : ParametersDataConsumer
    {
        [SerializeField]
        private AppSettings appSettings;

        private float conversionFactor = 1;

        private const float sliderAtmMin = 253;
        private const float sliderAtmMax = 800;
        private const float sliderO2Min = 1;
        private const float sliderO2Max = 150;
        private const float sliderCO2Min = 1;
        private const float sliderCO2Max = 150;

        [SerializeField]
        private TextMeshProUGUI atmPressureUnit;
        [SerializeField]
        private TextMeshProUGUI alvPpO2Unit;
        [SerializeField]
        private TextMeshProUGUI alvPpCO2Unit;

        public ParameterSlider atmosphericPressureSlider;
        public ParameterSlider carbonDioxideSlider;
        public LabeledValue carbonDioxideProportion;
        public ParameterSlider oxygenSlider;
        public LabeledValue oxygenProportion;

        private void OnEnable()
        {
            appSettings.GasUnitChanged += OnGasUnitChanged;
            atmosphericPressureSlider.onValueChanged.AddListener(AtmosphericSliderChangedHandler);
            carbonDioxideSlider.onValueChanged.AddListener(CarbonDioxideSliderChangedHandler);
            oxygenSlider.onValueChanged.AddListener(OxygenSliderChangedHandler);
        }


        private void OnDisable()
        {
            appSettings.GasUnitChanged -= OnGasUnitChanged;
            atmosphericPressureSlider.onValueChanged.RemoveListener(AtmosphericSliderChangedHandler);
            carbonDioxideSlider.onValueChanged.RemoveListener(CarbonDioxideSliderChangedHandler);
            oxygenSlider.onValueChanged.RemoveListener(OxygenSliderChangedHandler);
        }

        protected override void ParametersDataChangedHandler()
        {
            atmosphericPressureSlider.SetValueWithoutNotify(ParametersDataInstance.atmosphericPressure *
                                                            conversionFactor);
            carbonDioxideSlider.SetValueWithoutNotify(ParametersDataInstance.alveolarPpCo2 * conversionFactor);
            carbonDioxideProportion.Value = ParametersDataInstance.co2Ratio * 100;
            oxygenSlider.SetValueWithoutNotify(ParametersDataInstance.alveolarPpO2 * conversionFactor);
            oxygenProportion.Value = ParametersDataInstance.oxygenRatio * 100;
        }

        private void AtmosphericSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.atmosphericPressure = v / conversionFactor;
        }

        private void OxygenSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.alveolarPpO2 = v / conversionFactor;
        }

        private void CarbonDioxideSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.alveolarPpCo2 = v / conversionFactor;
        }

        protected override void SubscribeHandler()
        {
            base.SubscribeHandler();
            if (!ParametersDataInstance)
                return;
            OnGasUnitChanged(appSettings.GasUnit);
        }

        private void OnGasUnitChanged(GasUnit unit)
        {
            conversionFactor = unit.GetConversionFactor();
            UpdateUnitLabels(unit);
            AdjustSliderOnGasUnitChanged();
        }

        private void AdjustSliderOnGasUnitChanged()
        {
            float atmosphericPressureValue = ParametersDataInstance.atmosphericPressure;
            float oxygenValue = ParametersDataInstance.alveolarPpO2;
            float carbonDioxideValue = ParametersDataInstance.alveolarPpCo2;

            atmosphericPressureSlider.valueSlider.minValue = sliderAtmMin * conversionFactor;
            atmosphericPressureSlider.valueSlider.maxValue = sliderAtmMax * conversionFactor;
            oxygenSlider.valueSlider.minValue = sliderO2Min * conversionFactor;
            oxygenSlider.valueSlider.maxValue = sliderO2Max * conversionFactor;
            carbonDioxideSlider.valueSlider.minValue = sliderCO2Min * conversionFactor;
            carbonDioxideSlider.valueSlider.maxValue = sliderCO2Max * conversionFactor;

            atmosphericPressureSlider.valueSlider.value = atmosphericPressureValue * conversionFactor;
            carbonDioxideSlider.valueSlider.value = carbonDioxideValue * conversionFactor;
            oxygenSlider.valueSlider.value = oxygenValue * conversionFactor;
        }

        private void UpdateUnitLabels(GasUnit unit)
        {
            atmPressureUnit.text = unit.ToString();
            alvPpO2Unit.text = unit.ToString();
            alvPpCO2Unit.text = unit.ToString();
        }
    }
}