using UnityEngine;
using System.Collections.Generic;

namespace EZObjectPools
{
    [AddComponentMenu("EZ Object Pool/Object Pool")]
    public class EZObjectPool : MonoBehaviour
    {
        static Dictionary<string, EZObjectPool> SharedPools = new Dictionary<string, EZObjectPool>();
        static GameObject Marker;

        /// <summary>
        /// The template GameObject this pool uses.
        /// </summary>
        public GameObject Template;
        /// <summary>
        /// The name of this pool.
        /// </summary>
        public string PoolName;
        /// <summary>
        /// The list of all objects owned by this pool.
        /// </summary>
        public List<GameObject> ObjectList;
        /// <summary>
        /// Should this pool automatically resize when empty?
        /// </summary>
        public bool AutoResize = false;
        /// <summary>
        /// The default pool size.
        /// </summary>
        public int PoolSize = 100;
        /// <summary>
        /// Only used for pools created in the editor. Should this pool be instantiated when the scene loads?
        /// </summary>
        public bool InstantiateOnAwake;
        /// <summary>
        /// Only used for pools created in the editor. Any scripts that request a pool with the same name as this one will recieve this pool.
        /// </summary>
        public bool Shared;

        /// <summary>
        /// Creates a new Object Pool and returns a reference to the created pool.
        /// </summary>
        /// <param name="template">The template this pool will make copies of.</param>
        /// <param name="name">The name of this pool. Only used to identify it in the hierarchy.</param>
        /// <param name="size">How many objects will be pre-instantiated.</param>
        /// <param name="autoResize">Should this pool create new objects if it is empty?</param>
        /// <param name="instantiateImmediate">Should this pool be instantiated immediately?</param>
        /// <param name="shared">Should pools with the same name be shared?</param>
        /// <returns>A reference to the created pool.</returns>
        public static EZObjectPool CreateObjectPool(GameObject template, string name, int size, bool autoResize, bool instantiateImmediate, bool shared)
        {
            if (Marker == null)
            {
                Marker = new GameObject("EZ Object Pools Container");
                SharedPools.Clear();
            }

            if (shared)
            {
                if (SharedPools.ContainsKey(name))
                    return SharedPools[name];
                else
                {
                    GameObject g = new GameObject(name);
                    EZObjectPool pool = g.AddComponent<EZObjectPool>();
                    pool.InstantiateOnAwake = false;
                    pool.SetProperties(template, size, name, autoResize);
                    SharedPools.Add(name, pool);

                    if (instantiateImmediate)
                        pool.InstantiatePool();

                    g.transform.parent = Marker.transform;

                    return pool;
                }
            }
            else
            {
                GameObject g = new GameObject(name);
                EZObjectPool pool = g.AddComponent<EZObjectPool>();
                pool.InstantiateOnAwake = false;
                pool.SetProperties(template, size, name, autoResize);

                if (instantiateImmediate)
                    pool.InstantiatePool();

                g.transform.parent = Marker.transform;

                return pool;
            }
        }

        void Awake()
        {
            if (Marker == null)
            {
                Marker = new GameObject("EZ Object Pools Container");
                SharedPools.Clear();
            }

            if (InstantiateOnAwake)
            {
                ObjectList = new List<GameObject>(PoolSize);
                AvailableObjects = new List<GameObject>(PoolSize);
                InstantiatePool();
            }

            if (Shared)
            {
                SharedPools.Add(this.PoolName, this);
            }
        }

        #region Instance Functions

        List<GameObject> AvailableObjects;

        /// <summary>
        /// Set the properties of the pool.
        /// </summary>
        /// <param name="objectTemplate">The template object to clone.</param>
        /// <param name="size">The amount of clones to create</param>
        /// <param name="name">Name of this pool</param>
        /// <param name="autoResize">Should new objects be instantiated if the pool is empty?</param>
        public void SetProperties(GameObject objectTemplate, int size, string name, bool autoResize)
        {
            Template = objectTemplate;
            PoolSize = size;
            ObjectList = new List<GameObject>(size);
            AvailableObjects = new List<GameObject>(size);
            PoolName = name;
            this.AutoResize = autoResize;
        }

        /// <summary>
        /// Creates the set number of objects at the world origin (0,0,0) and sets them to inactive. Deletes any previous objects.
        /// </summary>
        public void InstantiatePool()
        {
            if (Template == null)
            {
                Debug.LogError("EZ Object Pool: " + name + ": Template GameObject is null! Make sure you assigned a template either in the inspector or in your scripts.");
                return;
            }

            ClearPool();

            for (int i = 0; i < PoolSize; i++)
            {
                GameObject g = NewActiveObject();
                g.SetActive(false);
                ObjectList.Add(g);
            }
        }

