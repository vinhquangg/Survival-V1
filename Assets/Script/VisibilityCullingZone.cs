using UnityEngine;
using System.Collections.Generic;

public class VisibilityCullingZone : MonoBehaviour
{
    public float chunkSize = 100f;
    public int visibleRadius = 2;
    public string chunkTag = "Chunk";

    private List<GameObject> allChunks = new List<GameObject>();
    private Vector2Int lastPlayerChunkCoord = Vector2Int.zero;
    public Transform player;
    private Vector3 terrainOrigin = Vector3.zero;

    private float checkTimer = 0f;
    public float checkInterval = 1f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("❌ Không tìm thấy player!");
            enabled = false;
            return;
        }

        if (Terrain.activeTerrain != null)
            terrainOrigin = Terrain.activeTerrain.GetPosition();

        UpdateChunkVisibility(force: true);
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
        GameObject[] chunks = GameObject.FindGameObjectsWithTag(chunkTag);
        allChunks.Clear();
        allChunks.AddRange(chunks);
        Debug.Log("🔄 Chunk list cập nhật lại: " + allChunks.Count);
        UpdateChunkVisibility(force: true);
    }

    void UpdateChunkVisibility(bool force = false)
    {
        Vector3 relativePlayerPos = player.position - terrainOrigin;
        Vector2 playerPos2D = new Vector2(relativePlayerPos.x, relativePlayerPos.z);
        float maxDistance = visibleRadius * chunkSize;

        foreach (GameObject chunk in allChunks)
        {
            if (chunk == null) continue;

            Vector3 relativeChunkPos = chunk.transform.position - terrainOrigin;
            Vector2 chunkPos2D = new Vector2(relativeChunkPos.x, relativeChunkPos.z);
            float dist = Vector2.Distance(playerPos2D, chunkPos2D);
            bool isVisible = dist <= maxDistance;

            if (chunk.activeSelf != isVisible || force)
                chunk.SetActive(isVisible);
        }
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 pos)
    {
        Vector3 relativePos = pos - terrainOrigin;
        return new Vector2Int(
            Mathf.FloorToInt(relativePos.x / chunkSize),
            Mathf.FloorToInt(relativePos.z / chunkSize));
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, visibleRadius * chunkSize);
    }
}
