using Support;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.Elements;
using UserInterface.MultiInstance;

namespace UserInterface.HealthStatus
{
    public class HealthStatusElement : MonoBehaviour
    {
        public enum HealthCondition
        {
            Nominal,
            Abnormal,
            Serious,
            Unrealistic
        }

        public AppSettings appSettings;

        [SerializeField]
        private HealthCondition condition;

        [SerializeField]
        private Image buttonImage;


        public string Label
        {
            get => m_labelText.text;
            set => m_labelText.text = value;
        }


        public Color GetColor(HealthCondition condition)
        {
            switch (condition)
            {
                case HealthCondition.Nominal:
                    return appSettings.ColorPallete.nominal;
                case HealthCondition.Abnormal:
                    return appSettings.ColorPallete.abnormal;
                case HealthCondition.Serious:
                    return appSettings.ColorPallete.severe;
                case HealthCondition.Unrealistic:
                    return appSettings.ColorPallete.unrealistic;
                default:
                    return appSettings.ColorPallete.nominal;
            }
        }

        public HealthCondition Condition
        {
            get => condition;
            set
            {
                condition = value;
                m_image.color = GetColor(condition);
                if (m_slider)
                {
                    m_slider.valueSlider.fillRect.GetComponent<Image>().color = GetColor(condition);
                    m_slider.valueSlider.image.color = GetColor(condition);
                }
                if (buttonImage)
                    buttonImage.color = GetColor(condition);
            }
        }

        private TMP_Text m_labelText;
        private Image m_image;
        private ParameterSlider m_slider;

        private void Awake()
        {
            m_labelText = GetComponentInChildren<TMP_Text>();
            m_image = GetComponent<Image>();
            m_slider = transform.parent.GetComponentInChildren<ParameterSlider>();
            Condition = condition;
            Label = condition.ToString();
        }
        
    }
}