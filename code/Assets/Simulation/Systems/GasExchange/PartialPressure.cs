using Simulation.Systems.Erythrocytes;
using System;

namespace Simulation.Systems.GasExchange
{
    /// <summary>
    /// This class handles all calculations for partial pressures of respiratory gases in the blood across the capillary.
    /// It determines the pressure gradient of gases between capillary sections and alveolar space and calculates the resulting 
    /// diffusion of gas molecules, as well as values deriving from that for visualization purposes.
    /// </summary>
    /// <remarks>
    /// This class is a pivotal point of the alveolar gas exchange simulation since it represents the first of two steps of
    /// the underlying mathematical model. This step describes the diffusion of oxygen out of the alveolus and into the
    /// capillary depending on partial pressure gradients and morphological parameters (surface area, barrier thickness).
    /// The diffusion of oxygen is calculated based on Fick's law and was adapted from (Weibel et al., 1993):
    /// :
    /// $$ \nu = K_{O_{2}} * \frac{s}{\tau} * \Delta PO_{2}$$
    /// $\nu$: $O_{2}$-flow [$\frac{\mu m^{3}}{sec}$] 
    /// $K_{ O_{ 2} }$: Krogh 's permeability coefficient for oxygen [$\frac{\mu m^{2}}{sec * mmHg}$] 
    /// s: surface area[$\mu m ^{ 2}$]
    /// $\tau$: barrier thickness[$\mu m$]
    /// 
    /// In a second step, O2-uptake by the red blood cells will be determined by calculating O2 saturation of hemoglobin <see cref="HbSaturation"/>.
    /// 
    /// A single visualized capillary was chosen to represent an averaged behaviour for blood flow and gas exchange as it
    /// occurs in the whole capillary network surrounding the alveolus. To approximate the gas uptake as blood flows through 
    /// this capillary network, the representative capillary is divided into a number of discrete sections of equal length. 
    /// That number is defined in <see cref="numberSections"/>. Initial gas partial pressure in the blood is determined by 
    /// a user-adjustable setting. Based on this value, and the likewise user-adjustable alveolar partial pressure, the 
    /// pressure gradient in the first section is determined. Based on the time the blood spends in a section of the capillary,
    /// determined by blood flow velocity, Fick's law is used to determine the diffusion of oxygen into that capillary section, 
    /// and how this changes its partial pressure in the blood in the time it takes to traverse the section. 
    /// The resulting value - i.e. the partial pressure the blood should have when entering the next section - is propagated 
    /// as initial partial pressure for the next capillary section, which calculates the new oxygen flow and resulting uptake and 
    /// blood oxygen partial pressure in the same manner, and so on.
    /// 
    /// The amount of carbon dioxide diffusing out of the capillary and into the alveolus is determined via the respiratory exchange
    /// ratio <see cref="RQ"/> from the amount of oxygen that is taken up by the blood.
    ///
    /// These partial pressure values are integral to simulating and visualizing the process of gas exchange as deoxygenated blood
    /// flows around the alveolus and takes up oxygen, both directly and through derived values like the amount of particles
    /// that diffuse from the alveolar space into the blood.
    /// </remarks>
    public class PartialPressure : AlveolusComponent
    {
        /// <summary> Number of sections that the capillary should be divided into for math/simulation purposes. </summary>
        /// <remarks> Defines the size of arrays holding per-section values for the capillary, and is as such relevant
        /// for anything relying on capillary partial pressure values or gas diffusion numbers.</remarks>
        public int numberSections { get; private set; } = 50;
        
        /// <summary> Mean length of respiratory capillaries surrounding an alveolus [µm]. </summary>
        /// <remarks>
        /// This value was taken from (Weibel et al. 1993). In reality,cthe alveolus is surrounded by a network, 
        /// rather multiple distinct, non-branching capillaries. Taking an average path of blood through that network 
        /// to represent as a single capillary is an acceptable abstraction for the purpose of this project.
        /// </remarks>
        private const int MeanCapLength = 500;

