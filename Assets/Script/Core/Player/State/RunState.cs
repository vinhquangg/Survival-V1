using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Update()
    {
        Vector2 moveInput = player.inputHandler.playerAction.Move.ReadValue<Vector2>();
        bool isMoving = moveInput.magnitude > 0.1f;

        // Ngừng di chuyển → Idle
        if (!isMoving)
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        // Thả LeftShift hoặc không tiến → quay lại Movement
        if (!Input.GetKey(KeyCode.LeftShift) || moveInput.y <= 0f)
        {
            playerState.ChangeState(new MovementState(playerState, player));
            return;
        }

        Vector3 moveDir = player.transform.right * moveInput.x + player.transform.forward * moveInput.y;
        player.controller.Move(moveDir * player.moveSpeed * Time.deltaTime); // full speed

        player.animationController.UpdateAnimationState(moveInput, true, player.moveSpeed);
        if (player.inputHandler.IsAttackInputPressed())
        {
            var weapon = player.equipManager.GetEquippedItem(EquipType.Weapon) as WeaponClass;
            if (weapon != null)
            {
                switch (weapon.weaponType)
                {
                    case WeaponClass.WeaponType.Bow:
                        playerState.ChangeState(new BowAttackState(playerState, player));
                        return;

                    case WeaponClass.WeaponType.Machete:
                        playerState.ChangeState(new AttackState(playerState, player));
                        return;
                    case WeaponClass.WeaponType.Sword:
                        break;
                        // nếu sau này có Tool thì gọi state riêng
                }
            }
        }

        player.ApplyGravity();
        player.HandleLook();

        // Footstep chạy
        FootStepManager.Instance.PlayFootstep(player.transform.position, true);
    }
}
