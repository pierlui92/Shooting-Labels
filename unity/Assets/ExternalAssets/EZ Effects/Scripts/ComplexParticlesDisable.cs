using UnityEngine;
using EZObjectPools;

namespace EZEffects
{
    [AddComponentMenu("EZ Effects/Complex Particles Disable")]
    public class ComplexParticlesDisable : PooledObject
    {
        public ComplexParticles Particles;

        void Awake()
        {
            if (Particles == null)
                Particles = GetComponentInChildren<ComplexParticles>();
        }

        void OnEnable()
        {
            if (Particles == null)
            {
                Debug.LogError("ParticlesComplexDisable " + gameObject.name + " could not find any particle systems!");
            }
        }

        void Update()
        {
            if (!Particles.IsAlive())
            {
                transform.parent = ParentPool.transform;
                gameObject.SetActive(false);
            }
        }
    }
}