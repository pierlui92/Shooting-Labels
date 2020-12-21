using UnityEngine;

namespace EZEffects
{
    [AddComponentMenu("EZ Effects/Complex Particles")]
    public class ComplexParticles : MonoBehaviour
    {
        /// <summary>
        /// An optional light to fade in and out with the particle systems.
        /// </summary>
        public ComplexLight Light;
        /// <summary>
        /// Should the particle systems keep looping when disabled? This shoudl be false when used with EffectImpact or EffectMuzzleFlash.
        /// </summary>
        public bool KeepLoop = false;

        ParticleSystem[] children;

        void Awake()
        {
            if (children == null)
                children = GetComponentsInChildren<ParticleSystem>();

            if (children.Length == 0)
            {
                Debug.LogError("ParticlesComplex " + gameObject.name + " could not find any children particle systems!");
            }
        }

        /// <summary>
        /// Enables or disables the emission of particles based on the enable value.
        /// </summary>
        /// <param name="enable">True to enable, False to disable.</param>
        public void EnableEmissions(bool enable)
        {
            if (children == null)
                children = GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem p in children)
            {
                p.loop = enable || KeepLoop;
                p.enableEmission = enable;
            }

            if (Light)
            {
                if (enable)
                    Light.FadeIn();
                else
                    Light.FadeOut();
            }
        }

        /// <summary>
        /// Checks if any of the child particle systems are still emitting or have particles that are still active.
        /// </summary>
        /// <returns>True if any child particle system is alive, False otherwise.</returns>
        public bool IsAlive()
        {
            if (children == null)
                children = GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem p in children)
            {
                if (p.IsAlive())
                    return true;
            }

            return false;
        }
    }
}