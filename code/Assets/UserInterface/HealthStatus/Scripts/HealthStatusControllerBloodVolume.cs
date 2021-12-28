using UnityEngine;
using UserInterface.MultiInstance;
using UserInterface.Parameters.Data;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>
    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerBloodVolume : ParametersDataConsumer 
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }

        protected override void ParametersDataChangedHandler()
        {
            float vol = ParametersDataInstance.bloodVolume;
            if (vol < 100000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Shunt";
            }
            else if (vol >= 100000 && vol < 250000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Vaso-constriction";
            }
            else if (vol > 550000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Vasodilation";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}