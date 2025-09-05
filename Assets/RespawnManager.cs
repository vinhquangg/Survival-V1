using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Spawn Point (Main Island)")]
    public Transform spawnPoint;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float raycastHeight = 5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Trả về vị trí spawn hợp lệ trên mặt đất.
    /// </summary>
    public Vector3 GetRespawnPosition(float playerHeight = 2f)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnPoint chưa gán trong RespawnManager!");
            return Vector3.zero;
        }

        Vector3 checkPos = spawnPoint.position + Vector3.up * raycastHeight;

        if (Physics.Raycast(checkPos, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            return hit.point + Vector3.up * (playerHeight / 2f);
        }

        // fallback: nếu không raycast được thì dùng vị trí spawnpoint
        return spawnPoint.position + Vector3.up * (playerHeight / 2f);
    }
}
