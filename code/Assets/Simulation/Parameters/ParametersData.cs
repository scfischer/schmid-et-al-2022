using System;
using UnityEngine;

namespace Simulation
{
    /// <summary>
    /// This script holds the values for parameters which can be adjusted by the user, makes them accessible in a single
    /// location - and independent from GUI components - to all other scripts needing those values, and calculates some
    /// derived values that depend directly on these parameters.
    /// </summary>
    [CreateAssetMenu]
    public class ParametersData : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary> Placeholder for a more detailed description of the parameter set. </summary>
        [SerializeField]
        [Multiline(30)]
        private string description;
        public string Description => description;
        
        /// <summary> Atmospheric pressure [mmHg]. </summary>
        [SerializeField]
        private float atmosphericPressureDefault = 760;

        /// <summary> Partial pressure of oxygen in alveolar air [mmHg]. </summary>
        /// <remarks> Default value taken from (Sharma et al., 2020). </remarks>
        [SerializeField]
        private float alveolarPpO2Default = 100;

        /// <summary> Partial pressure of oxygen in incoming blood [mmHg]. </summary>
        /// <remarks> Default value taken from (Dash et al., 2016). </remarks>
        [SerializeField]
        private float bloodPpO2Default = 40;

        /// <summary> Partial pressure of carbon dioxide in alveolar air [mmHg]. </summary>
        /// <remarks> Default value taken from (Sharma et al., 2020). </remarks>
        [SerializeField]
        private float alveolarPpCo2Default = 40;

        /// <summary> Partial pressure of carbon dioxide in incoming blood [mmHg]. </summary>
        /// <remarks> Default value taken from (Dash et al., 2016). </remarks>
        [SerializeField]
        private float bloodPpCo2Default = 45;

        /// <summary> pH inside red blood cells (unitless). </summary>
        /// <remarks> Default value taken from (Dash et al., 2016). </remarks>
        [SerializeField]
        private float bloodpHRBCDefault = 7.24f;

        /// <summary> Blood volume in alveolar capillaries [µm³]. </summary>
        /// <remarks> For the default value, 50% capillary recruitment is assumed (Okada et al., 1992). </remarks>
        [SerializeField]
        private float bloodVolumeDefault = 404000;

        /// <summary> Blood flow velocity in alveolar capillaries [mm/s]. </summary>
        /// <remarks> Default value derived from results published in (Weibel et al., 1993; Petersson and Glenny, 2014) </remarks>
        [SerializeField]
        private float bloodFlowVelocityDefault = 1.0f;

        /// <summary> Blood Temperature in the lung [°C]. </summary>
        /// <remarks> Default value taken from (Dash et al., 2016). </remarks>
        [SerializeField]
        private float bloodTemperatureDefault = 37;

        ///<summary> Concentration of (2,3)-diphosphoglycerate ([DPG]) in the blood [M]. </summary> 
        ///<remarks> Default value taken from (Dash et al., 2016) </remarks>
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private float concentrationDPGDefault = (float) (4.65 * Math.Pow(10, -3));

        /// <summary> Alveolar surface area available for gas exchange [µm²]. </summary>
        /// <remarks> Default value taken from (Mercer et al., 1994). </remarks>
        [SerializeField]
        private float surfaceAreaDefault = 121000;

        /// <summary> Thickness of tissue barrier between alveolar space and capillary blood [µm]. </summary>
        /// <remarks> Default value taken from (Gehr et al., 1978; Weibel et al., 1993). </remarks>
        [SerializeField]
        private float barrierThicknessDefault = 1.11f;

        /// <summary> Partial pressure of water in alveolar air at 37°C [mmHg]. </summary>
        [SerializeField]
        private float alveolarPpH2O = 45;

        /// <summary> Average number of capillary paths making up the capillary network around the alveolus. </summary>
        public float NumberCapillaries = 52;

        /// <summary> Percentage of oxygen in the air. </summary>
        public float oxygenRatio { get; private set; }

        /// <summary> Percentage of carbon dioxide in the air. </summary>
        public float co2Ratio { get; private set; }

        /// <summary> "Membrane Diffusing Capacity" for oxygen in the lung - DMO2. Depends on surface area, barrier thickness and permeability coefficient. </summary>
        public float dmO2 { get; private set; }

        /// <summary> Permeability coefficient for oxygen in the lung [µm²/(sec*mmHg)]. Taken from Weibel et al. 1993. </summary>
        private const float PermCoefficientO2 = 0.055f;

