using UnityEngine;
using UserInterface.Highlighting;

namespace UserInterface.Highlighting
{
    /// <summary>
    /// Makes GameObjects eligible highlighting targets and puts them on the respective layer
    /// when they should be highlighted.
    /// </summary>
    /// <remarks>
    /// Part of the highlighting system, this script goes into game objects which should be subject to being highlighted.
    /// It creates a valid target for associations assigned to the instance's <see cref="InstanceHighlightTargets"/>.
    /// When highlighting is triggered and associated with a GameObject with this script, the GameObject is placed on
    /// the Highlight layer and rendered on a different camera, which allows exemption from post processing effects
    /// (e.g. fading out everything else to leave this object more visible).  
    /// the <see cref="affectChildren"/> setting can be used to toggle between including all child GameObjects in the
    /// highlighting, or only this one.
    /// </remarks>
    public class Highlightable : MonoBehaviour, IHighlightable
    {
        private int m_defaultLayer;
        private int m_highlightLayer;

        [Tooltip("Whether the children in the hierarchy are affected as well.")]
        public bool affectChildren = true;

        private void Start()
        {
            m_defaultLayer = gameObject.layer;
            m_highlightLayer = LayerMask.NameToLayer("Highlight");
        }

        public void EnableHighlights(string parameters = "")
        {
            ChangeLayer(gameObject, m_highlightLayer);
        }

        public void DisableHighlights(string parameters = "")
        {
            ChangeLayer(gameObject, m_defaultLayer);
        }

        private void ChangeLayer(GameObject go, int layer)
        {
            go.layer = layer;
            if (!affectChildren) return;
            foreach (Transform child in go.transform)
            {
                ChangeLayer(child.gameObject, layer);
            }
        }
    }
}