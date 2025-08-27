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

        // Nếu giữ LeftShift + tiến lên → RunState
        if (Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0f)
        {
            playerState.ChangeState(new RunState(playerState, player));
            return;
        }

        Vector3 moveDir = player.transform.right * moveInput.x + player.transform.forward * moveInput.y;
        float finalSpeed = player.moveSpeed * 0.5f; // đi bộ

        player.controller.Move(moveDir * finalSpeed * Time.deltaTime);

        // 🎯 Cập nhật animation thông qua AnimationStateController
        player.animationController.UpdateAnimationState(moveInput, false, finalSpeed);

        if (player.inputHandler.IsAttackInputPressed())
            playerState.ChangeState(new AttackState(playerState, player));

        player.ApplyGravity();
        player.HandleLook();
        HandleFootstep(isMoving);
    }


    private void HandleFootstep(bool isMoving)
    {
        if (isMoving)
            FootStepManager.Instance.PlayFootstep(player.transform.position, false); // đi bộ
        //else
        //    FootStepManager.Instance.StopFootstep();
    }
}
