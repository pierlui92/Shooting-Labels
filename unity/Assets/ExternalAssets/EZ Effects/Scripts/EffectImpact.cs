using UnityEngine;
using EZObjectPools;

namespace EZEffects
{
    public class EffectImpact : ScriptableObject
    {
        /// <summary>
        /// The template GameObject to use for this effect.
        /// </summary>
        public GameObject Template;
        /// <summary>
        /// An optional list of sounds to play, only if Template has an AudioSource component attached to the main GameObject.
        /// </summary>
        public AudioClip[] Sounds;
        /// <summary>
        /// The size of the object pool.
        /// </summary>
        public int PoolSize = 50;

        EZObjectPool impactEffectPool;

        /// <summary>
        /// Sets up this effect's object pool. The pool will only be set up on the first call to this method, so calling it multiple times is safe.
        /// </summary>
        public void SetupPool()
        {
            if (impactEffectPool == null)
            {
                impactEffectPool = EZObjectPool.CreateObjectPool(Template, Template.name, PoolSize, false, true, true);
            }
        }

        /// <summary>
        /// Shows the effect at the given position and rotates it to match the given normal. Plays a sound if Template has an audio source component.
        /// </summary>
        public void ShowImpactEffect(Vector3 position, Vector3 normal)
        {
            doEffect(position, Quaternion.LookRotation(normal));
        }

        /// <summary>
        /// Shows the effect at the given position. Plays a sound if Template has an audio source component.
        /// </summary>
        public void ShowImpactEffect(Vector3 position)
        {
            doEffect(position, Quaternion.identity);
        }

        private void doEffect(Vector3 pos, Quaternion rot)
        {
            if (impactEffectPool == null)
            {
                SetupPool();
            }

            GameObject g;

            if (impactEffectPool.TryGetNextObject(pos, rot, out g))
            {
                AudioSource audioSource = g.GetComponent<AudioSource>();

                if (Sounds.Length > 0)
                {
                    if (!audioSource)
                    {
                        Debug.LogWarning("EffectImpact " + name + " has sounds defined but the template has no audio source!");
                        return;
                    }

                    audioSource.PlayOneShot(Sounds[Random.Range(0, Sounds.Length)]);
                }
            }
        }
    }
}