        /// <summary> Maximum blood volume [µm³] (see <see cref="ParametersData"/>). </summary>
        private const float volumeCapillaryNet = 808000;
        /// <summary> Blood flow velocity [mm/s] (see <see cref="ParametersData"/>). </summary>
        private float bloodVelocity;
        /// <summary> Alveolar surface area [µm²] (see <see cref="ParametersData"/>). </summary>
        private float surfaceArea;
        /// <summary> Thickness of tissue barrier [µm] (see <see cref="ParametersData"/>). </summary>
        private float tissueBarrier;
        /// <summary> Alveolar partial pressure of oxygen [mmHg] (see <see cref="ParametersData"/>). </summary>
        private float pO2_alv;
        /// <summary> Partial pressure of oxygen in the blood [mmHg] (see <see cref="ParametersData"/>). </summary>
        private float pO2_blood;
        /// <summary> Alveolar partial pressure of carbom dioxide [mmHg] (see <see cref="ParametersData"/>). </summary>
        private float pCO2_alv;
        /// <summary> Partial pressure of carbon dioxide [mmHg] (see <see cref="ParametersData"/>). </summary>
        private float pCO2_blood;
        /// <summary> Atmospheric pressure [mmHg] (see <see cref="ParametersData"/>). </summary>
        private float atmosphericPressure;
        /// <summary>
        /// Respiratory exchange ratio or "respiratory quotient" (RQ) 
        /// </summary>
        /// <remarks>
        /// RQ is defined as the amount of carbon dioxide produced divided by the amount of oxygen consumed. This ratio is 
        /// assessed by analyzing exhaled air in comparison with the environmental air and its average value for the human diet (it 
        /// can vary depending upon the type of diet and metabolic state) is around 0.82 (Sharma et al., 2020).
        /// </remarks>
        private const float RQ = 0.82f;

        /// <summary>Length of a capillary section [µm], depends of the number of sections. </summary>
        private float sectionLength;

        /// <summary> Time period [s], in which blood resides in the same capillary section.</summary>
        /// <remarks> Calculated from the section length and blood flow velocity.
        /// This is of importance since diffusion is expressed as volume / time. </remarks>
        public float timePeriod { get; private set; }
        /// <summary> Volume of a capillary section, summed for the whole capillary network around an alveolus. </summary>
        private float sectionVolume;
        /// <summary> Alveolar surface area available for gas exchange into a capillary section, summed for the entire alveolus. </summary>
        private float sectionArea;

        /// <summary> Capillary partial pressure of oxygen per capillary section. </summary>
        /// <remarks> <c>partialPressures.Length</c> equals <see cref="numberSections"/> </remarks>
        public float[] partialPressures { get; private set; }
        /// <summary> The amount of oxygen [µm³] that diffuses into each section. </summary>
        /// <remarks> Results from oxygen flow [µm³/s] and the <see cref="timePeriod"/> [s] in which blood resides in a section. </remarks>
        private float[] o2GainPerSection;
        /// <summary> Capillary partial pressure of carbon dioxide per capillary section. </summary>
        /// <remarks> <c>co2partialPressures.Length</c> equals <see cref="numberSections"/> </remarks>
        public float[] co2partialPressures { get; private set; }
        /// <summary> The amount of carbon dioxide [µm³] that diffuses from one section into the alveolar space. </summary>
        /// <remarks> Derived from oxygen gain <see cref="o2GainPerSection"/> and the respiratory exchange ratio <see cref="RQ"/>. </remarks>
        private float[] co2GainPerSection;

        /// <summary> How many "spheres", i.e. simulation particles that represent a specific number of oxygen molecules,
        /// diffuse into a given section of the capillary each second. </summary>
        /// <remarks> <c>crossingSpheresO2.Length</c> equals <see cref="numberSections"/> </remarks>
        public float[] crossingSpheresO2 { get; private set; }
        /// <summary> How many "spheres", i.e. simulation particles that represent a specific number of carbon dioxide molecules,
        /// diffuse out of a given section of the capillary each second. </summary>
        /// <remarks> <c>crossingSpheresCO2.Length</c> equals <see cref="numberSections"/> </remarks>
        public float[] crossingSpheresCO2 { get; private set; }

        /// <summary> Volume of oxygen [µm³] represented by one sphere, needed for visualization. </summary>
        private float volumePerSphereO2;
        /// <summary> Volume of carbon dioxide [µm³] represented by one sphere, needed for visualization. </summary>
        private float volumePerSphereCO2;

        private ParametersData.UpdateValues updateHandler;
        
        protected override void HandleReset(AlveolusController instance)
        {
            Init();
        }

