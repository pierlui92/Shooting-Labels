using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisualizationFacesHidden : MonoBehaviour {
    void Start()
    { }
    //public GameObject controllerLeft;
    //public Dictionary<int, int> originalToHidden;

    //private GameObject[] meshInScene;
    //private SteamVR_TrackedController controller;
    //private Mesh meshToShow;
    //private GameObject facesHidden;
    //private Transform transformOfHiddenMesh;
    //private bool hiddenViewActive;
    //private GameObject gameObjectOfMeshOriginal;
   
    //// Use this for initialization
    //void Start () {
    //    originalToHidden = new Dictionary<int, int>();
    //    controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
    //    controller.Gripped += ShowFacesUnlabeled;
    //    int children = GameObject.Find("LoadMeshFromScript").transform.childCount;
    //    meshToShow = new Mesh();
    //    meshInScene = new GameObject[children];
       
    //    for (int i = 0; i < children; ++i)
    //    {
    //        meshInScene[i] = GameObject.Find("LoadMeshFromScript").transform.GetChild(i).gameObject;
    //        CopyMesh(meshInScene[i].GetComponent<MeshFilter>().mesh);
    //        transformOfHiddenMesh = meshInScene[i].transform;
    //    }

    //    facesHidden = null;
    //    hiddenViewActive = false;


    //}

    //void CopyMesh(Mesh meshInScene)
    //{
    //    meshToShow.vertices = meshInScene.vertices;
    //    meshToShow.triangles = meshInScene.triangles;
    //    meshToShow.uv = meshInScene.uv;
    //    meshToShow.normals = meshInScene.normals;
    //    meshToShow.colors = meshInScene.colors;
    //    meshToShow.tangents = meshInScene.tangents;
    //}

    ////public Dictionary<int,int> GetMappingFaces()
    ////{
    ////    return originalToHidden;
    ////}

    //private void ShowFacesUnlabeled(object sender, ClickedEventArgs e)
    //{
    //    originalToHidden = new Dictionary<int, int>();
    //    int countHiddenFaces = 0;
    //    if (!hiddenViewActive)
    //    {
    //        foreach (GameObject current in meshInScene)
    //        {
    //            countHiddenFaces = 0;
    //            MeshFilter meshCurrent = current.GetComponent<MeshFilter>();
    //            int[] faces = LoadMesh.faceLabel[current.name];

    //            List<Vector3> listOfVertices = new List<Vector3>();
    //            List<Color> listOfColors = new List<Color>();
    //            List<int> listOfTriangles = new List<int>();
                
    //            for (int i = 0; i < meshCurrent.mesh.triangles.Length / 3; i++)
    //            {
    //                if (faces[i] == 0)
    //                {
    //                    int v1 = meshCurrent.mesh.triangles[i * 3 + 0];
    //                    int v2 = meshCurrent.mesh.triangles[i * 3 + 1];
    //                    int v3 = meshCurrent.mesh.triangles[i * 3 + 2];


    //                    listOfTriangles.Add(v1);
    //                    listOfTriangles.Add(v2);
    //                    listOfTriangles.Add(v3);

    //                    //i=original index, countHiddenFace: new index
    //                    originalToHidden.Add(countHiddenFaces, i);
    //                    countHiddenFaces++;

    //                }

    //            }


    //            meshToShow.triangles = listOfTriangles.ToArray();

    //            //only the first time
    //            if (facesHidden == null)
    //            {
    //                Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Material/Test 1.mat");
    //                facesHidden = new GameObject("faceHidden");
                    
    //                facesHidden.transform.position = transformOfHiddenMesh.position;
    //                facesHidden.transform.rotation = transformOfHiddenMesh.rotation;
    //                facesHidden.AddComponent<MeshFilter>();
    //                facesHidden.AddComponent<MeshRenderer>().material = mat;

    //                //Add LODMehses
    //                LODMeshes lodMehses = facesHidden.AddComponent<LODMeshes>() as LODMeshes;
    //                lodMehses.highPolyMesh = meshToShow;
    //                lodMehses.mediumPolyMesh = meshToShow;
    //                lodMehses.lowPolyMesh = meshToShow;
    //                lodMehses.distanceLOD1 = 50f;
    //                lodMehses.distanceLOD2 = 100f;

    //                //Add Collider
    //                MeshCollider meshCollider = facesHidden.AddComponent<MeshCollider>();
    //                meshCollider.sharedMesh = meshToShow;

    //                //Change Layer to Drawable
    //                facesHidden.layer = LayerMask.NameToLayer("Drawable");

    //                gameObjectOfMeshOriginal = transformOfHiddenMesh.gameObject;
    //            }
               
    //            facesHidden.SetActive(true);          
    //            facesHidden.GetComponent<MeshFilter>().mesh = meshToShow;
               
              
                
    //            foreach (GameObject inScene in meshInScene)
    //            {
    //                inScene.SetActive(false);
    //            }
                
    //            hiddenViewActive = true;
    //        }
    //    }
    //    else
    //    {
    //        //Join with original meshes



    //        facesHidden.SetActive(false);
    //        foreach (GameObject inScene in meshInScene)
    //        {
    //            inScene.SetActive(true);
    //        }
    //        hiddenViewActive = false;
    //    }
       
    //}

    //public GameObject GetGameObjectCollider()
    //{
    //    return gameObjectOfMeshOriginal;
    //}

    //// Update is called once per frame
    void Update () {
		
	}
}