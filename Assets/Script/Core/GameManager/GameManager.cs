using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetCursorLock(true);
    }

    public void SetCursorLock(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public bool IsCursorLocked()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        Time.timeScale = (newState == GameState.Paused) ? 0 : 1;
    }

}
