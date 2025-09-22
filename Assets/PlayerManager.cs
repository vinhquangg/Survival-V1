using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Player Prefab")]
    public GameObject playerPrefab;

    private GameObject currentPlayer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnPlayer(); // Spawn khi start game
    }

    public void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null; // 🔥 đảm bảo xoá tham chiếu
        }

        float playerHeight = 2f;
        Vector3 spawnPos = RespawnManager.Instance.GetRespawnPosition(playerHeight);

        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        var cc = currentPlayer.GetComponent<CharacterController>();
        if (cc != null) playerHeight = cc.height;

        // 🔥 Gắn camera target vào player vừa spawn
        if (CameraTarget.Instance != null)
        {
            CameraTarget.Instance.AttachToPlayer(currentPlayer.transform);
        }

        var playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // 🔥 Gọi SetPlayer cho InventoryManager trước
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SetPlayer(playerController);
            }

            // Sau đó mới gọi cho các script khác
            foreach (var dep in FindObjectsOfType<MonoBehaviour>())
            {
                if (dep is IPlayerDependent playerDependent && dep != InventoryManager.Instance)
                {
                    playerDependent.SetPlayer(playerController);
                }
            }
        }
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public Transform GetPlayerTransform()
    {
        return currentPlayer != null ? currentPlayer.transform : null;
    }

    public PlayerController GetPlayerController()
    {
        return currentPlayer != null ? currentPlayer.GetComponent<PlayerController>() : null;
    }
}
