using System.Collections;
using System.Collections.Generic;
using de.jmu.ge.Plotter;
using Simulation.Systems;
using Simulation.Systems.Erythrocytes;
using UnityEngine;
using UserInterface.MultiInstance;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// This class uses the plugin <see cref="Plotter"/> to plot the total oxygen uptake summed up over the course of the simulation.
    /// </summary>
    /// <remarks> This class inherits from the <see cref="MultiPlot"/> class and thus supports plotting results 
    /// of multiple simulation instances. The elapsed time since the start of the simulation is determined. 
    /// The current values for the total oxygen uptake are received from the <see cref="Simulation.Systems.Output.OutputCollector"/>
    /// of the respective instance. </remarks>
    public class OxygenUptakePlot : MultiPlot
    {
        [Header("Evaluation")]
        [SerializeField]
        private float dataUpdateInterval = 0.5f;
        
        [SerializeField]
        private float timeAxisRange = 1.5f;
        
        [SerializeField]
        private float initialYRange = 0.1f;

        private float m_lastMaxUptake;

        private void Start()
        {
            StartCoroutine(SetDirtyCoroutine());
            Plotter.dataMax.y = initialYRange;

            //adding an event listener could also be done in MultiPlot.cs, for generalization purposes.
            //for now it's here to not risk breaking things, but might be worth considering if other plots generally
            //need to do something when all instances get reset.
            var instanceMenu = transform.root.GetComponentInChildren<InstanceMenu>();
            if (instanceMenu)
            {
                instanceMenu.AllInstancesReset += HandleAllInstancesReset;
            }
        }

        void OnDisable()
        {
            //this shouldn't happen before the program terminates,
            //but anything that subscribes should probably unsubscribe.
            var instanceMenu = transform.root.GetComponentInChildren<InstanceMenu>();
            if (instanceMenu)
            {
                instanceMenu.AllInstancesReset -= HandleAllInstancesReset;
            }
        }
        

        IEnumerator SetDirtyCoroutine()
        {
            yield return null;
            while (true)
            {
                SetDirty();
                yield return new WaitForSeconds(dataUpdateInterval);
            }
        }

        private float timeAtGraphStart = 0f;
        float TimeElapsed => (Time.timeSinceLevelLoad - timeAtGraphStart) / SpawnErythrocyte.sloMo;

        protected override void PreparePlot()
        {
            Plotter.SetDataRange(
                TimeElapsed - timeAxisRange,
                TimeElapsed,
                0,
                m_lastMaxUptake > Plotter.dataMax.y ? m_lastMaxUptake * 1.75f : Plotter.dataMax.y
            );

            Plotter.ConfigureRegularGrid(10.0f / SpawnErythrocyte.sloMo, Plotter.dataMax.y,
                                         -(TimeElapsed % 1.0f), 0);
                m_lastMaxUptake = 0.0001f;
        }

        protected override void UpdateDataSet(AlveolusController instance, Plotter.GraphDescriptor graph)
        {
            if (graph.points == null)
                graph.points = new List<Vector2>(100000);

            var uptake = instance.results.o2UptakeMass;
            graph.points.Add(new Vector2
            {
                x = TimeElapsed,
                y = uptake
            });

            m_lastMaxUptake = Mathf.Max(m_lastMaxUptake, uptake);
        }

        protected override void HandleInstanceReset(AlveolusController instance)
        {
            instanceGraphMap[instance].points = new List<Vector2>();
        }

        private void HandleAllInstancesReset()
        {
            m_lastMaxUptake = 0.0001f;
            Plotter.dataMax.y = initialYRange;
            timeAtGraphStart = Time.timeSinceLevelLoad;
        }
    }
}