using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Screen;

namespace UserInterface.Camera
{
    /// <summary>
    /// A simple script to control camera movement, primarily using the mouse, to allow moving and rotating
    /// around the alveolus, and to zoom in or out.
    /// </summary>
    public class CameraControls : MonoBehaviour
    {
        //the following are public to allow changing from a hypothetical "settings" menu
        /// <summary> Panning speed multiplier. </summary>
        public float mousePanSensitivity = 1.0f;
        /// <summary> Camera rotation speed multiplier. </summary>
        public float mouseRotationSpeed = 1.0f;
        /// <summary> Zoom speed multiplier. </summary>
        public float zoomSpeed = 10.0f;

        private EventSystem eventSystem;

        private bool ignoreClick = false;
        private Vector3 recordedMousePosition;

        /// <summary> Ratio of mouse movement in pixels across the screen to camera translation. </summary>
        [SerializeField]
        private float pixelToLengthRatio = 0.1f; //1 pixel mouse movement to 0.1 unity units camera movement
        /// <summary> Ratio of rotation in degrees to mouse movement in pixels across the screen. </summary>
        [SerializeField]
        private float pixelToRotRatio = 360f / 1000f; //full rotation for 1000 pixel mouse move

        private float panningFactor;
        private float rotationFactor;

        public Transform pivotObject;

        private Vector3 pivot => pivotObject.position;

        private const int Lmb = 0;
        private const int Rmb = 1;

        private Quaternion initialRotation;
        private Vector3 initialPosition;

        void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }

        // Start is called before the first frame update
        void Start()
        {
            panningFactor = mousePanSensitivity * pixelToLengthRatio;
            rotationFactor = mouseRotationSpeed * pixelToRotRatio;
            initialRotation = transform.rotation;
            initialPosition = transform.position;

            ResetCamera();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetCamera();
            }

            if (ignoreClick)
            {
                if (Input.GetMouseButtonUp(Lmb) || Input.GetMouseButtonUp(Rmb))
                {
                    if (Input.GetMouseButton(Lmb) || Input.GetMouseButton(Rmb))
                    {
                        //a button might be clicked or let go while still dragging something on the GUI
                        //thus, we keep ignoring, also making the rest of anything in this script irrelevant
                        return;
                    }
                    else
                    {
                        //but if we let go and stop manipulating the GUI, we don't want to ignore future clicks anymore
                        ignoreClick = false;
                    }
                }
                else
                {
                    //and if we just want to ignore clicks (because they're on the GUI), we disregard the rest of the script
                    return;
                }
            }

            if (Input.GetMouseButtonDown(Lmb) || Input.GetMouseButtonDown(Rmb))
            {
                if (IsMouseOverGui())
                {
                    ignoreClick = true;
                    return;
                }
                else
                {
                    RecordMousePosition();
                }
            }

            /* pan view */
            if (Input.GetMouseButton(Lmb))
            {
                Vector3 delta = Input.mousePosition - recordedMousePosition;
                if (delta.sqrMagnitude > 0)
                {
                    //delta = this.transform.rotation * delta; //erm, might not need that
                    this.transform.Translate(-delta * panningFactor, Space.Self);
                }

                RecordMousePosition();
            }
            /* rotate view around object center (pivot) */
            else if (Input.GetMouseButton(Rmb))
            {
                Vector3 delta = Input.mousePosition - recordedMousePosition;
                if (delta.sqrMagnitude > 0)
                {
                    delta *= rotationFactor;
                    transform.RotateAround(pivot, Vector3.up, delta.x); //left-right
                    transform.RotateAround(pivot, transform.right, -delta.y); //upward-downward
                }

                RecordMousePosition();
            }

            /* mousewheel zoom */
            float mouseWheelTurn = Input.mouseScrollDelta.y;
            if (mouseWheelTurn != 0)
            {
                float zoomDistance = mouseWheelTurn * zoomSpeed;
                Vector3 zoomTranslation = Vector3.forward * zoomDistance;
                transform.Translate(zoomTranslation, Space.Self);
            }
        }

        /// <summary>
        /// Resets the camera to the default position (with respect to currently opened GUI elements)
        /// </summary>
        private void ResetCamera()
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        /// <summary>
        /// Checks if the Mouse is currently over GUI elements. 
        /// </summary>
        /// <returns>true if mouse is considered to be over the GUI, false otherwise.</returns>
        private bool IsMouseOverGui()
        {
            return eventSystem.IsPointerOverGameObject();
        }

        /// <summary>
        /// Saves the current position of the mouse pointer.
        /// </summary>
        private void RecordMousePosition()
        {
            recordedMousePosition = Input.mousePosition;
        }
    }
}