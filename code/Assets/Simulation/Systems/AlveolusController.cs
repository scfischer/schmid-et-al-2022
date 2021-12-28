using System;
using Simulation.Systems.Erythrocytes;
using Simulation.Systems.GasExchange;
using Simulation.Systems.Output;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UserInterface.Camera;
using UserInterface.Highlighting;
using UserInterface.TimeControls;

namespace Simulation.Systems
{
    /// <summary>
    /// Represents the Alveolus Agent-Based Model as a single entity for external access.
    /// Provides internal access for AlveolusComponents.
    /// Manages the ParametersData and the Reset, Pause, Focus and Unfocus events.
    /// </summary>
    public class AlveolusController : MonoBehaviour
    {
        [SerializeField][FormerlySerializedAs("instanceParameters")]
        private ParametersData _instanceParameters;


        public ParametersData instanceParameters
        {
            get => _instanceParameters;
            set => SetInstanceParameters(value);
        }

        /// <summary>
        /// The part of the prefab that's visualization only - set within the prefab
        /// </summary>
        [SerializeField]
        private GameObject visualization;

        /// <summary>
        /// The capillary simulation and visualization.
        /// </summary>
        [SerializeField]
        private SpawnErythrocyte erythrocyteSpawner;

        /// <summary>
        /// <see cref="TimeControl"/>s for this instace.
        /// </summary>
        public TimeControl timeControl { get; private set; }



        /// <summary>
        /// <see cref="PartialPressure"/>s for this instance. 
        /// </summary>
        public PartialPressure partialPressures { get; private set; }

        /// <summary>
        /// <see cref="HbSaturation"/>s for this instance. 
        /// </summary>
        public HbSaturation hbSaturation { get; private set; }

        /// <summary>
        /// <see cref="BloodOutflow"/> script for this instance, that collects data on blood leaving the capillary. 
        /// </summary>
        public BloodOutflow bloodOutflow { get; private set; }

        /// <summary>
        /// Collected simulation results for this instance.
        /// </summary>
        public OutputCollector results { get; private set; }

        /// <summary>
        /// Holds highlightable elements on this alveolus instance. 
        /// </summary>
        public InstanceHighlightTargets highlightTargets { get; private set; }

        [Serializable]
        public class AlveolusEvent : UnityEvent<AlveolusController>
        {
        }

        public AlveolusEvent onParametersInstanceChange = new AlveolusEvent(); 
        public AlveolusEvent onFocus = new AlveolusEvent();
        public AlveolusEvent onDefocus = new AlveolusEvent();
        public AlveolusEvent onReset = new AlveolusEvent();
        public AlveolusEvent onPause = new AlveolusEvent();

        [Serializable]
        public class ParametersResetEvent : UnityEvent
        {
        };
        
        private void Awake()
        {
            timeControl = FindObjectOfType<TimeControl>();
            _instanceParameters = Instantiate(instanceParameters);
            // Instantiate the non-GameObject components
            results = new OutputCollector();
            partialPressures = new PartialPressure();
            hbSaturation = new HbSaturation();
            bloodOutflow = new BloodOutflow();

            // Propagate this instance (note: order is important!) 
            results.Alveolus = this;
            partialPressures.Alveolus = this;
            hbSaturation.Alveolus = this;
            bloodOutflow.Alveolus = this;

            highlightTargets = GetComponentInChildren<InstanceHighlightTargets>();
            
            onParametersInstanceChange?.Invoke(this);
        }

        private void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            Destroy(instanceParameters);
        }

        private void SetInstanceParameters(ParametersData value)
        {
            if (_instanceParameters != null)
            {
                Destroy(_instanceParameters);
            }
            
            _instanceParameters = value;
            
            onParametersInstanceChange?.Invoke(this);
        }

        /// <summary>
        /// Pause the simulation. 
        /// </summary>
        public void Pause()
        {
            onPause.Invoke(this);
        }

        /// <summary>
        /// Reset the simulation to the parameters as defined in the current ParametersData.
        /// </summary>
        public void Reset()
        {
            onReset.Invoke(this);
            timeControl.simulationStart = Time.timeSinceLevelLoad; 
        }


        /// <summary>
        /// Move this alveolus prefab instance out of view and turn of unnecessary visualization,
        /// but keep the simulation going.  
        /// </summary>
        public void MoveOutOfFocus()
        {
            DisableVisualization();
            onDefocus?.Invoke(this);
        }

        /// <summary>
        /// Move this alveolus prefab instance into view and make sure everything is visible again.
        /// </summary>
        public void ReturnToFocus()
        {
            EnableVisualization();
            Camera.main.GetComponent<CameraControls>().pivotObject = transform;
            onFocus?.Invoke(this);
        }

        /// <summary>
        /// Disable the visualization elements of the current prefab instance. 
        /// </summary>
        private void DisableVisualization()
        {
            visualization.SetActive(false);
            erythrocyteSpawner.VisualsEnabled = false;
        }

        /// <summary>
        /// Enable the visualization elements of the current prefab instance.
        /// </summary>
        private void EnableVisualization()
        {
            visualization.SetActive(true);
            erythrocyteSpawner.VisualsEnabled = true;
        }
    }
}