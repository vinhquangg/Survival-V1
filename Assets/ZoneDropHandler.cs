using UnityEngine;

[System.Serializable]
public class ZoneLoot
{
    public string poolID;
    public int amount = 1;
}

public class ZoneDropHandler : MonoBehaviour, IZoneDropHandler
{
    [Header("Loot Settings")]
    public ZoneLoot[] lootOnClear;

    [Header("Ground Check")]
    public LayerMask groundLayer; // gán Ground layer trong Inspector

    [Header("Drop Offset")]
    public float groundOffset = 0.1f; // chỉnh trong Inspector nếu cần

    public void OnZoneCleared(SpawnZone zone, Vector3 lastDeathPos)
    {
        Debug.Log($"Zone {zone.name} cleared → drop loot!");

        foreach (var loot in lootOnClear)
        {
            for (int i = 0; i < loot.amount; i++)
            {
                Vector3 spawnPos = lastDeathPos;

                // Raycast xuống tìm mặt đất trong layer Ground
                if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                {
                    spawnPos = hit.point + Vector3.up * groundOffset;
                }

                // Spawn item từ pool
                GameObject obj = ObjectPoolManager.Instance.SpawnFromPool(
                    loot.poolID,
                    spawnPos,
                    Quaternion.identity
                );
            }
        }
    }
}
