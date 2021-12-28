using UnityEngine;
using UserInterface.Tooltips;

namespace UserInterface.MultiInstance
{
    public class InstanceTemplateDropdownTooltip : MouseOverTooltip
    {
        private InstanceTemplateElement m_instanceTemplateElement;
        private int m_index;
        private RectTransform rectTransform;

        private void Start()
        {
            m_instanceTemplateElement = GetComponentInParent<InstanceTemplateElement>();
            m_index = int.Parse(name.Split(':')[0].Split(' ')[1]);

            rectTransform = GetComponent<RectTransform>();
        }

        protected override string GetText()
        {
            return m_instanceTemplateElement.GetElementTooltip(m_index);
        }

        protected override Vector2 TooltipPosition()
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);
            return worldCorners[3];
        }
    }
}