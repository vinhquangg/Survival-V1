using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput InputActions { get; private set; }
    public PlayerInput.PlayerActions playerAction { get; private set; }
    private bool CanAttack = true;
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

    //------Attack Controller------

    public void EnableAttackInput() => CanAttack = true;
    public void DisableAttackInput() => CanAttack = false;  

    public bool IsAttackInputPressed()
    {
        return CanAttack && playerAction.Attack.triggered;
    }
}