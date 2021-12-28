using System.Collections;
using Support;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface.Tooltips
{
    /// <summary>
    /// Creates a Tooltip that gets hidden again if the mouse moves, works best with larger tooltip delays.
    /// </summary>
    public class MouseOverTooltipReappearing : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Content")]
        [Multiline(10)]
        public string text;

        [Header("Configuration")]
        [SerializeField]
        private AppSettings appSettings;

        [SerializeField]
        private GameObject tooltipPrefab;

        public bool mouseOverInfoButton;

        private TooltipTextBox m_tooltip;
        private Coroutine delayCoroutine;

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

        private IEnumerator TrackWhenTooltipShouldAppear()
        {
            Vector3 lastRecordedMousePosition;
            while (true)
            {
                lastRecordedMousePosition = Input.mousePosition;

                if (TooltipTextBox.gameObject.activeSelf)
                {
                    TooltipTextBox.Hide();
                }

                RestartDelayedTooltipOpen();

                Vector3 position = lastRecordedMousePosition;
                yield return new WaitUntil(() => position != Input.mousePosition);
            }
        }

        private void RestartDelayedTooltipOpen()
        {
            if (delayCoroutine != null)
                StopCoroutine(delayCoroutine);
            if (!mouseOverInfoButton)
                delayCoroutine = StartCoroutine(DelayTooltipOpen());
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
            if (m_tooltip != null)
            {
                m_tooltip.Hide();
            }
        }

        private IEnumerator DelayTooltipOpen()
        {
            yield return new WaitForSeconds(appSettings.TooltipDelay);
            TooltipTextBox.Show(GetText(), Input.mousePosition);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(TrackWhenTooltipShouldAppear());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            TooltipTextBox.Hide();
        }
    }
}
