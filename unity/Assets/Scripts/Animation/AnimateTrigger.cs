using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTrigger : MonoBehaviour {


    public GameObject controllerRight;
    private Animation anim;
    private SteamVR_TrackedController controller;

    // Use this for initialization
    void Start () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += TriggerPressed;
        anim = GetComponent<Animation>();
        Debug.Log(anim);
    }

    private void TriggerPressed(object sender, ClickedEventArgs e)
    {
        anim.Play("TriggerPush");
    }

    // Update is called once per frame
    void Update () {
       
    }
}
