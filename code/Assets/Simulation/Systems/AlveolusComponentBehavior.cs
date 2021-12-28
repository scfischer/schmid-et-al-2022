using System;
using Simulation.Systems.GasExchange;
using Simulation.Systems.Output;
using UnityEngine;

namespace Simulation.Systems
{
    /// <summary>
    /// A base class for simulation components that are also MonoBehaviors, searching for an Instance either as a serialized reference or
    /// upwards in the hierarchy.
    /// It provides easy access to other simulation components through AlveolusController.
    /// When SetInstance is called with a new AlveolusController, Listeners are automatically updated, e.g to react to changes in the
    /// ParameterData instance.
    /// Furthermore, if a ParameterData instance is already set on the AlveolusController, the HandleReset method is called.
    /// This allows to react to any kind of initialization by properly implementing HandleReset.
    /// </summary>
    public class AlveolusComponentBehavior : MonoBehaviour
    {
        [SerializeField]
        private AlveolusController alveolus;

        public AlveolusController Alveolus
        {
            get => alveolus;
            set => SetInstance(value);
        }

        private void SetInstance(AlveolusController value)
        {
            if (alveolus != null)
            {
                RemoveAlveolusListeners();
            }

            alveolus = value;

            if (alveolus != null)
            {
                AddAlveolusListeners();
                
                if (alveolus.instanceParameters)
                {
                    HandleParametersInstanceChanged(Alveolus);
                    HandleResetInternal(Alveolus);
                    HandleReset(Alveolus);
                }
            }
        }

        private void AddAlveolusListeners()
        {
            alveolus.onParametersInstanceChange.AddListener(HandleParametersInstanceChanged);
            alveolus.onReset.AddListener(HandleReset);
            alveolus.onReset.AddListener(HandleResetInternal);
            alveolus.onPause.AddListener(HandlePause);
        }

        private void RemoveAlveolusListeners()
        {
            alveolus.onParametersInstanceChange.RemoveListener(HandleParametersInstanceChanged);
            alveolus.onReset.RemoveListener(HandleReset);
            alveolus.onReset.RemoveListener(HandleResetInternal);
            alveolus.onPause.RemoveListener(HandlePause);
        }
        
        private void HandleParametersInstanceChanged(AlveolusController instance)
        {
            if (Parameters != null)
            {
                Parameters.Unsubscribe(HandleParametersUpdated);
            }

            Parameters = alveolus.instanceParameters;
            Parameters.Subscribe(HandleParametersUpdated);
        }

        private void HandleResetInternal(AlveolusController instance)
        {
        }

        protected virtual void OnDestroy()
        {
            if (alveolus != null)
            {
                RemoveAlveolusListeners();
            }
            
            if (Parameters != null)
            {
                Parameters.Unsubscribe(HandleParametersUpdated);
            }
        }

        protected ParametersData Parameters { get; private set; }

        protected PartialPressure PartialPressure => alveolus ? alveolus.partialPressures : null;
        protected HbSaturation HbSaturation => alveolus ? alveolus.hbSaturation : null;
        protected BloodOutflow BloodOutflow => alveolus ? alveolus.bloodOutflow : null;

        protected OutputCollector OutputCollector => alveolus ? alveolus.results : null;

        protected void Awake()
        {
            Alveolus = alveolus ? alveolus : GetComponentInParent<AlveolusController>();
        }

        protected void Start()
        {
            if (!Alveolus)
            {
                Alveolus = alveolus ? alveolus : GetComponentInParent<AlveolusController>();
                if (Parameters)
                {
                    HandleResetInternal(Alveolus);
                    HandleReset(Alveolus);
                }
            }
        }

        protected virtual void HandleReset(AlveolusController instance)
        {
        }

        protected virtual void HandlePause(AlveolusController instance)
        {
        }

        protected virtual void HandleParametersUpdated()
        {
        }
    }
}