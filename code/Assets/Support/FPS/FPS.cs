using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Support.FPS
{
    public class FPS : MonoBehaviour
    {
        public float FrameTimeMs => Time.smoothDeltaTime * 1000;
        public float FramesPerSecond => 1.0f/Time.smoothDeltaTime;
    
        void OnGUI()
        {
            GUILayout.TextArea($"{FrameTimeMs:F2} ms / {FramesPerSecond:F2} FPS");
        }
    }
}
