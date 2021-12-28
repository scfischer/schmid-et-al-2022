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
    public class HealthStatusControllerPCO2Blood : ParametersDataConsumer
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }


        protected override void ParametersDataChangedHandler()
        {
            float pCo2 = ParametersDataInstance.bloodPpCo2;
            if (pCo2 < 35)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Hypocapnia";
            }
            else if (pCo2 > 50 && pCo2 <= 60)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Exercise";
            }
            else if (pCo2 > 60)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Hypercapnia";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}