using System.Collections.Generic;
using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.MultiInstance
{
    public class InstanceTemplateElement : MonoBehaviour
    {
        [SerializeField]
        private InstanceMenu menu;

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private Button createButton;

        [SerializeField]
        private List<ParametersData> templates;

        private void RebuildDropdown()
        {
            dropdown.ClearOptions();
            var options = new List<string>();
            foreach (ParametersData template in templates)
            {
                options.Add(template.name);
            }

            dropdown.AddOptions(options);

            menu.InstanceCreationAllowedChanged += MenuOnInstanceCreationAllowedChanged;
            MenuOnInstanceCreationAllowedChanged(menu.InstanceCreationAllowed);
        }

        public string GetElementTooltip(int index)
        {
            return templates[index].Description;
        }

        private void MenuOnInstanceCreationAllowedChanged(bool isAllowed)
        {
            createButton.gameObject.SetActive(isAllowed);
        }

        private void Awake()
        {
            RebuildDropdown();

            dropdown.onValueChanged.AddListener(HandleValueChanged);

            menu.parametersTemplate = templates[dropdown.value];
        }

        private void HandleValueChanged(int arg0)
        {
            menu.parametersTemplate = templates[arg0];
        }
    }
}