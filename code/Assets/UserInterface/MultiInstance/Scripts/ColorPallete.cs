using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.MultiInstance
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "Alvoelus/ColorPalette", order = 0)]
    public class ColorPallete : ScriptableObject
    {
        [FormerlySerializedAs("colors")] 
        public List<Color> instanceColors;

        [Header("Health Condition")]
        public Color nominal;
        public Color abnormal;
        public Color severe;
        public Color unrealistic;
        
        [Header("Highlights")]
        [SerializeField]
        public Color HighlightPanelColor = new Color(0.9f, 0.9f, 0.7f, 1.0f);
    }
}