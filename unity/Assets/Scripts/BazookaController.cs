using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaController : Weapons
{ 
   
    public GameObject laserPrefab; // The laser prefab
      
    private GameObject rayOfFire; // A reference to the spawned laser
      
    private Transform fireTransform; // The transform component of the laser for ease of use

    // Use this for initialization
    void Start()
    {
        Initialize();

        rayOfFire = Instantiate(laserPrefab);
        rayOfFire.transform.parent = transform;
        fireTransform = rayOfFire.transform;

        radiusOfFire = 50;
        factorOfScale = 0.001f;
}

    // Update is called once per frame
    void Update()
    {

       
        if (TriggerIsPressed())
        {
            Fire();
        }
        else
        {
            rayOfFire.SetActive(false);
        }
    }

    private void Fire()
    {
        
            if (ShootRay() && !MenuLaserPointer.menuActive && !MenuLaserPointer.otherMenu)
            {
                hitPoint = hit.point;
                Renderer fire_render = rayOfFire.GetComponent<Renderer>();
                fire_render.material.SetColor("_Color", current_color);
                MeshCollider meshCollider = hit.collider as MeshCollider;
                GameObject meshHits;
                meshHits = GameObject.Find(hit.collider.name);
                MeshFilter meshFilter = meshHits.GetComponent<MeshFilter>();
                Mesh highPoly = meshFilter.mesh;

                if (meshCollider != null || meshCollider.sharedMesh != null)
                {
                    ChangeFactorOfScale(highPoly);
                    ColorMesh(highPoly, hit, current_color, meshHits, true);
                    SaveColorTemporary(meshHits);
                    ShowFire(hit);
                }
            }
    }

    private void ShowFire(RaycastHit hit)
    {
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(muzzleTrasform.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F, hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }
}
