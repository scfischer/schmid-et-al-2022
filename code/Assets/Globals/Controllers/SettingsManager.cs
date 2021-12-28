using UnityEngine;

namespace Globals.Controllers
{
    /// <summary>
    /// A script to hold global values in a single location, as well as some derived values for convenience,
    /// accessible through <c>SettingsManager.Settings.value</c>.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        /// <summary>
        /// Property to access the SettingsManager without having to use <c>Find</c>, instead saving the reference singleton-style. 
        /// </summary>
        public static SettingsManager Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = FindObjectOfType<SettingsManager>();
                }

                return _settings;
            }
        }

        private static SettingsManager _settings;
        
        
        /// <summary>
        /// Global value for the base speed that gas particles in the simulation should use, for consistency
        /// and allowing to change gas particle speed in a single location.
        /// </summary>
        public float gasParticleBaseSpeed //because the inspector can't expose auto properties nor readonly variables
        {
            get => _gasParticleBaseSpeed;
            private set => _gasParticleBaseSpeed = value;
        }
        [SerializeField] 
        private float _gasParticleBaseSpeed = 1.0f; //base speed for alveolar gas particles
    
    
        /*// Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePerFrameValues();
        }*/
    
        /// <summary>
        /// Updates values which change each frame, e.g. those reliant on <c>Time.deltaTime</c>. 
        /// </summary>
        private void UpdatePerFrameValues()
        {
        }
    }
}
