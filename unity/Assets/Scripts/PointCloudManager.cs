using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PointCloudManager : MonoBehaviour
{

    // File
    private string dataPath;
    private string filename;
    private Material matVertex;

    private bool loaded = false;

    // PointCloud
    private GameObject pointCloud;

    private float scale = 1;
    private bool invertYZ = false;
    private bool forceReload = false;
    private bool rightToLeftHand = false;

    private int numPoints;
    private int numPointGroups;
    private int limitPoints = 65000;

    private Vector3[] points;
    private Color[] colors;
    private Vector3 minValue;

    public void Start()
    {
        
    }

    public PointCloudManager(string dataPath, Material matVertex, float scale, bool invertYZ, bool forceReload,int numPoints, int numPointGroups, bool rightToLeftHand)
    {
        filename = Path.GetFileName(dataPath);
        this.matVertex = matVertex;
        this.dataPath = dataPath;
        this.scale = scale;
        this.invertYZ = invertYZ;
        this.forceReload = forceReload;
        this.numPoints = numPoints;
        this.numPoints = numPointGroups;
        this.rightToLeftHand = rightToLeftHand;
        loadScene();
    }

    void loadScene()
    {
        // Check if the PointCloud was loaded previously
        if (!Directory.Exists(Application.dataPath + "/Resources/Prefabs/RGBMeshes/" + filename))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Prefabs/RGBMeshes" , filename);
            loadPointCloud();
        }
        else if (forceReload)
        {
            UnityEditor.FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/Prefabs/RGBMeshes/" + filename);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Prefabs/RGBMeshes", filename);
            loadPointCloud();
        }
        else
            loadStoredMeshes();
    }

    void loadPointCloud()
    {
        // Check what file exists
        if (File.Exists(Application.dataPath + "/" + dataPath + ".ply"))
        {
            loadPointCloudFromFile("/" + dataPath + ".ply");
        }
        else
            Debug.Log("File '" + Application.dataPath + "/" + dataPath + "' could not be found");
    }

    // Load stored PointCloud
    void loadStoredMeshes()
    {

        Debug.Log("Using previously loaded PointCloud: " + filename);

        GameObject pointGroup = Instantiate(Resources.Load("Prefabs/RGBMeshes/" + filename)) as GameObject;
        loaded = true;
    }

    void loadPointCloudFromFile(string dPath, string type=".ply")
    {
        StreamReader sr = new StreamReader(Application.dataPath + dPath);
        string[] buffer;
        if (type == ".off")
        {
            sr.ReadLine(); // OFF
            buffer = sr.ReadLine().Split(); // nPoints, nFaces
        }
        else if (type == ".ply")
        {
            sr.ReadLine(); // ply
            sr.ReadLine(); // header
            sr.ReadLine(); // header
            buffer = sr.ReadLine().Split(); // nPoints
            numPoints = int.Parse(buffer[2]);
            int numProperties = 0;
            while (sr.ReadLine() != "end_header")
            {
                numProperties++;
            }
        }

        points = new Vector3[numPoints];
        colors = new Color[numPoints];
        minValue = new Vector3();

        for (int i = 0; i < numPoints; i++)
        {
            buffer = sr.ReadLine().Split();

            if (!invertYZ)
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[1]) * scale, float.Parse(buffer[2]) * scale);
            else
                points[i] = new Vector3(float.Parse(buffer[0]) * scale, float.Parse(buffer[2]) * scale, float.Parse(buffer[1]) * scale);

            if (buffer.Length >= 5)
                colors[i] = new Color(int.Parse(buffer[3]) / 255.0f, int.Parse(buffer[4]) / 255.0f, int.Parse(buffer[5]) / 255.0f);
            else
                colors[i] = Color.cyan;
        }


        // Instantiate Point Groups
        numPointGroups = Mathf.CeilToInt(numPoints * 1.0f / limitPoints * 1.0f);

        pointCloud = new GameObject(filename);

        for (int i = 0; i < numPointGroups - 1; i++)
        {
            InstantiateMesh(i, limitPoints);
        }
        InstantiateMesh(numPointGroups - 1, numPoints - (numPointGroups - 1) * limitPoints);

        //Store PointCloud
        UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/RGBMeshes/" + filename + ".prefab", pointCloud);
        loaded = true;
    }

    void InstantiateMesh(int meshInd, int nPoints)
    {
        // Create Mesh
        GameObject pointGroup = new GameObject(filename + meshInd);
        pointGroup.AddComponent<MeshFilter>();
        pointGroup.AddComponent<MeshRenderer>();
        pointGroup.GetComponent<Renderer>().material = matVertex;

        pointGroup.GetComponent<MeshFilter>().mesh = CreateMesh(meshInd, nPoints, limitPoints);
        pointGroup.transform.parent = pointCloud.transform;

        //if (rightToLeftHand)
        //{
        //    pointCloud.transform.eulerAngles = pointCloud.transform.eulerAngles + (new Vector3(0, 180, 0));
        //}

        // Store Mesh
        UnityEditor.AssetDatabase.CreateAsset(pointGroup.GetComponent<MeshFilter>().mesh, "Assets/Resources/Prefabs/RGBMeshes/" + filename + @"/" + filename + meshInd + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    Mesh CreateMesh(int id, int nPoints, int limitPoints)
    {

        Mesh mesh = new Mesh();

        Vector3[] myPoints = new Vector3[nPoints];
        int[] indecies = new int[nPoints];
        Color[] myColors = new Color[nPoints];

        for (int i = 0; i < nPoints; ++i)
        {
            myPoints[i] = points[id * limitPoints + i] - minValue;
            indecies[i] = i;
            myColors[i] = colors[id * limitPoints + i];
        }


        mesh.vertices = myPoints;
        mesh.colors = myColors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
        mesh.uv = new Vector2[nPoints];
        mesh.normals = new Vector3[nPoints];
        return mesh;
    }

    void calculateMin(Vector3 point)
    {
        if (minValue.magnitude == 0)
            minValue = point;
        if (point.x < minValue.x)
            minValue.x = point.x;
        if (point.y < minValue.y)
            minValue.y = point.y;
        if (point.z < minValue.z)
            minValue.z = point.z;
    }
}