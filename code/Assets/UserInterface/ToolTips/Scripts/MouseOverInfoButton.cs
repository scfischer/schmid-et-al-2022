using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace UserInterface.Tooltips
{
    public class MouseOverInfoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private MouseOverTooltipReappearing tooltipCreator;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            tooltipCreator.mouseOverInfoButton = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            tooltipCreator.mouseOverInfoButton = false;
        }
    }
}

