using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public BiomeRegion[] allBiomes;

    private void Start()
    {
        allBiomes = FindObjectsOfType<BiomeRegion>();
    }

    public BiomeData GetBiomeAtPosition(Vector3 worldPosition)
    {
        foreach (BiomeRegion biome in allBiomes)
        {
            Collider col = biome.GetComponent<Collider>();
            if (col != null && col.bounds.Contains(worldPosition))
            {
                return biome.biomeData;
            }
        }

        Debug.LogWarning("Không tìm thấy biome tại vị trí: " + worldPosition);
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        BiomeRegion[] biomes = FindObjectsOfType<BiomeRegion>();
        foreach (BiomeRegion biome in biomes)
        {
            if (biome.biomeData != null)
            {
                Gizmos.color = biome.biomeData.fogColor;
                Collider col = biome.GetComponent<Collider>();
                if (col != null)
                {
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                }
            }
        }
    }
#endif
}
