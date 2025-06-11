using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput InputActions { get; private set; }
    public PlayerInput.PlayerActions playerAction { get; private set; }

    private void Awake()
    {
        InputActions = new PlayerInput();
        playerAction = InputActions.Player;

        playerAction.Enable();
    }

    public void DisablePlayerInput()
    {
        playerAction.Disable();

    }

    public void EnablePlayerInput()
    {
        playerAction.Enable();

    }
}