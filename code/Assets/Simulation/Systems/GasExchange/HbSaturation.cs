using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation.Systems.GasExchange
{
    /// <summary>
    /// This class performs the calculations of hemoglobin (Hb) saturation with oxygen as red blood cells
    /// progress along the respiratory capillary - i.e. the resulting saturation values are obtained for every capillary section.
    /// </summary>
    /// <remarks>
    /// O2 binding to and dissociation from hemoglobin is depending on factors like the respective partial pressures of the respiratory
    /// gases in the blood <see cref="pO2BloodArray"/> and <see cref="pCo2BloodArray"/>, blood temperature <see cref="bloodTemperature"/>,
    /// the pH of red blood cells <see cref="PHrbc"/> and concentration of the organic phosphate 2,3-DPG in the blood <see cref="concentrationDpg"/>.
    /// The mathematical model used to express these dependencies and respective contstants was adopted from (Dash et al., 2016).
    /// </remarks>
    public class HbSaturation : AlveolusComponent
    {
        private ParametersData.UpdateValues updateHandler;

        /// <summary>
        /// Hemoglobin oxygen saturation, final output of <see cref="CalculateHbO2Saturations"/>.
        /// </summary>
        public float[] hbO2Saturation { get; private set; } = new float[1];



        /* Parameters that need to be collected from the respective parameters instance.*/
        private float[] pO2BloodArray;
        private float[] pCo2BloodArray;
        private float pHRBC;
        private float bloodTemperature;
        private float concentrationDpg;
        /* end of "external" parameters */

        /*
         * Constants that need to ba calculated depending on changing parameters.
         */
        /// <summary>
        /// Result of the polynomial expression for the effect of pH RBC on
        /// the oxygen partial pressure for 50% HbO2 saturation (P50).
        /// </summary>
        /// <remarks>
        /// Used as an instance state since it only changes when the related instance parameter changes
        /// and as such can be calculated once and cached after such a parameter change.
        /// </remarks>
        private float p50deltaPH;
        /// <summary>
        /// Result of the polynomial expression for the effect of DPG concentration on
        /// the oxygen partial pressure for 50% HbO2 saturation (P50).
        /// </summary>
        /// <remarks>
        /// Used as an instance state since it only changes when the related instance parameter changes
        /// and as such can be calculated once and cached after such a parameter change.
        /// </remarks>
        private float p50deltaDpg;
        /// <summary>
        /// Result of the polynomial expression for the effect of temperature on
        /// the oxygen partial pressure for 50% HbO2 saturation (P50).
        /// </summary>
        /// <remarks>
        /// Used as an instance state since it only changes when the related instance parameter changes
        /// and as such can be calculated once and cached after such a parameter change.
        /// </remarks>
        private float p50deltaT;
        /*
         * End of "unknown" constants.
         */


        /*
         * Known constants:
         */
        /// <summary> Standard blood temperature [°C], value taken from (Dash et al., 2016) </summary>
        const float TempStandard = 37;
        /// <summary>
        /// The value of oxygen partial pressure at which hemoglobin is 50 % saturated by oxygen at standard physiological levels
        /// of pCO2, pHrbc, [DPG] and T, value taken from (Dash et al., 2016).
        /// </summary>
        const float P50Standard = 26.8f;
        /*
         * End of known constants.
         */

        private PartialPressure partialPressureScript;

        ~HbSaturation()
        {
            partialPressureScript?.Unsubscribe(HandlePartialPressuresUpdate);
        }

        protected override void HandleReset(AlveolusController instance)
        {
            Init();
        }

        /// <summary>
        /// Subscribe a method to be called whenever HbSaturation updates.
        /// </summary>
        /// <param name="method">Method to be called.</param>
        public void Subscribe(ParametersData.UpdateValues method)
        {
            updateHandler += method;
        }

        /// <summary>
        /// Unsubscribe a method from getting called on HbSaturation changes.
        /// </summary>
        /// <param name="method">Method to unsubscribe.</param>
        public void Unsubscribe(ParametersData.UpdateValues method)
        {
            updateHandler -= method;
        }


        /// <summary>
        /// Fetch required values and calculate initial state.
        /// </summary>
        private void Init()
        {
            
            partialPressureScript?.Unsubscribe(HandlePartialPressuresUpdate);
            partialPressureScript = PartialPressure;
            partialPressureScript.Subscribe(HandlePartialPressuresUpdate);

            pHRBC = Parameters.bloodpHRBC;
            bloodTemperature = Parameters.bloodTemperature;
            concentrationDpg = Parameters.concentrationDPG * 0.001f;

            pO2BloodArray = partialPressureScript.partialPressures;
            pCo2BloodArray = partialPressureScript.co2partialPressures;

            hbO2Saturation = new float[partialPressureScript.numberSections];

            CalculateP50DeltaPH();
            CalculateP50DeltaT();
            CalculateP50DeltaDpg();

            RecalculateSaturationsAlongCapillary();
        }


        /// <summary>
        /// Fetch new parameter values, recalculate solubilities and Hb saturation accordingly,
        /// then alert all subscribers if there are updates. 
        /// </summary>
        protected override void HandleParametersUpdated()
        {
            bool anyChange = false;

            if (pHRBC != Parameters.bloodpHRBC)
            {
                anyChange = true;
                pHRBC = Parameters.bloodpHRBC;
                CalculateP50DeltaPH();
            }

            if (bloodTemperature != Parameters.bloodTemperature)
            {
                anyChange = true;
                bloodTemperature = Parameters.bloodTemperature;
                CalculateP50DeltaT();
            }

            if (concentrationDpg != (Parameters.concentrationDPG * 0.001f))
            {
                anyChange = true;
                concentrationDpg = Parameters.concentrationDPG * 0.001f;
                CalculateP50DeltaDpg();
            }

            if (anyChange)
            {
                RecalculateSaturationsAlongCapillary();
                PropagateUpdates();
            }
        }

        /// <summary>
        /// Fetch new partial pressure values, recalculate Hb saturation, then alert all subscribers to those updates. 
        /// </summary>
        private void HandlePartialPressuresUpdate()
        {
            pO2BloodArray = partialPressureScript.partialPressures;
            pCo2BloodArray = partialPressureScript.co2partialPressures;

            RecalculateSaturationsAlongCapillary();

            PropagateUpdates();
        }

        /// <summary>
        /// Recalculates the arrays of saturation values in <see cref="hbO2Saturation"/> for each capillary segment, 
        /// based on the partial pressures passed into <see cref="pO2BloodArray"/> and <see cref="pCo2BloodArray"/>. 
        /// </summary>
        private void RecalculateSaturationsAlongCapillary()
        {
            CalculateHbO2Saturations(pO2BloodArray, pCo2BloodArray,
                out float[] o2Sats);
            hbO2Saturation = o2Sats;
        }

        /// <summary>
        /// This method instructs the calculation of hemoglobin oxygen saturation for each capillary section and stores the results in an array.
        /// </summary>
        /// <remarks>
        /// This method iterates over the values in the arrays and calls
        /// <see cref="CalculateHbO2Saturation(in float,in float,out float)"/> for each set of values.
        /// <c>hbO2Saturations[i]</c> is calculated from <c>pO2[i]</c> and <c>pCo2[i]</c>. 
        /// </remarks>
        /// <param name="pO2">Array of oxygen partial pressure values of all capillary sections. </param>
        /// <param name="pCo2">Array of carbon dioxide partial pressure values of all capillary sections. </param>
        /// <param name="hbO2Saturations"> Output array of hemoglobin oxygen saturation values of all capillary sections.< /param>
        /// <exception cref="ArgumentException">Thrown when length of O2 and CO2 partial pressure arrays doesn't match.</exception>
        public void CalculateHbO2Saturations(in float[] pO2, in float[] pCo2,
            out float[] hbO2Saturations)
        {
            int length = pO2.Length;

            if (length != pCo2.Length)
            {
                throw new ArgumentException("Input arrays of are different size! " +
                                            "pO2 is size " + pO2.Length + ", pCo2 is size " + pCo2.Length);
            }

            hbO2Saturations = new float[length];

            for (int i = 0; i < length; i++)
            {
                CalculateHbO2Saturation(pO2[i], pCo2[i], out hbO2Saturations[i]);
            }
        }

        /// <summary>
        /// This method performs the key calculations that yield the saturation of hemoglobin with oxygen.
        /// </summary>
        /// <remarks>
        /// First, the p50 value for the current conditions is obtained from <see cref="P50(float)"/> and the
        /// Hill coefficient nH for the current pO2 is obatined from <see cref="Hill(float)"/>. 
        /// With these values, hemoglobin oxygen saturation can be calculated in a Hill equation as proposed by (Dash et al., 2016).
        /// </remarks>
        /// <param name="pO2"> Partial pressure of oxygen in the blood.</param>
        /// <param name="pCo2"> Partial pressure of carbon dioxide in the blood.</param>
        /// <param name="hbO2Sat">Output for hemoglobin oxygen saturation.</param>
        public void CalculateHbO2Saturation(in float pO2, in float pCo2, out float hbO2Sat)
        {
            float p50 = P50(pCo2);
            float nH = Hill(pO2);

            double ppPower = Math.Pow((pO2 / p50), nH);
            hbO2Sat = (float)(ppPower / (1 + ppPower));
        }


        /// <summary>
        /// This method instructs the calculation of hemoglobin oxygen saturation values to be plotted in the oxygen dissociation curve.
        /// </summary>
        /// <remarks> 
        /// It creates an array of <see cref="numValues"/> evenly distributed oxygen partial pressure values from
        /// <see cref="pO2Min"/> (inclusive) to <see cref="pO2Max"/> (inclusive). 
        /// The method <see cref="CalculateHbO2Saturation(in float, in float, out float)"/> is called to calculate 
        /// the respective hemoglobin oxygen saturation value from every pO2 value in this array and the current pCO2 of the blood.
        /// </remarks>
        /// <param name="pCo2">Current partial pressure of carbon dioxide in the (incoming) blood.</param>
        /// <param name="pO2Min"> Lower limit of the range of oxygen partial pressure values.</param>
        /// <param name="pO2Max"> Upper limit of the range of oxygen partial pressure values.</param>
        /// <param name="numValues"> Number of datapoints, minimum 2 (for <c>pO2Min</c> and <c>PO2Max</c>).</param>
        /// <returns> Array of <c>numValues</c> evenly distributed pO2 values in order from <c>pO2Min</c> to <c>PO2Max</c>
        /// and the corresponding hemoglobin oxygen saturation values.</returns>
        /// <exception cref="ArgumentException">Thrown when numValues is less than 2</exception>
        public void ProvideDissociationCurveData(float pCo2, float pO2Min, float pO2Max, int numValues,
            out float[] pO2Set, out float[] hbO2Saturations)
        {
            if (pO2Min > pO2Max)
            {
                throw new ArgumentException("Range minimum must not be greater than maximum! "
                                            + "pO2Min is " + pO2Min + ", pO2Max is " + pO2Max);
            }

            if (numValues < 2)
            {
                throw new ArgumentException("numValues is " + numValues + ", must be at least 2");
            }

            //numValues should be inclusive of pO2Min
            float stepSize = (pO2Max - pO2Min) / (numValues - 1);

            pO2Set = new float[numValues];

            int lastElement = numValues - 1;
            pO2Set[0] = pO2Min;
            for (int i = 1; i < lastElement; i++)
            {
                pO2Set[i] = i * stepSize;
            }

            //set last element to max to avoid risk of precision issues
            pO2Set[lastElement] = pO2Max;

            hbO2Saturations = new float[numValues];

            for (int i = 0; i < pO2Set.Length; i++)
            {
                CalculateHbO2Saturation(pO2Set[i], pCo2, out hbO2Saturations[i]);
            }
        }
     


        /// <summary>
        /// Estimation of Hill coefficient that considers cooperativity of O2 for Hb.
        /// </summary>
        /// <param name="pCo2">CO2 partial pressure for which to determine the Hill coefficient.</param>
        private static float Hill(float pO2)
        {
            const float alpha = 2.82f; //[unitless]
            const float beta = 1.2f; //[unitless]
            const float gamma = 29.25f; //[mmHg]

            float nH = alpha - beta * (float) (Math.Pow(10, -(pO2 / gamma)));
            return nH;
        }



        /// <summary>
        /// p50 is the value of oxygen partial pressure in the plasma at which Hb is 50% saturated by O2.
        /// </summary>
        /// <returns>
        /// p50 is a function of pCO2, pHrbc, [DPG], and temperature. The polynomal expression for p50 implemented in this method
        /// was obtained by Dash and Bassingthwaighte (2010) and Dash et al. (2016) who fitted it to experimental data.
        /// </returns>
        private float P50(float pCo2)
        {
            //reference values for standard conditions:
            const float pCO2Standard = 40;

            float deltaPCo2 = pCo2 - pCO2Standard;

            float p50deltapCO2 = P50Standard + 0.1273f * deltaPCo2 + 0.0001083f * (float) (Math.Pow(deltaPCo2, 2));

            float p50 = P50Standard * (p50deltaPH / P50Standard) * (p50deltapCO2 / P50Standard) *
                        (p50deltaDpg / P50Standard) * (p50deltaT / P50Standard);
            return p50;
        }

        /// <summary>
        /// Calculate the polynomial expression for the effect of pH on the oxygen partial pressure for P50.
        /// </summary>
        private void CalculateP50DeltaPH()
        {
            float pHStandard = 7.24f;
            float deltaPH = Alveolus.instanceParameters.bloodpHRBC - pHStandard;
            p50deltaPH = P50Standard - 25.535f * deltaPH + 10.646f * (float)(Math.Pow(deltaPH, 2)) -
                               1.764f * (float)(Math.Pow(deltaPH, 3));
        }

        /// <summary>
        /// Calculate the polynomial expression for the effect of DPG concentration on the oxygen partial pressure for P50.
        /// </summary>
        private void CalculateP50DeltaDpg()
        {
            //float cDPGStandard = (float)(4.65 * Math.Pow(10, -3));
            float cDpgStandard = 0.00465f;

            float deltaDpg = concentrationDpg - cDpgStandard;

            p50deltaDpg = P50Standard + 795.63f * deltaDpg - 19660.89f * (float) (Math.Pow(deltaDpg, 2));
        }

        /// <summary>
        /// Calculate the polynomial expression for the effect of temperature on the oxygen partial pressure for P50.
        /// </summary>
        public void CalculateP50DeltaT()
        {
            float deltaT = bloodTemperature - TempStandard;

            p50deltaT = P50Standard + 1.435f * deltaT + 0.04163f * (float) (Math.Pow(deltaT, 2)) +
                        0.000686f * (float) (Math.Pow(deltaT, 3));
        }
        
        /// <summary>
        /// Alert all subscribed scripts to updates.
        /// </summary>
        private void PropagateUpdates()
        {
            updateHandler?.Invoke();
        }
    }
}