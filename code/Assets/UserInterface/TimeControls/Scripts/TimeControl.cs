using System.Globalization;
using UnityEngine;
using TMPro;
using Simulation.Systems.Erythrocytes;

namespace UserInterface.TimeControls
{
    /// <summary> This class monitors simulation time and passes it on to a display in the UI. </summary>
    public class TimeControl : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timeDisplay;
        NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
    
        public float simulationStart { get; set; }
        public float simulationTime { get; private set; }

        void Start()
        {
            nfi.NumberDecimalSeparator = ".";
            simulationStart = Time.timeSinceLevelLoad;
        }
        void Update()
        {
            DisplayTime();
        }

        public void TogglePause()
        {
            if (Mathf.Approximately(Time.timeScale, 0))
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }

        private void DisplayTime()
        {
            simulationTime = (Time.timeSinceLevelLoad - simulationStart) / SpawnErythrocyte.sloMo;
            timeDisplay.text = simulationTime.ToString("00:00.00", nfi);
        }
    }
}


