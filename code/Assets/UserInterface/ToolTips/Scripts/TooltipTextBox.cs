using TMPro;
using UnityEngine;

namespace UserInterface.Tooltips
{
    public class TooltipTextBox : MonoBehaviour
    {
        private float m_paddingLeft;
        private float m_paddingRight;
        private float m_paddingTop;
        private float m_paddingBottom;
        private TMP_Text m_text;
        private RectTransform m_rectTransform;
        protected Vector3 lastPointerPosition;

        public virtual void Show(string text, Vector2 screenPosition)
        {
            gameObject.SetActive(true);
            m_text.text = text;
            var pivot = new Vector2(screenPosition.x < Screen.width / 2.0f ? 0 : 1,
            screenPosition.y < Screen.height / 2.0f ? 0 : 1);
            m_rectTransform.pivot = pivot;
            m_rectTransform.position = screenPosition;
            lastPointerPosition = Input.mousePosition;
            FitSizeToText();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            m_text = GetComponentInChildren<TMP_Text>();
            m_rectTransform = GetComponent<RectTransform>();
            m_paddingLeft = m_text.rectTransform.offsetMin.x;
            m_paddingRight = -m_text.rectTransform.offsetMax.x;
            m_paddingTop = m_text.rectTransform.offsetMin.y;
            m_paddingBottom = -m_text.rectTransform.offsetMax.y;
        }

        private void FitSizeToText()
        {
            var size = m_text.GetPreferredValues(m_text.text);
            m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                size.x + m_paddingLeft + m_paddingRight);
            m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + m_paddingBottom + m_paddingTop);
            m_text.rectTransform.offsetMin = new Vector2(m_paddingLeft, m_paddingBottom);
            m_text.rectTransform.offsetMax = new Vector2(-m_paddingRight, -m_paddingTop);
        }
    }
}
