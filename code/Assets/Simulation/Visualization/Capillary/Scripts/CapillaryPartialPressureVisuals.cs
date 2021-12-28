using System;
using Simulation.SimulatedCapillary.GasExchange;
using Simulation.Systems;
using Simulation.Systems.GasExchange;
using Unity.Collections;
using UnityEngine;

namespace Simulation.SimulatedCapillary.GasExchange
{
    /// <summary>
    /// This class is responsible for tinting the capillary material to represent oxygenation of the blood flowing through.
    /// </summary>
    /// <remarks>
    /// This is achieved by taking the oxygen partial pressure of the blood in each capillary section, 
    /// determining a color to represent it, then using that color for the central column of pixels of a section. 
    /// To achieve a more appealing result, a gradient is calculated for the transition between adjacent sections. 
    /// The color values are applied dynamically to the main texture of the capillary material.
    /// </remarks>
    public class CapillaryPartialPressureVisuals : AlveolusComponentBehavior
    {
        /// <summary> <c>Color</c> used for oxygen partial pressure value at lower limit. </summary>
        [SerializeField]
        private Color minValueColor = Color.blue;

        /// <summary> <c>Color</c> used for oxygen partial pressure value at upper limit. </summary>
        [SerializeField]
        private Color maxValueColor = Color.red;

        /// <summary> Oxygen partial pressure at or below which the <c>minValueColor</c> is used. </summary>
        [SerializeField]
        private float minValue = 40.0f;

        /// <summary> Oxygen partial pressure at or above which the <c>maxValueColor</c> is used. </summary>
        [SerializeField]
        private float maxValue = 100.0f;

        private Material m_capillaryMaterial;
        public Renderer m_inflowRenderer;
        public Renderer m_outflowRenderer;

        public bool shaderSupportsArray = false;

        private string[] sectionColorUniformNames;
        
        
        private PartialPressure partialPressureScript;

        private new void Awake()
        {
            m_capillaryMaterial = GetComponentInChildren<Renderer>().material;
            base.Awake();
        }

        private new void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Creates an array of identically formatted names for the colors. 
        /// The length of the array corresponds to the number of capillary sections.
        /// </summary>
        /// <param name="n_sections"> Number of capillary sections, see <see cref="PartialPressure.numberSections"/>. </param>
        private string[] SectionColorUniformNames(int n_sections)
        {
            string[] names = new string[n_sections];
            for (int i = 0; i < n_sections; i++)
            {
                names[i] = string.Format("_Color{0}",i);
            }

            return names;
        }

        protected override void HandleReset(AlveolusController instance)
        {
            if (PartialPressure == null)
                return;
            
            partialPressureScript?.Unsubscribe(PartialPressuresUpdate);
            partialPressureScript = PartialPressure;
            partialPressureScript.Subscribe(PartialPressuresUpdate);

            if (Mathf.Approximately(maxValue, minValue))
            {
                maxValue += 1.0f;
            }

            UpdateSectionColors();
        }

        protected override void OnDestroy()
        {
            partialPressureScript?.Unsubscribe(PartialPressuresUpdate);
            base.OnDestroy();
        }

        // Update is called once per frame
        void PartialPressuresUpdate()
        {
            UpdateSectionColors();
        }

        /// <summary>
        /// This method retrieves the current array of capillary partial pressures and determines the new color for
        /// a section.
        /// </summary>
        private void UpdateSectionColors()
        {
            sectionColorUniformNames = SectionColorUniformNames(partialPressureScript.numberSections);
            float[] pressureValues = partialPressureScript.partialPressures;
            if (shaderSupportsArray)
            {
                Color[] colors = new Color[pressureValues.Length];
                for (int i = 0; i < pressureValues.Length; i++)
                {
                    float value = Mathf.Clamp(pressureValues[i], minValue, maxValue);
                    float maxColorFraction = (value - minValue) / (maxValue - minValue);
                    Color color = Color.Lerp(minValueColor, maxValueColor, maxColorFraction);
                    colors[i] = color;
                }

                m_capillaryMaterial.SetInt("_Segments", colors.Length);
                m_capillaryMaterial.SetColorArray("_ColorArray", colors);

                m_inflowRenderer.material.color = colors[0];
                m_outflowRenderer.material.color = colors[colors.Length -1];
            }
            else
            {
                for (int i = 0; i < pressureValues.Length; i++)
                {
                    float value = Mathf.Clamp(pressureValues[i], minValue, maxValue);
                    float maxColorFraction = (value - minValue) / (maxValue - minValue);
                    Color color = Color.Lerp(minValueColor, maxValueColor, maxColorFraction);
                    m_capillaryMaterial.SetColor(sectionColorUniformNames[i], color);

                    if (i == 0)
                    {
                        m_inflowRenderer.material.color = color;
                    }
                    if (i == pressureValues.Length -1)
                    {
                        m_outflowRenderer.material.color = color;
                    }
                }
            }
        }
    }
}