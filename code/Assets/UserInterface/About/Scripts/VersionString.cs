using TMPro;
using UnityEngine;

namespace UserInterface.About
{
    [ExecuteInEditMode]
    public class VersionString : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text label;

        [SerializeField]
        private string formatString = "Alveolus {0}";

        private void Start()
        {
            if (label)
                label.text = string.Format(formatString, Application.version);
        }
    }
}
