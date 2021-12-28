using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Elements
{ 
    /// <summary>
    /// Set a slider that this script is attached to a value from an external source.
    /// </summary>
    public class SliderTextField : MonoBehaviour
    {
        private Slider m_slider;

        /// <summary> Reference to a text input field from which to set the slider value. </summary>
        [SerializeField]
        private InputField inputField;
        
        void Awake()
        {
            m_slider = GetComponent<Slider>();
            inputField.onEndEdit.AddListener(SetValue);
        }

        /// <summary>
        /// Set the slider to a value provided as parameter. Limits the value to min/max allowed by the slider. 
        /// </summary>
        /// <param name="value">Value that the slider should be set to.</param>
        public void SetValue(string value)
        {
            float input = float.Parse(value);
            m_slider.value = Mathf.Max(m_slider.minValue, Mathf.Min(m_slider.maxValue, input));
        }
    }
}
