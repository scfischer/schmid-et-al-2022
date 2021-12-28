using TMPro;
using UnityEngine;
using UserInterface.TimeControls;

namespace UserInterface.Tooltips
{
    /// <summary>
    /// Tooltip box that follows the mouse cursor.
    /// </summary>
    public class TooltipTextBoxPersistentFollowMouse : TooltipTextBox
    {
        [SerializeField]
        private Vector2 offset;


        public override void Show(string text, Vector2 screenPosition)
        {
            base.Show(text, screenPosition);

            Cursor.visible = true;
            UpdatePosition();
        }

        protected virtual void Update()
        {
            if (lastPointerPosition != Input.mousePosition)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector2 cursorSizeOffset = new Vector2(10, 10);
            Vector2 mousePosition = Input.mousePosition;
            Vector2 totalOffset;
            totalOffset.x = mousePosition.x < Screen.width / 2.0f ? (offset.x + cursorSizeOffset.x) : -offset.x;
            totalOffset.y = mousePosition.y < Screen.height / 2.0f ? offset.y : -(offset.y + cursorSizeOffset.y);

            transform.position = mousePosition + totalOffset;
        }
    }
}
