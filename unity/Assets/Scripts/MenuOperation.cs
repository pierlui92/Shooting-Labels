using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class MenuOperation : MonoBehaviour {

    public GameObject controllerLeft;

    private SteamVR_TrackedController controller;
    //private List<string> meshName = new List<string>();
    public List<String> meshName;
    
    
    // Use this for initialization
    void Start () {
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        meshName = MeshController.singleton.meshName;
        createFolders();
    }

    private void createFolders()
    {
        if (!Directory.Exists(Application.dataPath + "/Data"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Data");

        if (!Directory.Exists(Application.dataPath + "/Data/Exported"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Exported/");

        if (!Directory.Exists(Application.dataPath + "/Data/Saves"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Data", "Saves");

    }

    private void FastSaveColor(object sender, ClickedEventArgs e)
    {
        for (int i = 0; i < MeshController.singleton.Count(); i++)
        {
            GameObject meshModified = GameObject.Find(MeshController.singleton.meshName[i]);
            MeshFilter realMesh = MeshController.singleton.GetComponent<MeshFilter>();
            Color[] colorVector = realMesh.sharedMesh.colors;
            PlayerPrefsX.SetColorArray(MeshController.singleton.meshName[i], colorVector);
            PlayerPrefsX.SetStringArray("ListOfKeys", MeshController.singleton.meshName.ToArray());
        }

    }

    public static void ExportPLY()
    {
        GameObject parent = MeshController.singleton.meshesEmpty;
        LODMeshes[] lods = parent.GetComponentsInChildren<LODMeshes>();
        foreach (Transform child in parent.transform)
        {
            LODMeshes lod = child.GetComponentInParent<LODMeshes>();
            Mesh instance = lod.highPolyMesh;
            int[] faces = MeshController.faceLabel[child.name];
            TextWriter writer = File.CreateText(Application.dataPath + "/Data/Exported/" + lod.name.Replace("(Clone)", "") + ".ply");
            SaveAsciiPlyMesh(instance, lod.mainColor, writer, false, true, faces);
            writer.Close();
        }
    }

    //Load a txt file: x,y,z,idColor
    public static void LoadColorFile()
    {
        string folderPath = Application.dataPath + "/Data/Saves/SaveN" + MainMenuLaserPointer.numberofsaves + "/";
        string folderPathPly = Application.dataPath + "/Data/Saves/SaveN" + MainMenuLaserPointer.numberofsaves + "/Ply/";

        int i = 0;

        foreach (string file in Directory.GetFiles(folderPath, "*.txt"))
        {
            string[] lines = File.ReadAllLines(file);
            Vector3[] vertices = new Vector3[lines.Length];
            Color[] colors = new Color[lines.Length];

            foreach (string line in lines)
            {
                if (line != "")
                {
                    string[] token = line.Split(' ');
                    vertices[i] = new Vector3(float.Parse(token[0]), float.Parse(token[1]), float.Parse(token[2]));
                    int idColor = int.Parse(token[3]);
                    Color color = new Color();
                    if (idColor == -1) //white - blank
                    {
                        colors[i] = Color.white;
                    }
                    else if (idColor == 99) //black - void
                    {
                        colors[i] = Color.black;
                    }
                    else if (LoadColorsConfiguration.RGBFromCategories.TryGetValue(idColor, out color))
                    {
                        colors[i] = color;
                    }
                    i++;
                }

            }

            i = 0;
            string[] filetoken = file.Split('/');
            string filename = filetoken[filetoken.Length - 1];
            string noExtension = filename.Substring(0, filename.Length - 4);
            GameObject mesh = GameObject.Find(noExtension);
            LODMeshes lodMeshes = mesh.GetComponent<LODMeshes>();
            lodMeshes.UpdateMainColor(colors);
        }

        int numVertices = 0;
        int startFaces = 0;
        int indexArrayFaceLabel = 0;
        bool complete = false;
        foreach (string file in Directory.GetFiles(folderPathPly, "*.ply"))
        {
            string[] filetoken = file.Split('/');
            string filename = filetoken[filetoken.Length - 1];
            string noExtension = filename.Substring(0, filename.Length - 4);

            GameObject mesh = GameObject.Find(noExtension + "(Clone)");

            int[] faces =  MeshController.faceLabel[mesh.name];
            string[] lines = File.ReadAllLines(file);

            for(int j=0;j<lines.Length;j++)
            {
                if(lines[j].StartsWith("element vertex"))
                {
                    string[] token = lines[j].Split(' ');
                    numVertices = int.Parse(token[2]);
                    
                }              
                if(lines[j].StartsWith("end_header"))
                {
                    startFaces = j + numVertices;
                    complete = true;
                }

                if(j>startFaces && complete)
                {
                    string[] token = lines[j].Split(' ');
                    int id = int.Parse(token[4]);
                    faces[indexArrayFaceLabel] = id;
                    indexArrayFaceLabel++;
                }
            }

            indexArrayFaceLabel = 0;
        }

    }

    //Save a txt file with x,y,z(vertices) - idColor
    public static void SaveColorFile()
    {
        if (MainMenuLaserPointer.NewSession)
        {
            Directory.CreateDirectory(Application.dataPath + "/Data/Saves/SaveN" + (++MainMenuLaserPointer.numberofsaves));
            Directory.CreateDirectory(Application.dataPath + "/Data/Saves/SaveN" + MainMenuLaserPointer.numberofsaves +"/Ply");
        }
        List<string> meshName = MeshController.singleton.meshName;
        for (int i = 0; i <meshName.Count; i++)
        {
            GameObject foundObjects = GameObject.Find(meshName[i]);
            if (meshName[i] != "batman")
            {
                MeshCollider collider = foundObjects.GetComponent<MeshCollider>();
                LODMeshes lodMeshes = foundObjects.GetComponent<LODMeshes>();
                Color[] colorVector = lodMeshes.mainColor;
                ColorExporter exporter = new ColorExporter(Application.dataPath + "/Data/Saves/SaveN" + MainMenuLaserPointer.numberofsaves + "/" + MeshController.singleton.meshName[i] + ".txt", collider.sharedMesh.vertices, colorVector);
                exporter.MeshToFile();
                TextWriter writer = File.CreateText(Application.dataPath + "/Data/Saves/SaveN" + MainMenuLaserPointer.numberofsaves + "/Ply/" + MeshController.singleton.meshName[i].Replace("(Clone)", "") + ".ply");
                ColorExporter.SaveAsciiPlyMeshLigth(lodMeshes.highPolyMesh, writer, MeshController.faceLabel[MeshController.singleton.meshName[i]]);
                writer.Close();
            }
        }
    }

   
    private static void SaveAsciiPlyMesh(Mesh mesh,Color[] mainColors, TextWriter writer, bool flipAxes, bool outputColor,int[] idCategoryFaces)
    {
        if (null == mesh || null == writer)
        {
            return;
        }

        var vertices = mesh.vertices;
        var indices = mesh.triangles;
        //var colors = mesh.colors;
        var colors = mainColors;

        int faces = indices.Length / 3;

        // Write the PLY header lines
        writer.WriteLine("ply");
        writer.WriteLine("format ascii 1.0");
      
        writer.WriteLine("comment file created by CV_LAB");

        writer.WriteLine("element vertex " + vertices.Length.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine("property float x");
        writer.WriteLine("property float y");
        writer.WriteLine("property float z");

        if (outputColor)
        {
            writer.WriteLine("property uchar red");
            writer.WriteLine("property uchar green");
            writer.WriteLine("property uchar blue");
        }

        writer.WriteLine("element face " + faces.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine("property list uchar int vertex_indices");
        writer.WriteLine("property int category_id");
        writer.WriteLine("end_header");

        // Sequentially write the 3 vertices of the triangle, for each triangle
        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];

            string vertexString = (-vertex.x).ToString("F6") + " ";

            if (flipAxes)
            {
                vertexString += (-vertex.y).ToString("F6") + " " + (-vertex.z).ToString("F6");
            }
            else
            {
                vertexString += vertex.y.ToString("F6") + " " + vertex.z.ToString("F6");
            }

            if (outputColor)
            {
                
                Color searchColor = new Color(colors[i].r, colors[i].g, colors[i].b, 1);
                int idColor;
                Color finalColor = new Color(0,0,0,255);
                if (LoadColorsConfiguration.categoriesFromRGB.TryGetValue(searchColor, out idColor))
                {
                    LoadColorsConfiguration.RGBFromCategories.TryGetValue(idColor, out finalColor);
                    finalColor.r = finalColor.r * 255;
                    finalColor.g = finalColor.g * 255;
                    finalColor.b = finalColor.b * 255;
                }

              
                vertexString += " " + finalColor.r.ToString(CultureInfo.InvariantCulture) + " " + finalColor.g.ToString(CultureInfo.InvariantCulture) + " "
                            + finalColor.b.ToString(CultureInfo.InvariantCulture);
            }

            writer.WriteLine(vertexString);
        }

        // Sequentially write the 3 vertex indices of the triangle face, for each triangle, 0-referenced in PLY files
        int indexColorCategory = 0;

        for (int i = 0; i < indices.Length/3; i++)
        {
            string baseIndex0 = (indices[i * 3 + 0]).ToString(CultureInfo.InvariantCulture);
            string baseIndex1 = (indices[i * 3 + 1]).ToString(CultureInfo.InvariantCulture);
            string baseIndex2 = (indices[i * 3 + 2]).ToString(CultureInfo.InvariantCulture);

            Color colorMappingV1 = colors[indices[i * 3 + 0]];
            Color colorMappingV2 = colors[indices[i * 3 + 1]];
            Color colorMappingV3 = colors[indices[i * 3 + 2]];

            string faceString = "3 " + baseIndex0 + " " + baseIndex1 + " " + baseIndex2 + " " + idCategoryFaces[indexColorCategory];
            
            writer.WriteLine(faceString);
            indexColorCategory++;
        }


    }
}
