using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface.ModalDialog
{
    public class ModalDialogController : MonoBehaviour
    {
        public static ModalDialogController Instance =>
            GameObject.FindWithTag("ModalDialog").GetComponent<ModalDialogController>();

        public enum DialogOption
        {
            Confirm,
            Cancel
        }

        public delegate void DialogCallback(DialogOption option);

        public delegate void DialogCallback<in TReturnValue>(DialogOption option, TReturnValue value);

        private Image m_backgroundOverlay;
        private GameObject m_dialogWindow;

        private void Start()
        {
            m_backgroundOverlay = GetComponent<Image>();
            if (m_backgroundOverlay.IsActive())
            {
                m_backgroundOverlay.enabled = false;
            }
        }

        private bool SetupDialogInstance(GameObject prefab)
        {
            if (m_dialogWindow)
            {
                Debug.LogError("ModalDialog was requested to open, but is already active!");
                return false;
            }

            m_backgroundOverlay.enabled = true;
            m_dialogWindow = Instantiate(prefab, transform);
            return true;
        }

        public void ShowModalDialog<TReturnValue>(GameObject prefab,
            DialogCallback onConfirmed = null,
            DialogCallback onCanceled = null,
            DialogCallback<TReturnValue> genericCallback = null)
        {
            if (!SetupDialogInstance(prefab))
                return;

            var dialogBox = m_dialogWindow.GetComponentInChildren<IModalDialog<TReturnValue>>();
            dialogBox.Callback = WrapCallback(onConfirmed, onCanceled, genericCallback);
        }

        public void ShowModalDialog(GameObject prefab)
        {
            ShowModalDialog(prefab,null,null,null);
            
        }

        public void ShowModalDialog(GameObject prefab,
            DialogCallback onConfirmed = null,
            DialogCallback onCanceled = null,
            DialogCallback genericCallback = null)
        {
            if (!SetupDialogInstance(prefab))
                return;

            var dialogBox = m_dialogWindow.GetComponentInChildren<IModalDialog>();
            dialogBox.Callback = WrapCallback(onConfirmed, onCanceled, genericCallback);
        }

        private DialogCallback<TResult> WrapCallback<TResult>(DialogCallback onConfirmed, DialogCallback onCanceled,
            DialogCallback<TResult> genericCallback)
        {
            void WrappedCallback(DialogOption option, TResult value)
            {
                ClearModalDialog();
                switch (option)
                {
                    case DialogOption.Confirm:
                        onConfirmed?.Invoke(option);
                        break;
                    case DialogOption.Cancel:
                        onCanceled?.Invoke(option);
                        break;
                }

                genericCallback?.Invoke(option, value);
            }

            return WrappedCallback;
        }

        private DialogCallback WrapCallback(DialogCallback onConfirmed, DialogCallback onCanceled,
            DialogCallback genericCallback)
        {
            void WrappedCallback(DialogOption option)
            {
                ClearModalDialog();
                switch (option)
                {
                    case DialogOption.Confirm:
                        onConfirmed?.Invoke(option);
                        break;
                    case DialogOption.Cancel:
                        onCanceled?.Invoke(option);
                        break;
                }

                genericCallback?.Invoke(option);
            }

            return WrappedCallback;
        }

        private void ClearModalDialog()
        {
            m_backgroundOverlay.enabled = false;
            m_dialogWindow = null;
            DestroyChildren();
        }

        private void DestroyChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}