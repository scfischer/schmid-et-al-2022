using System.Collections;
using System.Collections.Generic;
using de.jmu.ge.Plotter;
using Simulation.Systems;
using UnityEngine;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// This class uses the plugin <see cref="Plotter"/> to plot the oxygen saturation over the transit time 
    /// along the representative alveolar capillary.
    /// </summary>
    /// <remarks> This class inherits from the <see cref="MultiPlot"/> class and thus supports plotting results 
    /// of multiple simulation instances. It receives the datapoints from the <see cref="Simulation.Systems.Output.OutputCollector"/>
    /// of the respective instance. </remarks>
    public class OxygenSaturationPlot : MultiPlot
    {
        [Header("Evaluation")]
        [SerializeField]
        private float dataUpdatePeriod = 0.5f;

        private void Start()
        {
            StartCoroutine(UpdateCoroutine());
        }


        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                SetDirty();
                yield return new WaitForSeconds(dataUpdatePeriod);
            }
        }

        protected override void PreparePlot()
        {
            float maxBloodTransitTime = 0.01f;
            float minInitialSaturation = 0.9f;
            foreach (var instance in instanceGraphMap.Keys)
            {
                if (!instance)
                    continue;
                maxBloodTransitTime = Mathf.Max(maxBloodTransitTime, instance.results.bloodTransitTime);
                minInitialSaturation = Mathf.Min(minInitialSaturation, instance.results.hbO2Saturations[0]);
            }

            Plotter.SetDataRange(
                0, maxBloodTransitTime,
                minInitialSaturation, 1.0f
            );

            //determine grid setup based on data ranges
            float xDataRangePerGridCell = 0.1f;
            if (maxBloodTransitTime > 1.0f)
            {
                xDataRangePerGridCell *= 10;
            }

            const float yDataRangePerGridCell = 0.1f;
            float yDataRange = (1.0f - minInitialSaturation);
            float yOffset = yDataRange % yDataRangePerGridCell;
            
            Plotter.ConfigureRegularGrid(xDataRangePerGridCell, yDataRangePerGridCell, 0, yOffset);
        }

        protected override void UpdateDataSet(AlveolusController instance, Plotter.GraphDescriptor graph)
        {
            var results = instance.results;
            graph.points = new List<Vector2>(results.hbO2Saturations.Length);
            for (int i = 0; i < results.hbO2Saturations.Length; i++)
            {
                graph.points.Add(new Vector2
                {
                    x = i * results.timePeriod,
                    y = results.hbO2Saturations[i]
                });
            }

            graph.points.Add(new Vector2
            {
                x = results.bloodTransitTime,
                y = results.hbO2Saturations[results.hbO2Saturations.Length - 1]
            });

            Plotter.SetPlotsDirty();
        }
    }
}