        /// <summary> Blood volume when the capillary network around an alveolus is completely perfused [µm³]. </summary>
        /// <remarks> The value was derived from results published in (Ochs et al., 2004; Gehr et al., 1978) </remarks>
        public const int MaxBloodVolume = 808000;

        /// <summary> Percentage of capillaries perfused. </summary>
        public float capillaryRecruitment => bloodVolume / MaxBloodVolume;

        /// <summary> Number of capillaries currently perfused. </summary>
        public float capillariesPerfused => capillaryRecruitment * NumberCapillaries;

        /// <summary> Setting for atmospheric pressure. </summary>
        /// <remarks> When atmospheric pressure changes, the alveolar partial pressures of oxygen <see cref="alveolarPpO2"/>
        /// and carbon dioxide <see cref="alveolarPpCo2"/> are adjusted. </remarks>
        public float atmosphericPressure
        {
            get => m_runtimeValues.atmosphericPressure;
            set
            {
                m_runtimeValues.atmosphericPressure = value;
                m_runtimeValues.alveolarPpO2 = oxygenRatio * (value - alveolarPpH2O);
                m_runtimeValues.alveolarPpCo2 = co2Ratio * (value - alveolarPpH2O);
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for alveolar pO2. </summary>
        /// <remarks> From the partial pressure, the ratio of oxygen in the air mixture (<see cref="oxygenRatio"/>) is derived. </remarks>
        public float alveolarPpO2
        {
            get => m_runtimeValues.alveolarPpO2;
            set
            {
                m_runtimeValues.alveolarPpO2 = value;
                oxygenRatio = value / (atmosphericPressure - alveolarPpH2O);
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for blood pO2. </summary>
        public float bloodPpO2
        {
            get => m_runtimeValues.bloodPpO2;
            set
            {
                m_runtimeValues.bloodPpO2 = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for alveolar pCO2. </summary>
        /// <remarks> From the partial pressure, the ratio of carbon dioxide in the air mixture (<see cref="co2Ratio"/>) is derived. </remarks>
        public float alveolarPpCo2
        {
            get => m_runtimeValues.alveolarPpCo2;
            set
            {
                m_runtimeValues.alveolarPpCo2 = value;
                co2Ratio = value / (atmosphericPressure - alveolarPpH2O);
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for blood pCO2. </summary>
        public float bloodPpCo2
        {
            get => m_runtimeValues.bloodPpCo2;
            set
            {
                m_runtimeValues.bloodPpCo2 = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for pH inside red blood cells. </summary>
        public float bloodpHRBC
        {
            get => m_runtimeValues.bloodpHRBC;
            set
            {
                m_runtimeValues.bloodpHRBC = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }



        /// <summary> Setting for blood volume. </summary>
        public float bloodVolume
        {
            get => m_runtimeValues.bloodVolume;
            set
            {
                m_runtimeValues.bloodVolume = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for blood flow velocity. </summary>
        public float bloodFlowVelocity
        {
            get => m_runtimeValues.bloodFlowVelocity;
            set
            {
                m_runtimeValues.bloodFlowVelocity = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for blood temperature. </summary>
        public float bloodTemperature
        {
            get => m_runtimeValues.bloodTemperature;
            set
            {
                m_runtimeValues.bloodTemperature = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for concentration of [DPG] in red blood cells. </summary>
        public float concentrationDPG
        {
            get => m_runtimeValues.concentrationDPG;
            set
            {
                m_runtimeValues.concentrationDPG = value;
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for alveolar surface area. </summary>
        /// <remarks> When alveolar surface area changes, the membrane diffusion capacity (<see cref="dmO2"/>) is adjusted. </remarks>
        public float surfaceArea
        {
            get => m_runtimeValues.surfaceArea;
            set
            {
                m_runtimeValues.surfaceArea = value;
                dmO2 = PermCoefficientO2 * (value / barrierThickness);
                m_sendUpdatesHandler?.Invoke();
            }
        }

        /// <summary> Setting for thickness of the tissue barrier. </summary>
        /// <remarks> When barrier thickness changes, the membrane diffusion capacity (<see cref="dmO2"/>) is adjusted. </remarks>
        public float barrierThickness
        {
            get => m_runtimeValues.barrierThickness;
            set
            {
                m_runtimeValues.barrierThickness = value;
                dmO2 = PermCoefficientO2 * (surfaceArea / value);
                m_sendUpdatesHandler?.Invoke();
            }
        }

        public delegate void UpdateValues();

        private UpdateValues m_sendUpdatesHandler;
        private ParameterValues m_runtimeValues;

        /// <summary>
        /// Get all current parameter values defining the simulation at once in a single struct.
        /// </summary>
        /// <returns> A struct containing the current parameter values used by the simulation instance. </returns>
        public ParameterValues GetAllParameters()
        {
            return m_runtimeValues;
        }

        /// <summary>
        /// Get the default parameter values set in the ParametersData asset used.
        /// </summary>
        /// <returns> A struct containing the default values for all parameters. </returns>
        public ParameterValues GetDefaults()
        {
            return new ParameterValues(atmosphericPressureDefault,
                alveolarPpO2Default,
                alveolarPpCo2Default,
                bloodPpO2Default,
                bloodPpCo2Default,
                bloodpHRBCDefault,
                bloodVolumeDefault,
                bloodFlowVelocityDefault,
                bloodTemperatureDefault,
                concentrationDPGDefault,
                surfaceAreaDefault,
                barrierThicknessDefault);
        }

        /// <summary>
        /// Accept a struct of parameter values to set the current parameters to. 
        /// </summary>
        /// <param name="values"> Struct of parameter values to set.</param>
        public void SetCurrentValues(ParameterValues values)
        {
            atmosphericPressure = values.atmosphericPressure;
            alveolarPpO2 = values.alveolarPpO2;
            alveolarPpCo2 = values.alveolarPpCo2;
            bloodPpO2 = values.bloodPpO2;
            bloodPpCo2 = values.bloodPpCo2;
            bloodpHRBC = values.bloodpHRBC;
            bloodVolume = values.bloodVolume;
            bloodFlowVelocity = values.bloodFlowVelocity;
            bloodTemperature = values.bloodTemperature;
            concentrationDPG = values.concentrationDPG;
            surfaceArea = values.surfaceArea;
            barrierThickness = values.barrierThickness;
        }

        /// <summary>
        /// Reset the current parameter settings for the current simulation instance to the defaults defined by its
        /// ParamtersData asset.
        /// </summary>
        public void ResetToDefaults()
        {
            SetCurrentValues(GetDefaults());
        }

        public void OnAfterDeserialize()
        {
            SetCurrentValues(GetDefaults());
        }

        public void OnBeforeSerialize()
        {
        }

        /// <summary>
        /// Subscribe a method to be called whenever ParametersData updates.
        /// </summary>
        /// <param name="method">Method to be called.</param>
        public void Subscribe(UpdateValues method)
        {
            m_sendUpdatesHandler += method;
        }

        /// <summary>
        /// Unsubscribe a method from getting called on ParametersData changes.
        /// </summary>
        /// <param name="method">Method to unsubscribe.</param>
        public void Unsubscribe(UpdateValues method)
        {
            m_sendUpdatesHandler -= method;
        }

        /// <summary>
        /// Struct with all paramter values, for moving them in bulk.
        /// </summary>
        public struct ParameterValues
        {
            public float atmosphericPressure;
            public float alveolarPpO2;
            public float alveolarPpCo2;
            public float bloodPpO2;
            public float bloodPpCo2;
            public float bloodpHRBC;
            public float bloodVolume;
            public float bloodFlowVelocity;
            public float bloodTemperature;
            public float concentrationDPG;
            public float surfaceArea;
            public float barrierThickness;

            public ParameterValues(
                float atmosphericPressure,
                float alveolarPpO2,
                float alveolarPpCo2,
                float bloodPpO2,
                float bloodPpCo2,
                float bloodpHRBC,
                float bloodVolume,
                float bloodFlowVelocity,
                float bloodTemperature,
                float concentrationDPG,
                float surfaceArea,
                float barrierThickness)
            {
                this.atmosphericPressure = atmosphericPressure;
                this.alveolarPpO2 = alveolarPpO2;
                this.alveolarPpCo2 = alveolarPpCo2;
                this.bloodPpO2 = bloodPpO2;
                this.bloodPpCo2 = bloodPpCo2;
                this.bloodpHRBC = bloodpHRBC;
                this.bloodVolume = bloodVolume;
                this.bloodFlowVelocity = bloodFlowVelocity;
                this.bloodTemperature = bloodTemperature;
                this.concentrationDPG = concentrationDPG;
                this.surfaceArea = surfaceArea;
                this.barrierThickness = barrierThickness;
            }
        }
    }
}