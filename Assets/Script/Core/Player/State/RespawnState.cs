using UnityEngine;

public class RespawnState : PlayerState
{
    private Vector3 respawnPoint = Vector3.zero;

    public RespawnState(PlayerStateMachine stateMachine, PlayerController player, Vector3 respawnPoint)
        : base(stateMachine, player)
    {
        this.respawnPoint = respawnPoint;
    }

    public override void Enter()
    {
        Debug.Log("Respawn player...");

        // Reset các stat
        PlayerStatus.Instance.health.ResetStat();
        PlayerStatus.Instance.hunger.ResetStat();
        PlayerStatus.Instance.thirst.ResetStat();

        // Đặt lại vị trí
        player.transform.position = respawnPoint;

        // Reset animation death
        player.animationController.ResetDead();

        // Bật lại controller và input
        player.controller.enabled = true;
        if (player.inputHandler != null)
            player.inputHandler.enabled = true;

        // Sau khi respawn → chuyển về Idle
        playerState.ChangeState(new IdleState(playerState, player));
    }

    public override void Update()
    {
        // RespawnState thường chỉ dùng cho Enter() thôi, không cần Update.
    }

    public override void Exit()
    {
        Debug.Log("Player respawned successfully!");
    }
}



