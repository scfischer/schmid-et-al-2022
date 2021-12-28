using UserInterface.Elements;
using UserInterface.Parameters.Data;
using Support;
using UnityEngine;
using TMPro;

namespace UserInterface.Parameters.Controller
{
    /// <summary>
    /// This class controls the user configuration of model parameters related to incoming, deoxygenated blood. 
    /// </summary>
    /// <remarks>
    /// Input from the user is passed to <see cref="ParametersData"/> of the corresponding instance.
    /// It takes into account which pressure unit is currently selected and converts the values accordingly if necessary 
    /// (the model calculations are all performed in mmHg). If changes are made in ParametersData or another instance is selected, 
    /// the parameter sliders are updated immediately.
    /// </remarks>
    public class IncomingBlood : ParametersDataConsumer
    {
        [SerializeField]
        private AppSettings appSettings;
        private float conversionFactor = 1;

        private const float sliderMin = 1;
        private const float sliderMax = 150;

        [SerializeField]
        private TextMeshProUGUI bloodPpO2Unit;
        [SerializeField]
        private TextMeshProUGUI bloodPpCO2Unit;

        public ParameterSlider oxygenSlider;
        public ParameterSlider carbonDioxideSlider;
        public ParameterSlider pHSlider;
        public ParameterSlider bloodVolumeSlider;
        public ParameterSlider bloodFlowVelocitySlider;
        public ParameterSlider bloodTemperatureSlider;
        public ParameterSlider bloodDPGConcentrationSlider;

        private void OnEnable()
        {
            appSettings.GasUnitChanged += OnGasUnitChanged;
            carbonDioxideSlider.onValueChanged.AddListener(CarbonDioxideSliderChangedHandler);
            pHSlider.onValueChanged.AddListener(pHSliderChangedHandler);
            oxygenSlider.onValueChanged.AddListener(OxygenSliderChangedHandler);
            bloodVolumeSlider.onValueChanged.AddListener(BloodVolumeSliderChangedHandler);
            bloodFlowVelocitySlider.onValueChanged.AddListener(BloodFlowVelocitySliderChangedHandler);
            bloodDPGConcentrationSlider.onValueChanged.AddListener(BloodDpgConcentrationSliderChangedHandler);
            bloodTemperatureSlider.onValueChanged.AddListener(BloodTemperatureSliderChangedHandler);
        }

        private void OnDisable()
        {
            appSettings.GasUnitChanged -= OnGasUnitChanged;
            carbonDioxideSlider.onValueChanged.RemoveListener(CarbonDioxideSliderChangedHandler);
            pHSlider.onValueChanged.RemoveListener(pHSliderChangedHandler);
            oxygenSlider.onValueChanged.RemoveListener(OxygenSliderChangedHandler);
            bloodVolumeSlider.onValueChanged.RemoveListener(BloodVolumeSliderChangedHandler);
            bloodFlowVelocitySlider.onValueChanged.RemoveListener(BloodFlowVelocitySliderChangedHandler);
            bloodDPGConcentrationSlider.onValueChanged.RemoveListener(BloodDpgConcentrationSliderChangedHandler);
            bloodTemperatureSlider.onValueChanged.RemoveListener(BloodTemperatureSliderChangedHandler);
        }

        protected override void ParametersDataChangedHandler()
        {
            carbonDioxideSlider.SetValueWithoutNotify(ParametersDataInstance.bloodPpCo2 * conversionFactor);
            pHSlider.SetValueWithoutNotify(ParametersDataInstance.bloodpHRBC);
            oxygenSlider.SetValueWithoutNotify(ParametersDataInstance.bloodPpO2 * conversionFactor);
            bloodVolumeSlider.SetValueWithoutNotify(ParametersDataInstance.bloodVolume);
            bloodFlowVelocitySlider.SetValueWithoutNotify(ParametersDataInstance.bloodFlowVelocity);
            bloodTemperatureSlider.SetValueWithoutNotify(ParametersDataInstance.bloodTemperature);
            bloodDPGConcentrationSlider.SetValueWithoutNotify(ParametersDataInstance.concentrationDPG);
        }

        private void OxygenSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodPpO2 = v / conversionFactor;
        }

        private void CarbonDioxideSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodPpCo2 = v / conversionFactor;
        }

        private void pHSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodpHRBC = v;
        }

        private void BloodVolumeSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodVolume = v;
        }

        private void BloodFlowVelocitySliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodFlowVelocity = v;
        }

        private void BloodTemperatureSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.bloodTemperature = v;
        }

        private void BloodDpgConcentrationSliderChangedHandler(float v)
        {
            if (ParametersDataInstance)
                ParametersDataInstance.concentrationDPG = v;
        }


        protected override void SubscribeHandler()
        {
            base.SubscribeHandler();
            if (!ParametersDataInstance)
                return;


            OnGasUnitChanged(appSettings.GasUnit);
        }


        private void OnGasUnitChanged(GasUnit newUnit)
        {
            float carbonDioxideValue;
            float oxygenValue;

            //cache values:
            carbonDioxideValue = ParametersDataInstance.bloodPpCo2;
            oxygenValue = ParametersDataInstance.bloodPpO2;

            switch (newUnit)
            {
                case GasUnit.mmHg:
                    conversionFactor = 1;
                    AdjustSliderRange(newUnit);
                    SetCachedParameterValues(carbonDioxideValue, oxygenValue);
                    UpdateUnitLabels(newUnit);
                    break;
                case GasUnit.kPa:
                    conversionFactor = 0.133f;
                    AdjustSliderRange(newUnit);
                    SetCachedParameterValues(carbonDioxideValue, oxygenValue);
                    UpdateUnitLabels(newUnit);
                    break;
            }
        }

        private void AdjustSliderRange(GasUnit newUnit)
        {
            switch (newUnit)
            {
                case GasUnit.kPa:
                    carbonDioxideSlider.valueSlider.minValue = sliderMin * conversionFactor;
                    carbonDioxideSlider.valueSlider.maxValue = sliderMax * conversionFactor;
                    oxygenSlider.valueSlider.minValue = sliderMin * conversionFactor;
                    oxygenSlider.valueSlider.maxValue = sliderMax * conversionFactor;
                    break;
                case GasUnit.mmHg:
                    carbonDioxideSlider.valueSlider.minValue = sliderMin;
                    carbonDioxideSlider.valueSlider.maxValue = sliderMax;
                    oxygenSlider.valueSlider.minValue = sliderMin;
                    oxygenSlider.valueSlider.maxValue = sliderMax;
                    break;
            }
        }

        private void SetCachedParameterValues(float carbonDioxideValue, float oxygenValue)
        {
            carbonDioxideSlider.valueSlider.value = carbonDioxideValue * conversionFactor;
            oxygenSlider.valueSlider.value = oxygenValue * conversionFactor;
        }

        private void UpdateUnitLabels(GasUnit newUnit)
        {
            switch (newUnit)
            {
                case GasUnit.kPa:
                    bloodPpO2Unit.text = "kPa";
                    bloodPpCO2Unit.text = "kPa";
                    break;
                case GasUnit.mmHg:
                    bloodPpO2Unit.text = "mmHg";
                    bloodPpCO2Unit.text = "mmHg";
                    break;
            }
        }
    }
}