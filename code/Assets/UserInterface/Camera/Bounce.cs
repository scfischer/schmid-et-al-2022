using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface.Camera
{
    public class Bounce : MonoBehaviour
    {
        public float frequency = 0.1f;
        public float amplitude = 0.05f;

        private Vector3 initialPosition;
    
        void Start()
        {
            initialPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            var offset = Vector3.up * (Mathf.Sin(frequency * Time.timeSinceLevelLoad) * amplitude);
            transform.position = initialPosition + offset;
        }
    }
}

