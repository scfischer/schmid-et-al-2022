using System.Collections;
using PathCreation;
using UnityEngine;
using System;

namespace Simulation.Systems.Erythrocytes
{
    /// <summary>
    /// Class to spawn new erythrocytes at the beginning of the inflow capillary.
    /// </summary>
    /// <remarks>
    /// Erythrocytes are spawned on the path of the inflow capillary. This class also stores the respective instances of the following paths (main and outflow), 
    /// in order to pass them to the <see cref="ErythrocyteMovement"/> component of child erythrocytes.
    /// Initially, erythrocytes are spread along the path with the appropriate distance resembling the current blood volume, i.e. erythrocyte density.
    /// As the erythrocytes move, new clones are spawned at the start of the path in appropriate distance to the previous erythrocyte.
    /// </remarks>
   

    public class SpawnErythrocyte : AlveolusComponentBehavior
    {
        /// <summary> The prefab for the erythrocytes to spawn. To be assigned in the editor. </summary>
        [SerializeField]
        private GameObject erythrocytePrefab;

        /// <summary> The <c>pathCreator</c> paths along which the erythrocytes move. To be assigned in the editor.</summary>
        /// <remarks> Erythrocytes are only spawned on <see cref="inflowPath"/>. </remarks>
        public PathCreator inflowPath;
        public PathCreator mainPath;
        public PathCreator outflowPath;

        /// <summary> Boundaries for the random pick of a distance between two erythrocytes. </summary>
        private float minDistance = 0.8f;
        private float maxDistance = 1.2f;
        /// <summary> This scale sets the chosen distance in proportion to changes in <see cref="ParametersData.bloodVolume"/>. </summary>
        private float distanceScale;
        /// <summary> Maximum blood volume perfusing a single alveolus [µm³]. </summary>
        /// <remarks> Estimated from data taken from (Gehr et al., 1978; Ochs et al., 2004) . </remarks>
        private const float maxBloodVolume = 808000.0f;
        /// <summary> Number of erythrocytes per volume blood [/µm³] </summary>
        /// <remarks> Taken from (Rosen, R. Optimality principles in biology. Plenum press 1967, p.56 </remarks>
        private const float erysPerVolume = 0.005f;


        /// <summary> Mean length of capillaries surrounding a single alveolus. Unit: [mm]. </summary>
        /// <remarks> Taken from (Weibel et al., 1993). </remarks>
        private const float MeanCapillaryLength = 0.5f;
        /// <summary> Volume [µm³] of a capillary with the length 500µm and radius 3.15µm (Mühlfeld et al.2010) </summary>
        private const float capillaryVolume = 15586;


        /// <summary> This factor sets the simulation time in relation to the time, the physiological process would take. </summary>
        public const float sloMo = 40;

        /// <summary> Conversion factor between real speed of the gas exchange process and the speed of the simulation. </summary>
        private float speedFactor;
        /// <summary> Speed of the simulation. </summary>
        private float speed;


        private bool m_visualsEnabled = true;
        
        /// <summary>
        /// Enable or disable erythrocyte visuals, i.e. mesh. For when the alveolus is in or out of focus.
        /// </summary>
        public bool VisualsEnabled
        {
            set
            {
                m_visualsEnabled = value;
                UpdateVisuals();
            }
            get => m_visualsEnabled;
        }

        protected override void HandleReset(AlveolusController instance)
        {
            DestroyObjects();
            Init();
        }

        /// <summary>
        /// Erythrocyte spawning is initiated according to the current parameters.
        /// </summary>
        /// <remarks>
        /// <see cref="distanceScale"/> is determined depending on <see cref="ParametersData.bloodVolume"/> and affects the
        /// distance between erythrocytes so that a higher blood volume leads to a higher erythrocyte density.
        /// </remarks>
        private void Init()
        {
            CalculateDistanceScale();
            Generate(inflowPath, AssignPath.Inflow);
            Generate(mainPath, AssignPath.Main);

            speedFactor = mainPath.path.length / (MeanCapillaryLength * SpawnErythrocyte.sloMo);
            speed = Parameters.bloodFlowVelocity * speedFactor;

            StopAllCoroutines();
            StartCoroutine(SpawnErythrocytes());
            
        }


        protected override void HandleParametersUpdated()
        {
            CalculateDistanceScale();
            speed = Parameters.bloodFlowVelocity * speedFactor;
        }

        /// <summary>
        /// Initial spreading of erythrocytes along the path of the inflow capillary.
        /// </summary>
        private void Generate(PathCreator pathCreator, AssignPath assignPath)
        {
            float distance = 0;
            float spacing;

            while (distance < pathCreator.path.length)
            {
                Vector3 position = pathCreator.path.GetPointAtDistance(distance);
                Quaternion rot = pathCreator.path.GetRotationAtDistance(distance);
                GameObject ery = Instantiate(erythrocytePrefab, position, rot, transform);
                ery.layer = transform.gameObject.layer;
                ery.GetComponent<MeshRenderer>().enabled = m_visualsEnabled;
                ery.GetComponent<ErythrocyteMovement>().distanceTravelled += distance;
                ery.GetComponent<ErythrocyteMovement>().assignPath = assignPath;

                spacing = (UnityEngine.Random.Range(minDistance, maxDistance)) * distanceScale;
                distance += spacing;
            }
        }


        /// <summary>
        /// The coroutine that triggers spawning of erythrocytes.
        /// </summary>
        /// <remarks>
        /// The timepoint of spawning a new erythrocyte is determined by tracking the distance to the last spawned erythrocyte.
        /// It is depending on <see cref="speed"/> and <see cref="distanceScale"/>.  
        /// </remarks>
        /// <returns> Wait for the randomized interval. </returns>
        private IEnumerator SpawnErythrocytes()
        {
            yield return null;

            float lastEryDistance = 0;
            float spawnDistance = minDistance;

            while (true)
            {
                lastEryDistance += speed * Time.deltaTime;

                if (lastEryDistance >= (spawnDistance * distanceScale))
                {
                    CreateErythrocyte(erythrocytePrefab, transform, AssignPath.Inflow);
                    spawnDistance = UnityEngine.Random.Range(minDistance, maxDistance);
                    lastEryDistance = 0;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Creates a new Erythrocyte and places it at the beginning of the path it'll follow through the capillary.
        /// </summary>
        /// <param name="prefab">The prefab to spawn as erythrocyte and set on the path.</param>
        /// <param name="parent">The parent <c>Transform</c>to assign the new erythrocyte to.</param>
        /// <returns>The newly created Erythrocyte.</returns>
        private GameObject CreateErythrocyte(GameObject prefab, Transform parent, AssignPath assign)
        {
            Vector3 position = inflowPath.path.GetPoint(0);
            GameObject erythrocyte = Instantiate(prefab, position, Quaternion.identity, parent);
            erythrocyte.layer = parent.gameObject.layer;
            erythrocyte.GetComponent<MeshRenderer>().enabled = m_visualsEnabled;
            erythrocyte.GetComponent<ErythrocyteMovement>().assignPath = assign;
            return erythrocyte;
        }

        private void CalculateDistanceScale()
        {
            float volume = capillaryVolume * (Parameters.bloodVolume / maxBloodVolume);
            double n_erys = erysPerVolume * volume;
            distanceScale = mainPath.path.length / Convert.ToSingle(n_erys);
        }


        private void UpdateVisuals()
        {
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.enabled = m_visualsEnabled;
            }
        }

        /// <summary> Destroy all erythrocate clones. </summary>
        private void DestroyObjects()
        {
            int numChildren = transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject, false);
            }
        }
    }
}
