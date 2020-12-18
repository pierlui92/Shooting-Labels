using UnityEngine;
using EZObjectPools;

[AddComponentMenu("EZ Object Pools/Pooled Objects/Timed Disable")]
public class TimedDisable : PooledObject
{
    float timer = 0;
    public float DisableTime;

    void OnEnable()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > DisableTime)
        {
            transform.parent = ParentPool.transform;
            gameObject.SetActive(false);
        }
    }
}
