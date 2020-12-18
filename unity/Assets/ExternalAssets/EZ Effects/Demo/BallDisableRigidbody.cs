using UnityEngine;
using EZObjectPools;

public class BallDisableRigidbody : PooledObject 
{
    float timer = 0;
    public float DisableTime;

    Rigidbody r;

    void Awake()
    {
        r = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > DisableTime)
        {
            r.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
