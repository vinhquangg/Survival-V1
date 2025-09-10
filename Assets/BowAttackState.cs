using UnityEngine;

public class BowAttackState : BaseAttackState
{
    private bool isAiming = false;

    public BowAttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Enter()
    {
        base.Enter();
         player.inputHandler.playerAction.Move.Disable();
        useAttackTimer = false;
        isAiming = true;
        player.animationController.StartAim();


        // Spawn arrow hiển thị khi kéo cung
        if (player.combat.arrowPrefab != null && player.combat.arrowSpawnPoint != null)
        {
            player.combat.currentArrow = GameObject.Instantiate(
                player.combat.arrowPrefab,
                player.combat.arrowSpawnPoint.position,
                player.combat.arrowSpawnPoint.rotation
            );
            player.combat.currentArrow.transform.SetParent(player.combat.arrowSpawnPoint);

            Rigidbody rb = player.combat.currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // tạm tắt vật lý
            }

            Collider col = player.combat.currentArrow.GetComponent<Collider>();
            if (col != null && player.controller != null)
            {
                Physics.IgnoreCollision(col, player.controller);
            }
        }

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

        player.combat.ShootArrow();

        player.animationController.ReleaseBow(); // fire Recoil Trigger

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
        player.inputHandler.playerAction.Move.Enable();
        isAiming = false;
        player.animationController.StopAimImmediate();
    }
}
