using System;
using Simulation.Systems;
using UnityEngine;

namespace Simulation.Visualization.TissueBarrier
{
    /// <summary>
    /// This class selects the appropriate tissue mesh according to the current barrier thickness and activates it
    /// while all other meshes are deactivated.
    /// </summary>
    public class TissueBarrierThickness : AlveolusComponentBehavior
    {
        [SerializeField] private GameObject thickness1;
        [SerializeField] private GameObject thickness2;
        [SerializeField] private GameObject thickness3;
        [SerializeField] private GameObject thickness4;
        [SerializeField] private GameObject thickness5;

        private GameObject[] m_meshes;
        private GameObject m_activeMesh;

        private float m_barrierThickness;

        protected new void Start()
        {
            base.Start();
            m_meshes = new[] {thickness1, thickness2, thickness3, thickness4, thickness5};
            ChooseMesh();
            m_activeMesh.SetActive(true);
        }

        protected override void HandleReset(AlveolusController instance)
        {
            Init();
        }

        private void Init()
        {
            m_barrierThickness = Parameters.barrierThickness;
            HandleParametersUpdated();
        }

        protected override void HandleParametersUpdated()
        {
            if (m_meshes == null || (m_meshes.Length <= 0))
            {
                return;
            }
            
            if (Mathf.Approximately(m_barrierThickness, Parameters.barrierThickness))
                return;

            ChooseMesh();

            m_activeMesh.SetActive(true);
            foreach (GameObject mesh in m_meshes)
            {
                if (mesh != m_activeMesh)
                {
                    mesh.SetActive(false);
                }
            }
        }

        /// <summary>
        /// According to the result of the method <see cref="Remap(float) "/> the appropriate mesh that represents 
        /// the current tissue barrier thickness gets picked out of the array of meshes <see cref="m_meshes"/> and 
        /// is stored in the variable <see cref="m_activeMesh"/>.
        /// </summary>
        private void ChooseMesh()
        {
            m_barrierThickness = Parameters.barrierThickness;

            int thicknessScale = Remap(m_barrierThickness);
            m_activeMesh = m_meshes[thicknessScale];
        }

        /// <summary>
        /// This method takes a given tissue barrier thickness value, normalizes it to the range of possible thickness values and 
        /// maps it to a new scale: The amount of meshes available. 
        /// </summary>
        private int Remap(float thickness)
        {
            float thicknessMin = 0.2f;
            float thicknessMax = 3;
            float scaleMin = 0;
            float scaleMax = m_meshes.Length - 1;
            return (int) Math.Round((thickness - thicknessMin) / (thicknessMax - thicknessMin) * (scaleMax - scaleMin) + scaleMin);
        }
    }
}