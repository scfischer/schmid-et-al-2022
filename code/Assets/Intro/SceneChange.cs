using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Intro
{
    /// <summary>
    /// Used to load the main Simulation scene after the video in the current scene finishes playing.
    /// Primarily for the intro, it can also be used for any other instruction or tutorial scenes that are meant
    /// to return to the Simulation after.
    /// </summary>
    public class SceneChange : MonoBehaviour
    {
        /// <summary>
        /// Path to the folder containing Scenes. (unused) 
        /// </summary>
        public const string scenePath = "Assets/Scenes/";
    
        // Start is called before the first frame update
        void Start()
        {
            var video = transform.GetComponent<VideoPlayer>();
            video.loopPointReached += EndReached;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Loads the main Simulation scene. To be invoked when the video ends.
        /// </summary>
        /// <param name="player">The VideoPlayer invoking this Method (unused)</param>
        private void EndReached(UnityEngine.Video.VideoPlayer player)
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Method that skips the intro video and loads the main Simulation scene right away.
        /// </summary>
        public void Skip()
        {
            SceneManager.LoadScene(1);
        }
    
        /// <summary>
        /// OnGUI event looks for presses of the Esc key, which can be used to skip the Intro scene.
        /// </summary>
        private void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent("escape")))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
