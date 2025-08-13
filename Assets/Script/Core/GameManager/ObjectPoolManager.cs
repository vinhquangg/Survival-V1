using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string poolID;                      // dùng thay cho tag
        public List<GameObject> prefabs;           // nhiều prefab cùng loại
        public int size;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new();
    private Dictionary<string, List<GameObject>> prefabReference = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        InitializePools();
    }

    #region Init

    private void InitializePools()
    {
        System.Random prng = new System.Random(WorldSeedManager.Seed);

        foreach (var pool in pools)
        {
            Queue<GameObject> objectQueue = new();
            prefabReference[pool.poolID] = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject prefab = GetRandomPrefab(pool, prng);
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);

                // ⛳ Đảm bảo prefab gắn đúng poolID
                PoolableObject po = obj.GetComponent<PoolableObject>();
                if (po == null)
                {
                    Debug.LogWarning($"❌ Prefab '{prefab.name}' thiếu PoolableObject!");
                    continue;
                }

                if (po.poolID != pool.poolID)
                {
                    Debug.LogWarning($"⚠ Prefab '{prefab.name}' có poolID='{po.poolID}' nhưng đang trong pool '{pool.poolID}'");
                }

                objectQueue.Enqueue(obj);
                prefabReference[pool.poolID].Add(prefab);
            }

            poolDictionary[pool.poolID] = objectQueue;
        }
    }

    private GameObject GetRandomPrefab(Pool pool, System.Random prng)
    {
        int index = prng.Next(0, pool.prefabs.Count);
        return pool.prefabs[index];
    }

    #endregion

    #region Spawn

    public GameObject SpawnFromPool(string poolID, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolID))
        {
            Debug.LogWarning($"❌ Không tìm thấy poolID '{poolID}' trong poolDictionary!");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[poolID];

        int maxAttempts = queue.Count;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            GameObject obj = queue.Dequeue();
            if (obj != null)
            {
                SetupObject(obj, position, rotation);
                queue.Enqueue(obj);
                return obj;
            }
            attempts++;
        }

        Debug.LogWarning($"❌ Pool '{poolID}' không còn object hợp lệ hoặc đã bị destroy!");
        return null;
    }

    private void SetupObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.transform.SetPositionAndRotation(position, rotation);

        var tree = obj.GetComponent<TreeInstance>();
        if (tree != null && tree.isChopped)
        {
            obj.SetActive(false);
        }

        obj.SetActive(true);
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawned();
    }

    #endregion

    #region Return

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        var po = obj.GetComponent<PoolableObject>();
        if (po == null || string.IsNullOrEmpty(po.poolID))
        {
            Debug.LogWarning("❌ Object trả về pool thiếu PoolableObject hoặc poolID!");
            return;
        }

        string poolID = po.poolID;
        if (!poolDictionary.ContainsKey(poolID))
        {
            Debug.LogWarning($"❌ Không tìm thấy pool '{poolID}' khi ReturnToPool.");
            return;
        }

        obj.SetActive(false);
        poolDictionary[poolID].Enqueue(obj);

        // Gọi OnReturned() nếu có
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnReturned();
    }

    #endregion
}

