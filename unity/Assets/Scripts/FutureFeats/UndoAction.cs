using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoAction : MonoBehaviour {

    private SteamVR_TrackedController controller;
    public GameObject controllerLeft;
    private DataHit hits;

    // Use this for initialization
    void Start () {
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        controller.Gripped += GripPressed;
        hits = DataHit.Instance;
    }

    private void GripPressed(object sender, ClickedEventArgs e)
    {
        if (hits.CurrentIndex < 5)
        {
            List<string> meshHitsNames = hits.MeshName;
            meshHitsNames.Reverse();
            int indexCurrent = hits.CurrentIndex;

            List<Color[]> listColorPrec = hits.ColorPrec;
            listColorPrec.Reverse();
            Color[] colorPrec = hits.ColorPrec[indexCurrent];

            string name = meshHitsNames[indexCurrent];

            GameObject gameObjectHits = GameObject.Find(name);
            MeshFilter meshFilter = gameObjectHits.GetComponent<MeshFilter>();
            Mesh highPoly = meshFilter.mesh;

            highPoly.colors = colorPrec;

            hits.CurrentIndex++;
        }
        //hits.Remove(indexCurrent);
        //Debug.Log("undo " + hits.CurrentIndex);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
