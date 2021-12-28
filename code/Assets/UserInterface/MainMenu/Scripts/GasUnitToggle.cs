using Support;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MainMenu
{
    public class GasUnitToggle : MonoBehaviour
    {
        [SerializeField]
        private AppSettings appSettings;

        private Button m_button;
        private IconToggle m_iconToggle;

        private void Toggle()
        {
            appSettings.GasUnit = appSettings.GasUnit == GasUnit.kPa ? GasUnit.mmHg : GasUnit.kPa;
        }

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_iconToggle = GetComponent<IconToggle>();
            m_button.onClick.AddListener(Toggle);
            appSettings.GasUnitChanged += HandleGasUnitChanged;
        }

        private void HandleGasUnitChanged(GasUnit unit)
        {
            m_iconToggle.SetIcon(unit == GasUnit.mmHg);
        }

        private void Start()
        {
            HandleGasUnitChanged(appSettings.GasUnit);
        }
    }
}