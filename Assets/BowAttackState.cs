using UnityEngine;

public class BowAttackState : BaseAttackState
{
    private bool isAiming = false;

    public BowAttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Enter()
    {
        base.Enter();
        useAttackTimer = false;
        isAiming = true;
        player.animationController.StartAim();
    }

    public override void Update()
    {
        base.Update();

        if (player.inputHandler.playerAction.Attack.ReadValue<float>() > 0f)
            return; // giữ chuột → tiếp tục Aim

        if (isAiming)
            ShootArrow();
    }

    private void ShootArrow()
    {
        if (!isAiming) return;
        isAiming = false;

        player.animationController.ReleaseBow(); // fire Recoil Trigger

        Debug.Log("[BowAttackState] Shoot Arrow!");
        // TODO: ObjectPool.Spawn(arrowPrefab, bowSocket.position, bowSocket.rotation);

        playerState.ChangeState(new IdleState(playerState, player));
    }

    protected override void OnAttackEnter(ItemClass equippedWeapon)
    {
        player.animationController.TriggerAttack(WeaponClass.WeaponType.Bow);
        // Không gọi recoil ở đây nữa, tránh double
    }
    public override void Exit()
    {
        base.Exit();
        isAiming = false;
        player.animationController.StopAimImmediate();
    }
}
