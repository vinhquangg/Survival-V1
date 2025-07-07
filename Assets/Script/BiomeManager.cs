using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public BiomeRegion[] allBiomes;

    [Tooltip("Bật để in ra log khi không tìm thấy biome tại vị trí spawn")]
    public bool showMissingBiomeWarning = false;

    private void Start()
    {
        allBiomes = FindObjectsOfType<BiomeRegion>();

        // ✅ Sắp xếp theo vị trí X hoặc tên biome để ổn định
        System.Array.Sort(allBiomes, (a, b) =>
            a.transform.position.sqrMagnitude.CompareTo(b.transform.position.sqrMagnitude));

        Debug.Log($"🌿 Đã tìm thấy {allBiomes.Length} vùng biome.");
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

        if (showMissingBiomeWarning)
        {
            Debug.LogWarning($"⚠️ Không tìm thấy biome tại vị trí: {worldPosition}");
        }

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
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = biome.biomeData.fogColor;
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(col.bounds.center + Vector3.up * 2, biome.biomeData.name, style);
#endif
                }
            }
        }
    }
#endif
}
