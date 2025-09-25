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

        if (!player.combat.CanShoot())
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        player.inputHandler.playerAction.Move.Disable();
        useAttackTimer = false;
        aimStartTime = Time.time;
        //isAiming = true;
        //player.animationController.StartAim();
        SpawnArrow();

    }


    public override void Update()
    {
        base.Update();

        // Nếu đang giữ chuột
        if (player.inputHandler.IsAttackHeld())
        {
            float holdTime = Time.time - aimStartTime;

            // Đủ thời gian mới bắt đầu Aim
            if (!isAiming && holdTime >= minAimTime)
            {
                isAiming = true;
                player.animationController.StartAim();
                SpawnArrow();
            }
            //playerState.ChangeState(new IdleState(playerState, player));
            return; // vẫn đang giữ thì chưa bắn
        }


        // Nếu buông chuột
        if (isAiming && !isShoot && player.inputHandler.IsAttackReleased())
        {
            float holdTime = Time.time - aimStartTime;

            if (holdTime < minAimTime)
            {
                // Bỏ qua, không bắn cũng không recoil
                isAiming = false;
                //player.animationController.ResetAttack();
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
                rb.isKinematic = true;
            }

            Collider col = player.combat.currentArrow.GetComponent<Collider>();
            if (col != null && player.controller != null)
            {
                Physics.IgnoreCollision(col, player.controller);
            }
        }
    }

    // Coroutine ép quay lại Idle sau 1 delay ngắn
    private IEnumerator ForceIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isAiming && !isShoot) // vẫn còn trong trạng thái Aim mà chưa bắn
        {
            isAiming = false;
            player.animationController.StopAimImmediate();
            playerState.ChangeState(new IdleState(playerState, player));
        }
    }



    private void ShootArrow()
    {
        if (!isAiming) return;

        player.combat.MarkShootTime(); // cập nhật cooldown

        isAiming = false;
        player.combat.ShootArrow();
        player.animationController.ReleaseBow();
        player.animationController.StopAimImmediate(); // reset anim
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
        isAiming = false;
        isShoot = false;

    }
}
