using System;
using System.Collections.Generic;
using Simulation;
using Simulation.Systems;
using Support;
using UnityEngine;

namespace UserInterface.MultiInstance
{
    /// <summary>
    /// This class organizes simultaneous simulation of multiple <see cref="AlveolusController"/> instances.
    /// </summary>
    /// <remarks>
    /// It keeps a list of active AlveolusController instances. The length of this list is confined to a limit
    /// defined in <see cref="AppSettings"/>. Here, main functionalities such as creation of new instances, 
    /// cloning or deletion of existing instances are implemented. Only the 3D model associated to the selected instance
    /// is visualized.
    /// </remarks>
    public class InstanceMenu : MonoBehaviour
    {
        [SerializeField]
        private AppSettings appSettings;
        
        public InstanceProvider instanceProvider;
        public GameObject simulationPrefab;
        public GameObject instanceButtonPrefab;
        public ParametersData parametersTemplate;
        public GameObject instanceButtonContainer;

        private readonly List<AlveolusController> m_instances = new List<AlveolusController>();
        private readonly List<GameObject> m_instanceButtons = new List<GameObject>();
        private InstanceColorMap m_instanceColorMap;
        
        public delegate void InstanceCreationAllowedChangedEvent(bool isAllowed);
        public event InstanceCreationAllowedChangedEvent InstanceCreationAllowedChanged;

        public delegate void AllInstancesResetEvent();
        public event AllInstancesResetEvent AllInstancesReset;

        public bool InstanceCreationAllowed => m_instances?.Count < appSettings.MaxNumInstances;
        
        private void Start()
        {
            m_instanceColorMap = GetComponentInParent<InstanceColorMap>();
            var instances = FindObjectsOfType<AlveolusController>();
            m_instances.AddRange(instances);
            RebuildButtons();
            if (m_instances.Count > 0)
            {
                SelectInstance(m_instances[0]);
            }
            
            appSettings.onMaxInstancesChanged += () => InstanceCreationAllowedChanged?.Invoke(InstanceCreationAllowed);
            InstanceCreationAllowedChanged?.Invoke(InstanceCreationAllowed);
        }

        public void ApplyParameterSetToCurrentInstance()
        {
            instanceProvider.SimulationAlveolus.instanceParameters.SetCurrentValues(parametersTemplate.GetDefaults());
            //ResetInstances();
        }

        public void DuplicateInstance(AlveolusController template)
        {
            if (!InstanceCreationAllowed)
            {
                throw new Exception("Maximum number of instances reached!");
            }
            
            var instance = Instantiate(simulationPrefab).GetComponent<AlveolusController>();
            var parameters = Instantiate(template.instanceParameters);
            parameters.SetCurrentValues(template.instanceParameters.GetAllParameters());
            instance.instanceParameters = parameters; 
            instance.name = template.name + " (clone)";
            m_instances.Add(instance);
            CreateButton(instance);
            SelectInstance(instance);
            InstanceCreationAllowedChanged?.Invoke(InstanceCreationAllowed);
            ResetInstances();
        }
        
        public void AddInstance()
        {
            if (!InstanceCreationAllowed)
            {
                throw new Exception("Maximum number of instances reached!");
            }
            var instance = Instantiate(simulationPrefab).GetComponent<AlveolusController>();
            instance.instanceParameters = Instantiate(parametersTemplate);
            instance.name = parametersTemplate.name;
            m_instances.Add(instance);
            CreateButton(instance);
            SelectInstance(instance);
            InstanceCreationAllowedChanged?.Invoke(InstanceCreationAllowed);
            ResetInstances();
        }

        public void DeleteInstance(AlveolusController alveolus)
        {
            m_instances.Remove(alveolus);
            m_instanceColorMap.ReturnColor(alveolus);
            Destroy(alveolus.gameObject);
            if (m_instances.Count > 0)
                SelectInstance(m_instances[m_instances.Count - 1]);
            InstanceCreationAllowedChanged?.Invoke(InstanceCreationAllowed);
        }

        public void SelectInstance(AlveolusController alveolusToSelect)
        {
            alveolusToSelect.ReturnToFocus();
            instanceProvider.SimulationAlveolus = alveolusToSelect;
            foreach (AlveolusController instance in m_instances)
            {
                if (instance != alveolusToSelect)
                    instance.MoveOutOfFocus();
            }
        }

        public void TogglePause()
        {
            foreach (AlveolusController instance in m_instances)
            {
                instance.Pause();
            }
        }

        public void ResetInstances()
        {
            foreach (AlveolusController instance in m_instances)
            {
                instance.Reset();
            }
            AllInstancesReset?.Invoke();
        }

        private void CreateButton(AlveolusController alveolus)
        {
            GameObject go = Instantiate(instanceButtonPrefab, instanceButtonContainer.transform);
            InstanceMenuEntry entry = go.GetComponent<InstanceMenuEntry>();
            entry.SetInstance(alveolus);
            entry.SetInstanceMenu(this);
            m_instanceButtons.Add(go);
        }

        private void RebuildButtons()
        {
            foreach (GameObject go in m_instanceButtons)
            {
                Destroy(go);
            }

            m_instanceButtons.Clear();

            foreach (AlveolusController instance in m_instances)
            {
                CreateButton(instance);
            }
        }
    }
}