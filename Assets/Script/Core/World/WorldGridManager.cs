//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WorldGridManager : MonoBehaviour
//{
//    public float cellSize = 100f;
//    public int gridWidth = 10;   
//    public int gridHeight = 10;  
//    public GameObject player;
//    public ObjectScatterer scattererData;
//    public LayerMask terrainLayer;
//    public int grassLayerIndex = 1; 
//    public float grassThreshold = 0.5f; 
//    private ForestCell[,] grid;

//    void Awake()
//    {
//        Terrain terrain = Terrain.activeTerrain;
//        Vector3 terrainSize = terrain.terrainData.size;

//        gridWidth = Mathf.CeilToInt(terrainSize.x / cellSize);
//        gridHeight = Mathf.CeilToInt(terrainSize.z / cellSize);

//        grid = new ForestCell[gridWidth, gridHeight];

//        for (int x = 0; x < gridWidth; x++)
//        {
//            for (int z = 0; z < gridHeight; z++)
//            {
//                Vector3 rayOrigin = new Vector3(
//                    x * cellSize + cellSize / 2f,
//                    1000f,
//                    z * cellSize + cellSize / 2f
//                );

//                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 2000f, terrainLayer))
//                {
//                    Vector3 worldPos = hit.point;
//                    TerrainData data = terrain.terrainData;
//                    Vector3 terrainPos = worldPos - terrain.transform.position;

//                    int mapX = Mathf.FloorToInt((terrainPos.x / data.size.x) * data.alphamapWidth);
//                    int mapZ = Mathf.FloorToInt((terrainPos.z / data.size.z) * data.alphamapHeight);

//                    mapX = Mathf.Clamp(mapX, 0, data.alphamapWidth - 1);
//                    mapZ = Mathf.Clamp(mapZ, 0, data.alphamapHeight - 1);

//                    float[,,] alphamaps = data.GetAlphamaps(mapX, mapZ, 1, 1);

//                    if (grassLayerIndex < alphamaps.GetLength(2))
//                    {
//                        float grassValue = alphamaps[0, 0, grassLayerIndex];

//                        if (grassValue >= grassThreshold)
//                        {
//                            grid[x, z] = new ForestCell(worldPos, cellSize);
//                            Debug.Log($"✅ Cell ({x},{z}) có Grass Layer Value = {grassValue}");
//                        }
//                        else
//                        {
//                            Debug.Log($"⛔ Skip cell ({x},{z}) - Không phải grass ({grassValue})");
//                        }
//                    }
//                    else
//                    {
//                        Debug.LogWarning($"⚠️ Layer index {grassLayerIndex} vượt quá số layer ({alphamaps.GetLength(2)})");
//                    }
//                }
//                else
//                {
//                    Debug.LogWarning($"❌ Không tìm thấy mặt đất tại ô ({x},{z})");
//                }
//            }
//        }

//    }


//    void Update()
//    {
//        Vector3 pos = player.transform.position;
//        ForestCell cell = GetCellAtPosition(pos);

//        if (cell != null && !cell.isSpawned)
//        {
//            cell.Scatter(
//                scattererData.biomeManager,
//                scattererData.groundLayer,
//                scattererData.baseSpawnCount,
//                scattererData.scatterRadius

//            );
//        }
//    }


//    public ForestCell GetCellAtPosition(Vector3 pos)
//    {
//        int x = Mathf.FloorToInt(pos.x / cellSize);
//        int z = Mathf.FloorToInt(pos.z / cellSize);

//        if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight) return null;

//        return grid[x, z];
//    }

//    private void OnDrawGizmos()
//    {
//        if (grid == null) return;

//        Gizmos.color = Color.green;
//        for (int x = 0; x < gridWidth; x++)
//        {
//            for (int z = 0; z < gridHeight; z++)
//            {
//                ForestCell cell = grid[x, z];
//                if (cell != null)
//                {
//                    Gizmos.DrawWireCube(cell.center + Vector3.up * 0.5f, new Vector3(cellSize, 1f, cellSize));
//                }
//            }
//        }
//    }

//}

