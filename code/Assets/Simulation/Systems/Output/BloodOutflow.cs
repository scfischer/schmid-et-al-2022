using System;
using UnityEngine;

namespace Simulation.Systems.Output
{
    /// <summary>
    /// This class calculates the oxygen uptake of individual erythrocytes as they reach the end of the capillary. 
    /// These results are added to a total oxygen uptake until the simulation is reset. 
    /// Additionally, the transit time of the erythrocyte along the capillary is determined.
    /// </summary>
    public class BloodOutflow : AlveolusComponent
    {
        private bool _eryAtEnd;
        /// <summary> Signal when an erythrocyte has reached the end of the main capillary. </summary>
        public bool eryAtEnd
        {
            get => _eryAtEnd;
            set
            {
                _eryAtEnd = value;
                CalculateO2Uptake();
                PropagateUpdates();
            }
        }

        /// <summary> Difference in oxygen saturation of an erythrocyte from the start of the main capillary to its end. </summary>
        public float o2SatDifference { get; set; }
        /// <summary> Total mass of oxygen [ng] taken up by the blood as it passes through the capillary, 
        /// added up over time until simulation is reset. </summary>
        public float o2UptakeTotal { get; private set; }
        /// <summary>  Total oxygen volume [µm³] taken up by the blood as it passes through the capillary, 
        /// added up over time until simulation is reset. </summary>
        public float o2UptakeVolume { get; private set; }

        /// <summary> The time it takes for an erythrocyte to pass the capillary. </summary>
        public float bloodTransitTime { get; private set; }

        /// <summary> Average number of hemoglobin molecules in an erythrocyte (Pierigè et al., 2008). </summary>
        private const int NumberHbPerEry = 270000000;
        /// <summary> Oxygen binding sites per hemoglobin molecule. </summary>
        private const int HbBindingSites = 4;
        /// <summary> Molecular weight of oxygen [g/mol]. </summary>
        private const float O2MolWeight = 31.9988f;
        /// <summary> Number of oxygen molecules per mole. </summary>
        private static readonly float O2PerMole = Convert.ToSingle(6.022 * Math.Pow(10, 23));
        /// <summary> 
        /// Specific volume of O2. (https://www.engineeringtoolbox.com/oxygen-O2-density-specific-weight-temperature-pressure-d_2082.html?vA=37&degree=C&pressure=1bar#)
        /// Converted from [m³/kg] to [µm³/ng]
        /// </summary>
        private static readonly float o2SpecificVolume = Convert.ToSingle(1 / 1.237 * Math.Pow(10, 6));


        /// <summary>
        /// Average distance for passage of an erythrocyte through the capillary network, i.e. representative capillary length (Weibel et al., 1993).
        /// </summary>
        private const int MeanCapillaryLength = 500;

        /// <summary>
        /// Number of oxygen molecules an erythrocyte takes up along the passage through the main capillary. 
        /// Extrapolated to the entire capillary network.
        /// </summary>
        private float o2UptakeMolecules;
        /// <summary>
        /// Mass of oxygen [ng] an erythrocyte takes up along the passage through the main capillary. 
        /// Calculated from <see cref="o2UptakeMolecules"/> and molecular weight.
        /// </summary>
        private float o2UptakeMass;

        /// <summary> Blood flow velocity [mm/s], see <see cref="ParametersData.bloodFlowVelocity"/>. </summary>
        private float bloodFlowVelocity;

        private ParametersData.UpdateValues updateHandler;

        protected override void HandleReset(AlveolusController instance)
        {
            o2UptakeTotal = 0;
            UpdateValues();
        }

        protected override void HandleParametersUpdated()
        {
            if (Parameters.bloodFlowVelocity != bloodFlowVelocity)
            {
                UpdateValues();
            }
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

       
        void UpdateValues()
        {
            CalculateO2Uptake();

            CalculateBloodTransitTime();

            PropagateUpdates();
        }

        /// <summary>
        /// Calculates mass and volume of the oxygen taken up by an erythrocyte along the passage through the capillary as soon 
        /// as this erythrocyte reaches the end of the capillary. These values are extrapolated to the whole capillary network 
        /// and added to the total oxygen uptake. 
        /// </summary>
        /// <remarks> First, the number of oxygen molecules taken up by the erythrocyte is determined from its
        /// difference in oxygen saturation compared to the start of the capillary (<see cref="o2SatDifference"/>). 
        /// From this, the mass of oxygen is calculated from its molecular weight and the number of molecules in a mole.
        /// Finally, oxygen volume is calculated from the mass and the specific volume of oxygen. </remarks>
        private void CalculateO2Uptake()
        {
            if (eryAtEnd)
            {

                o2UptakeMolecules = Parameters.NumberCapillaries * NumberHbPerEry * (HbBindingSites * o2SatDifference);

                // how many mols of O2 
                float numberO2Mol = o2UptakeMolecules / O2PerMole;

                // convert molecular weight to ng
                o2UptakeMass = numberO2Mol * O2MolWeight * 1000000000;

                o2UptakeTotal += o2UptakeMass;

                o2UptakeVolume = o2UptakeTotal * o2SpecificVolume;

                eryAtEnd = false;
            }
        }

        /// <summary>
        /// Determine the time blood takes to cross from the beginning to end of the capillary (in the real system,
        /// where the process happens much faster than our simulation). Changes based on <see cref="ParametersData.bloodFlowVelocity"/>.
        /// </summary>
        /// <remarks>
        /// For the sake of avoiding subsequent invalid math, if blood flow stops (i.e. flow velocity is 0),
        /// <see cref="bloodTransitTime"/> is set to 1000.
        /// </remarks>
        private void CalculateBloodTransitTime()
        {
            bloodFlowVelocity = Parameters.bloodFlowVelocity;
            float bloodFlowUmPerS = bloodFlowVelocity * 1000;

            if (bloodFlowVelocity > 0)
            {
                bloodTransitTime = MeanCapillaryLength / bloodFlowUmPerS;
            }
            else
            {
                bloodTransitTime = 1000;
            }
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