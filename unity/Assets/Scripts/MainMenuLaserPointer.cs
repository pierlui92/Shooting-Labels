using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLaserPointer : MonoBehaviour
{

    public static int numberofsaves = 0;
    public GameObject controllerRight;
    public LayerMask menuMask; // Mask to filter out areas where menu button 
    public GameObject laserPrefab; // The laser prefab
    public static bool load;
    public bool pointcloud = true;

    // FIELD FOR POINTCLOUD MANAGER
    private float scale = 1;
    private bool invertYZ = true;
    private bool forceReload = false;
    private bool rightToLeftHand = true;
    public int numPoints;
    public int numPointGroups;

    private GameObject prefab;
    private SteamVR_TrackedObject trackedObjRight;
    private GameObject laser; // A reference to the spawned laser
    private Transform laserTransform; // The transform component of the laser for ease of use
    private SteamVR_Controller.Device deviceRight;

    public static bool Tutorial = false;
    public static bool NewSession = false;

    // Use this for initialization
    void Start()
    {
        createFolders();
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        LoadUserConfiguration();
        trackedObjRight = controllerRight.GetComponent<SteamVR_TrackedObject>();
        deviceRight = SteamVR_Controller.Input((int)trackedObjRight.index);
        load = false;
        var directoryInfo = new DirectoryInfo("Assets/Data/Saves");
        numberofsaves = directoryInfo.GetDirectories().Length;
        MeshController.singleton.meshesEmpty.SetActive(false);
        MeshController.singleton.meshesRGB.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 1000, menuMask))
        {
            ShowLaser(hit);
            if (deviceRight.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                if (hit.collider.name == "LoadLastSave")
                {
                    NewSession = false;
                    Tutorial = false;

                    MeshController.singleton.meshesRGB.SetActive(false);
                    MeshController.singleton.meshesEmpty.SetActive(true);
                    MenuOperation.LoadColorFile();
                    SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
                }
                else if (hit.collider.name == "NewSession")
                {
                    NewSession = true;
                    Tutorial = false;
                    load = false;

                    SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
                }
                else if (hit.collider.name == "LoadNewMesh")
                {
                    NewSession = false;
                    Tutorial = false;
                    LoadNewMesh();

                }
                else if (hit.collider.name == "Quit")
                {
                    Application.Quit();
                }
                else if (hit.collider.name == "Tutorial")
                {
                    NewSession = false;
                    Tutorial = true;

                    SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
                }

            }
        }
        else
        {
            laser.SetActive(false);
        }

    }

    private void LoadNewMesh()
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/Test 1.mat");
        Debug.Log(mat);
        string folderPath = Application.dataPath + "/Resources/Models/Meshes/Empty/";
        string folderPathRGB = Application.dataPath + "/Resources/Models/Meshes/RGB/";

        int countFiles = 0;

        //BLANK
        foreach (string file in Directory.GetFiles(folderPath, "*.blend"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string prefabPath = "Assets/Resources/Prefabs/EmptyMeshes/" + fileName + ".prefab";
            string localPath = "Assets/Resources/Models/Meshes/Empty/" + fileName + ".blend";

            //Check if mesh contains LOD
            LODGroup lodGroup = AssetDatabase.LoadAssetAtPath<LODGroup>(localPath);

            if (lodGroup != null)
            {

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
                GameObject prefabInstatiated = Instantiate(prefab);
                //Destroy(prefabInstatiated.GetComponent<LODGroup>());

                MeshFilter[] meshFilterOfPrefab = prefabInstatiated.GetComponentsInChildren<MeshFilter>();

                Mesh[] meshes = new Mesh[meshFilterOfPrefab.Length];
                Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(localPath);
                int countMesh = 0;
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i] is Mesh)
                    {
                        meshes[countMesh] = objs[i] as Mesh;
                        countMesh++;
                    }

                }

                GameObject newPrefab = new GameObject(prefabInstatiated.name);
                Transform oldTransform = prefabInstatiated.transform.GetChild(1);
                newPrefab.transform.position = oldTransform.position;
                newPrefab.transform.rotation = oldTransform.rotation;
                Destroy(prefabInstatiated);

                if (oldTransform != null && meshFilterOfPrefab != null)
                {
                    //Add Material to mesh renderer
                    newPrefab.AddComponent<MeshRenderer>();
                    newPrefab.GetComponent<Renderer>().material = mat;

                    //Add VisualizationFacesHidden
                    // newPrefab.AddComponent<VisualizationFacesHidden>();

                    //Add Collider
                    MeshCollider meshCollider = newPrefab.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshes[0];

                    //Add LODMehses                
                    LODMeshes lodMehses = newPrefab.AddComponent<LODMeshes>() as LODMeshes;
                    lodMehses.highPolyMesh = meshes[0];
                    lodMehses.mediumPolyMesh = meshes[1];
                    lodMehses.lowPolyMesh = meshes[2];
                    lodMehses.distanceLOD1 = 50f;
                    lodMehses.distanceLOD2 = 100f;

                    //Mesh Filter
                    newPrefab.GetComponent<MeshFilter>().mesh = meshes[0];

                    //Change Layer to Drawable
                    newPrefab.layer = LayerMask.NameToLayer("Drawable");

                    //GameObject newPrefabInstatiated = Instantiate(newPrefab);
                    Object prefabObject = PrefabUtility.CreatePrefab(prefabPath, newPrefab);
                    Destroy(newPrefab);
                }
            }
            else
            {
                //Empty Mesh
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
                GameObject prefabInstatiated = Instantiate(prefab);
                Mesh meshOfPrefab = AssetDatabase.LoadAssetAtPath<Mesh>(localPath);
                if (prefab != null && meshOfPrefab != null)
                {
                    //Add Material to mesh renderer

                    prefabInstatiated.GetComponent<Renderer>().material = mat;

                    //Add LODMehses

                    LODMeshes lodMehses = prefabInstatiated.AddComponent<LODMeshes>() as LODMeshes;
                    lodMehses.highPolyMesh = meshOfPrefab;
                    lodMehses.mediumPolyMesh = meshOfPrefab;
                    lodMehses.lowPolyMesh = meshOfPrefab;
                    lodMehses.distanceLOD1 = 50f;
                    lodMehses.distanceLOD2 = 100f;

                    //Add VisualizationFacesHidden
                    //prefabInstatiated.AddComponent<VisualizationFacesHidden>();

                    //Add Collider
                    MeshCollider meshCollider = prefabInstatiated.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshOfPrefab;

                    //Change Layer to Drawable
                    prefabInstatiated.layer = LayerMask.NameToLayer("Drawable");

                    Object prefabObject = PrefabUtility.CreatePrefab(prefabPath, prefabInstatiated);
                    Destroy(prefabInstatiated);
                }
            }
            countFiles++;
        }

        //RGB
        if (!pointcloud)
        {
            foreach (string file in Directory.GetFiles(folderPathRGB, "*.blend"))
            {
                string fileNameRGB = Path.GetFileNameWithoutExtension(file);
                string prefabPathRGB = "Assets/Resources/Prefabs/RGBMeshes/" + fileNameRGB + ".prefab";
                string localPathRGB = "Assets/Resources/Models/Meshes/RGB/" + fileNameRGB + ".blend";

                GameObject prefabRGB = AssetDatabase.LoadAssetAtPath<GameObject>(localPathRGB);
                prefabRGB.GetComponent<Renderer>().material = mat;

                GameObject prefabInstatiatedRGB = Instantiate(prefabRGB);
                Mesh meshOfPrefabRGB = AssetDatabase.LoadAssetAtPath<Mesh>(localPathRGB);
                if (prefabRGB != null && meshOfPrefabRGB != null)
                {
                    //Add Material to mesh renderer                
                    prefabInstatiatedRGB.GetComponent<Renderer>().material = mat;

                    //Add LODMehses

                    LODMeshes lodMehsesRGB = prefabInstatiatedRGB.AddComponent<LODMeshes>() as LODMeshes;
                    lodMehsesRGB.highPolyMesh = meshOfPrefabRGB;
                    lodMehsesRGB.mediumPolyMesh = meshOfPrefabRGB;
                    lodMehsesRGB.lowPolyMesh = meshOfPrefabRGB;
                    lodMehsesRGB.distanceLOD1 = 50f;
                    lodMehsesRGB.distanceLOD2 = 100f;

                    //Add Collider
                    MeshCollider meshCollider = prefabInstatiatedRGB.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshOfPrefabRGB;

                    ////Add VisualizationFacesHidden
                    //prefabInstatiatedRGB.AddComponent<VisualizationFacesHidden>();

                    //Change Layer to Drawable
                    prefabInstatiatedRGB.layer = LayerMask.NameToLayer("Teleportable");

                    Object prefabObjectRGB = PrefabUtility.CreatePrefab(prefabPathRGB, prefabInstatiatedRGB);
                    Destroy(prefabInstatiatedRGB);
                }
            }
        }
        else
        {
            createMeshFromPointcloud();
        }
    }

    private void createMeshFromPointcloud()
    {

        Material matVertex = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/VertexColor.mat");
        string[] files = Directory.GetFiles(Application.dataPath + "/Resources/Models/Meshes/RGB/");
        foreach (string f in files)
        {
            string fTmp = f.Replace('\\', '/');
            fTmp = fTmp.Substring(Application.dataPath.Length + 1, fTmp.Length - Application.dataPath.Length - 1);
            if (f.EndsWith(".ply"))
            {
                fTmp = fTmp.Remove(fTmp.Length - 4);
                PointCloudManager pcd = new PointCloudManager(fTmp, matVertex, scale, invertYZ, forceReload, numPoints, numPointGroups, rightToLeftHand);
            }
        }
    }

    private void createFolders()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

        if (!Directory.Exists(Application.dataPath + "/Resources/Prefabs/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "Prefabs");

        if (!Directory.Exists(Application.dataPath + "/Resources/Prefabs/RGBMeshes"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Prefabs", "RGBMeshes");

        if (!Directory.Exists(Application.dataPath + "/Resources/Prefabs/EmptyMeshes"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Prefabs", "EmptyMeshes");
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); //Show the laser

        laserTransform.position = Vector3.Lerp(trackedObjRight.transform.position, hit.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hit.point); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }

    private void LoadUserConfiguration()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Resources/config.txt");
        string line;
        int found = 0;

        while ((line = file.ReadLine()) != null)
        {
            if (!string.Equals("#", line.Substring(0, 1)))
            {
                found = line.IndexOf(", ");
                switch (line.Substring(0, found))
                {
                    case "pointcloud":
                        this.pointcloud = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                        break;

                    case "scale":
                        this.scale = float.Parse(line.Substring(found + 2, line.Length - found - 2));
                        break;

                    case "invertYZ":
                        this.invertYZ = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                        break;

                    case "forceReload":
                        this.forceReload = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                        break;

                    case "rightToLeftHand":
                        this.rightToLeftHand = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                        break;

                    default:
                        Debug.Log("incorrect configuration file format");
                        break;

                }
            }
        }

        file.Close();
    }

}
