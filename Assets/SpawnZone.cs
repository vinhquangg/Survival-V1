using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;   // --- EDIT HERE: gán prefab quái
    public int maxEnemies = 5;          // số quái tối đa trong zone
    public float spawnCooldown = 3f;    // thời gian hồi spawn
    public float spawnRadius = 10f;     // bán kính spawn trong zone
    public LayerMask groundLayer;
    public List<string> forbiddenTags = new List<string>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnTime = 0f;
    private bool isEnabled = true;

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
        }

        // nếu disable zone thì clear quái còn sống (tuỳ thiết kế)
        // foreach (var enemy in activeEnemies) Destroy(enemy);
        // activeEnemies.Clear();
    }

    private void Update()
    {
        if (!isEnabled) return;

        TrySpawnEnemy();
    }

    private void TrySpawnEnemy()
    {
        if (activeEnemies.Count >= maxEnemies) return;
        if (Time.time < lastSpawnTime + spawnCooldown) return;

        // chọn prefab
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        PoolableObject poolable = enemyPrefab.GetComponent<PoolableObject>();
        if (poolable == null || string.IsNullOrEmpty(poolable.poolID))
        {
            Debug.LogError($"Prefab {enemyPrefab.name} chưa có PoolableObject hoặc poolID rỗng!");
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();

        // spawn từ pool
        GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(
            poolable.poolID,
            spawnPos,
            Quaternion.identity
        );
        activeEnemies.Add(enemy);

        // xử lý khi chết → remove + trả về pool
        BaseMonster enemyBase = enemy.GetComponent<BaseMonster>();
        PoolableObject spawnedPoolable = enemy.GetComponent<PoolableObject>();

        if (enemyBase != null && spawnedPoolable != null)
        {
            enemyBase.OnSpawned();
            enemyBase.OnDeath += () =>
            {
                ObjectPoolManager.Instance.ReturnToPool(enemy);
                activeEnemies.Remove(enemy);
            };
        }

        lastSpawnTime = Time.time;
    }


    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++) // thử tối đa 10 lần để tìm chỗ hợp lệ
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = transform.position.y + 20f;

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                // nếu hit phải tag cấm → skip
                if (forbiddenTags.Contains(hit.collider.tag))
                {
                    continue;
                }

                return hit.point; // hợp lệ → return
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
