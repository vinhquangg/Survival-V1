using System.Collections.Generic;
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
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            if (!prefabReference.ContainsKey(pool.tag))
                prefabReference[pool.tag] = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // Chọn ngẫu nhiên một prefab từ danh sách
                GameObject prefab = pool.prefabs[Random.Range(0, pool.prefabs.Count)];
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

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
        poolDictionary[tag].Enqueue(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
