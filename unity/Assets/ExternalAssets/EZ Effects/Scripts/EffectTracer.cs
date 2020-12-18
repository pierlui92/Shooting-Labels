using UnityEngine;
using EZObjectPools;

namespace EZEffects
{
    public class EffectTracer : ScriptableObject
    {
        /// <summary>
        /// The template GameObject to use for this effect.
        /// </summary>
        public ParticleSystem Template;
        /// <summary>
        /// The speed of the tracer.
        /// </summary>
        public float Speed = 100f;
        /// <summary>
        /// The chance the effect has of showing.
        /// </summary>
        public float ChanceToShow = 0.33f;
        /// <summary>
        /// If false, tracers will be randomly displayed based on ChanceToShow. If true, tracers will display consistently based on ChanceToShow.
        /// </summary>
        public bool ConsistentShowing;
        /// <summary>
        /// The size of the object pool.
        /// </summary>
        public int PoolSize = 50;

        EZObjectPool tracerEffectPool;
        int currCount;
        int countLimit;

        /// <summary>
        /// Sets up this effect's object pool. The pool will only be set up on the first call to this method, so calling it multiple times is safe.
        /// </summary>
        public void SetupPool()
        {
            if (tracerEffectPool == null)
            {
                tracerEffectPool = EZObjectPool.CreateObjectPool(Template.gameObject, Template.name, PoolSize, false, true, true);

                if (ConsistentShowing)
                {
                    countLimit = (int)(1 / ChanceToShow);
                    currCount = 1;
                }
            }
        }

        /// <summary>
        /// Shows the tracer effect starting at the given transform and traveling the given distance.
        /// </summary>
        /// <param name="origin">The position the tracer should start at.</param>
        /// <param name="direction">The direction the tracer should travel.</param>
        /// <param name="distance">How far the tracer should travel.</param>
        public void ShowTracerEffect(Vector3 origin, Vector3 direction, float distance)
        {
            if (tracerEffectPool == null)
            {
                SetupPool();
            }

            if (ConsistentShowing)
            {
                if (currCount == countLimit)
                {
                    doEffect(origin, direction, distance);
                    currCount = 0;
                }

                currCount++;
            }
            else
            {
                if (Random.value <= ChanceToShow)
                {
                    doEffect(origin, direction, distance);
                }
            }
        }

        private void doEffect(Vector3 pos, Vector3 dir, float dist)
        {
            GameObject g;
            if (tracerEffectPool.TryGetNextObject(pos, Quaternion.LookRotation(dir), out g))
            {
                ParticleSystem p = g.GetComponent<ParticleSystem>();
                p.startSpeed = Speed;
                p.startLifetime = dist / Speed;
            }
        }
    }
}