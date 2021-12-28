using System.Collections.Generic;
using PathCreation;
using Simulation.Systems;
using Simulation.Systems.GasExchange;
using UnityEngine;

namespace Simulation.Visualization.Capillary
{
    /// <summary>
    /// Creates a visual representation of oxygen molecules diffusing from alveolar space into the capillary.
    /// </summary>
    /// <remarks>
    /// This class is used to visualizes the flow of oxygen into the capillary, using the number of crossing particles
    /// from the <c>crossingSpheresO2</c> array provided by <see cref="PartialPressure"/>. They are distributed randomly
    /// across the the length of their given capillary section. The particles are created in the alveolar space and move
    /// towards the center of the capillary, using target points on the <c>PathCreator</c> path through the capillary.
    /// To represent absorption into the blood, the particles fade to transparent as they move into the capillary.
    /// </remarks>
    public class AlveolarCapillaryDiffusionParticleBased : AlveolusComponentBehavior
    {
        /// <summary> The distance from the center of the capillary at which the particles should spawn. </summary>
        /// <remarks> The spawn-in point is on a line from the GameObject containing this script to the capillary path. </remarks>
        [SerializeField] 
        private float spawnInDistanceFromCapillaryCenter = 3.0f;
        /// <summary> The distance particles move past the center of the capillary before they disappear. </summary>
        [SerializeField] 
        private float disappearDistancePastCapillaryCenter = 0.6f;
    
        /// <summary> The color for the diffusing oxygen particles. </summary>
        [SerializeField]
        private Color o2ParticleColor = Color.red;
        /// <summary> The color for the diffusing carbon dioxide particles. </summary>
        [SerializeField]
        private Color co2ParticleColor = Color.blue;

        [SerializeField]
        private PathCreator capillaryPathCreator;

        [SerializeField]
        private ParticleSystem prefab;

        private List<ParticleSystem> oxygenParticleSystems = new List<ParticleSystem>();
        private List<ParticleSystem> co2ParticleSystems = new List<ParticleSystem>();
        
        private VertexPath capillaryPath;
        private PartialPressure flowScript;

        private int numSections;
        private Vector3[] sectionStartPoints; //positions on the capillary path that mark the end of a section

        private float fadeStartDistanceFromSpawn;
        private float moveDistance;
        private float fadeDistance;
        

        protected override void OnDestroy()
        {
            flowScript?.Unsubscribe(HandlePartialPressuresUpdated);
            base.OnDestroy();
        }

        protected override void HandleReset(AlveolusController instance)
        {
            if (flowScript != PartialPressure)
            {
                flowScript?.Unsubscribe(HandlePartialPressuresUpdated);
                flowScript = PartialPressure;
                flowScript?.Subscribe(HandlePartialPressuresUpdated);
            }
            
            if (flowScript == null)
            {
                return;
            }
            
            if (capillaryPath != capillaryPathCreator.path)
            {
                capillaryPath = capillaryPathCreator.path;
                InitValues();
                SetupParticleSystems();
            }
            
            HandlePartialPressuresUpdated();
        }

        void HandlePartialPressuresUpdated()
        {
            UpdateParticleSystems();
        }
        

        /// <summary>
        /// Determine the basic layout: number of capillary sections, start points of each section along the path. 
        /// </summary>
        private void InitValues()
        {
            numSections = flowScript.numberSections;
            
            sectionStartPoints = new Vector3[numSections + 1];
            float sectionPathIncrement = 1.0f / numSections;
            for (int i = 0; i < numSections; i++)
            {
                float distance = i * sectionPathIncrement;
                sectionStartPoints[i] = capillaryPath.GetPointAtTime(distance, EndOfPathInstruction.Stop);
            }
            //to have the end of the last section, we add one more point,
            //basically the start point of the nonexisting section after the last
            sectionStartPoints[numSections] = capillaryPath.GetPointAtTime(1.0f, EndOfPathInstruction.Stop);
            
            moveDistance = spawnInDistanceFromCapillaryCenter + disappearDistancePastCapillaryCenter;
        }

