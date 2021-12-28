using Simulation.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.ModalDialog;

namespace UserInterface.MultiInstance
{
    public class InstanceMenuEntry : MonoBehaviour
    {
        private Color m_focusedColor = Color.white;
        private Color m_defocusedColor = new Color(1, 1, 1, 0.5f);

        [SerializeField]
        private Image colorDot;

        [SerializeField]
        private Button deleteButton;

        [SerializeField]
        private Button duplicateButton;

        [SerializeField]
        private Button resetButton;

        [SerializeField]
        private TMP_InputField labelInput;

        [SerializeField]
        private Button selectButton;

        [SerializeField]
        private GameObject deleteConfirmDialog;

        private AlveolusController m_alveolusController;
        private InstanceColorMap m_instanceColorMap;

        private InstanceMenu m_instanceMenu;

        private void Awake()
        {
            m_instanceColorMap = GetComponentInParent<InstanceColorMap>();
        }

        private void Start()
        {
            deleteButton.onClick.AddListener(HandleOnDeleteButtonClick);
            labelInput.onEndEdit.AddListener(HandleNameInputChanged);
            selectButton.onClick.AddListener(HandleSelectClick);
            resetButton.onClick.AddListener(HandleResetClick);
            duplicateButton.onClick.AddListener(HandleDuplicateClick);

            m_instanceMenu.InstanceCreationAllowedChanged += InstanceMenuOnInstanceCreationAllowedChanged;
            InstanceMenuOnInstanceCreationAllowedChanged(m_instanceMenu.InstanceCreationAllowed);
        }

        private void HandleResetClick()
        {
            m_alveolusController.instanceParameters.ResetToDefaults();
        }

        private void OnDisable()
        {
            m_instanceMenu.InstanceCreationAllowedChanged -= InstanceMenuOnInstanceCreationAllowedChanged;
        }

        private void InstanceMenuOnInstanceCreationAllowedChanged(bool isAllowed)
        {
            duplicateButton.enabled = isAllowed;
        }

        private void HandleSelectClick()
        {
            m_instanceMenu.SelectInstance(m_alveolusController);
        }

        public void SetInstanceMenu(InstanceMenu menu)
        {
            m_instanceMenu = menu;
        }

        public void SetInstance(AlveolusController alveolusController)
        {
            m_alveolusController = alveolusController;
            SetLabels(m_alveolusController.name);
            alveolusController.onFocus.AddListener(HandleInstanceOnFocus);
            alveolusController.onDefocus.AddListener(HandleInstanceOnDefocus);
            m_focusedColor = m_instanceColorMap.GetColor(m_alveolusController);
            m_defocusedColor = m_focusedColor;
        }

        private void SetLabels(string value)
        {
            labelInput.text = value;
            selectButton.GetComponentInChildren<TMP_Text>().text = value;
        }

        private void HandleOnDeleteButtonClick()
        {
            ModalDialogController.Instance.ShowModalDialog(deleteConfirmDialog, HandleOnDeleteConfirmed);
        }

        private void HandleOnDeleteConfirmed(ModalDialogController.DialogOption option)
        {
            if (option.Equals(ModalDialogController.DialogOption.Cancel))
                return;
            m_instanceMenu.DeleteInstance(m_alveolusController);
            Destroy(gameObject);
        }

        private void HandleInstanceOnDefocus(AlveolusController arg0)
        {
            colorDot.color = m_defocusedColor;
            labelInput.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            duplicateButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        }

        private void HandleInstanceOnFocus(AlveolusController arg0)
        {
            colorDot.color = m_focusedColor;
            labelInput.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
            resetButton.gameObject.SetActive(true);
            duplicateButton.gameObject.SetActive(m_instanceMenu.InstanceCreationAllowed);
            selectButton.gameObject.SetActive(false);
        }

        private void HandleNameInputChanged(string value)
        {
            m_alveolusController.gameObject.name = value;
            SetLabels(m_alveolusController.name);
        }

        private void HandleDuplicateClick()
        {
            m_instanceMenu.DuplicateInstance(m_alveolusController);
        }
    }
}