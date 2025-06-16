using UnityEngine;

public class ForestCell
{
    public Vector3 center;
    public float size;

    public bool isActive = false;
    public bool isSpawned = false;

    // Optional: bạn có thể mở rộng thêm prefab, biome,... về sau

    public ForestCell(Vector3 center, float size)
    {
        this.center = center;
        this.size = size;
    }

    public void Scatter(BiomeManager biomeManager, LayerMask groundLayer, int count, float scatterRadius, float spawnHeight, bool useRendererYOffset)
    {
        if (isSpawned) return;

        int spawnSuccess = 0;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector3 spawnPos = new Vector3(center.x + offset.x, center.y + spawnHeight, center.z + offset.y);

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, spawnHeight * 2f, groundLayer))
            {
                Vector3 groundPos = hit.point;

                BiomeData biome = biomeManager.GetBiomeAtPosition(groundPos);
                if (biome == null || biome.spawnableObjects == null || biome.spawnableObjects.Length == 0) continue;

                GameObject prefab = biome.spawnableObjects[Random.Range(0, biome.spawnableObjects.Length)];
                if (prefab == null) continue;

                GameObject instance = GameObject.Instantiate(prefab, groundPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

                if (useRendererYOffset)
                {
                    Renderer rend = instance.GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        float yOffset = rend.bounds.extents.y;
                        instance.transform.position = groundPos + Vector3.up * yOffset;
                    }
                }

                spawnSuccess++;
            }
        }

        Debug.Log($"[Cell @ {center}] Spawned {spawnSuccess} objects.");
        isSpawned = true;
    }
}
