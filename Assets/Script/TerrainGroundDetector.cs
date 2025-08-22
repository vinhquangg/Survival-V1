using UnityEngine;

public class TerrainGroundDetector : MonoBehaviour
{
    public GroundData[] groundMappings;
    private Terrain terrain;

    private void Awake()
    {
        terrain = Terrain.activeTerrain;
    }

    public GroundData GetGroundData(Vector3 pos)
    {
        if (terrain == null) return null;

        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = pos - terrain.transform.position;

        int mapX = Mathf.FloorToInt(terrainPos.x / data.size.x * data.alphamapWidth);
        int mapZ = Mathf.FloorToInt(terrainPos.z / data.size.z * data.alphamapHeight);

        float[,,] splatMaps = data.GetAlphamaps(mapX, mapZ, 1, 1);

        int layerIndex = 0;
        float maxWeight = 0f;
        for (int i = 0; i < data.terrainLayers.Length; i++)
        {
            if (splatMaps[0, 0, i] > maxWeight)
            {
                layerIndex = i;
                maxWeight = splatMaps[0, 0, i];
            }
        }

        TerrainLayer currentLayer = data.terrainLayers[layerIndex];

        foreach (var mapping in groundMappings)
        {
            if (mapping.terrainLayer == currentLayer)
                return mapping;
        }

        return null;
    }
}
