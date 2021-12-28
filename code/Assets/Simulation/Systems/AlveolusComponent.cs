using System.Collections.Generic;
using Simulation.Systems.GasExchange;
using Simulation.Systems.Output;
using UnityEngine;

namespace Simulation.Systems
{
    /// <summary>
    /// A base class for simulation components that provides easy access to other simulation components through AlvoelusController.
    /// When SetInstance is called with a new AlveolusController, Listeners are automatically updated, e.g to react to changes in the
    /// ParameterData instance.
    /// Furthermore, if a ParameterData instance is already set on the AlveolusController, the HandleReset method is called.
    /// This allows to react to any kind of initialization by properly implementing HandleReset.
    /// </summary>
    public class AlveolusComponent
    {
        private AlveolusController m_alveolus;

        public AlveolusController Alveolus
        {
            get => m_alveolus;
            set => SetInstance(value);
        }


        ~AlveolusComponent()
        {
            if (Parameters != null)
            {
                Parameters.Unsubscribe(HandleParametersUpdated);
            }
            
            if (m_alveolus != null)
            {
                RemoveAlveolusListeners();
            }
        }
        
        
        private void SetInstance(AlveolusController value)
        {
            if (m_alveolus != null)
            {
                RemoveAlveolusListeners();
            }

            m_alveolus = value;
            
            if (m_alveolus != null)
            {
                AddAlveolusListeners();
                
                if (m_alveolus.instanceParameters)
                {
                    HandleParametersInstanceChanged(Alveolus);
                    HandleResetInternal(Alveolus);
                    HandleReset(Alveolus);
                }
            }
        }

        private void AddAlveolusListeners()
        {
            m_alveolus.onParametersInstanceChange.AddListener(HandleParametersInstanceChanged);
            m_alveolus.onReset.AddListener(HandleReset);
            m_alveolus.onReset.AddListener(HandleResetInternal);
            m_alveolus.onPause.AddListener(HandlePause);
        }

        private void RemoveAlveolusListeners()
        {
            m_alveolus.onParametersInstanceChange.RemoveListener(HandleParametersInstanceChanged);
            m_alveolus.onReset.RemoveListener(HandleReset);
            m_alveolus.onReset.RemoveListener(HandleResetInternal);
            m_alveolus.onPause.RemoveListener(HandlePause);
        }

        private void HandleParametersInstanceChanged(AlveolusController instance)
        {
            if (Parameters != null)
            {
                Parameters.Unsubscribe(HandleParametersUpdated);
            }

            Parameters = m_alveolus.instanceParameters;
            Parameters.Subscribe(HandleParametersUpdated);
        }

        private void HandleResetInternal(AlveolusController instance)
        {
        }

        protected ParametersData Parameters { get; private set; }

        protected PartialPressure PartialPressure => m_alveolus ? m_alveolus.partialPressures : null;
        protected HbSaturation HbSaturation => m_alveolus ? m_alveolus.hbSaturation : null;
        protected BloodOutflow BloodOutflow => m_alveolus ? m_alveolus.bloodOutflow : null;

        protected OutputCollector OutputCollector => m_alveolus ? m_alveolus.results : null;

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