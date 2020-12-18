using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class onHover : UnityEvent<int>{}
public class onClick : UnityEvent<int>{}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class IP_VR_RadialMenu : MonoBehaviour 
{
    #region Variables
    [Header("Controller Properties")]
    public SteamVR_TrackedController controller;
    private SteamVR_TrackedController controllerLeft;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    [Header("UI Properties")]
    public List<IP_VR_MenuButton> menuButtons = new List<IP_VR_MenuButton>();
    public RectTransform m_ArrowContainer;
    public Text m_DebugText;

    [Header("Events")]
    public UnityEvent OnMenuChanged = new UnityEvent();


    private Vector2 currentAxis;
    private SteamVR_Controller.Device controllerDevice;
    private Animator animator;

    public bool menuOpen = false;
    private bool allowNavigation = false;
    public bool isTouching = false;
    private float currentAngle;

    private int currentMenuID = -1;
    private int previousMenuID = -1;

    private onHover OnHover = new onHover();
    private onClick OnClick = new onClick();
    public bool triggered = false;

    private GameObject radialMenuButtons;
    private Image[] images;

  
    

    // Use this for initialization
    void Start () 
    {
        controllerLeft = controller.GetComponent<SteamVR_TrackedController>();
        animator = GetComponent<Animator>();
        trackedObj = controller.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);

   

        if (controller)
        {
            controllerDevice = SteamVR_Controller.Input((int)controller.controllerIndex);
        }

        if(menuButtons.Count > 0)
        {
            foreach(var button in menuButtons)
            {
                OnHover.AddListener(button.Hover);
                OnClick.AddListener(button.Click);
            }
        }


        radialMenuButtons = FindObject(gameObject,"Button_GRP");
        images = radialMenuButtons.GetComponentsInChildren<Image>();
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



    void OnDisable()
    {
        if(controller)
        {
            controller.PadTouched -= HandlePadTouched;
            controller.PadUntouched -= HandlePadUnTouched;     
            controller.PadClicked -= HandlePadClicked;
            controller.PadUnclicked -= HandlePadUnClicked;          
        }

    }
    	
    // Update is called once per frame
    void Update () 
    {
        if(controllerDevice != null)
        {
          
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
                triggered = true;
            
                HandleDebugText("Menu is: " + menuOpen);

                HandleDebugText("Clicked Pad");
                if (OnClick != null)
                {
                    OnClick.Invoke(currentMenuID);
                    //Debug.Log(currentMenuID);

                    menuOpen = !menuOpen;
                    HandleAnimator();

                }

               

            }            
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                triggered = false;
                menuOpen = !menuOpen;
                HandleAnimator();
                
            }
          

            if(triggered)
            {

                
                HandleAnimator();
                HandleDebugText("Menu is: " + menuOpen);

                HandleDebugText("Clicked Pad");
                if (OnClick != null)
                {
                    OnClick.Invoke(currentMenuID);
                    //Debug.Log(currentMenuID);

                }
                UpdateMenu();
            }
           
        }
    }
    #endregion




    #region Custom Methods
    void HandlePadTouched(object sender, ClickedEventArgs e)
    {
        isTouching = true;
        HandleDebugText("Touched Pad");
    }

    void HandlePadUnTouched(object sender, ClickedEventArgs e)
    {
        isTouching = false;
        HandleDebugText("Un Touched Pad");
        
          
            
    }

    void HandlePadClicked(object sender, ClickedEventArgs e)
    {
       
        isTouching = true;
        HandleDebugText("Menu is: " + menuOpen);

        HandleDebugText("Clicked Pad");
        if(OnClick != null)
        {
            OnClick.Invoke(currentMenuID);
            //Debug.Log(currentMenuID);
            
            menuOpen = !menuOpen;
            HandleAnimator();

        }
    }

 
    void HandleAnimator()
    {
        if(animator)
        {
            animator.SetBool("open", menuOpen);
        }
    }

    void HandlePadUnClicked(object sender, ClickedEventArgs e)
    {
        isTouching = false;
        HandleDebugText("Un Clicked Pad");

        menuOpen = !menuOpen;
        HandleAnimator();
    }


    void UpdateMenu()
    {
       
        currentAxis = controllerDevice.GetAxis();
        currentAngle = Vector2.SignedAngle(Vector2.up, currentAxis);

        float menuAngle = currentAngle;
        if (menuAngle < 0)
        {
            menuAngle += 360f;
        }
      
        int updateMenuID = (int)(menuAngle / (360f / images.Length));
        updateMenuID = images.Length - updateMenuID - 1;
        int index = menuButtons[updateMenuID].buttonID;
        HandleDebugText(menuButtons[index].buttonText);

        //Update Current Menu ID
        if (updateMenuID != currentMenuID)
        {
            if (OnHover != null)
            {
                OnHover.Invoke(updateMenuID);
            }

            if (OnMenuChanged != null)
            {
                OnMenuChanged.Invoke();
            }

            previousMenuID = currentMenuID;
            currentMenuID = updateMenuID;

            for (int i = 0; i < menuButtons.Count; i++)
            {
                IP_VR_MenuButton button = menuButtons[i];
                if (updateMenuID == button.buttonID)
                {
                    ChangeColorWeapons(i);
                    ChangeColorSelectedWeaponCanvas(i);
                }

            }
        }


        //Rotate Arrow
        if (m_ArrowContainer)
        {

            m_ArrowContainer.localRotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
        }
    //}
           
    }

    void ChangeColorSelectedWeaponCanvas(int i)
    {
       
        GunController.colorSelected.GetComponent<Image>().color = images[i].color;
        LaserGunController.colorSelected.GetComponent<Image>().color = images[i].color;
        MachineGunController.colorSelected.GetComponent<Image>().color = images[i].color;
        BazookaController.colorSelected.GetComponent<Image>().color = images[i].color;
    }

    void ChangeColorWeapons(int i)
    {
        LaserGunController.current_color = images[i].color;
        GunController.current_color = images[i].color;
        MachineGunController.current_color = images[i].color;
        BazookaController.current_color = images[i].color;
    }

    void HandleDebugText(string aString)
    {
        if(m_DebugText)
        {
            m_DebugText.text = aString;
        }
    }
    #endregion
}

