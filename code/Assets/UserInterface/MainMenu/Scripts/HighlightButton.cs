using Support;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenu
{
    public class HighlightButton : MonoBehaviour
    {
        [SerializeField]
        private AppSettings appSettings;

        private Button m_button;
        private IconToggle m_iconToggle;

        private void ToggleHighlightsEnabled()
        {
            appSettings.HighlightsEnabled = !appSettings.HighlightsEnabled;
        }

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_iconToggle = GetComponent<IconToggle>();
            m_button.onClick.AddListener(ToggleHighlightsEnabled);
            appSettings.HighlightsEnabledChanged += m_iconToggle.SetIcon;
        }

        private void Start()
        {
            m_iconToggle.SetIcon(appSettings.HighlightsEnabled);
        }
    }
}