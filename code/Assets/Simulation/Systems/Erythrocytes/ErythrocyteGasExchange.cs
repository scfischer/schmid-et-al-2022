using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Simulation.Systems.Erythrocytes
{
    /// <summary>
    /// This class handles oxygen uptake and oxygen-saturation-related tinting of erythrocytes
    /// as they pass through the capillary.
    /// </summary>
    /// <remarks>
    /// Erythrocytes are the main agents for transporting oxygen in the blood stream. Along their passage through the
    /// alveolar capillaries - represented in this model by their their traversal of a single, representative capillary -
    /// oxygen diffuses from the blood plasma into the erythrocytes where it binds to haemoglobin.
    /// 
    /// The changing oxygen saturation of haemoglobin is visually represented by change of tint of the erythrocyte.
    /// The exact tint results from an interpolation between the oxygenated and deoxygenated tint colours, blended with the model material's
    /// initial tint colour.
    /// </remarks>
    public class ErythrocyteGasExchange : AlveolusComponentBehavior
    {
        /// <summary> The colour to tint an erythrocyte with maximum oxygen saturation. </summary>
        [SerializeField]
        private Color oxygenatedTintColour = Color.red;
        /// <summary> The colour to tint a deoxygenated erythrocyte. </summary>
        [SerializeField]
        private Color deoxygenatedTintColour = Color.blue;

        /// <summary> The oxygenation threshold at which the oxygenation tint should start to be applied.</summary>
        /// <remarks>
        /// Below this value, the <c>deoxygenatedTintColor</c> is used. This is because physiologically, such low values
        /// are largely not possible in an alive individual. So by narrowing the range at which the tint is applied,
        /// the changes become more visible in the physiologically relevant range.  
        /// </remarks>
        [SerializeField]
        private float oxygenationTintThreshold = 0.5f;

        /// <summary> This variable always stores the current saturation value. </summary>
        private float o2Saturation = 0.0f;

        /// <summary> This variable stores the saturation of this erythrocyte at the start of the capillary - before gas exchange takes place. </summary>
        private float o2SaturationStart;

        private MeshRenderer m_renderer;
        private MaterialPropertyBlock m_materialPropertyBlock;

        private int nearestNodeIndex;

        private float[] hbO2Saturations;

        private ErythrocyteMovement eryMovement;
        private int section;

        // Start is called before the first frame update
        private new void Awake()
        {
            enabled = false;
            eryMovement = GetComponent<ErythrocyteMovement>();
            m_materialPropertyBlock = new MaterialPropertyBlock();
            m_renderer = GetComponent<MeshRenderer>();
            base.Awake();
        }

        protected override void HandleReset(AlveolusController instance)
        {
            StopAllCoroutines();
            StartCoroutine(DelayedInit());
        }

        IEnumerator DelayedInit()
        {
            while (HbSaturation == null)
                yield return null;

            Init();
            InitStartingValues();
            enabled = true;
        }

        private void Init()
        {
            HbSaturation.Subscribe(UpdateSaturationArray);
            UpdateSaturationArray();
        }

        void Update()
        {
            if (section != eryMovement.section)
            {
                UpdateSaturation();
            }
        }

        /// <summary>
        /// Fetch the current oxygen saturation data and update this erythrocyte accordingly.
        /// </summary>
        void UpdateSaturationArray()
        {
            hbO2Saturations = HbSaturation.hbO2Saturation;
            UpdateSaturation();
        }

        protected override void OnDestroy()
        {
            HbSaturation?.Unsubscribe(UpdateSaturationArray);
            base.OnDestroy();
        }
        
        /// <summary>
        /// Set values and tint as the erythrocyte spawns at the entrance to the capillary (incoming blood).
        /// </summary>
        private void InitStartingValues()
        {
            hbO2Saturations = HbSaturation.hbO2Saturation;
            o2SaturationStart = hbO2Saturations[0];
            o2Saturation = o2SaturationStart;

            UpdateTint();
        }

        /// <summary>
        /// Determines the O2 saturation the erythrocyte should currently have, based on the capillary section it is in.
        /// </summary>
        private void UpdateSaturation()
        {
            section = eryMovement.section;
            o2Saturation = hbO2Saturations[section];
            UpdateTint();
        }

        /// <summary>
        /// Fetches the tint for the erythrocyte's current oxygenation and applies it to the material.
        /// (Blended with the material's initial colour.)
        /// </summary>
        private void UpdateTint()
        {
            Color tint = CalculateTint();
            m_materialPropertyBlock.SetColor("_Color", tint);
            m_renderer.SetPropertyBlock(m_materialPropertyBlock);
        }

        /// <summary>
        /// Determines the tint for the erythrocyte's current oxygenation level via linear interpolation between
        /// <c>deoxygenatedTintColor</c> and <c>oxygenatedTintColor</c>.
        /// </summary>
        /// <returns>The tint colour for the erythrocytes current oxygenation.</returns>
        private Color CalculateTint()
        {
            Color tint;

            if (o2Saturation < oxygenationTintThreshold)
            {
                tint = deoxygenatedTintColour;
            }
            else
            {
                // (x - threshold) / threshold = x/threshold - threshold/threshold = x/threshold - 1
                float oxygenationTintFactor = (o2Saturation / oxygenationTintThreshold) - 1;
                //tint = oxygenatedTintColour * oxygenationTintFactor + deoxygenatedTintColour * (1 - oxygenationTintFactor);
                tint = Color.Lerp(deoxygenatedTintColour, oxygenatedTintColour, oxygenationTintFactor);
            }

            return tint;
        }

        /// <summary> Takes stock of changes in oxygen saturation across the main capillary where gas exchange takes place. </summary>
        public void DeliverSaturationDifference()
        {
            float satDifference = o2Saturation - o2SaturationStart;
            if (satDifference < 0)
            {
                satDifference = 0;
            }
            BloodOutflow.o2SatDifference = satDifference;
        }
    }
}