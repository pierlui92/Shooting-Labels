using UnityEngine;
using EZObjectPools;

[AddComponentMenu("EZ Object Pool/Pooled Objects/Particle System Disable")]
public class ParticleSystemDisable : PooledObject 
{
    public ParticleSystem Particles;

    void Awake()
    {
        if(Particles == null)
            Particles = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        if (Particles == null)
        {
            Debug.LogError("ParticleSystemDisable " + gameObject.name +  " could not find any particle systems!");
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
