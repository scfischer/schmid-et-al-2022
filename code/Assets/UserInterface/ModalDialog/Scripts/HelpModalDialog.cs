using UnityEngine;

namespace UserInterface.ModalDialog
{
    public class HelpModalDialog : MonoBehaviour, IModalDialog
    {
        public ModalDialogController.DialogCallback Callback { get; set; }

        public void CloseClicked()
        {
            Callback?.Invoke(ModalDialogController.DialogOption.Confirm);
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(0))
            {
                CloseClicked();
            }
        }
    }
}