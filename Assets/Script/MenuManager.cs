using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    [Header("Canvas References")]
    public GameObject pauseMenuCanvas;   // menu tạm dừng
    public GameObject ingameUICanvas;    // UI ingame (HUD...)

    private PlayerController playerController;
    public bool isMenuOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenuOpen)
            {
                OpenMenu();
            }
            else
            {
                ResumeGame();  // gọi hàm resume để đóng menu
            }
        }
    }

    public void OpenMenu()
    {
        ingameUICanvas.SetActive(false);      // ẩn HUD
        pauseMenuCanvas.SetActive(true);      // bật menu
        isMenuOpen = true;

        GameManager.instance.SetState(GameState.Paused);
        GameManager.instance.SetCursorLock(false);
        playerController.inputHandler.DisablePlayerInput();
    }

    public void ResumeGame()
    {
        ingameUICanvas.SetActive(true);       // bật lại HUD
        pauseMenuCanvas.SetActive(false);     // ẩn menu
        isMenuOpen = false;

        GameManager.instance.SetState(GameState.Playing);
        GameManager.instance.SetCursorLock(true);
        playerController.inputHandler.EnablePlayerInput();
    }
}
