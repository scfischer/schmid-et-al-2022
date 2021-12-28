using UnityEngine;
using UserInterface.Parameters.Data;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>
    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerTissueBarrier : ParametersDataConsumer 
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }


        protected override void ParametersDataChangedHandler()
        {
            float barrier = ParametersDataInstance.barrierThickness;
            if (barrier < 0.4f)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Extremely Thin";
            }
            else if (barrier > 2)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Thickened";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}