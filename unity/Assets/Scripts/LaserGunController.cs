using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunController : Weapons
{

    public TextMesh powerText;
    public GameObject laserPrefab; // The laser prefab
    public GameObject targetPrefab;

    private GameObject rayOfFire; // A reference to the spawned laser
    private GameObject targetOfFire;
    private Transform fireTransform; // The transform component of the laser for ease of use

    private Vector2 padScrolling;


    private float xtemp = float.MaxValue;  //variable for the function ModifiyRadius
    private float ytemp = float.MaxValue;  //variable for the function ModifiyRadius  
    private double angle = 0F;
    private float magnification = 1.5F;
    private float deltaX = 0F;
    private float deltaY = 0F;


    // Use this for initialization
    void Start () {
        Initialize();

        targetOfFire = Instantiate(targetPrefab);
        targetOfFire.transform.parent = transform;
        rayOfFire = Instantiate(laserPrefab);
        rayOfFire.transform.parent = transform;
        fireTransform = rayOfFire.transform;

        radiusOfFire = 1;
        factorOfScale = 0.001f;

        padScrolling = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {

            ModifiyRadius();

            RaycastHit costantRay;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out costantRay, distanceOfShoot, shootableMask))
            {
                ShowLaserTarget(costantRay);
                if (TriggerIsPressed())
                {
                    Fire();
                }
            }
            else
            {
                rayOfFire.SetActive(false);
                targetOfFire.SetActive(false);
            }

    }

    public void Fire()
    {
        if (ShootRay() && !MenuLaserPointer.menuActive && !MenuLaserPointer.otherMenu)
        {
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
                    ColorMesh(highPoly, hit, current_color, meshHits, radiusOfFire > 1);
                    SaveColorTemporary(meshHits);
                    ShowFire(hit);
                }
            }
        }
    }

    private void RegisterAction(Mesh meshToRegister)
    {
        //Save the precedent situation to perform Undo action
        DataHit.Instance.CurrentIndex = 0;

        if (DataHit.Instance.OccupyASpace())
        {
            if (!DataHit.Instance.MeshName.Contains(hit.collider.name))
            {
                DataHit.Instance.MeshName.Add(hit.collider.name);
                DataHit.Instance.ColorPrec.Add(meshToRegister.colors);
                DataHit.Instance.TimeOrder.Add(DataHit.Instance.CurrentTime);
                DataHit.Instance.CurrentTime++;
                DataHit.Instance.FreeSpace--;
            }
            else
            {
                DataHit.Instance.UpdateArrayColor(hit.collider.name, meshToRegister.colors);
            }
           
        }
        else
        {
            DataHit.Instance.Remove(0);
            //Shift left
            ShiftLeft(DataHit.Instance.MeshName, 1);
            ShiftLeft(DataHit.Instance.ColorPrec, 1);

            if (!DataHit.Instance.MeshName.Contains(hit.collider.name))
            {
                DataHit.Instance.MeshName.Add(hit.collider.name);
                DataHit.Instance.ColorPrec.Add(meshToRegister.colors);
                DataHit.Instance.TimeOrder.Add(DataHit.Instance.CurrentTime);
                DataHit.Instance.CurrentTime++;
                DataHit.Instance.FreeSpace--;
            }
            else
            {
                DataHit.Instance.UpdateArrayColor(hit.collider.name, meshToRegister.colors);
            }
        
        }
        
    }

    private void ShiftLeft<T>(List<T> lst, int shifts)
    {
        for (int i = shifts; i < lst.Count; i++)
        {
            lst[i - shifts] = lst[i];
        }  
    }

    private void ShowFire(RaycastHit hit)
    {
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point

    }

    private void ShowLaserTarget(RaycastHit target)
    {
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(trackedObj.transform.position, target.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(target.point); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F,
            target.distance);

        targetOfFire.SetActive(true);
        targetOfFire.transform.position = target.point;
        targetOfFire.transform.LookAt(trackedObj.transform.position); // Rotate laser facing the hit point
        targetOfFire.transform.localScale = new Vector3(0.01f * radiusOfFire, 0.01f * radiusOfFire, targetOfFire.transform.localScale.z);
    }

    protected override bool ShootRay()
    {
        return Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, distanceOfShoot, shootableMask);
    }

    private bool PadIsPressing()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsRelasing()
    {
        return device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsTouching()
    {
        return device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private void ModifiyRadius()
    {

        if (PadIsRelasing())
        {
            xtemp = float.MaxValue;
            ytemp = float.MaxValue;
        }

        if (!PadIsPressing() && PadIsTouching())
        {
            padScrolling = device.GetAxis();

            if (xtemp == float.MaxValue && ytemp == float.MaxValue)
            {
                xtemp = padScrolling.x;
                ytemp = padScrolling.y;
            }
            else
            {

                angle = Math.Atan2(padScrolling.y, padScrolling.x);
                deltaX = xtemp - padScrolling.x;
                deltaY = ytemp - padScrolling.y;

                if (angle < Math.PI / 8 && angle >= -Math.PI / 8)
                {
                    if (deltaY > 0)
                    {
                        radiusOfFire += deltaY * magnification;
                    }
                    if (deltaY < 0) radiusOfFire -= Math.Abs(deltaY) * magnification;
                }
                if (angle < 3 * Math.PI / 8 && angle >= Math.PI / 8)
                {
                    if ((deltaX < 0) && (deltaY > 0))
                    {
                        radiusOfFire += deltaY * magnification;
                    }
                    if ((deltaX > 0) && (deltaY < 0)) radiusOfFire -= Math.Abs(deltaY) * magnification;
                }
                if (angle < 5 * Math.PI / 8 && angle >= 3 * Math.PI / 8)
                {
                    if (deltaX < 0)
                    {
                        radiusOfFire += Math.Abs(deltaX) * magnification;
                    }
                    if (deltaX > 0) radiusOfFire -= deltaX * magnification;
                }
                if (angle < 7 * Math.PI / 8 && angle >= 5 * Math.PI / 8)
                {
                    if ((deltaX < 0) && (deltaY < 0))
                    {
                        radiusOfFire += Math.Abs(deltaY) * magnification;
                    }
                    if ((deltaX > 0) && (deltaY > 0)) radiusOfFire -= deltaY * magnification;
                }
                if (angle < -7 * Math.PI / 8 || angle >= 7 * Math.PI / 8)
                {
                    if (deltaY < 0)
                    {
                        radiusOfFire += Math.Abs(deltaY) * magnification;
                    }
                    if (deltaY > 0) radiusOfFire -= (deltaY) * magnification;
                }
                if (angle < -5 * Math.PI / 8 && angle >= -7 * Math.PI / 8)
                {
                    if ((deltaX > 0) && (deltaY < 0))
                    {
                        radiusOfFire += Math.Abs(deltaX) * magnification;
                    }
                    if ((deltaX < 0) && (deltaY > 0)) radiusOfFire -= Math.Abs(deltaX) * magnification;
                }
                if (angle < -3 * Math.PI / 8 && angle >= -5 * Math.PI / 8)
                {
                    if (deltaX > 0)
                    {
                        radiusOfFire += deltaX * magnification;
                    }
                    if (deltaX < 0) radiusOfFire -= Math.Abs(deltaX) * magnification;
                }
                if (angle < -Math.PI / 8 && angle >= -3 * Math.PI / 8)
                {
                    if ((deltaX > 0) && (deltaY > 0))
                    {
                        radiusOfFire += deltaX * magnification;
                    }
                    if ((deltaX < 0) && (deltaY < 0)) radiusOfFire -= Math.Abs(deltaX) * magnification;
                }

                if (radiusOfFire < 1.0) radiusOfFire = 1.0F;
                if (radiusOfFire > 20) radiusOfFire = 20.0f;

                if (powerText != null)
                {
                    powerText.text = radiusOfFire.ToString();
                }
            }
        }

        xtemp = padScrolling.x;
        ytemp = padScrolling.y;
    }

}
