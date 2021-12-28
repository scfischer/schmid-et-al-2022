using System.Collections.Generic;
using Simulation.Systems;
using Support;
using UnityEngine;

namespace UserInterface.MultiInstance
{
    public class InstanceColorMap : MonoBehaviour
    {
        [SerializeField]
        private AppSettings m_appSettings;
        private readonly Dictionary<AlveolusController, int> m_instanceColorMap =
            new Dictionary<AlveolusController, int>();
        private readonly Queue<int> m_freeColorIndices = new Queue<int>();

        public void Awake()
        {
            for (int i = 0; i < m_appSettings.ColorPallete.instanceColors.Count; i++)
                m_freeColorIndices.Enqueue(i);
        }

        public Color GetColor(AlveolusController instance)
        {
            if (m_instanceColorMap.ContainsKey(instance))
                return m_appSettings.ColorPallete.instanceColors[m_instanceColorMap[instance]];

            var index = m_freeColorIndices.Dequeue();
            m_instanceColorMap.Add(instance, index);
            return m_appSettings.ColorPallete.instanceColors[m_instanceColorMap[instance]];
        }

        public void ReturnColor(AlveolusController instance)
        {
            if (m_instanceColorMap.ContainsKey(instance))
            {
                m_freeColorIndices.Enqueue(m_instanceColorMap[instance]);
                m_instanceColorMap.Remove(instance);
            }
        }
    }
}