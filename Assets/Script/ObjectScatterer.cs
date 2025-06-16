using System.Collections.Generic;
using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{
    public BiomeManager biomeManager;
    public int baseSpawnCount = 100;
    public float scatterRadius = 50f;
    public float spawnHeight = 20f;
    public LayerMask groundLayer;

    [Tooltip("Tự động offset prefab theo chiều cao Renderer")]
    public bool useRendererYOffset = true;

    private void Start()
    {
        ScatterObjects();
    }

    void ScatterObjects()
    {
        int spawnSuccessCount = 0;

        for (int i = 0; i < baseSpawnCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                Vector3 groundPos = hit.point;

                BiomeData biome = biomeManager.GetBiomeAtPosition(groundPos);
                if (biome == null || biome.spawnableObjects == null || biome.spawnableObjects.Length == 0)
                {
                    Debug.LogWarning($"⚠️ Biome lỗi hoặc không có prefab tại {groundPos}");
                    continue;
                }

                GameObject prefab = biome.spawnableObjects[Random.Range(0, biome.spawnableObjects.Length)];
                if (prefab == null)
                {
                    Debug.LogWarning("⚠️ Prefab null!");
                    continue;
                }
                Vector3 spawnPos = groundPos;

                GameObject instance = Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

                if (useRendererYOffset)
                {
                    Renderer renderer = instance.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        float yOffset = renderer.bounds.extents.y;
                        instance.transform.position = groundPos + Vector3.up * yOffset;
                    }
                }

                spawnSuccessCount++;
            }
        }

        Debug.Log($"✅ Tổng số object đã spawn: {spawnSuccessCount}");
    }

    Vector3 GetRandomPosition()
    {
        Vector2 offset = Random.insideUnitCircle * scatterRadius;
        return new Vector3(transform.position.x + offset.x, transform.position.y + spawnHeight, transform.position.z + offset.y);
    }
}
