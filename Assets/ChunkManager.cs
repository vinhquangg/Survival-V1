using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;
    public float chunkSize = 100f;
    public Transform chunkContainer;

    private Vector3 terrainOrigin;
    private Dictionary<string, GameObject> chunkMap = new();

    private void Awake()
    {
        Instance = this;

        if (Terrain.activeTerrain != null)
            terrainOrigin = Terrain.activeTerrain.GetPosition();

        if (chunkContainer == null)
        {
            GameObject container = new GameObject("ChunkContainer");
            chunkContainer = container.transform;
        }
    }

    public GameObject GetOrCreateChunk(Vector3 worldPos)
    {
        int chunkX = Mathf.FloorToInt((worldPos.x - terrainOrigin.x) / chunkSize);
        int chunkZ = Mathf.FloorToInt((worldPos.z - terrainOrigin.z) / chunkSize);
        string name = $"Chunk_{chunkX}_{chunkZ}";

        if (chunkMap.TryGetValue(name, out GameObject chunk))
            return chunk;

        chunk = new GameObject(name);
        chunk.transform.position = new Vector3(
            chunkX * chunkSize + chunkSize * 0.5f + terrainOrigin.x,
            0f,
            chunkZ * chunkSize + chunkSize * 0.5f + terrainOrigin.z
        );

        chunk.tag = "Chunk";
        chunk.transform.SetParent(chunkContainer);

        chunkMap[name] = chunk;
        return chunk;
    }

    public GameObject GetChunkByWorldPosition(Vector3 worldPos)
    {
        int chunkX = Mathf.FloorToInt((worldPos.x - terrainOrigin.x) / chunkSize);
        int chunkZ = Mathf.FloorToInt((worldPos.z - terrainOrigin.z) / chunkSize);
        string name = $"Chunk_{chunkX}_{chunkZ}";
        chunkMap.TryGetValue(name, out GameObject chunk);
        return chunk;
    }
}
