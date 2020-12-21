using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineGunController : Weapons
{

    private ParticleSystem bullets;

    // Use this for initialization
    void Start()
    {
        Initialize();
        GameObject particleSystem = GameObject.Find("Particle System");
        bullets = particleSystem.GetComponent<ParticleSystem>();

        radiusOfFire = 5f;
        factorOfScale = 0.001f;
    }

    // Update is called once per frame
    void Update()
    {
        bool trigger = TriggerIsPressed();
        Animation(trigger);
        if (trigger)
        {
            Fire();
        }
    }

    private void Fire()
    {
            if (ShootRay() && !MenuLaserPointer.menuActive && !MenuLaserPointer.otherMenu)
            {
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
                }

            }
        
    }

    private void Animation(bool TriggerIsPressed)
    {
        if (bullets.isStopped && TriggerIsPressed)
        {
            bullets.Play();
        }

        if (bullets.isPlaying && !TriggerIsPressed)
        {
            bullets.Stop();
        }
    }

}
