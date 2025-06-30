using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnInfo
    {
        public string tag;
        public Vector3 localPosition;
        public Vector3 localRotation;
    }

    public List<SpawnInfo> objectsToSpawn = new();
    private List<GameObject> spawnedObjects = new();

    private bool hasSpawned = false;

    public void SpawnObjects()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        foreach (SpawnInfo info in objectsToSpawn)
        {
            Vector3 worldPos = transform.TransformPoint(info.localPosition);
            Quaternion worldRot = Quaternion.Euler(info.localRotation);

            GameObject obj = ObjectPoolManager.Instance?.SpawnFromPool(info.tag, worldPos, worldRot);
            if (obj != null)
            {
                obj.transform.SetParent(transform);
                spawnedObjects.Add(obj);
            }
            else
            {
                Debug.LogWarning($"❌ Không tìm thấy tag '{info.tag}' trong Pool!");
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                ObjectPoolManager.Instance?.ReturnToPool(obj);
        }
        spawnedObjects.Clear();
        hasSpawned = false;
    }

    public void DespawnObjects()
{
    foreach (GameObject obj in spawnedObjects)
    {
        if (obj != null)
        {
            obj.transform.SetParent(null); // ✅ đặt parent về null trước
            ObjectPoolManager.Instance?.ReturnToPool(obj);
        }
    }

    spawnedObjects.Clear();
    hasSpawned = false;
}

}
