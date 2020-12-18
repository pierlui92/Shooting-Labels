using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class MeshController : MonoBehaviour
{

    private bool pointcloud = true;
    public GameObject meshesRGB, meshesEmpty;

    protected string[] emptyNameFiles;
    protected string[] rgbNameFiles;

    protected int numberOfChuncks;
    public static Dictionary<string, int[]> faceLabel;
    public static MeshController singleton = null;
    public List<string> meshName = new List<string>();
    //public MeshModified meshesModified;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadUserConfiguration();
        if (MeshController.singleton == null)
        {
            singleton = this;
            meshesEmpty = GameObject.Find("MeshesEmpty");
            meshesRGB = GameObject.Find("MeshesRGB");
            faceLabel = new Dictionary<string, int[]>();
            InstatiatePrefabs();
            InstatiatePrefabs(true);
            if (pointcloud)
            {
                GameObject.Find("MeshesRGB").transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public int GetIndexByName(string name)
    {
        if (meshName.Contains(name))
        {
            for (int i = 0; i < meshName.Count; i++)
            {
                if (meshName[i] == name)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public int Count()
    {
        return meshName.Count;
    }

    public void InstatiatePrefabs(bool isRGB = false)
    {
        if (isRGB)
        {
            rgbNameFiles = getListFiles("/Resources/Prefabs/RGBMeshes/");
            for (int i = 0; i < rgbNameFiles.Length; i++)
            {
                string nameFile = rgbNameFiles[i].Substring(Application.dataPath.Length + "Resources".Length + 2);
                nameFile = nameFile.Split('.')[0];
                UnityEngine.Object pPrefab = Resources.Load(nameFile);
                LoadPrefab(i, pPrefab, true);
            }
        }
        else
        {
            UnityEngine.Object[] emptyPrefabs = Resources.LoadAll("Prefabs/EmptyMeshes/");
            for (int i = 0; i < emptyPrefabs.Length; i++)
            {
                LoadPrefab(i, emptyPrefabs[i], false);
            }
        }
    }

    private void LoadPrefab(int i, UnityEngine.Object pPrefab, bool isRgb = false)
    {
        GameObject pNewObject = (GameObject)GameObject.Instantiate(pPrefab);

        if (isRgb)
        {
            pNewObject.transform.parent = meshesRGB.transform;
            pNewObject.SetActive(true);
        }
        else
        {
            pNewObject.transform.parent = meshesEmpty.transform;

            GameObject controllerLeft = GameObject.Find("Controller (left)");
            if (pNewObject.transform.childCount == 0)
            {
                Mesh region = pNewObject.GetComponent<LODMeshes>().highPolyMesh;
                int[] faceLabelArray = new int[region.triangles.Length / 3];
                faceLabel.Add(pNewObject.name, faceLabelArray);
            }
            else if (pNewObject.transform.childCount > 0)
            {
                for (int j = 0; j < pNewObject.transform.childCount; j++)
                {
                    GameObject child = pNewObject.transform.GetChild(j).gameObject;
                    Mesh region = child.GetComponent<LODMeshes>().highPolyMesh;
                    int[] faceLabelArray = new int[region.triangles.Length / 3];
                    faceLabel.Add(child.name, faceLabelArray);
                }
            }
        }
    }

    protected string[] getListFiles(String s)
    {
        string[] nameFiles = Directory.GetFiles(Application.dataPath + s, "*.prefab");
        return nameFiles;
    }

    private void LoadUserConfiguration()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Resources/config.txt");
        string line;
        int found = 0;

        while ((line = file.ReadLine()) != null)
        {
            found = line.IndexOf(", ");
            if (!string.Equals("#", line.Substring(0, 1)))
            {
                if (string.Equals("pointcloud", line.Substring(0, found)))
                {
                    this.pointcloud = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                }
            }
        }

        file.Close();
    }
}

