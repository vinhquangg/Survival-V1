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

        for (int i = 0; i < objectsToSpawn.Count; i++)
        {
            // Nếu đã có object trong list → skip
            if (i < spawnedObjects.Count && spawnedObjects[i] != null)
            {
                // Bật lại object cũ thay vì tạo mới
                ObjectPoolManager.Instance?.ReenableFromPool(spawnedObjects[i]);

                // Đặt lại vị trí/rotation trong trường hợp object bị xê dịch trước đó
                spawnedObjects[i].transform.SetPositionAndRotation(
                    transform.TransformPoint(objectsToSpawn[i].localPosition),
                    Quaternion.Euler(objectsToSpawn[i].localRotation)
                );

                spawnedObjects[i].transform.SetParent(transform);
                continue;
            }


            SpawnInfo info = objectsToSpawn[i];
            Vector3 worldPos = transform.TransformPoint(info.localPosition);
            Quaternion worldRot = Quaternion.Euler(info.localRotation);

            GameObject obj = ObjectPoolManager.Instance?.SpawnFromPool(info.tag, worldPos, worldRot);
            if (obj != null)
            {
                obj.transform.SetParent(transform);
                if (i < spawnedObjects.Count)
                    spawnedObjects[i] = obj;
                else
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
        //spawnedObjects.Clear();
        hasSpawned = false;
    }

    public void DespawnObjects()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject obj = spawnedObjects[i];
            if (obj != null)
            {
                obj.transform.SetParent(null);
                ObjectPoolManager.Instance?.ReturnToPool(obj);
            }
        }
        // spawnedObjects.Clear(); ❌ KHÔNG CLEAR
        hasSpawned = false;
    }


}
