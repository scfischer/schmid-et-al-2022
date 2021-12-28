using System;
using TMPro;
using UnityEngine;

namespace UserInterface.Elements
{
    public class LabeledValue : MonoBehaviour
    {
        public string numberFormat = "N2";

        [SerializeField]
        private TMP_Text _valueText;
        
        [SerializeField]
        private TMP_Text _labelText;

        public float Value
        {
            get => float.Parse(_valueText.text);
            set => _valueText.text = value.ToString(numberFormat);
        }

        private void Start()
        {
            Value = Value;
        }

    }
    
}