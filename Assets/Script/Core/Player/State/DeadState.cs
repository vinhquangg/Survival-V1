using UnityEngine;

public class DeadState : PlayerState
{
    private float respawnDelay = 5f;   // thời gian chờ
    private float timer = 0f;

    public DeadState(PlayerStateMachine playerState, PlayerController player)
        : base(playerState, player) { }

    public override void Enter()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.playerDeadSound);
        timer = 0f; // reset timer mỗi lần vào DeadState
        player.animationController.TriggerDead();

        player.controller.enabled = false;
        if (player.inputHandler != null)
            player.inputHandler.enabled = false;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= respawnDelay)
        {
            // chuyển sang respawn state
            player.playerStateMachine.ChangeState(
                new RespawnState(player.playerStateMachine, player, Vector3.zero) // test tại (0,0,0)
            );
        }
    }

    public override void Exit()
    {
        // reset flag chết để chuẩn bị hồi sinh
        player.animationController.ResetDead();
    }
}
