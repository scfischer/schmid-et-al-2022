using System;
using Support;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UserInterface.ModalDialog;

namespace UserInterface.MainMenu
{
    /// <summary> This class controls the unlocking of multiple simulation instances in a previously restricted version of the app. </summary>
    public class UnlockFeature : MonoBehaviour
    {
        [SerializeField]
        private int instanceCountWhenUnlocked = 6;
        
        [SerializeField]
        private AppSettings appSettings;

        [SerializeField]
        private UnlockModalDialog dialogWindow;
        
        [SerializeField]
        private GameObject notificationWindow;

        public const string accessCode = "1512";
        
        private string feedback;


        private void Start()
        {
            if (appSettings.MaxNumInstances >= instanceCountWhenUnlocked)
            {
                SetButtonToUnlocked();
            }
        }

        public void Unlock(ModalDialogController.DialogOption option, string accessCodeInput)
        {
            if (option != ModalDialogController.DialogOption.Confirm)
            {
                return;
            }
            
            if (accessCodeInput == accessCode)
            {
                appSettings.MaxNumInstances = instanceCountWhenUnlocked;
                SetButtonToUnlocked();
                feedback = "You have unlocked the use of multiple alveolus instances!";
            }
            else
            {
                feedback = "The feature code you entered is unknown.";
            }

            if (notificationWindow != null)
            {
                TMP_Text textComponent = notificationWindow.GetComponentInChildren<TMP_Text>();
                if ( textComponent != null ) {
                    textComponent.text = feedback;
                    ModalDialogController.Instance.ShowModalDialog(notificationWindow);
                }
                else
                {
                    Debug.LogError("Notification window prefab for feature unlock has no TMP_Text component.");
                }
            }
            else
            {
                Debug.LogError("No notification window prefab for feature unlock success/failure assigned.");
            }
        }

        public void UnlockButtonClicked()
        {
            ModalDialogController.Instance.ShowModalDialog<string>(dialogWindow.gameObject, null, null, Unlock);
        }

        private void SetButtonToUnlocked()
        {
            GetComponent<IconToggle>().SetIcon(false);
            GetComponent<Button>().enabled = false;
        }
    }
}