using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataHit : MonoBehaviour
{

    private List<string> meshName;
    private List<Color[]> colorPrec;
    private List<int> timeOrder;
    private int currentIndex;
    private int freeSpace;
    private int currentTime;

    private static DataHit _instance;

    public DataHit()
    {
        meshName = new List<string>();
        colorPrec = new List<Color[]>();
        timeOrder = new List<int>();
        CurrentIndex = 0;
        FreeSpace = 5;
        CurrentTime = 0;
    }

    public static DataHit Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = new DataHit();
          
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

  

    public List<Color[]> ColorPrec
    {
        get
        {
            return colorPrec;
        }

        set
        {
            colorPrec = value;
        }
    }

   
    public int CurrentIndex
    {
        get
        {
            return currentIndex;
        }

        set
        {
            currentIndex = value;
        }
    }

    public int FreeSpace
    {
        get
        {
            return freeSpace;
        }

        set
        {
            freeSpace = value;
        }
    }

    public List<int> TimeOrder
    {
        get
        {
            return timeOrder;
        }

        set
        {
            timeOrder = value;
        }
    }

    public int CurrentTime
    {
        get
        {
            return currentTime;
        }

        set
        {
            currentTime = value;
        }
    }

    public bool OccupyASpace()
    { 
        if(FreeSpace > 0)
        {
           
            return true;
        }
        else
        {
            return false;
        }
        
    }

   

    public void Empty()
    {
        CurrentIndex = 0;
        _instance.meshName = new List<string>();
        _instance.colorPrec = new List<Color[]>();
        FreeSpace = 5;
    }

    public void Remove(int i)
    {
        _instance.meshName.RemoveAt(i);
        _instance.colorPrec.RemoveAt(i);
        FreeSpace++;
    }

    public void UpdateArrayColor(string name, Color[] colors)
    {
        if (MeshName.Contains(name))
        {
            int indexMesh = MeshName.IndexOf(name);
            ColorPrec[indexMesh] = colors;
        }
    }
}
