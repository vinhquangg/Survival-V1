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
        player.inputHandler.DisablePlayerInput();
        attackTimer = attackDuration;
        player.animationController.TriggerAttack();
    }

    public override void Update()
    {
        player.ApplyGravity();
        player.HandleLook();

        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0f)
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }
    }

    public override void Exit()
    {
        player.inputHandler.EnablePlayerInput();
    }
}
