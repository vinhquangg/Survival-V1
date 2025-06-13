using System.Collections.Generic;
using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{
    [Header("Danh sách prefab có trọng số spawn")]
    public List<SpawnablePrefab> spawnablePrefabs;

    public int spawnCount = 50;
    public float scatterRadius = 20f;
    public float spawnHeight = 10f;
    public LayerMask groundLayer;

    private void Start()
    {
        ScatterObjects();
    }

    void ScatterObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                GameObject selectedPrefab = GetRandomPrefabByWeight();
                if (selectedPrefab == null) continue;

                float offset = selectedPrefab.GetComponent<Renderer>()?.bounds.extents.y ?? 0.5f;
                Vector3 spawnPos = hit.point + Vector3.up * offset;

                Instantiate(selectedPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
    }

    GameObject GetRandomPrefabByWeight()
    {
        int totalWeight = 0;
        foreach (var prefab in spawnablePrefabs)
            totalWeight += prefab.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var prefab in spawnablePrefabs)
        {
            currentSum += prefab.weight;
            if (randomValue < currentSum)
                return prefab.prefab;
        }

        return null;
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
        Vector3 basePosition = transform.position;
        return new Vector3(basePosition.x + randomCircle.x, basePosition.y + spawnHeight, basePosition.z + randomCircle.y);
    }
}


[System.Serializable]
public class SpawnablePrefab
{
    public GameObject prefab;
    [Range(1, 100)]
    public int weight = 1;
}