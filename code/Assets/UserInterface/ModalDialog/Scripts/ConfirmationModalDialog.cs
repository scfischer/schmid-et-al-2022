using UnityEngine;

namespace UserInterface.ModalDialog
{
    public class ConfirmationModalDialog : MonoBehaviour, IModalDialog
    {
        public ModalDialogController.DialogCallback Callback { get; set; }

        public void CancelClicked()
        {
            Callback?.Invoke(ModalDialogController.DialogOption.Cancel);
        }

        public void OkClicked()
        {
            Callback?.Invoke(ModalDialogController.DialogOption.Confirm);
        }
        
        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
               CancelClicked(); 
            }
        }
    }
}