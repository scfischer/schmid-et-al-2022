using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenu
{
    public class IconToggle : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private Sprite iconEnabled;

        [SerializeField]
        private Sprite iconDisabled;

        public void SetIcon(bool switchEnabled)
        {
            icon.sprite = switchEnabled ? iconEnabled : iconDisabled;
        }
    }
}
