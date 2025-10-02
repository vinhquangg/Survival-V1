using System.Collections;
using UnityEngine;

public class BowAttackState : BaseAttackState
{
    private bool isAiming = false;
    private bool isShoot = false;
    private float aimStartTime;
    private float minAimTime = 0.15f;

    public BowAttackState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Enter()
    {
        base.Enter();

        // Nếu đang cooldown → không bắn
        if (!player.combat.CanShoot())
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        // ❗ Kiểm tra có arrow trong inventory không
        if (player.combat.currentAmmo == null ||
            !player.combat.HasArrow(player.combat.currentAmmo))
        {
            Debug.Log("Không còn mũi tên để bắn!");
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        player.inputHandler.playerAction.Move.Disable();

        useAttackTimer = false;
        aimStartTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // 🟢 Nếu đang giữ chuột
        if (player.inputHandler.IsAttackHeld())
        {
            float holdTime = Time.time - aimStartTime;

            // Chỉ bắt đầu Aim khi đủ thời gian
            if (!isAiming && holdTime >= minAimTime)
            {
                isAiming = true;
                player.animationController.StartAim();
                SpawnArrow(); // chỉ spawn khi đủ thời gian và còn đạn
            }

            return; // vẫn giữ chuột → chưa bắn
        }

        // 🔴 Nếu buông chuột
        if (isAiming && !isShoot && player.inputHandler.IsAttackReleased())
        {
            float holdTime = Time.time - aimStartTime;

            if (holdTime < minAimTime)
            {
                // Nếu chưa đủ thời gian → hủy Aim
                isAiming = false;
                player.animationController.StopAimImmediate();
                playerState.ChangeState(new IdleState(playerState, player));
                return;
            }

            // Nếu giữ đủ lâu → bắn
            isShoot = true;
            ShootArrow();
        }
    }

    private void SpawnArrow()
    {
        if (player.combat.currentAmmo == null) return;

        // ❗ Chặn nếu không còn arrow trong inventory
        if (!player.combat.HasArrow(player.combat.currentAmmo))
        {
            Debug.Log("Hết arrow → không spawn!");
            return;
        }

        var prefab = player.combat.currentAmmo.projectilePrefab;
        if (prefab == null || player.combat.arrowSpawnPoint == null) return;

        player.combat.currentArrow = GameObject.Instantiate(
            prefab,
            player.combat.arrowSpawnPoint.position,
            player.combat.arrowSpawnPoint.rotation
        );

        // Gắn vào bow để hiển thị
        player.combat.currentArrow.transform.SetParent(player.combat.arrowSpawnPoint);

        Rigidbody rb = player.combat.currentArrow.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = player.combat.currentArrow.GetComponent<Collider>();
        if (col != null && player.controller != null)
            Physics.IgnoreCollision(col, player.controller);
    }

    private void ShootArrow()
    {
        if (!isAiming) return;

        // Mark cooldown
        player.combat.MarkShootTime();

        isAiming = false;

        // Gọi bắn từ PlayerCombat
        player.combat.ShootArrow();

        // Animation
        player.animationController.ReleaseBow();
        player.animationController.StopAimImmediate();

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
        player.animationController.StopAimImmediate();
        player.animationController.ResetAttack();

        // Nếu đang Aim nhưng thoát state → cleanup arrow visual
        if (player.combat.currentArrow != null && !isShoot)
        {
            GameObject.Destroy(player.combat.currentArrow);
            player.combat.currentArrow = null;
        }

        isAiming = false;
        isShoot = false;
    }
}
