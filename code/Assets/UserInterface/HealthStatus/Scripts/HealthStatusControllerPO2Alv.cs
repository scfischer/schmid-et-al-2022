using UnityEngine;
using UserInterface.Parameters.Data;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>

    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerPO2Alv : ParametersDataConsumer  
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }

        protected override void ParametersDataChangedHandler()
        {
            float pO2 = ParametersDataInstance.alveolarPpO2;
            if (pO2 < 90)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Hypo-ventilation";
            }
            else if (pO2 > 110)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Oxygen Treatment";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}