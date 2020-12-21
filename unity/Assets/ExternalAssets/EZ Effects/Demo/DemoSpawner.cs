using UnityEngine;
using EZObjectPools;

public class DemoSpawner : MonoBehaviour 
{
    public EZObjectPool objectPool;
    public GameObject template;

    Vector3 position;
    Quaternion rotation;
    GameObject g;
    Rigidbody r;

    void Start()
    {
        objectPool = EZObjectPool.CreateObjectPool(template, "Spawner", 600, false, true, true);
    }

	void Update() 
    {
        if (objectPool.TryGetNextObject(transform.position + Random.insideUnitSphere * 0.1f, transform.rotation, out g))
        {
            r = g.GetComponent<Rigidbody>();
            r.AddForce(transform.up * 450f + Random.insideUnitSphere * 50f);
        }
	}
}
