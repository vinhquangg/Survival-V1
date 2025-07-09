using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public List<GameObject> prefabs; // ✅ Danh sách prefab dùng chung tag
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, List<GameObject>> prefabReference = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // ✅ Tạo Random có seed cố định để đảm bảo prefab được chọn giống nhau mỗi lần play
        System.Random prng = new System.Random(WorldSeedManager.Seed);

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            if (!prefabReference.ContainsKey(pool.tag))
                prefabReference[pool.tag] = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // ✅ Dùng System.Random thay vì UnityEngine.Random
                int prefabIndex = prng.Next(0, pool.prefabs.Count);
                GameObject prefab = pool.prefabs[prefabIndex];

                // ✅ Tạo object từ prefab được chọn
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);

                prefabReference[pool.tag].Add(prefab);
    
            }

            poolDictionary[pool.tag] = objectQueue;
        }
    }


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("❌ Không tìm thấy tag '" + tag + "' trong Pool!");
            return null;
        }

        GameObject obj = null;
        int attempts = 0;
        int maxAttempts = poolDictionary[tag].Count;

        while (attempts < maxAttempts)
        {
            obj = poolDictionary[tag].Dequeue();

            if (obj != null)
            {
                obj.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);
                poolDictionary[tag].Enqueue(obj);
                return obj;
            }

            attempts++;
        }

        Debug.LogWarning($"❌ Pool '{tag}' không còn object hợp lệ hoặc đã bị destroy!");
        return null;
    }


    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
