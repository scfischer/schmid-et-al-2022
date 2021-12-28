using UserInterface.Elements;
using UserInterface.MultiInstance;

namespace UserInterface.Output
{
    /// <summary>
    /// This class passes the starting value of oxygen saturation in the blood, obtained from 
    /// <see cref="Simulation.Systems.Output.OutputCollector"/> to the appropriate display in the UI.
    /// </summary>
    public class OxygenSaturationReadout : InstanceConsumer
    {
        private LabeledValue m_value;

        private void Awake()
        {
            m_value = GetComponent<LabeledValue>();
        }

        protected override void HandleSimulationOutputChanged()
        {
            float saturation = Alveolus.results.hbO2Saturations[0] * 100;
            m_value.Value = saturation;
        }
    }
}
