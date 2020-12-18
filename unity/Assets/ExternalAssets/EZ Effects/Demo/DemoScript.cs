using UnityEngine;
using EZObjectPools;
using EZEffects;

public class DemoScript : MonoBehaviour {

    public static EZObjectPool[] pools;

    public Camera MainCamera;
    public AudioSource Audio;
    public GameObject Cannon;

    public EffectImpact ImpactEffect;
    public EffectMuzzleFlash MuzzleEffect;
    public EffectTracer TracerEffect;

    void Awake()
    {
        ImpactEffect.SetupPool();
        MuzzleEffect.SetupPool();
        TracerEffect.SetupPool();
    }

	// Update is called once per frame
	void Update () 
    {
        pools = FindObjectsOfType<EZObjectPool>();

        Ray r = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (Physics.Raycast(r, out h, Mathf.Infinity))
        {
            Cannon.transform.LookAt(h.point);

            if (Input.GetMouseButtonDown(0))
            {
                fireCannon(h.point, h.normal, h.distance);
            }
        }
        else
        {
            Vector3 targetPos = r.origin + r.direction * 100;
            Cannon.transform.LookAt(targetPos);

            if (Input.GetMouseButtonDown(0))
            {
                fireCannon(targetPos, Vector3.zero, 100);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    void fireCannon(Vector3 hitPos, Vector3 hitNormal, float distance)
    {
        MuzzleEffect.ShowMuzzleEffect(Cannon.transform, true, Audio);
        TracerEffect.ShowTracerEffect(Cannon.transform.position, Cannon.transform.forward, distance);

        if(hitNormal != Vector3.zero)
            ImpactEffect.ShowImpactEffect(hitPos, hitNormal);

        Collider[] colliders = Physics.OverlapSphere(hitPos, 2f);

        foreach (Collider c in colliders)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();

            if (rb)
            {
                rb.AddExplosionForce(600f, hitPos, 2f);
            }
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 250, 50 + pools.Length * 35), "Object Pool List");
        GUILayout.BeginArea(new Rect(29f, 40, 215, Screen.height - 40));

        foreach (EZObjectPool e in pools)
        {
            GUILayout.Label(e.PoolName + ": Size - " + e.PoolSize + ", " + e.AvailableObjectCount() + " available objects. ");
        }

        GUILayout.EndArea();
    }
}
