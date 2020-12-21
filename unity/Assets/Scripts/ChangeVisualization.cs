using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVisualization : MonoBehaviour {

    public GameObject controllerRight;

    private SteamVR_TrackedController controller;
    public static GameObject meshRGB;
    public static GameObject meshEmpty;
    private static int lastscene = 1; //1 is for empty 2 is for RGB

    // Use this for initialization
    void Start () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += VisualizeRGB;
        if (MainMenuLaserPointer.Tutorial)
        {
            MeshController.singleton.meshesEmpty.SetActive(false);
            MeshController.singleton.meshesRGB.SetActive(false);
            meshEmpty = GameObject.Find("MeshesEmpty");
            meshRGB = GameObject.Find("MeshesRGB");
            meshRGB.SetActive(false);
            meshEmpty.SetActive(true);
        }
        else
        {
            meshRGB = MeshController.singleton.meshesRGB;
            meshEmpty = MeshController.singleton.meshesEmpty;
            meshRGB.SetActive(false);
            meshEmpty.SetActive(true);
        }
    }

    private void VisualizeRGB(object sender, ClickedEventArgs e)
    {
        if (!MenuLaserPointer.menuActive)
        {
            if (meshRGB.activeSelf)
            {
                lastscene = 1;
                meshRGB.SetActive(false);
                meshEmpty.SetActive(true);
            }
            else
            {
                lastscene = 2;
                meshRGB.SetActive(true);
                meshEmpty.SetActive(false);
            }
        }
    }

    public static int  getlastscene()
    {
        return lastscene;
    }
   
}
