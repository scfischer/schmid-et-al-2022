using UserInterface.Elements;
using UserInterface.MultiInstance;

namespace UserInterface.Output
{
    /// <summary>
    /// This class determines the reaction half-time, i.e. the time it takes for 50 % of the oxygenation that blood undergoes
    /// during its transit along the alveolar capillary to be complete, from the simulation output.
    /// It passes the result to the appropriate display in the UI.
    /// </summary>
    public class T1_2Readout : InstanceConsumer
    {
        private LabeledValue m_value;

        private void Awake()
        {
            m_value = GetComponent<LabeledValue>();
        }

        protected override void HandleSimulationOutputChanged()
        {
            float[] sO2s = Alveolus.results.hbO2Saturations;
            float halfSatPosition = 0;
            float timePeriod = Alveolus.results.timePeriod;

            float halfSat = sO2s[0] + (sO2s[(sO2s.Length - 1)] - sO2s[0]) / 2;
            for (int i = 0; i < sO2s.Length; i++)
            {
                if (sO2s[i] > halfSat)
                {
                    halfSatPosition = i;
                    if (halfSatPosition < 0) { halfSatPosition = 0; }
                    break;
                }
            }

            float t1_2 = halfSatPosition * timePeriod;
            m_value.Value = t1_2;
        }
    }
}


