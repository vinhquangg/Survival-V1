using UnityEngine;

public class ChopState : PlayerState
{
    private TreeChopInteraction treeChop;
    private bool isChopping = false;
    public TreeChopInteraction TargetTree => treeChop;

    public ChopState(PlayerStateMachine playerState, PlayerController player, TreeChopInteraction treeChop)
        : base(playerState, player)
    {
        this.treeChop = treeChop;
    }

    public override void Enter()
    {
        player.inputHandler.DisablePlayerInput();
        player.animationController.TriggerChop();

        if (treeChop != null)
            player.RotateTowards(treeChop.transform.position, 8f);

        isChopping = true;
    }

    public override void Update() { }

    public override void Exit()
    {
        if (isChopping)
        {
            isChopping = false;
            player.animationController.ResetChop();
            player.inputHandler.EnablePlayerInput();
        }
    }

    public void OnChopImpact()
    {
        if (!isChopping) return;
    }

    // Gọi từ Animation Event -> ChopEnd()
    public void OnChopEnd()
    {
        if (!isChopping) return;

        if (treeChop != null)
        {
            treeChop.SpawnDrops();
            treeChop.HideTreeWithDelay();
        }

        isChopping = false;
        player.inputHandler.EnablePlayerInput();
        player.animationController.ResetChop();

        playerState.ChangeState(new IdleState(playerState, player));
    }
}
