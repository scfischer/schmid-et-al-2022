using System.Collections.Generic;
using de.jmu.ge.Plotter;
using Simulation.Systems;
using Support;
using UnityEngine;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// This class uses the plugin Plotter to draw a vertical line at the value of oxygen partial pressure of the incoming,
    /// deoxygenated blood. This value represents the partial pressure of mixed venous blood, returning from the tissues 
    /// to the heart and into the lungs. 
    /// </summary>
    public class OxygenDissociationVerticalLine : MultiPlot
    {
        [Header("Style")]
        [SerializeField]
        private float lineWidth = 3f;

        protected override void RegisterInstance(AlveolusController instance)
        {
            base.RegisterInstance(instance);
            instance.results.Subscribe(SetDirty);
        }

        protected override void UnregisterInstance(AlveolusController instance)
        {
            instance.results.Unsubscribe(SetDirty);
            base.UnregisterInstance(instance);
        }

        protected override void UpdateDataSet(AlveolusController instance, Plotter.GraphDescriptor graph)
        {
            graph.color = InstanceColorMap.GetColor(instance);
            graph.lineWidth = lineWidth;
            graph.labelFormatter = LabelFormatter;

            float conversionFactor = appSettings.GasUnit.GetConversionFactor();
            float tissuePp02 = instance.instanceParameters.bloodPpO2 * conversionFactor;

            List<Vector2> points = new List<Vector2> {new Vector2(tissuePp02, 0), new Vector2(tissuePp02, 1)};

            graph.points = points;
        }

        private string LabelFormatter(Vector2 datapoint)
        {
            return $"tissue pO2: {datapoint.x:0.00} {appSettings.GasUnit}";
        }
    }
}