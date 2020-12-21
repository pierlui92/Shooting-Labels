using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuLaserPointer : MonoBehaviour {


    public GameObject controllerLeft;
    public GameObject controllerRight;
    public LayerMask menuMask; // Mask to filter out areas where menu button 
    public GameObject laserPrefab; // The laser prefab

    private GameObject laser; // A reference to the spawned laser
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedObject trackedObjRight;

    private SteamVR_TrackedController controller;
    private SteamVR_TrackedController controllerRightHand;

    private SteamVR_Controller.Device device;
    private SteamVR_Controller.Device deviceRight;

    private Transform laserTransform; // The transform component of the laser for ease of use
    private Vector3 hitPoint; // Point where the raycast hits
    public static bool menuActive;
    public static bool otherMenu;
    private GameObject menu;

    public static GameObject meshEmpty;
    public static GameObject meshRGB;

    // Use this for initialization
    void Start () {
         //= ChangeVisualization.meshEmpty; // GameObject.Find("LoadMeshFromScript");
        //meshRGB = ChangeVisualization.meshRGB;  //GameObject.Find("LoadMeshRGBFromScript");
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        trackedObjRight = controllerRight.GetComponent<SteamVR_TrackedObject>();

        device = SteamVR_Controller.Input((int)trackedObj.index);
        deviceRight = SteamVR_Controller.Input((int)trackedObjRight.index);

        controllerRightHand = controllerRight.GetComponent<SteamVR_TrackedController>();
        //controllerRightHand.TriggerClicked += TriggerClicked;

        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += MenuButton;
        menuActive = false;
        otherMenu = false;
        menu = GameObject.Find("MenuButton");
        menu.SetActive(false);
        FileBrowser.HideDialog();

    }

    private void TriggerClicked()
    {
        
        RaycastHit hit;

        // Send out a raycast from the controller
        if ((Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 100, menuMask)) && menuActive)
        {
            hitPoint = hit.point;
            GameObject buttonMenu = GameObject.Find(hit.transform.name);
            //Debug.Log(buttonMenu.name);

           
            menuActive = false;
            menu.SetActive(false);
            MenuController.OpenMenu(buttonMenu.name,hit,otherMenu);

            otherMenu = true;
            ShowLaser(hit);

        }
        else if ((Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 100, menuMask)) && otherMenu)
        {
            
            menuActive = true;
            menu.SetActive(true);
            MenuController.OpenMenu(hit.transform.name, hit, otherMenu);
            otherMenu = false;
        }
        else
        {
            laser.SetActive(false);
        }
         
    }

    private void MenuButton(object sender, ClickedEventArgs e)
    {

        GameObject faceHidden = GameObject.Find("faceHidden");
        if (faceHidden == null)
        {
            if (menuActive == false && otherMenu == false)
            { 
                if(ChangeVisualization.getlastscene() == 1) ChangeVisualization.meshEmpty.SetActive(false);
                if(ChangeVisualization.getlastscene() == 2) ChangeVisualization.meshRGB.SetActive(false);
                menuActive = true;
                menu.SetActive(true);
            }
            else
            {
                menuActive = false;
                menu.SetActive(false);
                laser.SetActive(false);
                if(ChangeVisualization.getlastscene() == 1) ChangeVisualization.meshEmpty.SetActive(true);
                if(ChangeVisualization.getlastscene() == 2) ChangeVisualization.meshRGB.SetActive(true);
                
            }
        }
    }

// Update is called once per frame
    void Update()
    {
       
        if (menuActive == true || otherMenu == true)
        {
            
            RaycastHit hit;
            if (Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 100, menuMask))
            {
                ShowLaser(hit);
                if (deviceRight.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                {
                    TriggerClicked();
                }
            }
            else
            {
                laser.SetActive(false);
            }
        }
        else
        {
           
            laser.SetActive(false);
        }
        
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); //Show the laser

        laserTransform.position = Vector3.Lerp(trackedObjRight.transform.position, hit.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hit.point); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }

}