        /// <summary>
        /// Create particle systems for oxygen and carbon dioxide for each section, align them towards the capillary path and
        /// apply initial settings, then call <see cref="UpdateParticleSystems"/> to set proper particle spawn rates.
        /// </summary>
        private void SetupParticleSystems()
        {
            if ( (oxygenParticleSystems.Count > 0) || (co2ParticleSystems.Count > 0) )
            {
                oxygenParticleSystems.Clear();
                co2ParticleSystems.Clear();
                DestroyAllChildren();
            }

            Vector3 center = transform.position;

            for (int i = 0; i < numSections; i++)
            {
                GameObject oGo = Instantiate(prefab.gameObject, this.transform);
                ParticleSystem oPs = oGo.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = oPs.main;
                main.startColor = o2ParticleColor;
                ParticleSystem.ShapeModule shape = oPs.shape;

                //A particle system with circle shape and limited arc has the arc start at it's +x direction and
                //rotates around the z axis


                //first, align for relevant section
                Vector3 centerToSectionStart = sectionStartPoints[i] - center;
                Vector3 centerToSectionEnd = sectionStartPoints[i + 1] - center;
                
                //the new z vector is the cross product between the two vectors between which the section spans
                //facing "down" so that the arc goes from sectionstart[i] towards sectionstart[i+1]
                Vector3 zDirection = Vector3.Cross(centerToSectionStart, centerToSectionEnd);
                //for lookrotation, "forward" is the direction the z axis will face,
                //and "upwards" is the direction that the y axis will face
                //to make the x axis face along a vector, we need to get y via the cross product of z and x
                Vector3 yDirection = Vector3.Cross(zDirection, centerToSectionStart);
                Quaternion rot = Quaternion.LookRotation(zDirection, yDirection);
                oGo.transform.rotation = rot;
                
                //second, set arc correctly, starting with o2
                float arc = Vector3.Angle(centerToSectionStart, centerToSectionEnd);
                shape.arc = arc;

                //set the spawn point for particles
                shape.radius = centerToSectionStart.magnitude - spawnInDistanceFromCapillaryCenter;
                

                //calculate lifetime based on start and end point
                float lifetime = moveDistance / main.startSpeed.constant;
                main.startLifetime = lifetime;
                
                oxygenParticleSystems.Add(oPs);
                
                
                /* now for CO2 particles */
                GameObject cGo = Instantiate(prefab.gameObject, this.transform);
                ParticleSystem cPs = cGo.GetComponent<ParticleSystem>();
                
                cGo.transform.rotation = rot;
                shape = cPs.shape;
                //same arc
                shape.arc = arc;
                //this is why the particle system can't just be duplicated: different radius,
                //since CO2 particles start in the capillary
                shape.radius = centerToSectionStart.magnitude;
                
                main = cPs.main;
                main.startLifetime = lifetime;
                //also different CO2 particle colour
                main.startColor = co2ParticleColor;
                //and CO2 particles have opposite direction
                main.startSpeedMultiplier *= -1;

                co2ParticleSystems.Add(cPs);
            }
            
            UpdateParticleSystems();
        }
        
        /// <summary>
        /// Fetch the particle emission rates, set the particle systems accordingly, and restart them.
        /// </summary>
        private void UpdateParticleSystems()
        {
            for (int i = 0; i < numSections; i++)
            {
                ParticleSystem ps = oxygenParticleSystems[i];
                
                ps.Stop();
                ps.Clear();

                ParticleSystem.EmissionModule emission = ps.emission;
                emission.rateOverTime = flowScript.crossingSpheresO2[i];
                
                ps.Play();
                
                
                ps = co2ParticleSystems[i];
                
                ps.Stop();
                ps.Clear();

                emission = ps.emission;
                emission.rateOverTime = flowScript.crossingSpheresCO2[i];
                
                ps.Play();
            }
        }
        
        /// <summary>
        /// Destroys all child GameObjects.
        /// </summary>
        private void DestroyAllChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    
    }
}
