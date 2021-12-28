using System;
using System.Collections.Generic;
using Simulation.Systems;
using UnityEngine;

namespace UserInterface.Highlighting
{
    public class InstanceHighlightTargets : AlveolusComponentBehavior
    {
        /// <summary>
        /// Holds pairs of <see cref="HighlightTrigger"/> scripts and the target they are intended to enable highlighting on.
        /// </summary>
        /// <remarks>
        /// The target <c>GameObject</c> needs to have a component which implements <see cref="IHighlightable"/>.
        /// </remarks>
        [Header("Associations")]
        [SerializeField]
        private HighlightingAssociation[] highlightAssociations = new HighlightingAssociation[0];

        private Dictionary<string, List<IHighlightable>> highlightDictionary =
            new Dictionary<string, List<IHighlightable>>();
        private List<IHighlightable> currentlyHighlighted = new List<IHighlightable>(2);


        /// <summary>
        /// Holds a <see cref="HighlightTrigger"/> and the target this trigger should toggle highlights on. The <c>target</c>
        /// needs to contain a component that implements <see cref="IHighlightable"/>.
        ///
        /// This struct is used to expose these two associated items as elements of an array in the inspector.  
        /// </summary>
#pragma warning disable 0649
        [Serializable]
        private struct HighlightingAssociation
        {
            /// <summary>
            /// A <see cref="HighlightTrigger"/> that toggles highlighting for the associated <see cref="target"/>.
            /// </summary>
            public string triggerElementName;
            /// <summary>
            /// A game object with a component that implements <see cref="IHighlightable"/>, which should be toggled by
            /// the <see cref="trigger"/>.
            /// </summary>
            public GameObject target;
        }
#pragma warning restore 0649

        // Start is called before the first frame update
        private new void Start()
        {
            base.Start();
            PopulateDictionary();
            Alveolus.onDefocus.AddListener(HandleDeFocus);
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
        public bool ActivateHighlight(string caller, string parameters = "")
        {
            List<IHighlightable> targets;
            string callerName = caller;
            if (!highlightDictionary.TryGetValue(callerName, out targets))
            {
                Debug.LogWarning("No highlightable target for " + caller + " found.");
                return false;
            }

            if (currentlyHighlighted.Count > 0)
            {
                if (currentlyHighlighted.Count == targets.Count)
                {
                    bool nothingChanged = true;
                    foreach (var target in targets)
                    {
                        if (!currentlyHighlighted.Contains(target))
                        {
                            nothingChanged = false;
                            break;
                        }
                    }

                    if (nothingChanged)
                    {
                        return false;
                    }
                }

                ClearCurrentHighlights();
            }

            foreach (var target in targets)
            {
                target.EnableHighlights(parameters);
                currentlyHighlighted.Add(target);
            }

            return true;
        }

        /// <summary>
        /// Called by a <see cref="HighlightTrigger"/> to disable the highlighting in the script associated with this trigger
        /// in <see cref="highlightAssociations"/>. If the trigger has a string with additional parameters assigned, it is
        /// passed to the associated target.
        /// </summary>
        /// <param name="caller">The caller which invokes this method.</param>
        /// <param name="parameters"></param>
        /// <returns>
        /// True if <see cref="IHighlightable.DisableHighlights"/> was invoked on the associated target, false if no target
        /// was found for the provided <see cref="caller"/>.
        /// </returns>
        public bool DeactivateHighlight(string caller, string parameters = "")
        {
            if ((caller.ToLower() == "all") && (currentlyHighlighted.Count > 0))
            {
                ClearCurrentHighlights();
                return true;
            }

            List<IHighlightable> targets;
            if (!highlightDictionary.TryGetValue(caller, out targets))
            {
                //Would it be better to have the message here, too, or does it suffice that there's on for activation?
                //Debug.LogError("No highlightable target for " + caller.name + " found.");
                return false;
            }

            foreach (var target in targets)
            {
                target.DisableHighlights(parameters);
                currentlyHighlighted.Remove(target);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool AnyHighlights()
        {
            return (currentlyHighlighted.Count != 0);
        }

        /// <summary>
        /// Takes the inspector-assigned values from <see cref="highlightAssociations"/> and puts them into a dictionary.
        /// </summary>
        /// <remarks>
        /// The method extracts that component from the <c>GameObject</c> target which implements <see cref="IHighlightable"/>
        /// if available, checks if all associations are valid (i.e. have both a trigger and a valid target assigned) and
        /// adds only valid entries into a <c>Dictionary<string, IHighlightable></c>, while logging a warning
        /// for invalid assignments. 
        /// </remarks>
        private void PopulateDictionary()
        {
            foreach (var entry in highlightAssociations)
            {
                if ((entry.triggerElementName == "") || (entry.target == null))
                {
                    //warn if there are incomplete entries, rather than merely empty 
                    if (entry.triggerElementName != "")
                    {
                        Debug.LogWarning("Highlight associations trigger entry found for "
                                         + entry.triggerElementName + ", but has no target associated!");
                    }
                    else if (entry.target != null)
                    {
                        Debug.LogWarning("Highlight associations target entry found for " + entry.target.name
                            + ", but has no trigger associated!");
                    }

                    //otherwise ignore empty or incomplete entries
                    continue;
                }

                //next, check for and if available retrieve the component that implements IHighlightable

                IHighlightable highlightableScript = entry.target.GetComponent<IHighlightable>();
                if (highlightableScript == null)
                {
                    Debug.LogWarning("Highlight associations target " + entry.target.name + " is not highlightable!");
                }
                else
                {
                    if (highlightDictionary.TryGetValue(entry.triggerElementName, out var values))
                    {
                        values.Add(highlightableScript);
                    }
                    else
                    {
                        var list = new List<IHighlightable> {highlightableScript};
                        highlightDictionary.Add(entry.triggerElementName, list);
                    }
                }
            }
        }

        private void ClearCurrentHighlights()
        {
            foreach (var stillHighlighted in currentlyHighlighted)
            {
                stillHighlighted.DisableHighlights();
            }

            currentlyHighlighted.Clear();
        }

        private void HandleDeFocus(AlveolusController alveolusController)
        {
            ClearCurrentHighlights();
        }
    }
}