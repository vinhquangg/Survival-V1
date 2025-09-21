using System.Collections;
using UnityEngine;

public class ChopState : PlayerState
{
    private TreeChopInteraction treeChop;
    private bool isChopping = false;

    public ChopState(PlayerStateMachine playerState, PlayerController player, TreeChopInteraction treeChop)
        : base(playerState, player)
    {
        this.treeChop = treeChop;
    }

    public override void Enter()
    {
        player.inputHandler.DisablePlayerInput();

        player.animationController.TriggerChop();

        // 👉 Xoay hướng về cây
        if (treeChop != null)
        {
            player.RotateTowards(treeChop.transform.position, 8f);
        }

        isChopping = true;
    }

    // Gọi từ Animation Event (qua AnimationStateController)
    // 🪓 Gọi từ Animation Event (frame impact)
    public void OnChopImpact()
    {
        if (!isChopping) return;
    }

    // 🏁 Gọi từ Animation Event (frame cuối)
    public void OnChopEnd()
    {
        if (!isChopping) return;

        if (treeChop != null)
        {
            treeChop.HideTree();
            treeChop.SpawnDrops();  
        }

        isChopping = false;
        player.inputHandler.EnablePlayerInput();
        player.animationController.ResetChop();

        playerState.ChangeState(new IdleState(playerState, player));
    }


    public override void Update() { }

    public override void Exit()
    {
        if (isChopping)
        {
            isChopping = false;
            player.animationController.ResetChop();
        }

        player.inputHandler.EnablePlayerInput();
    }
}
