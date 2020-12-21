using IndiePixel.VR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoadColorsConfiguration : MonoBehaviour {

    //COSTANT
    public const string MENU5 = "RadialMenu_Canvas5Button";
    public const string MENU10 = "RadialMenu_Canvas10Button";
    public const string MENU15 = "RadialMenu_Canvas15Button";
    public const string MENU20 = "RadialMenu_Canvas20Button";
   
    //STATIC
    public static Dictionary<Color,int> categoriesFromRGB;
    public static Dictionary<int, Color> RGBFromCategories;
    public static GameObject[] radialMenuInstantiated;
    public static int realNumberOfRecords;
    public static string[] radialMenuNames;

    //PRIVATE
    private string pathCategoriesFile;
    private GameObject radialMenuButtons;
    private UnityEngine.Object radialMenuPrefab;
    private Transform[] children;
    private int numberOfRadialMenu20;
    private List<int> recordMissingToTheNextUnit;
    private int indexLine;
    private int realRecords;
    private int fileLength;
    private int idCategory;
    private string descriptionCategory;
    private float[] colorRGB;

    void Awake()
    {

        pathCategoriesFile = GetPath();
        recordMissingToTheNextUnit = new List<int>();
        categoriesFromRGB = new Dictionary<Color,int>();
        RGBFromCategories = new Dictionary<int, Color>();

        indexLine = 0;
       
        var textFileOfCategories = System.IO.File.ReadAllText(pathCategoriesFile); //Percorso del file da leggere
        var linesOfFiles = textFileOfCategories.Split('\n');
        linesOfFiles = linesOfFiles.Where((source, index) => index != 0).ToArray();

        colorRGB = new float[3];

        fileLength = linesOfFiles.Length;

        int totalNumberOfRadialMenu = 0;

        if ((fileLength ) <= 4) 
        {
          
            radialMenuInstantiated = new GameObject[1];        
            InstantiateMenu(5, 0, fileLength);
        }
        else if((fileLength) <= 9)
        {
          
            radialMenuInstantiated = new GameObject[1];      
            InstantiateMenu(10, 0, fileLength);
        }
        else if ((fileLength) <= 14)
        {
          
            radialMenuInstantiated = new GameObject[1];    
            InstantiateMenu(15, 0, fileLength);
        }
        else
        {
            numberOfRadialMenu20 = fileLength / 20; //numero Menu da 20 slice
            realRecords = fileLength % 20; //record in avanzo
            radialMenuInstantiated = new GameObject[numberOfRadialMenu20];

            //Se ho più di venti classi
            for (int i = 0; i < numberOfRadialMenu20; i++)
            {
            
                InstantiateMenu(20, i, 20);
                radialMenuInstantiated[i].name = radialMenuInstantiated[i].name + "_" + i;
            }

            //I restanti record
            if (realRecords > 0)
            {
                List<GameObject> listRadialMenu = radialMenuInstantiated.ToList<GameObject>();
                radialMenuInstantiated = new GameObject[numberOfRadialMenu20 + 1];
                for(int j=0;j<listRadialMenu.Count;j++)
                {
                    radialMenuInstantiated[j] = listRadialMenu[j];
                }

                if (realRecords <= 5)
                {
                    InstantiateMenu(5, numberOfRadialMenu20, realRecords);
                }
                else if (realRecords <= 10)
                {
                    InstantiateMenu(10, numberOfRadialMenu20, realRecords);
                }
                else if (realRecords <= 15)
                {
                    InstantiateMenu(15, numberOfRadialMenu20, realRecords);
                }
                else
                {
                    InstantiateMenu(20, numberOfRadialMenu20, realRecords);
                }

                totalNumberOfRadialMenu = numberOfRadialMenu20 + 1;

            }
            else
            {
                totalNumberOfRadialMenu = numberOfRadialMenu20;
            }
        }

        
        radialMenuNames = new string[totalNumberOfRadialMenu];
       
        for (int j = 0; j < totalNumberOfRadialMenu; j++)
        {
            radialMenuNames[j] = radialMenuInstantiated[j].name;
            children = radialMenuInstantiated[j].GetComponentsInChildren<Transform>();
            foreach (Transform go in children)
            {
                if (go.gameObject.name == "Button_GRP")
                {
                    radialMenuButtons = go.gameObject;
                    break;
                }

            }
          
            Image[] buttonImages = radialMenuButtons.GetComponentsInChildren<Image>();
            IP_VR_MenuButton[] buttonScript = radialMenuButtons.GetComponentsInChildren<IP_VR_MenuButton>();


            foreach (Image im in buttonImages)
            {
                im.color = Color.black;
            }

            realNumberOfRecords = buttonImages.Length - recordMissingToTheNextUnit[j];
            Dictionary<Color, int> categoriesFromRGBPartial = new Dictionary<Color, int>(); 
            Dictionary<int, Color> RGBFromCategoriesPartial = new Dictionary<int, Color>();
            
            for (int i = 0; i < realNumberOfRecords; i++)
            {
                
                string line = linesOfFiles[indexLine];
                string[] lineData = line.Trim().Split(',');
                if (lineData.Length == 5)
                {

                    ExtractRecordFromFile(lineData);

                    Color labelColor = new Color(colorRGB[0] / 255, colorRGB[1] / 255, colorRGB[2] / 255, 1);
                  
                    buttonImages[i].color = labelColor;
                    buttonScript[i].buttonText = descriptionCategory;

                    categoriesFromRGBPartial.Add(labelColor, idCategory);
                    Color labelColor2 = labelColor;
                    RGBFromCategoriesPartial.Add(idCategory, labelColor2);
                }
                indexLine++;
            }

            UnifyColorsTable(categoriesFromRGBPartial);
            UnifyColorsTableInverse(RGBFromCategoriesPartial);

        }
        HideIdleMenu();

    }

    private void ExtractRecordFromFile(string[] lineData)
    {
        int.TryParse(lineData[0], out idCategory);
        descriptionCategory = lineData[1];
        float.TryParse(lineData[2], out colorRGB[0]);
        float.TryParse(lineData[3], out colorRGB[1]);
        float.TryParse(lineData[4], out colorRGB[2]);
    }


    private void UnifyColorsTable(Dictionary<Color, int> source)
    {
        foreach (Color key in source.Keys)
        {
          
           categoriesFromRGB.Add(key, source[key]);
            
        }
    }

    private void UnifyColorsTableInverse(Dictionary<int, Color> source)
    {
        foreach (int key in source.Keys)
        {
            RGBFromCategories.Add(key, source[key]);
        }
    }



    private void HideIdleMenu()
    {
        for (int i = 0; i < radialMenuInstantiated.Length; i++)
        {
            if (i > 0)
            {
                radialMenuInstantiated[i].SetActive(false);

            }
        }
    }

    private void InstantiateMenu(int sizeOfMenu,int indexOfRadialMenu,int records)
    {
        radialMenuPrefab = AssetDatabase.LoadAssetAtPath("Assets/ExternalAssets/Radial_Menu/Prefabs/RadialMenu_Canvas" + sizeOfMenu + "Button.prefab", typeof(GameObject));
        radialMenuInstantiated[indexOfRadialMenu] = (GameObject)Instantiate(radialMenuPrefab, gameObject.transform);
        radialMenuInstantiated[indexOfRadialMenu].GetComponent<IP_VR_RadialMenu>().controller = gameObject.GetComponent<SteamVR_TrackedController>();
        recordMissingToTheNextUnit.Add(sizeOfMenu - records);
    }

    private string GetPath()
    {
        return Application.dataPath + "/../../resources/LabelMappings/ColorConfiguration.csv";
    }
}