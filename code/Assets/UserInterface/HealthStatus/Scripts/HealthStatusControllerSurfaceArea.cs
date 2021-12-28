using UnityEngine;
using UserInterface.Parameters.Data;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>
    
    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerSurfaceArea : ParametersDataConsumer 
    {
        private HealthStatusElement m_healthStatusElement;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }


        protected override void ParametersDataChangedHandler()
        {
            float area = ParametersDataInstance.surfaceArea;
            if (area < 20000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Collapse";
            }
            else if (area >= 20000 && area < 90000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Tissue Damage";
            }
            else if (area > 150000)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Enlarged";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Normal";
            }
        }
    }
}