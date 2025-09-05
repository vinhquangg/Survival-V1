using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f; 
    private float attackTimer = 0f; 
    public AttackState(PlayerStateMachine playerState, PlayerController player) : base(playerState, player) { }

    public override void Enter()
    {
        attackTimer = attackDuration;
        if (player.equipManager == null)
        {
            Debug.LogError("[AttackState] player.equipManager is NULL!");
            return;
        }
        var equipped = player.equipManager.GetEquippedItem(EquipType.Weapon);
        Debug.Log($"[AttackState] Equipped Weapon: {(equipped != null ? equipped.itemName : "None")}");

        if (equipped != null)
        {
            player.animationController.TriggerAttack();
            player.animationController.SetUpperBodyLayerWeight(1f);
        }
        else
        {
            player.animationController.SetUpperBodyLayerWeight(0f);
            Debug.LogWarning("[AttackState] Không thể attack vì chưa cầm vũ khí.");
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


        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {

            if (moveInput.magnitude >= 0.1f)
                playerState.ChangeState(new MovementState(playerState, player));
            else
                playerState.ChangeState(new IdleState(playerState, player));
        }
    }


    public override void Exit()
    {
        player.inputHandler.EnablePlayerInput();
        //player.animationController.DisableUpperBodyLayerDelayed(0.05f);
    }

}
