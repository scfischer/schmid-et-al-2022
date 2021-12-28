using System;
using System.Collections;
using Support;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface.Tooltips
{
    public class MouseOverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Content")]
        [Multiline(10)]
        public string text;

        [Header("Configuration")]
        [SerializeField]
        private AppSettings appSettings;

        [SerializeField]
        private GameObject tooltipPrefab;

        private TooltipTextBox m_tooltip;

        private TooltipTextBox TooltipTextBox
        {
            get
            {
                if (m_tooltip)
                    return m_tooltip;

                m_tooltip = GameObject.FindWithTag("Tooltip")?.GetComponent<TooltipTextBox>();

                if (!m_tooltip)
                    m_tooltip = Instantiate(tooltipPrefab, gameObject.GetComponentInParent<Canvas>().transform)
                        .GetComponent<TooltipTextBox>();
                return m_tooltip;
            }
        }

        protected virtual string GetText()
        {
            return text;
        }

        protected virtual Vector2 TooltipPosition()
        {
            return Input.mousePosition;
        }

        protected void OnDisable()
        {
            if (m_tooltip != null)
            {
                m_tooltip.Hide();
            }
        }

        private IEnumerator DelayTooltipOpen()
        {
            yield return new WaitForSeconds(appSettings.TooltipDelay);
            TooltipTextBox.Show(GetText(), TooltipPosition());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(DelayTooltipOpen());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            TooltipTextBox.Hide();
        }
    }
}
