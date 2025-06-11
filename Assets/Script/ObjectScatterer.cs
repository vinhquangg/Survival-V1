using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int spawnCount;
    public float scatterRadius;

    public float spawnHeight;

    public LayerMask groundLayer;

    void Start()
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
                float heightOffset = objectToSpawn.GetComponent<Renderer>().bounds.extents.y;
                Vector3 spawnPos = hit.point + Vector3.up * heightOffset;
                Instantiate(objectToSpawn, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

            }
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
        Vector3 basePosition = transform.position;
        return new Vector3(basePosition.x + randomCircle.x, basePosition.y + spawnHeight, basePosition.z + randomCircle.y);
    }
}
