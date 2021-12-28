using PathCreation;
using Simulation.Systems.GasExchange;
using UnityEngine;

namespace Simulation.Systems.Erythrocytes
{
    /// <summary>
    /// Controls the erythrocyte's movement along the paths through the representative capillary.
    /// </summary>
    /// <remarks>
    /// The erythrocyte moves along the paths in the following order: inflow, main, then outflow. As soon as it reaches the end of one path, 
    /// it is positioned at the start of the next path which it and subsequenty follows. At the end of the main path, <see cref="ErythrocyteGasExchange"/> is 
    /// instructed to take stock of oxygen saturation changes across the main capillary. Once it reaches the end of outflow path, it is destroyed.
    /// 
    /// This class uses the plugin <see cref="PathCreation"/> as a tool to orientate along a <c> VertexPath</c>. Depending on <c> EndOfPathInstruction/c>, 
    /// it i.a. offers the possibility to access the position and rotation of a transform travelling a given distance along the given path. 
    /// </remarks>
    

    /// <summary> Three different modes that control at which part of the simulation the erythrocyte is and how it has to move. </summary>
    public enum AssignPath { Inflow, Main, Outflow };

    public class ErythrocyteMovement : AlveolusComponentBehavior
    {
        private ErythrocyteGasExchange gasExchange;
        private SpawnErythrocyte spawner;

        public AssignPath assignPath { get; set; } 
        private EndOfPathInstruction endBehavior = EndOfPathInstruction.Stop;

        
        /// <summary> References to the <c>PathCreator</c> path that the erythrocyte travels along. </summary>
        private VertexPath path;

        /// <summary> The <c>PathCreator</c> path that lies in the inflow capillary. Gets passed on from <see cref="SpawnErythrocyte"/> component of parent. </summary>
        private VertexPath inflowPath;
        /// <summary> The <c>PathCreator</c> path that lies in the main representative capillary. Gets passed on from <see cref="SpawnErythrocyte"/> component of parent. </summary>
        private VertexPath mainPath;
        /// <summary> The <c>PathCreator</c> path that lies in the outflow capillary. Gets passed on from <see cref="SpawnErythrocyte"/> component of parent. </summary>
        private VertexPath outflowPath;

        /// <summary>  Float that tracks how far along the current path the erythrocyte has moved. </summary>
        public float distanceTravelled { get; set; } 
        /// <summary> <see cref="PartialPressure.numberSections"/>. </summary>
        private int numberSections;
        /// <summary> Length of a section of the path, if it is subdivided as the capillary itself. </summary>
        private float pathSectionLength;
        /// <summary> Speed of the simulation. <see cref="SpawnErythrocyte.speed"/> </summary>
        private float speed;
        /// <summary> Conversion factor between real speed of the gas exchange process and the speed of the simulation. <see cref="SpawnErythrocyte.speedFactor"/> </summary>
        private float speedfactor;
        /// <summary> <see cref="SpawnErythrocyte.MeanCapillaryLength"/>. </summary>
        private const float MeanCapillaryLength = 0.5f; //[mm] because blood speed is in [mm/s]


        /// <summary> The current capillary section that this erythrocyte is in.</summary>
        /// <remarks> See <see cref="PartialPressure"/> for settings of sections.</remarks>
        public int section { get; private set; } = 0;


        private void Init()
        {
            spawner = transform.parent.GetComponent<SpawnErythrocyte>();
            gasExchange = GetComponent<ErythrocyteGasExchange>();
            inflowPath = spawner.inflowPath.path;
            mainPath = spawner.mainPath.path;
            outflowPath = spawner.outflowPath.path;

            speedfactor = mainPath.length / (MeanCapillaryLength * SpawnErythrocyte.sloMo);
            speed = Parameters.bloodFlowVelocity * speedfactor;
        }

        protected override void HandleReset(AlveolusController instance)
        {
            Init();
        }

        
        // Update is called once per frame
        void Update()
        {
            speed = Parameters.bloodFlowVelocity * speedfactor;
            distanceTravelled += speed * Time.deltaTime;

            if (assignPath == AssignPath.Inflow)
            {
                path = inflowPath;
                section = 0;
                if (distanceTravelled >= path.length)
                {
                    assignPath = AssignPath.Main;
                    distanceTravelled = 0;
                }
            }



            if (assignPath == AssignPath.Main)
            {
                path = mainPath;
                numberSections = Alveolus.partialPressures.numberSections;
                pathSectionLength = path.length / numberSections;
                section = Mathf.FloorToInt(distanceTravelled / pathSectionLength); 
               

                if (distanceTravelled >= path.length)
                {
                    assignPath = AssignPath.Outflow;
                    distanceTravelled = 0;

                    gasExchange.DeliverSaturationDifference();
                    BloodOutflow.eryAtEnd = true;
                }

            }

            if (assignPath == AssignPath.Outflow)
            {
                path = outflowPath;
                section = numberSections - 1;
                if (distanceTravelled >= path.length)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            transform.position = path.GetPointAtDistance(distanceTravelled, endBehavior);
            transform.rotation = path.GetRotationAtDistance(distanceTravelled,
                    endBehavior);
        }
    }
}