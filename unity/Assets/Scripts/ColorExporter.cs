using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Globalization;

public class ColorExporter{

    private string filename;
    private Vector3[] vertices;
    private Color[] colors;

    public ColorExporter(string filename, Vector3[] vertices,Color[] colors)
    {
        Filename = filename;
        Vertices = vertices;
        Colors = colors;
    }

    public string Filename
    {
        get
        {
            return filename;
        }

        set
        {
            filename = value;
        }
    }

    public Vector3[] Vertices
    {
        get
        {
            return vertices;
        }

        set
        {
            vertices = value;
        }
    }

    public Color[] Colors
    {
        get
        {
            return colors;
        }

        set
        {
            colors = value;
        }
    }

   

    private string MeshToString()
    {
        
        StringBuilder sb = new StringBuilder();

        for(int i=0;i<Vertices.Length;i++)
        {
            Vector3 v = new Vector3(Vertices[i].x, Vertices[i].y, Vertices[i].z);
            Color c = Colors[i];
            c.a = 1.0f;
            int id = 0;

           
            double min = Double.MaxValue;
            Color colorNearest = new Color();
            if (c != Color.white && c != Color.black)
            {
                foreach (Color col in LoadColorsConfiguration.RGBFromCategories.Values)
                {
                    double dist = System.Math.Sqrt((System.Math.Pow(c.r - col.r, 2)) + (System.Math.Pow(c.g - col.g, 2)) + (System.Math.Pow(c.b - col.b, 2)));
                    if (dist < min)
                    {
                        min = dist;
                        colorNearest = col;
                    }
                }

                LoadColorsConfiguration.categoriesFromRGB.TryGetValue(colorNearest, out id);
                sb.Append(string.Format("{0} {1} {2} {3}\n", v.x, v.y, v.z, id)); //primi tre sono le coord del vertice l'ultimo è l'id del colore
            }
            else
            {
                if(c == Color.white)
                    sb.Append(string.Format("{0} {1} {2} {3}\n", v.x, v.y, v.z, "-1"));
                else
                    sb.Append(string.Format("{0} {1} {2} {3}\n", v.x, v.y, v.z, "99"));
            }
        }

      
        return sb.ToString();
    }


    public void MeshToFile()
    {
        using (StreamWriter sw = new StreamWriter(Filename))
        {
            sw.Write(MeshToString());
        }
    }

    public static void SaveAsciiPlyMeshLigth(Mesh mesh, TextWriter writer,int[] faceLabels)
    {
        if (null == mesh || null == writer)
        {
            return;
        }

        var vertices = mesh.vertices;
        var indices = mesh.triangles;

        int faces = indices.Length / 3;

        // Write the PLY header lines
        writer.WriteLine("ply");
        writer.WriteLine("format ascii 1.0");
        writer.WriteLine("comment file created by CV_LAB");
        writer.WriteLine("element vertex " + vertices.Length.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine("property float x");
        writer.WriteLine("property float y");
        writer.WriteLine("property float z");
        writer.WriteLine("element face " + faces.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine("property list uchar int vertex_indices");
        writer.WriteLine("property int category_id");
        writer.WriteLine("end_header");

        // Sequentially write the 3 vertices of the triangle, for each triangle
        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];

            string vertexString = (-vertex.x).ToString("F6") + " ";

            vertexString += vertex.y.ToString("F6") + " " + vertex.z.ToString("F6");

            writer.WriteLine(vertexString);
        }

        // Sequentially write the 3 vertex indices of the triangle face, for each triangle, 0-referenced in PLY files
        int countFaces = 0;
        for (int i = 0; i < indices.Length/3; i++)
        {
            string baseIndex0 = (indices[i * 3 + 0]).ToString(CultureInfo.InvariantCulture);
            string baseIndex1 = (indices[i * 3 + 1]).ToString(CultureInfo.InvariantCulture);
            string baseIndex2 = (indices[i * 3 + 2]).ToString(CultureInfo.InvariantCulture);   
            string faceString = "3 " + baseIndex0 + " " + baseIndex1 + " " + baseIndex2 + " " + faceLabels[countFaces];
            countFaces++;
            writer.WriteLine(faceString);
        }


    }
}
