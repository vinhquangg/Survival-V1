using UnityEngine;

public class MovementState : PlayerState
{
    public MovementState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Update()
    {
        Vector2 moveInput = player.inputHandler.playerAction.Move.ReadValue<Vector2>();
        bool isMoving = moveInput.magnitude > 0.1f;

        if (!isMoving)
        {
            playerState.ChangeState(new IdleState(playerState, player));
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0f)
        {
            playerState.ChangeState(new RunState(playerState, player));
            return;
        }

        Vector3 moveDir = player.transform.right * moveInput.x + player.transform.forward * moveInput.y;
        float finalSpeed = player.moveSpeed * 0.5f; 

        player.controller.Move(moveDir * finalSpeed * Time.deltaTime);

        // 🎯 Cập nhật animation thông qua AnimationStateController
        player.animationController.UpdateAnimationState(moveInput, false, finalSpeed);

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
        HandleFootstep(isMoving);
    }


    private void HandleFootstep(bool isMoving)
    {
        if (isMoving)
            FootStepManager.Instance.PlayFootstep(player.transform.position, false); 
        else
            FootStepManager.Instance.StopFootstep();
    }
}
