using UserInterface.Elements;
using UserInterface.MultiInstance;
using System;
using UnityEngine;

namespace UserInterface.Output
{
    /// <summary>
    /// This class determines the diffusion capacity of the lung for oxygen ("DLO2") from the simulation output.
    /// It passes the result to the appropriate display in the UI.
    /// </summary>
    /// <remarks>
    /// <see cref="Simulation.Systems.Output.OutputCollector.o2UptakeVolume"/> is extrapolated to the whole
    /// lung by multiplication with the number of alveoli in the human lung (480*10^6, Ochs et al., 2004).
    /// DLO2 is calculated as oxygen uptake divided by simulation time and the mean gradient of pO2 between alveolar space and the blood. 
    /// </remarks>
    public class DLO2Readout : InstanceConsumer
    {
        private LabeledValue m_value;

        // number of alveoli in the human lung (Ochs et al., 2004)
        private int n_alv = 480000000;

        private void Awake()
        {
            m_value = GetComponent<LabeledValue>();
        }

        protected override void HandleSimulationOutputChanged()
        {
            // [µm³]
            float o2UptakeVolume = Alveolus.results.o2UptakeVolume;
            // [sec]
            float simulationTime = Alveolus.timeControl.simulationTime;

            float alvpPO2 = Alveolus.instanceParameters.alveolarPpO2;
            float[] capillarypPO2s = Alveolus.results.o2PartialPressures;
            float meanpPO2Gradient = alvpPO2 - Average(capillarypPO2s);

            // extrapolate volume to whole lung
            float UptakeLungVolume = o2UptakeVolume * n_alv;

            // convert Volume from µm³ to ml
            float uptakeVolumeML = UptakeLungVolume * Convert.ToSingle(Math.Pow(10, -12));
            //convert time from sec to min
            float timeMin = simulationTime / 60;

            float o2Consumption = uptakeVolumeML / timeMin;
            //calculate DLO2 [ml/mmHg/min]
            float dLO2 = o2Consumption / meanpPO2Gradient;

            m_value.Value = dLO2;
        }

        private float Average(float[] array)
        {
            float sum = 0;
            foreach (float entry in array)
            {
                sum += entry;
            }
            float average = sum / array.Length;
            return average;
        }

    }
}

