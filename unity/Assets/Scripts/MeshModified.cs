using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshModified : MonoBehaviour {

    private List<string> meshName;

    private static MeshModified _instance;

    public MeshModified()
    {
        meshName = new List<string>();
    }

    public MeshModified(List<string> meshName)
    {
        this.MeshName = meshName;
    }

    public static MeshModified Instance { get { return _instance; } }

    private void Awake()
    {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = new MeshModified();

            }
    }

    public List<string> MeshName
    {
        get
        {
            return meshName;
        }

        set
        {
            meshName = value;
        }
    }

    public void Empty()
    {
        _instance.MeshName.Clear();
    }

    public int GetIndexByName(string name)
    {
        if (MeshName.Contains(name))
        {
            for (int i = 0; i < MeshName.Count; i++)
            {
                if(MeshName[i] == name)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public int Count()
    {
        return MeshName.Count;
    }
}