        /// <summary>
        /// Subscribe a method to be called whenever PartialPressure updates.
        /// </summary>
        /// <param name="method">Method to be called.</param>
        public void Subscribe(ParametersData.UpdateValues method)
        {
            updateHandler += method;
        }

        /// <summary>
        /// Unsubscribe a method from getting called on PartialPressure changes.
        /// </summary>
        /// <param name="method">Method to unsubscribe.</param>
        public void Unsubscribe(ParametersData.UpdateValues method)
        {
            updateHandler -= method;
        }


        /// <summary>
        /// Fetch required references and values, set up starting values.
        /// </summary>
        private void Init()
        {
            GetValues();
            CharacterizeSections();
            CalculatePressures();
            SphereScale();
            CrossingSpheres();
        }


        /// <summary>
        /// Recalculate partial pressures and derived values, then alert all subscribers to those updates. 
        /// </summary>
        protected override void HandleParametersUpdated()
        {
            GetValues();
            CharacterizeSections();
            CalculatePressures();
            CrossingSpheres();

            PropagateUpdates();
        }


        /// <summary>
        /// Retrieve relevant values set in the parameter panel.
        /// </summary>
        private void GetValues()
        {
            //*1000 to convert from mm/s to um/s
            bloodVelocity = Parameters.bloodFlowVelocity * 1000;
            pO2_alv = Parameters.alveolarPpO2;
            pO2_blood = Parameters.bloodPpO2;
            pCO2_alv = Parameters.alveolarPpCo2;
            pCO2_blood = Parameters.bloodPpCo2;
            atmosphericPressure = Parameters.atmosphericPressure;
            surfaceArea = Parameters.surfaceArea;
            tissueBarrier = Parameters.barrierThickness;
        }

        /// <summary>
        /// This method determines various values that affect a capillary section, but are the same for each section,
        /// primarily pertaining to blood flow through the capillary. 
        /// </summary>
        private void CharacterizeSections()
        {
            sectionLength = MeanCapLength / (float) numberSections;
            timePeriod = sectionLength / bloodVelocity;

            sectionVolume = volumeCapillaryNet / numberSections;
            sectionArea = surfaceArea / numberSections;
        }

        /// <summary>
        /// This method determines partial pressures along the capillary, as well as values directly related to or
        /// derived from the partial pressure, by calculating the diffusion of oxygen into the capillary sections and
        /// the consequential changes of pO2 sequentially.
        /// </summary>
        /// <remarks>
        /// At this point, diffusion of o2 out of the capillary and into the alveolus is not allowed, hence negative values for
        /// <see cref="o2Gain"/> are set to 0.
        /// </remarks>
        private void CalculatePressures()
        {
            partialPressures = new float[numberSections];
            partialPressures[0] = pO2_blood;
            co2partialPressures = new float[numberSections];
            co2partialPressures[0] = pCO2_blood;

            o2GainPerSection = new float[numberSections];
            co2GainPerSection = new float[numberSections];

            //Krogh’s permeability coefficient for oxygen [um²/(sec*mmHg)]
            const float PermCoefficientO2 = 0.055f;
            float o2Flow;
            float oldO2Pressure;
            float deltaPO2;
            float o2Gain;
            float maxO2Gain;


            float oldCO2pressure;
            float deltaPCO2;
            float co2Gain;
            float maxCO2Gain;

            for (int i = 1; i < numberSections; i++)
            {
                oldO2Pressure = partialPressures[(i - 1)];
                deltaPO2 = pO2_alv - oldO2Pressure;
                oldCO2pressure = co2partialPressures[(i - 1)];
                deltaPCO2 = oldCO2pressure - pCO2_alv;

                //determine maximal possible amount of gas that can diffuse under the respective pressure difference 
                // mass = partial pressure * volume / atmospheric pressure
                maxO2Gain = deltaPO2 * sectionVolume / atmosphericPressure;
                maxCO2Gain = deltaPCO2 * sectionVolume / atmosphericPressure;

                //calculation based on Fick's Law:
                //[um³/sec] = [um²/(sec*mmHg)] * [um] * [mmHg]
                o2Flow = PermCoefficientO2 * (sectionArea / tissueBarrier) * deltaPO2;
                o2Gain = timePeriod * o2Flow;
                co2Gain = o2Gain * RQ;

                if (o2Gain > maxO2Gain)
                {
                    o2Gain = maxO2Gain;
                }

                if (o2Gain < 0)
                {
                    o2Gain = 0;
                }

                o2GainPerSection[(i - 1)] = o2Gain;
                float newO2Pressure = oldO2Pressure + ((o2Gain / sectionVolume) * atmosphericPressure);
                partialPressures[i] = newO2Pressure;

                if (co2Gain > maxCO2Gain)
                {
                    co2Gain = maxCO2Gain;
                }

                if (co2Gain < 0)
                {
                    co2Gain = 0;
                }

                co2GainPerSection[(i - 1)] = co2Gain;
                float newCO2Pressure = oldCO2pressure - ((co2Gain / sectionVolume) * atmosphericPressure);
                co2partialPressures[i] = newCO2Pressure;
            }

            //gas flow into / out of last section
            oldO2Pressure = partialPressures[partialPressures.Length - 1];
            deltaPO2 = pO2_alv - oldO2Pressure;
            maxO2Gain = deltaPO2 * sectionVolume / atmosphericPressure;
            oldCO2pressure = co2partialPressures[co2partialPressures.Length - 1];
            deltaPCO2 = oldCO2pressure - pCO2_alv;
            maxCO2Gain = deltaPCO2 * sectionVolume / atmosphericPressure;

            o2Flow = PermCoefficientO2 * (sectionArea / tissueBarrier) * deltaPO2;
            o2Gain = timePeriod * o2Flow;
            co2Gain = o2Gain * RQ;

            if (o2Gain > maxO2Gain)
            {
                o2Gain = maxO2Gain;
            }

            if (o2Gain < 0)
            {
                o2Gain = 0;
            }

            o2GainPerSection[o2GainPerSection.Length - 1] = o2Gain;

            if (co2Gain > maxCO2Gain)
            {
                co2Gain = maxCO2Gain;
            }

            if (co2Gain < 0)
            {
                co2Gain = 0;
            }

            co2GainPerSection[co2GainPerSection.Length - 1] = co2Gain;
        }

