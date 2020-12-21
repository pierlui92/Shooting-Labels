using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TeleportController : MonoBehaviour {

    public GameObject laserPrefab; // The laser prefab
    public GameObject teleportReticlePrefab; // Stores a reference to the teleport reticle prefab.
    public Transform cameraRigTransform;
    public Transform headTransform; // The camera rig's head
    public GameObject controllerRight;
    public LayerMask teleportMask; // Mask to filter out areas where teleports are allowed
    public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
    public Transform muzzleTransform;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private GameObject laserTeleport; // A reference to the spawned laser
    private Transform laserTransform; // The transform component of the laser for ease of use
    private Transform teleportReticleTransform; // Stores a reference to the teleport reticle transform for ease of use
    private GameObject reticle; // A reference to an instance of the reticle
    private RaycastHit hit;
    private float distanceOfTeleport;
    private Vector3 hitPoint;
    private bool shouldTeleport; // True if there's a valid teleport target

    private void Start()
    {
        laserTeleport = Instantiate(laserPrefab);
        laserTeleport.transform.parent = transform;
        laserTransform = laserTeleport.transform;
        reticle = Instantiate(teleportReticlePrefab);
        //reticle.transform.parent = transform;
        //reticle.transform.position = new Vector3(0,0,0);
        teleportReticleTransform = reticle.transform;
        distanceOfTeleport = 100;
        reticle.SetActive(false);

        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    private void Update()
    {
        Teleport();
    }

 

    private void Teleport()
    {
        if (PadIsPressing())
        {
            
            // Send out a raycast from the controller
            if (ShootRay())
            {
                hitPoint = hit.point;
                
                ShowLaser(hit);

                //Show teleport reticle
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                shouldTeleport = true;
            }
            else
            {
                laserTeleport.SetActive(false);
                reticle.SetActive(false);
                shouldTeleport = false;
            }
            

            // Touchpad released this frame & valid teleport position found
           
        }
        else // Touchpad not held down, hide laser & teleport reticle
        {
            laserTeleport.SetActive(false);
            reticle.SetActive(false);
        }

        if (!PadIsPressing() && shouldTeleport)
        {

            EffectivelyTeleport();
        }
    }

    private void EffectivelyTeleport()
    {
        shouldTeleport = false; // Teleport in progress, no need to do it again until the next touchpad release
        reticle.SetActive(false); // Hide reticle
        Vector3 difference = cameraRigTransform.position - headTransform.position; // Calculate the difference between the center of the virtual room & the player's head
        difference.y = 0; // Don't change the final position's y position, it should always be equal to that of the hit point

        cameraRigTransform.position = hitPoint + difference; // Change the camera rig position to where the the teleport reticle was. Also add the difference so the new virtual room position is relative to the player position, allowing the player's new position to be exactly where they pointed. (see illustration)
    }

    private bool PadIsPressing()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsUp()
    {
        return (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad));
    }

    private void ShowLaser(RaycastHit hit)
    {
        laserTeleport.SetActive(true); //Show the laser

        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }

    private bool ShootRay()
    {
        return Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, distanceOfTeleport, teleportMask);
    }
}
