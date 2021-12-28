using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UserInterface.ModalDialog
{
    public class UnlockModalDialog : MonoBehaviour, IModalDialog<string>
    {
        private TMP_InputField inputField;
    
        // Start is called before the first frame update
        void Start()
        {
            inputField = GetComponentInChildren<TMP_InputField>();
        }

        public ModalDialogController.DialogCallback<string> Callback { get; set; }

        public void ConfirmClicked()
        {
            Callback?.Invoke(ModalDialogController.DialogOption.Confirm, inputField.text);
        }

        public void CancelClicked()
        {
            Callback?.Invoke(ModalDialogController.DialogOption.Cancel, "");
        }
    }
}

