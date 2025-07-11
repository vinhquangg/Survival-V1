using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState : PlayerState
{
    public MovementState(PlayerStateMachine playerState, PlayerController player) : base(playerState, player) { }

    public override void Update()
    {
        Vector2 moveInput = player.inputHandler.playerAction.Move.ReadValue<Vector2>();

        if(moveInput.magnitude < 0.1f)
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        Vector3 moveDirection = player.transform.right * moveInput.x + player.transform.forward * moveInput.y;
        bool isRunning = moveInput.magnitude >= 0.1f && Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0f;

        float speedMultiplier = isRunning ? 1f : 0.5f;

        float finalSpeed = player.moveSpeed * speedMultiplier;

        Vector3 movement = moveDirection * finalSpeed * Time.deltaTime;
        player.controller.Move(movement);

        // truyền tốc độ thực tế vào Animator
        player.animationController.UpdateAnimationState(moveInput, isRunning, finalSpeed);

        if (player.inputHandler.IsAttackInputPressed())
        {
            playerState.ChangeState(new AttackState(playerState,player));
        }

        player.ApplyGravity();
        player.HandleLook();
    }
}
