using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class LODMeshes : MonoBehaviour
{
    
    public Mesh highPolyMesh;
    public Mesh mediumPolyMesh;
    public Mesh lowPolyMesh;
    public float distanceLOD1;
    public float distanceLOD2;
    public float updateInterval = 0.1f;
    public Color[] colorsUpdated;
    public Color[] mainColor;

    private GameObject player;
    private float currentTimeToInterval = -0.1f;
    private enum LOD_LEVEL { LOD0, LOD1, LOD2 };
    private LOD_LEVEL currentLOD;
    private MeshCollider collider;
    private List<string> meshModifiedInstance;

    private void Start()
    {
            meshModifiedInstance = MeshController.singleton.meshName;
            currentLOD = LOD_LEVEL.LOD0;
            currentTimeToInterval = updateInterval;
            collider = gameObject.GetComponent<MeshCollider>();
            colorsUpdated = new Color[collider.gameObject.GetComponent<MeshFilter>().mesh.colors.Length];


            mainColor = highPolyMesh.colors;
        
    }
    private void Update()
    {
        if (currentTimeToInterval <= 0.0f)
        {         
            player = GameObject.Find("[CameraRig]");
            Vector3 playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
           
            Vector3 nearestPointOnMesh = collider.ClosestPointOnBounds(playerPosition);

            float distancePlayerMesh = Vector2.Distance(new Vector2(playerPosition.x, playerPosition.z), new Vector2(nearestPointOnMesh.x, nearestPointOnMesh.z));
            
            if ((distancePlayerMesh < distanceLOD1) && (currentLOD != LOD_LEVEL.LOD0))
            {
                currentLOD = LOD_LEVEL.LOD0;
                
                ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = highPolyMesh;
                UpdateHighPolyMesh();

            }
            else if (distancePlayerMesh >= distanceLOD1 && distancePlayerMesh < distanceLOD2 && currentLOD != LOD_LEVEL.LOD1)
            {
                currentLOD = LOD_LEVEL.LOD1;
                ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = mediumPolyMesh;
               
            }
            else if (distancePlayerMesh >= distanceLOD2 && currentLOD != LOD_LEVEL.LOD2)
            {
                currentLOD = LOD_LEVEL.LOD2;
                ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh = lowPolyMesh;
            }

            //reset check timer
            currentTimeToInterval = updateInterval;
        }
        else
        {
            currentTimeToInterval -= Time.deltaTime;
        }
    }

    private void UpdateHighPolyMesh()
    {
        int indexOfMeshToUpdate = MeshController.singleton.GetIndexByName(collider.name);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.colors = mainColor;
    }

    public void UpdateMainColor(Color[] updatedColors)
    {
        mainColor = updatedColors;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.colors = mainColor;
    }
}
