using Simulation;
using Simulation.Systems;
using UnityEngine;
using UnityEngine.Events;

namespace UserInterface.MultiInstance
{
    /// <summary>
    /// This class provides the currently selected <see cref="AlveolusController"/> instance whose 
    /// <see cref="ParametersData"/> set and 3D model are to be displayed in the GUI.
    /// </summary>
    public class InstanceProvider : MonoBehaviour
    {
        [SerializeField]
        private AlveolusController simulationAlveolus;

        public AlveolusController SimulationAlveolus
        {
            get => simulationAlveolus;
            set
            {
                simulationAlveolus = value;
                onSimulationInstanceChanged.Invoke(simulationAlveolus);
            }
        }

        public class SimulationInstanceChangedEvent : UnityEvent<AlveolusController>
        {
        }

        public readonly SimulationInstanceChangedEvent onSimulationInstanceChanged = new SimulationInstanceChangedEvent();

        void Start()
        {
            SimulationAlveolus = simulationAlveolus;
        }
    }
}