using System;
using Simulation;
using Simulation.Systems;
using UnityEngine;
using UnityEngine.Events;
using UserInterface.MultiInstance;

namespace UserInterface.Parameters.Data
{
    /// <summary>
    /// Source for a <see cref="ParametersData"/> instance. Subscribers will be notified if the instance defined in ParametersData
    /// has changed, e.g. to update their own reference. 
    /// </summary>
    public class ParametersDataProvider : MonoBehaviour
    {
        private ParametersData m_parametersData;

        public InstanceProvider instanceProvider;

        public ParametersData ParametersData
        {
            get => m_parametersData;
            set
            {
                m_parametersData = value;
                onParametersDataInstanceChanged?.Invoke(m_parametersData);
            }
        }

        public class ParametersDataInstanceChangedEvent : UnityEvent<ParametersData>
        {
        }

        public ParametersDataInstanceChangedEvent onParametersDataInstanceChanged;

        public void Awake()
        {
            if (onParametersDataInstanceChanged == null)
                onParametersDataInstanceChanged = new ParametersDataInstanceChangedEvent();
        }

        public void Start()
        {
            instanceProvider.onSimulationInstanceChanged.AddListener(HandleSimulationInstanceChanged);
            HandleSimulationInstanceChanged(instanceProvider.SimulationAlveolus);
        }

        private void HandleSimulationInstanceChanged(AlveolusController alveolus)
        {
            ParametersData = alveolus != null ? alveolus.instanceParameters : null;
        }

        public void SetParametersDataInstanceWithoutNotify(ParametersData instance)
        {
            m_parametersData = instance;
        }
    }
}