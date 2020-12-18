using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZEffects;


public class Weapons : MonoBehaviour
{
    public GameObject controllerRight;
    public int distanceOfShoot;
    public static GameObject colorSelected;
    public static Color current_color;
    public LayerMask shootableMask;

    public float radiusOfFire;
    protected Vector3 hitPoint;
    protected RaycastHit hit;
    protected float factorOfScale;

    protected SteamVR_TrackedObject trackedObj;
    protected SteamVR_Controller.Device device;
    public List<String> meshModified;
    public Transform muzzleTrasform;


    protected void Initialize()
    {
        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        meshModified = MeshController.singleton.meshName;
        factorOfScale = 0.001f;
        colorSelected = gameObject.transform.Find("ColorSelected").Find("ImageColorSelected").gameObject;
    }

    private void Start()
    {
        Initialize();
    }


    protected void ChangeFactorOfScale(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        if (vertices.Length < 10000000f) //Cambiare il fattore di scala in base ai vertici della mesh, probabilmente bisognerebbe cambiarla in base alla grandezza dei voxel
            factorOfScale = 0.1f;
    }

    protected void SaveColorTemporary(GameObject hitten)
    {
        int indexOfMeshHitten = -1;
        indexOfMeshHitten = MeshController.singleton.GetIndexByName(hit.collider.name);
        if (indexOfMeshHitten >= 0)
        {
            LODMeshes lodMeshes = hitten.GetComponent<LODMeshes>();
            MeshFilter meshFilter = hitten.GetComponent<MeshFilter>();
            lodMeshes.mainColor = meshFilter.mesh.colors;
        }
    }

    protected virtual bool TriggerIsPressed()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }

    protected virtual bool ShootRay()
    {
        return Physics.Raycast(muzzleTrasform.position, muzzleTrasform.forward, out hit, distanceOfShoot, shootableMask);
    }

    protected void TrackModifiedMeshes()
    {
        if (!MeshController.singleton.meshName.Contains(hit.collider.name))
            MeshController.singleton.meshName.Add(hit.collider.name);
    }

    protected void ColorMesh(Mesh mesh, RaycastHit hit, Color col, GameObject meshHits, bool ColorArea)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color[] colors = mesh.colors;
        int[] faces = null;

        int v0 = triangles[hit.triangleIndex * 3 + 0];
        int v1 = triangles[hit.triangleIndex * 3 + 1];
        int v2 = triangles[hit.triangleIndex * 3 + 2];

        if (MeshController.faceLabel != null && MeshController.faceLabel.ContainsKey(meshHits.name))
        {
            faces = MeshController.faceLabel[meshHits.name];
            faces[hit.triangleIndex] = LoadColorsConfiguration.categoriesFromRGB[col];
        }

        TrackModifiedMeshes();

        if (ColorArea) {
            // Color Face In factorOfScale * radiusOfFire Range
            Vector3 p0 = vertices[v0];
            Vector3 p1 = vertices[v1];
            Vector3 p2 = vertices[v2];

            Vector3 center = (p0 + p1 + p2);
            center = center / 3.0F;
            Vector3 localSpaceCenter = center;

            for (int index = 0; index < triangles.Length / 3; index++)
            {
                int v0Area = triangles[index * 3 + 0];
                int v1Area = triangles[index * 3 + 1];
                int v2Area = triangles[index * 3 + 2];

                float distanceV1 = Vector3.Distance(vertices[v0Area], localSpaceCenter);
                float distanceV2 = Vector3.Distance(vertices[v1Area], localSpaceCenter);
                float distanceV3 = Vector3.Distance(vertices[v2Area], localSpaceCenter);

                if (distanceV1 < factorOfScale * radiusOfFire && distanceV2 < factorOfScale * radiusOfFire && distanceV3 < factorOfScale * radiusOfFire)
                {
                    colors[v0Area] = col;
                    colors[v1Area] = col;
                    colors[v2Area] = col;

                    if (faces != null)
                    {
                        faces[index] = LoadColorsConfiguration.categoriesFromRGB[col];
                    }
                }
            }

            mesh.colors = colors;
        }
        else
        {
            // Color Single Face
            colors[v0] = col;
            colors[v1] = col;
            colors[v2] = col;

            mesh.colors = colors;
        }

    }
}