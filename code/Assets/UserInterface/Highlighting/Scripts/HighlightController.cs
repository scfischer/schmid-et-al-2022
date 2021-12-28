using System.Collections;
using Simulation.Systems;
using Support;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UserInterface.MultiInstance;

namespace UserInterface.Highlighting
{
    /// <summary>
    /// The class which controls highlighting of objects based on GUI interactions.
    ///
    /// It allows assigning pairs of <see cref="HighlightTrigger"/>s and targets that should have highlighting toggles,
    /// thus allowing to re-use scripts for triggers without having to modify the code. Instead, the trigger element is
    /// assigned to <see cref="HighlightingAssociation.trigger"/> of <see cref="highlightAssociations"/> together with its
    /// intended target. The <see cref="HighlightingAssociation.target"/> needs to implement <see cref="IHighlightable"/>,
    /// the methods of which are invoked by this class to turn highlighting on or off.
    /// </summary>
    public class HighlightController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private AppSettings appSettings;

        [Header("Effects")]
        [SerializeField]
        private float fadedSaturation = -100;
        [SerializeField]
        private float fadedBrightness = 0;
        [SerializeField]
        private float fadedContrast = -80;

        [SerializeField]
        private PostProcessVolume postProcessVolume;

        private ColorGrading colorSettings;

        private InstanceProvider instanceProvider;
        private InstanceHighlightTargets alveolusInstanceTargets;


        //[Header("Transition")]
        private float fadeOutDuration = 1.0f;
        private float fadeInDuration = 1.0f;

        private float fadeOutSpeed;
        private float fadeInSpeed;

        private float fadeRatio;

        // Start is called before the first frame update
        void Start()
        {
            instanceProvider = transform.root.GetComponentInChildren<InstanceProvider>();
            instanceProvider.onSimulationInstanceChanged.AddListener(HandleSimulationInstanceChange);

            alveolusInstanceTargets = instanceProvider.SimulationAlveolus.highlightTargets;

            colorSettings = postProcessVolume.profile.GetSetting<ColorGrading>();

            if (appSettings != null)
            {
                fadeOutDuration = appSettings.FadeOutDuration;
                fadeInDuration = appSettings.FadeInDuration;
            }

            fadeOutSpeed = 1.0f / fadeOutDuration;
            fadeInSpeed = 1.0f / fadeInDuration;
        }


        /// <summary>
        /// Called by a <see cref="HighlightTrigger"/> to enable the highlighting in the script associated with this trigger
        /// in <see cref="highlightAssociations"/>. If the trigger has a string with additional parameters assigned, it is
        /// passed to the associated target. Disables other currently active highlights.
        /// </summary>
        /// <param name="caller"> The caller which invokes this method. </param>
        /// <returns>
        /// True if  <see cref="IHighlightable.EnableHighlights"/>  was invoked on the associated target, false if no target
        /// was found for the provided <see cref="caller"/>.
        /// </returns>
        public void ActivateHighlight(HighlightTrigger caller)
        {
            if (!appSettings.HighlightsEnabled)
                return;

            if (alveolusInstanceTargets == null)
            {
                return;
            }

            alveolusInstanceTargets.ActivateHighlight(caller.gameObject.name, caller.parameters);

            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            postProcessVolume.enabled = true;
            
            while (fadeRatio < 1)
            {
                colorSettings.saturation.value = Mathf.Lerp(0, fadedSaturation, fadeRatio);
                colorSettings.brightness.value =  Mathf.Lerp(0, fadedBrightness, fadeRatio);
                colorSettings.contrast.value = Mathf.Lerp(0, fadedContrast, fadeRatio);

                fadeRatio += Time.unscaledDeltaTime * fadeOutSpeed;
                
                yield return null;
            }
            
            //setting the fade ratio as an inelegant hack for if unity doesn't "run in background"
            fadeRatio = 1;
            colorSettings.saturation.value = fadedSaturation;
            colorSettings.brightness.value = fadedBrightness;
            colorSettings.contrast.value = fadedContrast;
        }
    
        
        /// <summary>
        /// Called by a <see cref="HighlightTrigger"/> to disable the highlighting in the script associated with this trigger
        /// in <see cref="highlightAssociations"/>. If the trigger has a string with additional parameters assigned, it is
        /// passed to the associated target.
        /// </summary>
        /// <param name="caller">The caller which invokes this method.</param>
        /// <returns>
        /// True if <see cref="IHighlightable.DisableHighlights"/> was invoked on the associated target, false if no target
        /// was found for the provided <see cref="caller"/>.
        /// </returns>
        public void DeactivateHighlight(HighlightTrigger caller)
        {
            if (alveolusInstanceTargets == null)
            {
                return;
            }

            //alveolusInstanceTargets.DeactivateHighlight(caller.gameObject.name, caller.parameters);

            StopAllCoroutines();
            StartCoroutine(FadeIn(caller));
        }

        IEnumerator FadeIn(HighlightTrigger caller)
        {
            while (fadeRatio > 0)
            {
                colorSettings.saturation.value = Mathf.Lerp(0, fadedSaturation, fadeRatio);
                colorSettings.brightness.value = Mathf.Lerp(0, fadedBrightness, fadeRatio);
                colorSettings.contrast.value = Mathf.Lerp(0, fadedContrast, fadeRatio);

                fadeRatio -= Time.unscaledDeltaTime * fadeInSpeed;

                yield return null;
            }

            //setting the fade ratio as an inelegant hack for if unity doesn't "run in background"   
            fadeRatio = 0;
            colorSettings.saturation.value = 0;
            colorSettings.brightness.value = 0;
            colorSettings.contrast.value = 0;

            postProcessVolume.enabled = false;
            alveolusInstanceTargets.DeactivateHighlight(caller.gameObject.name, caller.parameters);
        }

        private void HandleSimulationInstanceChange(AlveolusController alveolus)
        {
            alveolusInstanceTargets = alveolus.highlightTargets;
        }
    }
}