using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnInfo
    {
        public string poolID; // 🔁 Đổi từ 'tag' thành 'poolID' cho đồng bộ PoolableObject
        public Vector3 localPosition;
        public Vector3 localRotation;
    }

    public List<SpawnInfo> objectsToSpawn = new();
    private readonly List<GameObject> spawnedObjects = new();
    public bool HasSpawned => hasSpawned;

    private bool hasSpawned = false;

    public void SpawnObjects()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            SpawnInfo info = objectsToSpawn[i];
            Vector3 worldPos = transform.TransformPoint(info.localPosition);
            Quaternion worldRot = Quaternion.Euler(info.localRotation);

            GameObject obj = null;

            // Nếu object đã spawn → Reuse lại
            if (i < spawnedObjects.Count && spawnedObjects[i] != null)
            {
                obj = spawnedObjects[i];
                obj.transform.SetPositionAndRotation(worldPos, worldRot);
                obj.SetActive(true);
                obj.GetComponent<IPoolable>()?.OnSpawned();
            }
            else
            {
                obj = ObjectPoolManager.Instance?.SpawnFromPool(info.poolID, worldPos, worldRot);
                if (obj != null)
                {
                    if (i < spawnedObjects.Count)
                        spawnedObjects[i] = obj;
                    else
                        spawnedObjects.Add(obj);
                }
                else
                {
                    Debug.LogWarning($"❌ Không tìm thấy poolID '{info.poolID}' khi spawn!");
                    continue;
                }
            }

            obj.transform.SetParent(transform);
        }
    }

    public void DespawnObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                obj.transform.SetParent(null);
                ObjectPoolManager.Instance?.ReturnToPool(obj);
            }
        }

        hasSpawned = false;
    }

    //private void OnDisable()
    //{
    //    DespawnObjects();
    //}
}

