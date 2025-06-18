using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectScatterer : MonoBehaviour
{
    public BiomeManager biomeManager;
    public int baseSpawnCount = 1000;
    public LayerMask groundLayer;

    [Tooltip("Số lần thử spawn tối đa cho mỗi vùng nếu bị raycast fail")]
    public int maxAttemptsPerBiome = 500;

    [Tooltip("Chỉ spawn nếu vùng Grass chiếm tối thiểu bao nhiêu % (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float minGrassCoverage = 0.5f;

    [Tooltip("Index của texture Grass trong Terrain Layers")]
    public int grassTextureIndex = 1;

    private void Start()
    {
        ScatterObjectsPerBiome();
    }

    void ScatterObjectsPerBiome()
    {
        int totalSpawned = 0;
        int totalRaycastFail = 0;
        int totalPrefabFail = 0;
        int totalNotGrass = 0;

        BiomeRegion[] regions = biomeManager.allBiomes;
        if (regions == null || regions.Length == 0)
        {
            Debug.LogWarning("❌ Không có vùng biome nào được tìm thấy.");
            return;
        }

        Transform forestParent = GameObject.FindWithTag("Forest")?.transform;
        if (forestParent == null)
        {
            Debug.LogWarning("❌ Không tìm thấy GameObject có tag 'Forest' để gán object làm con.");
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

                if (Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundLayer))
                {
                    Terrain terrain = hit.collider.GetComponent<Terrain>();
                    if (terrain != null && !IsGrassTexture(hit.point, terrain))
                    {
                        totalNotGrass++;
                        attempts++;
                        continue;
                    }

                    GameObject prefab = biomeData.spawnableObjects[Random.Range(0, biomeData.spawnableObjects.Length)];
                    if (prefab == null)
                    {
                        totalPrefabFail++;
                        attempts++;
                        continue;
                    }

                    Vector3 spawnPos = hit.point;
                    GameObject obj = Instantiate(prefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

                    if (forestParent != null)
                        obj.transform.SetParent(forestParent);

                    if (IsBlockingObject(prefab))
                    {
                        AddNavMeshObstacle(obj);
                    }

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

        Debug.Log($"✅ Tổng object đã spawn: {totalSpawned} / {baseSpawnCount} | ❌ Raycast fail: {totalRaycastFail}, Prefab null: {totalPrefabFail}, Not Grass: {totalNotGrass}");
    }


    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.center.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    bool IsGrassTexture(Vector3 worldPos, Terrain terrain)
    {
        Vector3 terrainPos = worldPos - terrain.transform.position;
        TerrainData data = terrain.terrainData;

        int mapX = Mathf.FloorToInt((terrainPos.x / data.size.x) * data.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / data.size.z) * data.alphamapHeight);

        float[,,] splat = data.GetAlphamaps(mapX, mapZ, 1, 1);
        float grassStrength = splat[0, 0, grassTextureIndex];

        return grassStrength >= minGrassCoverage;
    }

    bool IsBlockingObject(GameObject prefab)
    {

        return prefab.CompareTag("Obstacle"); 
    }
    void AddNavMeshObstacle(GameObject obj)
    {
        LODGroup lodGroup = obj.GetComponentInChildren<LODGroup>();
        Renderer rend = null;

        if (lodGroup != null)
        {
            LOD[] lods = lodGroup.GetLODs();
            if (lods.Length > 0 && lods[0].renderers.Length > 0)
            {
                rend = lods[0].renderers[0]; 
            }
        }
        else
        {
            rend = obj.GetComponentInChildren<Renderer>(); 
        }

        if (rend == null)
        {
            Debug.LogWarning($"❌ Không tìm thấy Renderer trong object {obj.name}, không thêm NavMeshObstacle.");
            return;
        }

        Bounds bounds = rend.bounds;
        float height = bounds.size.y;
        float radius = Mathf.Max(bounds.size.x, bounds.size.z) * 0.5f;

        NavMeshObstacle obstacle = obj.AddComponent<NavMeshObstacle>();
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.height = height;
        obstacle.radius = radius * 0.01f;
        if(obstacle.radius < 0.14)
        {
            obstacle.radius = radius * 0.16f;
        }
        obstacle.center = new Vector3(0, height * 0.5f, 0);
        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;
    }

}
