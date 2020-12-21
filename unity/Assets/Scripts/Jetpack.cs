using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {


    public GameObject controllerLeft;

    private SteamVR_TrackedObject trackedObjLeft;
    private SteamVR_TrackedController controller;
    private SteamVR_Controller.Device deviceLeft;
    private GameObject player;

    // Use this for initialization
    void Start () {

        trackedObjLeft = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        deviceLeft = SteamVR_Controller.Input((int)trackedObjLeft.index);
        player = GameObject.Find("[CameraRig]");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControllerRadialMenu.triggered)
        {
            if (deviceLeft.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                player.transform.position += new Vector3(0, 0.03f, 0);

            }
        }
       
    }
}
