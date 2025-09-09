using UnityEngine;

public abstract class BaseAttackState : PlayerState
{
    protected float attackDuration = 0.5f;
    protected float attackTimer = 0f;
    protected bool useAttackTimer = true;
    public BaseAttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Enter()
    {
        attackTimer = attackDuration;

        if (player.equipManager == null)
        {
            Debug.LogError("[BaseAttackState] player.equipManager is NULL!");
            return;
        }

        var equipped = player.equipManager.GetEquippedItem(EquipType.Weapon);
        Debug.Log($"[BaseAttackState] Equipped Weapon: {(equipped != null ? equipped.itemName : "None")}");

        if (equipped != null)
        {
            OnAttackEnter(equipped);
            player.animationController.SetUpperBodyLayerWeight(1f);
        }
        else
        {
            player.animationController.SetUpperBodyLayerWeight(0f);
            Debug.LogWarning("[BaseAttackState] Không thể attack vì chưa cầm vũ khí.");
        }
    }

    public override void Update()
    {
        player.ApplyGravity();
        player.HandleLook();

        Vector2 moveInput = player.inputHandler.playerAction.Move.ReadValue<Vector2>();
        Vector3 moveDirection = player.transform.right * moveInput.x + player.transform.forward * moveInput.y;

        bool isRunning = moveInput.magnitude >= 0.1f && Input.GetKey(KeyCode.LeftShift);
        float speedMultiplier = isRunning ? 1f : 0.5f;
        float finalSpeed = player.moveSpeed * speedMultiplier;

        player.controller.Move(moveDirection * finalSpeed * Time.deltaTime);

        player.animationController.UpdateAnimationState(moveInput, isRunning, finalSpeed);

        if (useAttackTimer)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                if (moveInput.magnitude >= 0.1f)
                    playerState.ChangeState(new MovementState(playerState, player));
                else
                    playerState.ChangeState(new IdleState(playerState, player));
            }
        }

    }

    public override void Exit()
    {
        player.inputHandler.EnablePlayerInput();
        //player.animationController.DisableUpperBodyLayerDelayed(0.05f);
    }

    /// <summary>
    /// Override trong state con để gọi anim/logic riêng của từng loại vũ khí
    /// </summary>
    protected abstract void OnAttackEnter(ItemClass equippedWeapon);
}
