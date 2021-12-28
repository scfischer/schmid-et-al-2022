using TMPro;
using UnityEngine;

namespace UserInterface.Tooltips
{
    /// <summary>
    /// Tooltip box that follows the mouse cursor.
    /// </summary>
    public class TooltipTextBoxShowOnce : TooltipTextBox
    {
        public override void Show(string text, Vector2 screenPosition)
        {
            base.Show(text, screenPosition);

            Cursor.visible = false;
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.visible = true;
        }

        protected virtual void Update()
        {
            if (lastPointerPosition != Input.mousePosition)
                Hide();
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }
    }
}