        /// <summary>
        /// Attempts to get the next available object in the pool. Will return true and assign a reference to the out variable if successful.
        /// </summary>
        /// <param name="pos">The position to spawn the object at.</param>
        /// <param name="rot">The rotation to spawn the object at.</param>
        /// <param name="obj">A reference to the GameObject being spawned.</param>
        /// <returns>Returns true if an object was successfully retrieved, False if not.</returns>
        public bool TryGetNextObject(Vector3 pos, Quaternion rot, out GameObject obj)
        {
            if (ObjectList.Count == 0)
            {
                Debug.LogError("EZ Object Pool " + PoolName + ", the pool has not been instantiated but you are trying to retrieve an object!");
            }

            int lastIndex = AvailableObjects.Count - 1;

            if (AvailableObjects.Count > 0)
            {
                if (AvailableObjects[lastIndex] == null)
                {
                    Debug.LogError("EZObjectPool " + PoolName + " has missing objects in its pool! Are you accidentally destroying any GameObjects retrieved from the pool?");
                    obj = null;
                    return false;
                }

                AvailableObjects[lastIndex].transform.position = pos;
                AvailableObjects[lastIndex].transform.rotation = rot;
                AvailableObjects[lastIndex].SetActive(true);
                obj = AvailableObjects[lastIndex];
                AvailableObjects.RemoveAt(lastIndex);
                return true;
            }

            if (AutoResize)
            {
                GameObject g = NewActiveObject();
                g.transform.position = pos;
                g.transform.rotation = rot;
                ObjectList.Add(g);
                obj = g;
                return true;
            }
            else
            {
                obj = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the next available object in the pool. Will do nothing if the pool is empty. 
        /// </summary>
        /// <param name="pos">The position to put the object at.</param>
        /// <param name="rot">The rotation to put the object at.</param>
        public void TryGetNextObject(Vector3 pos, Quaternion rot)
        {
            if (ObjectList.Count == 0)
            {
                Debug.LogError("EZ Object Pool " + PoolName + ", the pool has not been instantiated but you are trying to retrieve an object!");
            }

            int lastIndex = AvailableObjects.Count - 1;

            if (AvailableObjects.Count > 0)
            {
                if (AvailableObjects[lastIndex] == null)
                {
                    Debug.LogError("EZObjectPool " + PoolName + " has missing objects in its pool! Are you accidentally destroying any GameObjects retrieved from the pool?");
                    return;
                }

                AvailableObjects[lastIndex].transform.position = pos;
                AvailableObjects[lastIndex].transform.rotation = rot;
                AvailableObjects[lastIndex].SetActive(true);
                AvailableObjects.RemoveAt(lastIndex);
                return;
            }

            if (AutoResize)
            {
                GameObject g = NewActiveObject();
                g.transform.position = pos;
                g.transform.rotation = rot;
                ObjectList.Add(g);
            }
        }

        GameObject NewActiveObject()
        {
            GameObject g = (GameObject)Instantiate(Template);
            g.transform.parent = transform;

            PooledObject p = g.GetComponent<PooledObject>();

            if (p)
                p.ParentPool = this;
            else
                g.AddComponent<PooledObject>().ParentPool = this;

            return g;
        }

        /// <summary>
        /// Destroys all objects that are a part of this pool.
        /// </summary>
        public void ClearPool()
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                Destroy(ObjectList[i]);
            }

            ObjectList.Clear();
            AvailableObjects.Clear();
        }

        /// <summary>
        /// Destroys this pool.
        /// </summary>
        /// <param name="deleteActiveObjects">Should it delete any active GameObjects that are part of this pool?</param>
        public void DeletePool(bool deleteActiveObjects)
        {
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (!ObjectList[i].activeInHierarchy || (ObjectList[i].activeInHierarchy && deleteActiveObjects))
                    Destroy(ObjectList[i]);
            }
        }

        /// <summary>
        /// Adds the given GameObject to the Available Objects list.
        /// </summary>
        /// <param name="obj">The GameObject to add.</param>
        public void AddToAvailableObjects(GameObject obj)
        {
            AvailableObjects.Add(obj);
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Gets the number of currently active objects.
        /// </summary>
        public int ActiveObjectCount()
        {
            return ObjectList.Count - AvailableObjects.Count;
        }

        /// <summary>
        /// Gets the number of currently available objects.
        /// </summary>
        public int AvailableObjectCount()
        {
            return AvailableObjects.Count;
        }

        #endregion
    }
}