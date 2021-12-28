using Support;
using UnityEngine;
using UnityEngine.EventSystems;
using UserInterface.MultiInstance;

namespace UserInterface.Highlighting
{
    /// <summary>
    /// This class should be attached to GUI elements that, when hovered over, should  cause other objects in the scene to be highlighted.
    /// Needs to be assigned to a target for highlighting using the <see cref="HighlightController"/>, by dragging this
    /// trigger and the target containing an <see cref="HighlightController.highlightAssociations"/> component into the
    /// <see cref="HighlightController"/> listing.
    /// </summary>
    public class HighlightTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private AppSettings appSettings;

        /// <summary>
        /// Field for setting additional parameters in the inspector, that get passed to the highlighting script.
        /// Used in conjunction with a property, since Unity can't make auto-properties available in the inspector.
        /// </summary>
        [SerializeField]
        private string _parameters;
        /// <summary>
        /// Getter property for the <see cref="_parameter"/> field. 
        /// </summary>
        public string parameters => _parameters;

        private HighlightController highlightController;

        private UnityEngine.UI.Image panel;

        private Color baseTint;

        void Start()
        {
            highlightController = transform.root.GetComponentInChildren<HighlightController>();

            panel = GetComponentInParent<UnityEngine.UI.Image>();
            baseTint = panel.color;
        }


        /// <summary>
        /// Inherited method from interface UnityEngine.EvenSystems.IPointerEnterHandler, used to tell the
        /// <see cref="HighlightController"/> to enable the associated highlights when the mouse is over the GUI element
        /// that this script is attached to.
        /// </summary>
        /// <param name="eventData">not used</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // TODO: don't change highlighted element when leaving the GUI element while still dragging a slider
            // PROBLEM: doing it with the following method in OnPointerEnter will make it not change when mouse button
            // is let go while in a different highlight trigger
            // const int lmb = 0;
            // if (Input.GetMouseButton(lmb))
            // {
            //     return;
            // }

            highlightController.ActivateHighlight(this);
            if (appSettings.HighlightsEnabled)
                panel.color = baseTint * appSettings.ColorPallete.HighlightPanelColor;
        }

        /// <summary>
        /// Inherited method from interface UnityEngine.EvenSystems.IPointerExitHandler, used to tell the
        /// <see cref="HighlightController"/> to disable the associated highlights when the mouse is over the GUI element
        /// that this script is attached to.
        /// </summary>
        /// <param name="eventData">not used</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // TODO: don't change highlighted element when leaving the GUI element while still dragging a slider
            // const int lmb = 0;
            // if (Input.GetMouseButton(lmb))
            // {
            //     return;
            // }

            highlightController.DeactivateHighlight(this);
            panel.color = baseTint;
        }
    }
}