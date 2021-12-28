using System.Collections.Generic;
using de.jmu.ge.Plotter;
using Simulation.Systems;
using Support;
using UnityEngine;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// This class uses the plugin Plotter to draw a vertical line at the value of transit time, i.e. the 
    /// time it takes one erythrocyte to flow along the capillary.
    /// </summary>
    public class OxygenSaturationVerticalLine : MultiPlot
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

            var results = instance.results;
            float transitTime = results.bloodTransitTime;

            List<Vector2> points = new List<Vector2> {new Vector2(transitTime, 0), new Vector2(transitTime, 1)};

            graph.points = points;
        }

        private string LabelFormatter(Vector2 datapoint)
        {
            return $"end of transit: {datapoint.x:0.00} s";
        }
    }
}