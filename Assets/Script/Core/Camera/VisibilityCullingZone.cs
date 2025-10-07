using System.Collections.Generic;
using UnityEngine;

public class VisibilityCullingZone : MonoBehaviour
{

    public float viewSize = 20f;            
    public int visibleRadius = 3;          
    public float bufferRadius = 0.5f;           
    public Transform player;                
    public float checkInterval = 0.25f;     

    private readonly List<ChunkObjectSpawner> allChunks = new();
    private Vector2Int lastPlayerChunkCoord = Vector2Int.zero;
    private Vector3 terrainOrigin = Vector3.zero;

    private float checkTimer = 0f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            enabled = false;
            return;
        }

        if (Terrain.activeTerrain != null)
            terrainOrigin = Terrain.activeTerrain.GetPosition();

        RefreshChunkList();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            UpdateChunkVisibility();
        }

        Vector2Int currentChunkCoord = GetChunkCoordFromPosition(player.position);
        if (currentChunkCoord != lastPlayerChunkCoord)
        {
            lastPlayerChunkCoord = currentChunkCoord;
            UpdateChunkVisibility();
        }
    }

    public void RefreshChunkList()
    {
        allChunks.Clear();

        GameObject[] containers = GameObject.FindGameObjectsWithTag("ChunkContainer");
        foreach (var container in containers)
        {
            ChunkObjectSpawner[] spawners = container.GetComponentsInChildren<ChunkObjectSpawner>();
            allChunks.AddRange(spawners);
        }

        Debug.Log("🔄 Cập nhật danh sách chunk spawner: " + allChunks.Count);
        UpdateChunkVisibility(force: true);
    }

    void UpdateChunkVisibility(bool force = false)
    {
        Vector3 relativePlayerPos = player.position - terrainOrigin;
        Vector2 playerPos2D = new Vector2(relativePlayerPos.x, relativePlayerPos.z);

        // 👉 thêm buffer vào bán kính hiển thị
        float maxDistance = (visibleRadius + bufferRadius) * viewSize;

        foreach (ChunkObjectSpawner chunk in allChunks)
        {
            if (chunk == null || chunk.name == "ChunkContainer") continue;

            Vector3 relativeChunkPos = chunk.transform.position - terrainOrigin;
            Vector2 chunkPos2D = new Vector2(relativeChunkPos.x, relativeChunkPos.z);
            float dist = Vector2.Distance(playerPos2D, chunkPos2D);

            bool isVisible = dist <= maxDistance;

            if (isVisible)
            {
                if (!chunk.HasSpawned || force)
                    chunk.SpawnObjects();
            }
            else
            {
                if (chunk.HasSpawned || force)
                    chunk.DespawnObjects();
            }
        }
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 pos)
    {
        Vector3 relativePos = pos - terrainOrigin;
        return new Vector2Int(
            Mathf.RoundToInt(relativePos.x / viewSize),
            Mathf.RoundToInt(relativePos.z / viewSize)
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, (visibleRadius + bufferRadius) * viewSize);
    }
}
