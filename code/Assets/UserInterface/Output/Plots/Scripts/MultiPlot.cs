using System.Collections;
using System.Collections.Generic;
using de.jmu.ge.Plotter;
using Simulation.Systems;
using Support;
using TMPro;
using UnityEngine;
using UserInterface.MultiInstance;

namespace UserInterface.Output.Plots
{
    /// <summary>
    /// Base class for all plotting classes. It provides organization for plotting results of
    /// multiple simulation instances in the same graph. The plots are updated at each frame.
    /// </summary>
    public class MultiPlot : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        protected AppSettings appSettings;

        private const float InstanceUpdatePeriod = 1f;

        protected InstanceColorMap InstanceColorMap { get; private set; }
        protected Plotter Plotter { get; private set; }
        protected PlotAxisLabels AxisLabels { get; private set; }

        [Header("Axis Labels")]
        [SerializeField]
        protected TMP_Text xLabel;

        [SerializeField]
        protected TMP_Text yLabel;

        protected readonly Dictionary<AlveolusController, Plotter.GraphDescriptor> instanceGraphMap =
            new Dictionary<AlveolusController, Plotter.GraphDescriptor>();

        private bool m_isDirty;

        protected void Awake()
        {
            Plotter = GetComponentInChildren<Plotter>();
            AxisLabels = GetComponentInChildren<PlotAxisLabels>();
            InstanceColorMap = GetComponentInParent<InstanceColorMap>();

            StartCoroutine(UpdateInstanceMapCoroutine());
        }

        IEnumerator UpdateInstanceMapCoroutine()
        {
            yield return null;
            while (true)
            {
                UpdateInstanceMap();
                yield return new WaitForSeconds(InstanceUpdatePeriod);
            }
        }

        protected virtual void RegisterInstance(AlveolusController instance)
        {
            var graph = Plotter.GraphDescriptor.WithStyle(InstanceColorMap.GetColor(instance));
            instanceGraphMap.Add(instance, graph);
            Plotter.AddGraph(graph);
            instance.onReset.AddListener(HandleInstanceReset);
            instance.onPause.AddListener(HandleInstancePause);
            instance.onFocus.AddListener(HandleInstanceFocus);
            instance.onDefocus.AddListener(HandleInstanceDefocus);
            m_isDirty = true;
        }


        protected virtual void UnregisterInstance(AlveolusController instance)
        {
            var graph = instanceGraphMap[instance];
            Plotter.RemoveGraph(graph);
            instanceGraphMap.Remove(instance);
            m_isDirty = true;
        }


        private void OnDestroy()
        {
            var instances = new List<AlveolusController>(instanceGraphMap.Keys);
            foreach (var instance in instances)
            {
                UnregisterInstance(instance);
            }
        }

        private void UpdateInstanceMap()
        {
            var instances = new List<AlveolusController>(FindObjectsOfType<AlveolusController>());
            var toRemove = new List<AlveolusController>();
            var toAdd = new List<AlveolusController>();

            foreach (var instance in instances)
            {
                if (!instanceGraphMap.ContainsKey(instance))
                {
                    toAdd.Add(instance);
                }
            }

            foreach (var instance in instanceGraphMap.Keys)
            {
                if (!instances.Contains(instance))
                {
                    toRemove.Add(instance);
                }
            }

            foreach (var instance in toRemove)
            {
                UnregisterInstance(instance);
            }

            foreach (var instance in toAdd)
            {
                RegisterInstance(instance);
            }
        }

        protected void SetDirty()
        {
            m_isDirty = true;
        }

        private void Update()
        {
            if (!m_isDirty) return;

            PreparePlot();
            UpdateDataSets();
            m_isDirty = false;
        }

        protected virtual void PreparePlot()
        {
        }

        protected virtual void UpdateDataSet(AlveolusController instance, Plotter.GraphDescriptor graph)
        {
        }

        protected virtual void HandleInstanceDefocus(AlveolusController instance)
        {
        }

        protected virtual void HandleInstanceFocus(AlveolusController instance)
        {
        }

        protected virtual void HandleInstancePause(AlveolusController instance)
        {
        }

        protected virtual void HandleInstanceReset(AlveolusController instance)
        {
        }

        private void UpdateDataSets()
        {
            foreach (var entry in instanceGraphMap)
            {
                if (!entry.Key)
                    continue;
                UpdateDataSet(entry.Key, entry.Value);
            }

            Plotter.SetPlotsDirty();
        }
    }
}