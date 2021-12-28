using UnityEngine;
using UnityEngine.UI;
using UserInterface.Elements;

namespace UserInterface.Elements
{
    public class ParameterSlider : MonoBehaviour
    {
        public Slider valueSlider;

        [SerializeField]
        private SliderTextFieldInputFormatter textField; 

        public float Value => valueSlider.value;

        public Slider.SliderEvent onValueChanged;

        private void Awake()
        {
            valueSlider.onValueChanged.AddListener(value => onValueChanged.Invoke(value));
        }

        public void SetValueWithoutNotify(float value)
        {
            valueSlider.SetValueWithoutNotify(value);
            textField.UpdateDisplayValue(value);
        }
    }
}