        /// <summary>
        /// Converts the calculated gas flow into the representative number of particles drawn for visualization [number spheres / sec].
        /// </summary>
        /// <remarks>
        /// The amount of co2 that diffuses out of the capillary is determined via the respiratory exchange ratio (RQ).
        /// </remarks>
        private void CrossingSpheres()
        {
            crossingSpheresO2 = new float[numberSections];
            crossingSpheresCO2 = new float[numberSections];

            for (int i = 0; i < numberSections; i++)
            {
                float o2Volume = o2GainPerSection[i];
                float o2VolumeInSpheres = o2Volume / volumePerSphereO2;
                crossingSpheresO2[i] = o2VolumeInSpheres / (timePeriod * SpawnErythrocyte.sloMo);

                float co2Volume = co2GainPerSection[i];
                float co2VolumeInSpheres = co2Volume / volumePerSphereCO2;
                crossingSpheresCO2[i] = co2VolumeInSpheres / (timePeriod * SpawnErythrocyte.sloMo);
            }
        }

        /// <summary>
        /// Calculates what volume one rendered gas sphere represents respectively.
        /// </summary>
        /// <remarks>
        /// 
        /// <param name="volumeO2Alv"> Total volume of oxygen in an alveolus. Calculated from default values for alveolar
        /// partial pressure of oxygen (100 mmHg), atmospheric pressure (760 mmHg) and the mean volume of an alveolus (4,2*10**6 µm³, Ochs et al., 2004). </param>
        /// <param name="volumeCO2Alv"> Total volume of carbon dioxide in an alveolus. Calculated from default values for alveolar
        /// partial pressure of carbon dioxide (40 mmHg), atmospheric pressure (760 mmHg) and the mean volume of an alveolus (4,2*10**6 [µm³], Ochs et al., 2004). </param> 

        /// </remarks>
        public void SphereScale()
        {
            int totalNumParticles = 15000;
            float oxygenCount = Parameters.oxygenRatio * totalNumParticles;
            float co2Count = Parameters.co2Ratio * totalNumParticles;

            float volumeO2Alv = 552631.57f;
            float volumeCO2Alv = 221052.63f;


            volumePerSphereO2 = volumeO2Alv / oxygenCount;
            volumePerSphereCO2 = volumeCO2Alv / co2Count;
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