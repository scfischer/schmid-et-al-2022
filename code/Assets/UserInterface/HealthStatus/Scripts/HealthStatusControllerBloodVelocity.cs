using UnityEngine;
using UserInterface.Parameters.Data;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>
    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerBloodVelocity : ParametersDataConsumer
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }

        protected override void ParametersDataChangedHandler()
        {
            float velocity = ParametersDataInstance.bloodFlowVelocity;
            if (velocity < 0.15f)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Shunt";
            }
            else if (velocity >= 0.15f && velocity < 0.3f)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Vaso-constriction";
            }
            else if (velocity > 1.5)
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