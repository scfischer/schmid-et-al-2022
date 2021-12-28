using System;
using UnityEngine;
using UserInterface.MultiInstance;

namespace Support
{
    /// <summary>
    /// This class stores some general settings that apply independently of the model simulation.
    /// These include gas units, color palletes and settings concerning the instance menu (<see cref="UserInterface.MultiInstance"/>, 
    /// visual highlighting (<see cref="UserInterface.Highlighting"/>) and tooltips.
    /// </summary>
    public enum GasUnit
    {
        mmHg,
        kPa
    };

    public static class GasUnitExtensions
    {
        public static float GetConversionFactor(this GasUnit unit)
        {
            switch (unit)
            {
                case GasUnit.mmHg:
                    return 1;
                case GasUnit.kPa:
                    return 0.133f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }
    }

    [CreateAssetMenu]
    public class AppSettings : ScriptableObject
    {
        [Header("Colors")]
        [SerializeField]
        private ColorPallete m_colorPallete;
        public ColorPallete ColorPallete => m_colorPallete;


        [Header("Highlighting")]

        [SerializeField]
        private float _fadeOutDuration = 1.0f;
        public float FadeOutDuration => _fadeOutDuration;

        [SerializeField]
        private float _fadeInDuration = 0.5f;
        public float FadeInDuration => _fadeInDuration;

        [SerializeField]
        private bool highlightsEnabled;
        public bool HighlightsEnabled
        {
            get => highlightsEnabled;
            set
            {
                highlightsEnabled = value;
                HighlightsEnabledChanged?.Invoke(highlightsEnabled);
            }
        }

        public delegate void HighlightsEnabledChangedEvent(bool enabled);

        public event HighlightsEnabledChangedEvent HighlightsEnabledChanged;

        [Header("Units")]
        [SerializeField]
        private GasUnit gasUnit = GasUnit.mmHg;
        public GasUnit GasUnit
        {
            get => gasUnit;
            set
            {
                gasUnit = value;
                GasUnitChanged?.Invoke(gasUnit);
            }
        }

        public delegate void GasUnitChangedEvent(GasUnit unit);

        public event GasUnitChangedEvent GasUnitChanged;


        [Header("Tooltips")]
        [SerializeField]
        private float tooltipDelay = .3f;
        public float TooltipDelay => tooltipDelay;

        [Header("Instance Creation")]
        [SerializeField]
        private int maxNumInstances = 6;
        public int MaxNumInstances
        {
            get => maxNumInstances;
            set
            {
                maxNumInstances = value;
                onMaxInstancesChanged?.Invoke();
            }
        }
        
        public delegate void MaxInstancesChangedEvent();
        public event MaxInstancesChangedEvent onMaxInstancesChanged;

        [Header("User Interface")]
        [SerializeField]
        private bool draggablePanels = false;
        public bool DraggablePanels => draggablePanels;
    }
}