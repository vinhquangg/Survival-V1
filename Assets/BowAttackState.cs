using UnityEngine;

public class BowAttackState : BaseAttackState
{
    public BowAttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    private bool isAiming = false;

    public override void Enter()
    {
        base.Enter();
        isAiming = true;

        // Bắt đầu Aim (loop)
        player.animationController.StartAim();
    }

    public override void Update()
    {
        base.Update();

        // Nếu còn giữ chuột trái → tiếp tục Aim
        if (player.inputHandler.playerAction.Attack.ReadValue<float>() > 0f)
        {
            // Có thể add logic "charge lực bắn" ở đây
            return;
        }

        // Nếu thả chuột → bắn
        if (isAiming)
        {
            ShootArrow();
            isAiming = false;
        }
    }

    private void ShootArrow()
    {
        // Anim bắn + recoil
        player.animationController.ReleaseBow();

        // Spawn arrow
        Debug.Log("[BowAttackState] Shoot Arrow!");
        // TODO: ObjectPool.Spawn(arrowPrefab, bowSocket.position, bowSocket.rotation);

        // Sau khi bắn → Idle
        playerState.ChangeState(new IdleState(playerState, player));
    }

    protected override void OnAttackEnter(ItemClass equippedWeapon)
    {
        Debug.Log("[BowAttackState] Attack with bow.");
        // Không gọi recoil ở đây nữa, tránh double
    }

    public override void Exit()
    {
        base.Exit();
        isAiming = false;
        player.animationController.StopAimImmediate();
    }
}
