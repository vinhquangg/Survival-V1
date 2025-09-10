using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerInput InputActions { get; private set; }
    public PlayerInput.PlayerActions playerAction { get; private set; }

    private bool CanAttack = true;
    private bool CanInteract = true;
    private bool CanCraft = true;
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

    //------Interact Controller------

    public void EnableInteractInput() => CanInteract = true;
    public void DisableInteractInput() => CanInteract = false;

    public bool IsInteractPressed()
    {
        return CanInteract && playerAction.Interact.triggered;
    }

    //------Crafting Controller------
    public void EnableCraftInput() => CanCraft = true;
    public void DisableCraftInput() => CanCraft = false;


    public bool IsCraftPressed()
    {
        return CanCraft && playerAction.Crafting.triggered;
    }
}