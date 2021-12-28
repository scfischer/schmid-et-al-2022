using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserInterface.ModalDialog;

namespace UserInterface.HealthStatus
{
    public class OpenInfoBox : MonoBehaviour
    {
        [SerializeField]
        private GameObject infoBox;

        public void Open()
        {
            ModalDialogController.Instance.ShowModalDialog(infoBox, HandleExit);
        }

        private void HandleExit(ModalDialogController.DialogOption option)
        {
            if (option.Equals(ModalDialogController.DialogOption.Cancel))
                return;
        }
    }
}