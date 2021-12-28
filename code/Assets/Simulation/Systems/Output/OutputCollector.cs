using System.Collections.Generic;
using Simulation.Systems.GasExchange;
using UnityEngine;

namespace Simulation.Systems.Output
{
    /// <summary>
    /// This class collects and calculates various data from blood at the end of the capillary to be available for 
    /// displaying to the user, e.g. in graphs.
    /// </summary>
    public class OutputCollector : AlveolusComponent
    {
        /// <summary> Array of oxygen partial pressures in the blood per capillary section,
        /// as calculated in <see cref="GasExchange.PartialPressure"/>. </summary>
        public float[] o2PartialPressures => PartialPressure.partialPressures;
        /// <summary> Partial pressure of oxygen in the blood at the end of the capillary. </summary>
        public float partialPressureAtEnd => o2PartialPressures[o2PartialPressures.Length - 1];

        /// <summary> Array of carbon dioxide partial pressures in the blood per capillary section,
        /// as calculated in <see cref="GasExchange.PartialPressure"/>. </summary>
        public float[] co2PartialPressures => PartialPressure.co2partialPressures;
        /// <summary> Partial pressure of carbon dioxide in incoming blood (at the start of the capillary). </summary>
        public float venousCo2PartialPressure => co2PartialPressures[0];
        /// <summary> Partial pressure of carbon dioxide in the blood at the end of the capillary. </summary>
        public float co2PartialPressureAtEnd => co2PartialPressures[co2PartialPressures.Length - 1];
        /// <summary> Time it takes an erythrocyte to cross one section of the capillary. </summary>
        public float timePeriod => PartialPressure.timePeriod;
        /// <summary> Array of hemoglobin oxygen saturation values that erythrocytes in a capillary section would have
        /// based on the partial pressure there, as calculated in <see cref="GasExchange.HbSaturation"/> </summary>
        public float[] hbO2Saturations => HbSaturation.hbO2Saturation;

        /// <summary> Blood oxygen saturation at the end of the capillary. </summary>
        public float saturationAtEnd => hbO2Saturations[hbO2Saturations.Length - 1];
        
        /// <summary> Mass of oxygen [ng] taken up by the blood while passing through the capillary. </summary>
        public float o2UptakeMass => BloodOutflow.o2UptakeTotal;

        /// <summary> Oxygen volume [µm³] taken up by the blood while passing through the capillary. </summary>
        public float o2UptakeVolume => BloodOutflow.o2UptakeVolume;

        /// <summary> Time taken for erythrocytes to pass through the entire capillary. </summary>
        public float bloodTransitTime => BloodOutflow.bloodTransitTime;

        /// <summary> Lower limit for the oxygen partial pressure values of the dissociation graph (x axis). </summary>
        public float dissociationGraphPO2Min { get; } = 0f;
        
        /// <summary> Upper limit for the oxygen partial pressure values of the dissociation graph (x axis). </summary>
        public float dissociationGraphPO2Max { get; } = 150f;

        private ParametersData.UpdateValues updateHandler;


        private BloodOutflow bloodOutflowScript;


        ~OutputCollector()
        {
            bloodOutflowScript?.Unsubscribe(PropagateUpdates);
        }

        
        protected override void HandleReset(AlveolusController instance)
        {
            bloodOutflowScript?.Unsubscribe(PropagateUpdates);
            bloodOutflowScript = BloodOutflow;
            bloodOutflowScript.Subscribe(PropagateUpdates);
        }

        protected override void HandleParametersUpdated()
        {
            PropagateUpdates();
        }

        /// <summary>
        /// Method that calls <see cref="HbSaturation.CalculateHemoglobinO2SaturationRange(float,float,float,int,out float[],out float[])"/>
        /// to create a list of oxygen saturation values based on the current carbon dioxide partial pressure of incoming blood 
        /// and a set range of oxygen partial pressures in the blood. To be plotted in the oxygen dissociation curve.  
        /// </summary>
        /// <param name="numDataPoints"> The total number of data points that should be generated. (Including
        /// the points at <see cref="dissociationGraphPO2Min"/> and <see cref="dissociationGraphPO2Max"/>). </param>
        public List<Vector2> CreateDissociationCurveData(int numDataPoints)
        {
            List<Vector2> points = new List<Vector2>(numDataPoints);

            HbSaturation.ProvideDissociationCurveData(venousCo2PartialPressure,
                dissociationGraphPO2Min, dissociationGraphPO2Max, numDataPoints,
                out float[] pO2Points, out float[] satO2Points);

            for (int i = 0; i < numDataPoints; i++)
            {
                points.Add(new Vector2(pO2Points[i], satO2Points[i]));
            }

            return points;
        }

        /// <summary>
        /// Subscribe a method to be called whenever OutputCollector updates.
        /// </summary>
        /// <param name="method">Method to be called.</param>
        public void Subscribe(ParametersData.UpdateValues method)
        {
            updateHandler += method;
        }

        /// <summary>
        /// Unsubscribe a method from getting called on OutputCollector changes.
        /// </summary>
        /// <param name="method">Method to unsubscribe.</param>
        public void Unsubscribe(ParametersData.UpdateValues method)
        {
            updateHandler -= method;
        }

        /// <summary>
        /// Alert all subscribers to changes.
        /// </summary>
        private void PropagateUpdates()
        {
            updateHandler?.Invoke();
        }
    }
}