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
        //player.inputHandler.DisablePlayerInput();
        attackTimer = attackDuration;
        player.animationController.TriggerAttack();
        player.animationController.SetUpperBodyLayerWeight(1f);
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
