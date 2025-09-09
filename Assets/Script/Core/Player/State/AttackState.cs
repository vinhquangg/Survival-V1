using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseAttackState
{
    public AttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    protected override void OnAttackEnter(ItemClass equippedWeapon)
    {
        Debug.Log("[MeleeAttackState] Attack with melee weapon.");
        player.animationController.TriggerAttack(WeaponClass.WeaponType.Machete);
        // TODO: Gọi hệ thống combat, apply damage, v.v.
    }
}
