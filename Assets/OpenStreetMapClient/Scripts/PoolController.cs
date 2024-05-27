using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSMClient
{
    /// <summary>
    /// GameObject Pool
    /// </summary>
    public class PoolController : MonoBehaviour
    {
        public int DefaultCount = 20;

        private Dictionary<GameObject, Pool> prefabToPool = new Dictionary<GameObject, Pool>();
        private Dictionary<GameObject, Pool> objectToPool = new Dictionary<GameObject, Pool>();

        public static PoolController Instance { get; private set; }

        class Pool
        {
            public LinkedList<GameObject> InUse = new LinkedList<GameObject>();
            public LinkedList<GameObject> NoUse = new LinkedList<GameObject>();
        }

        void Awake()
        {
            Instance = this;
        }

        public void PreparePool(GameObject prefab, int count)
        {
            GetOrCreatePool(prefab, count);
        }

        public void ReturnToPool(GameObject obj)
        {
            var pool = objectToPool[obj];
            obj.SetActive(false);
            obj.transform.SetParent(this.transform, false);
            pool.InUse.Remove(obj);
            pool.NoUse.AddLast(obj);
        }

        public GameObject CreateFromPool(GameObject prefab)
        {
            var pool = GetOrCreatePool(prefab, DefaultCount);
            if (pool.NoUse.Count == 0)
            {
                var obj = Instantiate(prefab, new Vector3(0, -100, 0), Quaternion.identity, this.transform);
                pool.NoUse.AddLast(obj);
                objectToPool[obj] = pool;
            }
            var item = pool.NoUse.Last.Value;
            pool.NoUse.RemoveLast();
            pool.InUse.AddLast(item);
            item.SetActive(true);

            return item;
        }

        private Pool GetOrCreatePool(GameObject prefab, int count)
        {
            Pool pool = null;
            if (!prefabToPool.TryGetValue(prefab, out pool))
            {
                prefabToPool[prefab] = pool = new Pool();

                for (int i = 0; i < count; i++)
                {
                    var obj = Instantiate(prefab, new Vector3(0, -100, 0), Quaternion.identity, this.transform);
                    obj.SetActive(false);
                    pool.NoUse.AddLast(obj);
                    objectToPool[obj] = pool;
                }
            }

            return pool;
        }
    }
}