using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class MenuController{
    public static GameObject meshEmpty;
    public static GameObject meshRGB;
   
    public static void OpenMenu(string ButtonName,RaycastHit hit, bool otherMenu)
    {        
        if (otherMenu == false)
        {
            if (ButtonName == "ButtonSave")
            {
                ChangeVisualization.meshEmpty.SetActive(true);
                ChangeVisualization.meshRGB.SetActive(false);
                FileBrowser.SingleClickMode = true;
                FileBrowser.ShowSaveDialog(null, null, false, Application.dataPath + "/Data/Saves/", "Save", "Save");
            }
            else if (ButtonName == "ButtonLoad")
            {
                FileBrowser.SingleClickMode = true;
                FileBrowser.ShowSaveDialog(null, null, false, Application.dataPath + "/Data/Saves/", "Load", "Load");
            }
            else if (ButtonName == "ButtonExport")
            {
                ChangeVisualization.meshEmpty.SetActive(true);
                ChangeVisualization.meshRGB.SetActive(false);
                FileBrowser.SingleClickMode = true;
                FileBrowser.ShowSaveDialog(null, null, false, Application.dataPath + "/Data/Exported/", "Export", "Export");
            }
            else if (ButtonName == "ButtonMenu")
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
        }
        else
        {
            if (hit.collider.transform.name == "CancelButton")
            {
                FileBrowser.HideDialog(false);
                ChangeVisualization.meshEmpty.SetActive(false);
                ChangeVisualization.meshRGB.SetActive(false);
            }
            else if (hit.collider.transform.name == "SubmitButton")
            {
                GameObject submit = GameObject.Find("SubmitButton");
                string text = submit.GetComponentInChildren<Text>().text;
                if(text == "Save") 
                {
                    MenuOperation.SaveColorFile();
                    ChangeVisualization.meshEmpty.SetActive(false);
                }
                else if(text == "Load") 
                {
                    MenuOperation.LoadColorFile();
                }
                else if(text == "Export")
                {
                    MenuOperation.ExportPLY();
                    ChangeVisualization.meshEmpty.SetActive(false);
                }
                FileBrowser.HideDialog(false);
            }           
        }
    }
}