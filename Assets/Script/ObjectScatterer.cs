using System.Collections.Generic;
using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{
    public BiomeManager biomeManager;
    public int baseSpawnCount = 1000;
    public LayerMask groundLayer;

    [Tooltip("Số lần thử spawn tối đa cho mỗi vùng nếu bị raycast fail")]
    public int maxAttemptsPerBiome = 500;

    [Range(0f, 1f)]
    public float minGrassCoverage = 0.5f;
    public int grassTextureIndex = 1;
    public float chunkSize = 100f;

    [Tooltip("Container chứa tất cả các chunk")]
    public Transform chunkContainer;

    private Vector3 terrainOrigin;
    private Dictionary<string, GameObject> chunkMap = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (Terrain.activeTerrain != null)
            terrainOrigin = Terrain.activeTerrain.GetPosition();

        if (chunkContainer == null)
        {
            GameObject container = GameObject.Find("ChunkContainer");
            if (container != null) chunkContainer = container.transform;
        }

        ScatterObjectsPerBiome();

        var cullingZone = FindObjectOfType<VisibilityCullingZone>();
        if (cullingZone != null)
            cullingZone.RefreshChunkList();
    }

    private void ScatterObjectsPerBiome()
    {
        int totalSpawned = 0;
        BiomeRegion[] regions = biomeManager.allBiomes;
        if (regions == null || regions.Length == 0) return;

        int spawnPerRegion = Mathf.CeilToInt((float)baseSpawnCount / regions.Length);

        for (int i = 0; i < regions.Length; i++)
        {
            BiomeRegion region = regions[i];
            Collider col = region.GetComponent<Collider>();
            BiomeData biomeData = region.biomeData;
            if (col == null || biomeData == null || biomeData.spawnableObjects == null) continue;

            Bounds bounds = col.bounds;
            int spawned = 0, attempts = 0;

            int regionSeed = WorldSeedManager.Seed
                             ^ i
                             ^ biomeData.name.GetHashCode();

            System.Random prng = new System.Random(regionSeed);

            while (spawned < spawnPerRegion && attempts < maxAttemptsPerBiome)
            {
                Vector3 randomPos = GetRandomPointInBounds(bounds, prng);
                if (Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundLayer))
                {
                    Debug.DrawLine(randomPos + Vector3.up * 50f, hit.point, Color.red, 10f);

                    Terrain terrain = hit.collider.GetComponent<Terrain>();
                    if (terrain != null && !IsGrassTexture(hit.point, terrain)) { attempts++; continue; }

                    GameObject prefab = biomeData.spawnableObjects[prng.Next(0, biomeData.spawnableObjects.Length)];
                    if (prefab == null) { attempts++; continue; };

                    GameObject chunk = GetOrCreateChunk(hit.point);

                    if (!chunk.TryGetComponent(out ChunkObjectSpawner spawner))
                        spawner = chunk.AddComponent<ChunkObjectSpawner>();

                    if (spawner.objectsToSpawn == null)
                        spawner.objectsToSpawn = new List<ChunkObjectSpawner.SpawnInfo>();

                    spawner.objectsToSpawn.Add(new ChunkObjectSpawner.SpawnInfo
                    {
                        tag = prefab.tag,
                        localPosition = chunk.transform.InverseTransformPoint(hit.point),
                        localRotation = new Vector3(0, (float)(prng.NextDouble() * 360f), 0)
                    });

                    spawned++;
                    totalSpawned++;
                }
                attempts++;

            }


        }

        Debug.Log($"✅ Tổng object đã chuẩn bị spawn bằng Pool: {totalSpawned}");
    }

    private GameObject GetOrCreateChunk(Vector3 worldPos)
    {
        int chunkX = Mathf.FloorToInt((worldPos.x - terrainOrigin.x) / chunkSize);
        int chunkZ = Mathf.FloorToInt((worldPos.z - terrainOrigin.z) / chunkSize);
        string name = $"Chunk_{chunkX}_{chunkZ}";

        if (chunkMap.TryGetValue(name, out GameObject chunk)) return chunk;

        chunk = new GameObject(name);
        chunk.transform.position = new Vector3(
            chunkX * chunkSize + chunkSize * 0.5f + terrainOrigin.x,
            0f,
            chunkZ * chunkSize + chunkSize * 0.5f + terrainOrigin.z);

        chunk.tag = "Chunk";
        if (chunkContainer != null)
            chunk.transform.SetParent(chunkContainer);
        else
            chunk.transform.SetParent(this.transform);

        chunkMap[name] = chunk;
        return chunk;
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds, System.Random prng)
    {
        float x = Mathf.Lerp(bounds.min.x, bounds.max.x, (float)prng.NextDouble());
        float z = Mathf.Lerp(bounds.min.z, bounds.max.z, (float)prng.NextDouble());
        return new Vector3(x, bounds.center.y, z);
    }

    private bool IsGrassTexture(Vector3 worldPos, Terrain terrain)
    {
        Vector3 terrainPos = worldPos - terrain.transform.position;
        TerrainData data = terrain.terrainData;
        int mapX = Mathf.FloorToInt((terrainPos.x / data.size.x) * data.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / data.size.z) * data.alphamapHeight);
        float[,,] splat = data.GetAlphamaps(mapX, mapZ, 1, 1);
        return splat[0, 0, grassTextureIndex] >= minGrassCoverage;
    }
}