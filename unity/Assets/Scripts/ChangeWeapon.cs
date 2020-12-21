using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour {

    public GameObject[] weapons;
    public GameObject controllerRight;

    private int currentWeapon;
    private int nextWeapon;
    private SteamVR_TrackedController controller;

    // Use this for initialization
    void Start () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.Gripped += NextWeapon;
        currentWeapon = 0;
        for(int i = 1; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
    }

    private void NextWeapon(object sender, ClickedEventArgs e)
    {
        
        nextWeapon = currentWeapon + 1;

        if (nextWeapon < weapons.Length)
        {
            EnableWeapon();
            currentWeapon++;
        }
        else{

            currentWeapon = 0;
            nextWeapon = 0;
            EnableWeapon();
        }
    }

    private void EnableWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == nextWeapon)
            {
                weapons[i].SetActive(true);
            }
            else
            {
                weapons[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
