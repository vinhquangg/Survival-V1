using System.Collections.Generic;
using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{
    public BiomeManager biomeManager;
    public int baseSpawnCount = 1000;
    public LayerMask groundLayer;

    [Tooltip("Số lần thử spawn tối đa cho mỗi vùng nếu bị raycast fail")]
    public int maxAttemptsPerBiome = 500;

    private void Start()
    {
        ScatterObjectsPerBiome();
    }

    void ScatterObjectsPerBiome()
    {
        int totalSpawned = 0;
        int totalRaycastFail = 0;
        int totalPrefabFail = 0;

        BiomeRegion[] regions = biomeManager.allBiomes;
        if (regions == null || regions.Length == 0)
        {
            Debug.LogWarning("❌ Không có vùng biome nào được tìm thấy.");
            return;
        }

        int spawnPerRegion = Mathf.CeilToInt((float)baseSpawnCount / regions.Length);

        foreach (BiomeRegion region in regions)
        {
            Collider col = region.GetComponent<Collider>();
            BiomeData biomeData = region.biomeData;

            if (col == null || biomeData == null || biomeData.spawnableObjects == null || biomeData.spawnableObjects.Length == 0)
            {
                Debug.LogWarning($"⚠️ BiomeRegion {region.name} bị thiếu dữ liệu hoặc collider.");
                continue;
            }

            Bounds bounds = col.bounds;
            int spawnedInThisRegion = 0;
            int attempts = 0;

            while (spawnedInThisRegion < spawnPerRegion && attempts < maxAttemptsPerBiome)
            {
                Vector3 randomPos = GetRandomPointInBounds(bounds);

                // Raycast từ trên cao xuống mặt đất
                if (Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundLayer))
                {
                    GameObject prefab = biomeData.spawnableObjects[Random.Range(0, biomeData.spawnableObjects.Length)];
                    if (prefab == null)
                    {
                        totalPrefabFail++;
                        continue;
                    }

                    Vector3 spawnPos = hit.point;
                    Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                    spawnedInThisRegion++;
                    totalSpawned++;
                }
                else
                {
                    totalRaycastFail++;
                }

                attempts++;
            }

            Debug.Log($"🌱 Biome '{region.name}': Spawned {spawnedInThisRegion}/{spawnPerRegion} (Attempts: {attempts})");
        }

        Debug.Log($"✅ Tổng object đã spawn: {totalSpawned} / {baseSpawnCount} | ❌ Raycast fail: {totalRaycastFail}, Prefab null: {totalPrefabFail}");
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
