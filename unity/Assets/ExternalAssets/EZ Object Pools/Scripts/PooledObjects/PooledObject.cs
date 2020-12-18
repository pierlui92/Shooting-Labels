using UnityEngine;

namespace EZObjectPools
{
    [AddComponentMenu("EZ Object Pools/Pooled Object")]
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// The object pool this object originated from.
        /// </summary>
        [HideInInspector]
        public EZObjectPool ParentPool;

        /// <summary>
        /// [OBSOLETE] Simply calls gameObject.SetActive(false). No longer needed in your scripts.
        /// </summary>
        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        void OnDisable()
        {
            transform.position = Vector3.zero;

            if (ParentPool)
                ParentPool.AddToAvailableObjects(this.gameObject);
            else
                Debug.LogWarning("PooledObject " + gameObject.name + " does not have a parent pool. If this occurred during a scene transition, ignore this. Otherwise reoprt to developer.");
        }
    }
}