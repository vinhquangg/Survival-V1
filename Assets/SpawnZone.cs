using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 5;
    public float spawnCooldown = 3f;
    public float spawnRadius = 10f;
    public LayerMask groundLayer;
    public List<string> forbiddenTags = new List<string>();

    // --- activeEnemies có public để debug trong inspector nếu cần
    public List<GameObject> activeEnemies = new List<GameObject>();

    private float lastSpawnTime = 0f;
    private bool isEnabled = true;
    private bool spawnedOnce = false;

    [Header("Zone Type")]
    public bool respawnEnabled = true;

    // --- EDIT HERE: nếu zone này có boss (ví dụ Dragon zone) thì bật true
    public bool hasBoss = false;

    public int ActiveEnemyCount => activeEnemies.Count;

    public void EnableZone(bool enable)
    {
        isEnabled = enable;

        if (!enable)
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                    ObjectPoolManager.Instance.ReturnToPool(enemy);
            }
            activeEnemies.Clear();
            spawnedOnce = false;
        }
    }

    private void Update()
    {
        if (!isEnabled) return;
        TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        if (Time.time < lastSpawnTime + spawnCooldown) return;

        if (respawnEnabled)
        {
            // Zone respawn: luôn giữ đủ số lượng
            if (activeEnemies.Count >= maxEnemies) return;

            int toSpawn = maxEnemies - activeEnemies.Count;
            SpawnEnemies(toSpawn);
        }
        else
        {
            // Zone clearable: chỉ spawn 1 batch
            if (spawnedOnce) return;

            SpawnEnemies(maxEnemies); // spawn nguyên batch
            spawnedOnce = true;
        }

        lastSpawnTime = Time.time;
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            PoolableObject poolable = enemyPrefab.GetComponent<PoolableObject>();
            if (poolable == null || string.IsNullOrEmpty(poolable.poolID))
            {
                Debug.LogError($"Prefab {enemyPrefab.name} chưa có PoolableObject hoặc poolID rỗng!");
                continue;
            }

            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(poolable.poolID, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);

            BaseMonster enemyBase = enemy.GetComponent<BaseMonster>();
            if (enemyBase != null)
            {
                System.Action<BaseMonster> deathHandler = null;
                deathHandler = (deadMonster) =>
                {
                    activeEnemies.Remove(deadMonster.gameObject);
                    enemyBase.OnDeath -= deathHandler;

                    // Nếu là clearable zone → xử lý khi quái cuối cùng chết
                    if (!respawnEnabled && activeEnemies.Count == 0)
                    {
                        if (hasBoss)
                        {
                            TrySpawnBoss(deadMonster.transform.position);
                        }
                        else
                        {
                            IZoneDropHandler handler = GetComponent<IZoneDropHandler>();
                            if (handler != null)
                            {
                                handler.OnZoneCleared(this, deadMonster.transform.position);
                            }
                        }
                    }
                };

                enemyBase.OnDeath += deathHandler;
                enemyBase.OnSpawned();
            }
        }
    }

    private void TrySpawnBoss(Vector3 pos)
    {
        StartCoroutine(SpawnBoss(pos, 1f)); // delay 1s (tùy chỉnh)
    }

    private IEnumerator SpawnBoss(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject dragonPrefab = enemyPrefabs[0];

        PoolableObject poolable = dragonPrefab.GetComponent<PoolableObject>();
        if (poolable == null || string.IsNullOrEmpty(poolable.poolID))
        {
            Debug.LogError($"Prefab {dragonPrefab.name} chưa có PoolableObject hoặc poolID rỗng!");
            yield break;
        }

        GameObject boss = ObjectPoolManager.Instance.SpawnFromPool(poolable.poolID, pos, Quaternion.identity);

        DragonMonster bossDragon = boss.GetComponent<DragonMonster>();
        if (bossDragon != null)
        {
            bossDragon.isBoss = true;
            bossDragon.OnSpawned();

            boss.transform.localScale = dragonPrefab.transform.localScale * 1.5f;
        }

        activeEnemies.Add(boss);

        BaseMonster bossBase = boss.GetComponent<BaseMonster>();
        if (bossBase != null)
        {
            System.Action<BaseMonster> bossDeathHandler = null;
            bossDeathHandler = (deadBoss) =>
            {

                activeEnemies.Remove(deadBoss.gameObject);
                bossBase.OnDeath -= bossDeathHandler;

                IZoneDropHandler handler = GetComponent<IZoneDropHandler>();
                if (handler != null)
                {
                    handler.OnZoneCleared(this, deadBoss.transform.position);
                }
            };

            bossBase.OnDeath += bossDeathHandler;
        }

        Debug.Log("Boss Dragon spawned (reused prefab)!");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y + 20f;

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                if (forbiddenTags.Contains(hit.collider.tag))
                {
                    continue;
                }

                return hit.point;
            }
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
