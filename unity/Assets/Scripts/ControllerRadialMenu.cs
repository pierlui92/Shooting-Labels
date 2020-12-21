using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRadialMenu : MonoBehaviour {

    private SteamVR_TrackedController controller;
    private bool padClicked;
    public static bool triggered = false;
    public GameObject controllerLeft;

    private GameObject[] radialMenus;
    private List<IP_VR_RadialMenu> radialMenuHandler;

    // Use this for initialization
    void Start () {
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
      
        if (LoadColorsConfiguration.radialMenuNames.Length > 1)
            controller.TriggerClicked += TriggerClicked;

        padClicked = false;
        radialMenus = LoadColorsConfiguration.radialMenuInstantiated;
        radialMenuHandler = new List<IP_VR_RadialMenu>();
        foreach(GameObject rm in radialMenus)
        {
            radialMenuHandler.Add(rm.GetComponent<IP_VR_RadialMenu>());
        }
    }


    private void TriggerClicked(object sender, ClickedEventArgs e)
    {
        if (controller.padPressed)
        {
           
            int numRadialMenu = LoadColorsConfiguration.radialMenuNames.Length;
            for(int i = 0; i < numRadialMenu; i++)
            {
                GameObject radialMenu = FindObject(gameObject, LoadColorsConfiguration.radialMenuNames[i]);
                if (radialMenu.activeSelf)
                {

                    radialMenu.GetComponent<IP_VR_RadialMenu>().triggered = false;                 
                    radialMenuHandler[i].menuOpen = !radialMenuHandler[i].menuOpen;
                    radialMenu.SetActive(false);
                    i = i + 1;
                    if(i >= numRadialMenu)
                    {
                       
                        GameObject radialMenuActive = FindObject(gameObject, LoadColorsConfiguration.radialMenuNames[0]);
                        radialMenuActive.SetActive(true);
                        radialMenuHandler[0].triggered = true;
                        radialMenuHandler[0].menuOpen = !radialMenuHandler[0].menuOpen;
                       

                        break;
                    }
                    else
                    {
                        GameObject radialMenuActive = FindObject(gameObject, LoadColorsConfiguration.radialMenuNames[i]);
                        radialMenuActive.SetActive(true);
                        radialMenuHandler[i].menuOpen = !radialMenuHandler[i].menuOpen;
                        radialMenuHandler[i].triggered = true;
                      
                        break;
                    }
                    
                    
                }
            }

            triggered = true;
        }
        else
        {
            triggered = false;
        }
    }



    private GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }


 
    // Update is called once per frame
    void Update () {
		
	}
}
