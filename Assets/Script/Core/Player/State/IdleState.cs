using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine playerState, PlayerController player) : base(playerState, player) { }

    public override void Enter()
    {
        base.Enter();
        if (FootStepManager.Instance != null)
        {
            FootStepManager.Instance.AttachToActor(player.transform);
            FootStepManager.Instance.StopFootstep();
        }
    }

    public override void Update()
    {
        Vector2 moveInput = player.inputHandler.playerAction.Move.ReadValue<Vector2>();
        
        if(moveInput.magnitude > 0.1f)
        {
            playerState.ChangeState(new MovementState(playerState, player));
            return; 
        }

        player.animationController.UpdateAnimationState(Vector2.zero, false, 0f);

        if (player.inputHandler.IsAttackInputPressed())
        {
            var weapon = player.equipManager.GetEquippedItem(EquipType.Weapon) as WeaponClass;
            if (weapon != null)
            {
                switch (weapon.weaponType)
                {
                    case WeaponClass.WeaponType.Bow:
                        playerState.ChangeState(new BowAttackState(playerState, player));
                        break;

                    case WeaponClass.WeaponType.Machete:
                        playerState.ChangeState(new AttackState(playerState, player));
                        break;
                    case WeaponClass.WeaponType.Sword:
                        break;
                        // nếu sau này có Tool thì gọi state riêng
                }
            }
        }

        player.ApplyGravity();
        player.HandleLook();
    }

    public override void Exit()
    {
        // Code to execute when exiting the idle state
        Debug.Log("Exiting Idle State");
    }
}

