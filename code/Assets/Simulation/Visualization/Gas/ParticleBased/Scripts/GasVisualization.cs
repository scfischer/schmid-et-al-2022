using Simulation.Systems;
using UnityEngine;

namespace Simulation.Alveolus.Visualization.GasVisualization
{
    /// <summary>
    /// Provides a particle-system based visualization system for the gas inside an alveolus.
    /// </summary>
    public class GasVisualization : AlveolusComponentBehavior
    {
        /// <summary> Particle system for carbon dioxide, to be assigned in the inspector. </summary>
        [SerializeField]
        private ParticleSystem carbonDioxideParticles;
        /// <summary> Particle system for nitrogen, to be assigned in the inspector. </summary>
        [SerializeField]
        private ParticleSystem nitrogenParticles;
        /// <summary> Particle system for oxygen, to be assigned in the inspector. </summary>
        [SerializeField]
        private ParticleSystem oxygenParticles;
        /// <summary> Total number of particles. </summary>
        [SerializeField]
        private int totalNumParticles = 15000;

        protected override void HandleReset(AlveolusController instance)
        {
            UpdateParticleSystems();
        }

        protected override void HandleParametersUpdated()
        {
            UpdateParticleSystems();
        }

        /// <summary>
        /// Updates the particle system configuration based on the oxygen, carbon dioxide and nitrogen ratio
        /// of the active <see cref="parameters"/> instance.
        /// </summary>
        private void UpdateParticleSystems()
        {
            var oxygenRatio = Parameters.oxygenRatio;
            var carbonDioxideRatio = Parameters.co2Ratio;
            var nitrogenRatio = 1 - oxygenRatio - carbonDioxideRatio;

            var normalization = oxygenRatio + carbonDioxideRatio + nitrogenRatio;
            oxygenRatio /= normalization;
            carbonDioxideRatio /= normalization;
            nitrogenRatio /= normalization;

            UpdateMaxNumParticles(carbonDioxideParticles, carbonDioxideRatio, totalNumParticles);
            UpdateMaxNumParticles(oxygenParticles, oxygenRatio, totalNumParticles);
            UpdateMaxNumParticles(nitrogenParticles, nitrogenRatio, totalNumParticles);
        }

        /// <summary>
        /// Reconfigure and restart a particle system with a maximum number of particles set to <paramref name="ratio"/> * <paramref name="maximumParticleCount"/>.
        /// </summary>
        /// <param name="system">The particle system to update</param>
        /// <param name="ratio">The ratio of particles relative to the maximum particle count.</param>
        /// <param name="maximumParticleCount">The maximum particle count.</param>
        private static void UpdateMaxNumParticles(ParticleSystem system, float ratio, int maximumParticleCount)
        {
            int maxParticles = Mathf.CeilToInt(ratio * maximumParticleCount);
            if (system.main.maxParticles == maxParticles)
                return;

            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = system.main;
            main.maxParticles = maxParticles;

            system.Play();
        }
    }
}