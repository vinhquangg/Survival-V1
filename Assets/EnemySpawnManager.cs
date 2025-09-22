using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;
    [Header("Global Setting")]
    public int maxEnemies;
    public float activeRadius;

    public List<SpawnZone> allZones = new List<SpawnZone>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Start is called before the first frame update
    }
    public void Start()
    {
        allZones.AddRange(FindObjectsOfType<SpawnZone>());
    }

    // Update is called once per frame
    void Update()
    {
        Transform player = PlayerManager.Instance.GetPlayerTransform();
        if (player == null) return;

        int totalEnemies = GetTotalEnemiesSpawn();
        foreach (SpawnZone zone in allZones)
        {
            float distanceToPlayer = Vector3.Distance(zone.transform.position, player.position);

            if (distanceToPlayer <= activeRadius && totalEnemies < maxEnemies)
            {
                zone.EnableZone(true);

                //int enemiesToSpawn = Mathf.Min(zone.spawnRate, maxEnemies - totalEnemies);
                //zone.SpawnEnemies(enemiesToSpawn);
                //totalEnemies += enemiesToSpawn;
            }
            else
            {
                zone.EnableZone(false);
            }
        }
    }

    public int GetTotalEnemiesSpawn()
    {
        int total = 0;
        foreach (SpawnZone zone in allZones)
        {
            total += zone.ActiveEnemyCount;
        }
        return total;
    }
}
