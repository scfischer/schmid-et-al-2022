using UnityEngine;

namespace UserInterface.MainMenu
{
    public class QuitButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject confirmationDialog; 
            
        private void Start()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                gameObject.SetActive(false);
        }

        public void Quit()
        {
            ModalDialog.ModalDialogController.Instance.
                ShowModalDialog(confirmationDialog, option => Application.Quit());
            
        }
    }
}