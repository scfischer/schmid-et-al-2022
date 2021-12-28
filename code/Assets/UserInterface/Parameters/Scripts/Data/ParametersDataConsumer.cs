using Simulation;
using UnityEngine;
using Support;

namespace UserInterface.Parameters.Data
{
    /// <summary>
    /// Abstract class for all child classes that manage the user configuration of model parameters in the UI.
    /// It organizes the passing of <see cref="ParameterData"/> of the active simulation instance to the UI 
    /// (via <see cref="ParametersDataProvider"/>).
    /// </summary>
    public abstract class ParametersDataConsumer : MonoBehaviour
    {
        private ParametersDataProvider dataProvider;

        private ParametersData parametersDataInstance;

        public ParametersData ParametersDataInstance
        {
            get => parametersDataInstance;
            private set
            {
                if (parametersDataInstance != null)
                {
                    parametersDataInstance.Unsubscribe(ParametersDataChangedHandler);
                }

                UnsubscribeHandler(parametersDataInstance);

                parametersDataInstance = value;

                if (parametersDataInstance != null)
                    parametersDataInstance.Subscribe(ParametersDataChangedHandler);

                SubscribeHandler();
            }
        }

        protected void Start()
        {
            dataProvider = gameObject.GetComponentInParent<ParametersDataProvider>();
            if (dataProvider == null)
            {
                Debug.LogError($"{name}: ParametersDataProvider not found! Disabling...");
                gameObject.SetActive(false);
                return;
            }

            dataProvider.onParametersDataInstanceChanged.AddListener(ParametersDataInstanceChangedHandler);
            ParametersDataInstanceChangedHandler(dataProvider.ParametersData);
        }

        private void ParametersDataInstanceChangedHandler(ParametersData instance)
        {
            ParametersDataInstance = instance;
            if(ParametersDataInstance != null)
                ParametersDataChangedHandler();
        }

        protected abstract void ParametersDataChangedHandler();

        protected virtual void UnsubscribeHandler(ParametersData oldInstance)
        {
        }

        protected virtual void SubscribeHandler()
        {
            if(ParametersDataInstance != null)
                ParametersDataChangedHandler();
        }
    }
}