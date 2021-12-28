using Simulation.Systems;
using UnityEngine;

namespace UserInterface.MultiInstance
{
    /// <summary>
    /// This is a base class for all child classes that should refer only to the currently selected <see cref="AlveolusController"/> instance.
    /// It obtaines the selected instance from <see cref="InstanceProvider"/> and triggers the necessary adjustments when a change is made.
    /// </summary>
    public abstract class InstanceConsumer : MonoBehaviour
    {
        protected AlveolusController Alveolus { get; private set; }

        protected void Start()
        {
            var instanceProvider = GetComponentInParent<InstanceProvider>();
            instanceProvider.onSimulationInstanceChanged.AddListener(HandleSimulationInstanceChanged);
            HandleSimulationInstanceChanged(instanceProvider.SimulationAlveolus);
        }

        protected abstract void HandleSimulationOutputChanged();

        protected void HandleSimulationInstanceChanged(AlveolusController newInstance)
        {
            if (Alveolus != null)
                Alveolus.results.Unsubscribe(HandleSimulationOutputChanged);

            Alveolus = newInstance;
            
            if (Alveolus != null)
            {
                HandleSimulationOutputChanged();
                Alveolus.results.Subscribe(HandleSimulationOutputChanged);
            }
        }
    }
}