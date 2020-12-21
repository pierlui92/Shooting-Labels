using UnityEngine;
using EZObjectPools;

namespace EZEffects
{
    public class EffectMuzzleFlash : ScriptableObject
    {
        /// <summary>
        /// The template GameObject to use for this effect.
        /// </summary>
        public GameObject Template;
        /// <summary>
        /// An optional list of sounds to play.
        /// </summary>
        public AudioClip[] Sounds;
        /// <summary>
        /// The size of the object pool.
        /// </summary>
        public int PoolSize = 50;

        EZObjectPool muzzleEffectPool;

        /// <summary>
        /// Sets up this effect's object pool. The pool will only be set up on the first call to this method, so calling it multiple times is safe.
        /// </summary>
        public void SetupPool()
        {
            if (muzzleEffectPool == null)
            {
                muzzleEffectPool = EZObjectPool.CreateObjectPool(Template.gameObject, Template.name, PoolSize, false, true, true);
            }
        }

        /// <summary>
        /// Shows the muzzle effect at the given transform.
        /// </summary>
        /// <param name="origin">The position and direction info for the effect.</param>
        /// <param name="parentToTransform">Should the effect get parented to the given transform?</param>
        /// <param name="audioSource">An optional audio source to play the sound effects from.</param>
        public void ShowMuzzleEffect(Transform origin, bool parentToTransform, AudioSource audioSource = null)
        {
            doEffect(origin.position, origin.forward, parentToTransform ? origin : null, audioSource);
        }

        /// <summary>
        /// Shows the muzzle effect using the given position and direction.
        /// </summary>
        /// <param name="origin">The position the muzzle effect should be spawned at.</param>
        /// <param name="direction">The direction the muzzle effect should face when spawned.</param>
        /// <param name="parentTo">If this parameter is not null, the effect will be parented to the given transform.</param>
        /// <param name="audioSource">An optional audio source to play the sound effects from.</param>
        public void ShowMuzzleEffect(Vector3 origin, Vector3 direction, AudioSource audioSource = null, Transform parentTo = null)
        {
            doEffect(origin, direction, parentTo, audioSource);
        }

        private void doEffect(Vector3 pos, Vector3 dir, Transform parent, AudioSource audio)
        {
            if (muzzleEffectPool == null)
            {
                SetupPool();
            }

            GameObject g;

            if (muzzleEffectPool.TryGetNextObject(pos, Quaternion.LookRotation(dir), out g))
            {
                if (parent != null)
                    g.transform.parent = parent;
            }

            if (audio)
            {
                if (Sounds.Length < 0)
                {
                    Debug.LogWarning("EffectMuzzleFlash " + name + " is given an audio source but has no sounds to play!");
                    return;
                }

                audio.PlayOneShot(Sounds[Random.Range(0, Sounds.Length - 1)]);
            }
        }
    }
}