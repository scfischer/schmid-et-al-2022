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
    public class HealthStatusControllerPO2Blood : ParametersDataConsumer
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }

        protected override void ParametersDataChangedHandler()
        {
            float pO2 = ParametersDataInstance.bloodPpO2;
            if (pO2 < 20)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Hypoxemia";
            }
            else if (pO2 >= 20 && pO2 < 30)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Exercise";
            }
            else if (pO2 > 45)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Unrealistic;
                m_healthStatusElement.Label = "Unrealistic";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}