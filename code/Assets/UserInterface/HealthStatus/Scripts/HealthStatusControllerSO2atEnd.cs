using UnityEngine;
using UserInterface.MultiInstance;
using TMPro;

namespace UserInterface.HealthStatus
{
    /// <summary>
    /// Provides classification of the parameter configuration in relation to physiological value ranges (by means of 
    /// a traffic light color code and keywords).
    /// </summary>

    [RequireComponent(typeof(HealthStatusElement))]
    public class HealthStatusControllerSO2atEnd : InstanceConsumer
    {
        private HealthStatusElement m_healthStatusElement;
        [SerializeField]
        private TMP_Text displayResult;

        private void Awake()
        {
            m_healthStatusElement = GetComponent<HealthStatusElement>();
        }

        protected override void HandleSimulationOutputChanged()
        {
            float sO2 = Alveolus.results.saturationAtEnd;

            displayResult.text = (sO2 * 100).ToString("n2") + " %" ;


            if (sO2 < 0.28f)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Serious;
                m_healthStatusElement.Label = "Lethal";
            }
            else if (sO2 > 0.95f)
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Nominal;
                m_healthStatusElement.Label = "Healthy";
            }
            else
            {
                m_healthStatusElement.Condition = HealthStatusElement.HealthCondition.Abnormal;
                m_healthStatusElement.Label = "Hypoxia";
            }
        }
    }
}