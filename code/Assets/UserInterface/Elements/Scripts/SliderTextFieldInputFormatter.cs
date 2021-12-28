using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Elements
{
    /// <summary>
    /// Script to displays a (numeric) value from an external source in the <c>InputField</c> this script is attached to.
    /// </summary>
    public class SliderTextFieldInputFormatter : MonoBehaviour
    {
        private InputField m_input;

        public string numberFormat = "{0:N2}";
        
        [SerializeField]
        private Slider m_slider;

        void Awake()
        {
            m_input = GetComponent<InputField>();
            m_slider.onValueChanged.AddListener(UpdateDisplayValue);
        }

        /// <summary>
        /// Updates the InputField this script is attached to with a string representation of the floating point number
        /// passed as parameter. 
        /// </summary>
        /// <remarks>
        /// Rounds the result to the nearest integer before string conversion for values of 3 or larger, and creates a
        /// string output as number with two decimal places for smaller values. 
        /// </remarks>
        /// <param name="value">The value that the input field should display.</param>
        public void UpdateDisplayValue(float value)
        {
            var formattedValue = string.Format(numberFormat, value);
            m_input.SetTextWithoutNotify(formattedValue); 
        }
    }
}
