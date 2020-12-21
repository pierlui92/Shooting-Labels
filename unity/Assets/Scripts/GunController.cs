using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZEffects;

public class GunController : Weapons
{
    public EffectTracer TracerEffect;

    // Use this for initialization
    void Start()
    {
        Initialize();
        radiusOfFire = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerIsPressed())
        {
            Animation();
            Fire();
        }

    }

    private void Animation()
    {
        device.TriggerHapticPulse(750);
        TracerEffect.ShowTracerEffect(muzzleTrasform.position, muzzleTrasform.forward, 250f);

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
                    ColorMesh(highPoly, hit, current_color, meshHits, false);
                    SaveColorTemporary(meshHits);
                }

            }
    }

    protected override bool TriggerIsPressed()
    {
        // Single shot weapon.
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }
}