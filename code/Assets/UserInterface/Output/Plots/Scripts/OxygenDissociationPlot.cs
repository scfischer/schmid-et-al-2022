using de.jmu.ge.Plotter;
using Simulation.Systems;
using Support;
using UnityEngine;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// This class uses the plugin <see cref="Plotter"/> to plot the oxygen dissociation curve, 
    /// which is the oxygen saturation of the blood over a range of oxygen partial pressure values.
    /// </summary>
    /// <remarks> This class inherits from the <see cref="MultiPlot"/> class and thus supports plotting results 
    /// of multiple simulation instances. It receives the datapoints from the <see cref="Simulation.Systems.Output.OutputCollector"/>
    /// of the respective instance. The x-axis (oxygen partial pressure values) is adjusted for 
    /// changes in the gas unit (<see cref="AppSettings.gasUnit"/>. </remarks>
    public class OxygenDissociationPlot : MultiPlot
    {
        [Header("Evaluation")]
        [SerializeField]
        private int numDataPoints = 101;

        protected new void Awake()
        {
            base.Awake();
            appSettings.GasUnitChanged += AppSettingsOnGasUnitChanged;
        }

        private void Start()
        {
            AppSettingsOnGasUnitChanged(appSettings.GasUnit);
        }

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

        protected override void PreparePlot()
        {
            float conversionFactor = appSettings.GasUnit.GetConversionFactor();
            
            var xMin = float.MaxValue;
            var xMax = float.MinValue;
            if (instanceGraphMap.Count > 0)
            {
                foreach (var pair in instanceGraphMap)
                {
                    xMin = Mathf.Min(pair.Key.results.dissociationGraphPO2Min * conversionFactor, xMin);
                    xMax = Mathf.Max(pair.Key.results.dissociationGraphPO2Max * conversionFactor, xMax);
                }
            }
            else
            {
                xMin = 0;
                xMax = 1.0f;
            }

            Plotter.SetDataRange(xMin, xMax, 0, 1.0f);
            Plotter.ConfigureRegularGrid(10, 10);
        }

        protected override void UpdateDataSet(AlveolusController instance, Plotter.GraphDescriptor graph)
        {
            graph.color = InstanceColorMap.GetColor(instance);
            graph.points = instance.results.CreateDissociationCurveData(numDataPoints);
            graph.labelFormatter = LabelFormatter;
            
            float conversionFactor = appSettings.GasUnit.GetConversionFactor();
            for (int i = 0; i < graph.points.Count; i++)
            {
                var p = graph.points[i];
                p.x *= conversionFactor;
                graph.points[i] = p;
            }
        }

        private string LabelFormatter(Vector2 datapoint)
        {
            return $"X: {datapoint.x} {appSettings.GasUnit}\nY: {datapoint.y:P}";
        }

        private void AppSettingsOnGasUnitChanged(GasUnit unit)
        {
            xLabel.text = $"partial pressure O2 ({unit})";
            SetDirty();
        }
    }
}