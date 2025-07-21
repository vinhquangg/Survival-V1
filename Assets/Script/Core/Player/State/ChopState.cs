using System.Collections;
using UnityEngine;

public class ChopState : PlayerState
{
    private TreeChopInteraction treeChop;
    private Coroutine chopRoutine;
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
        isChopping = true;
        chopRoutine = player.StartCoroutine(WaitForChopping());
    }

    private IEnumerator WaitForChopping()
    {
        yield return new WaitForSeconds(3f);

        if (!isChopping) yield break; 

        treeChop.OnChopped();
        isChopping = false;
        player.inputHandler.EnablePlayerInput();

        player.animationController.ResetChop(); 
        playerState.ChangeState(new IdleState(playerState, player));
    }


    public override void Update() { }

    public override void Exit()
    {
        if (chopRoutine != null)
        {
            player.StopCoroutine(chopRoutine);
            chopRoutine = null;
        }

        if (isChopping)
        {
            isChopping = false;
            player.animationController.ResetChop(); 
        }

        player.inputHandler.EnablePlayerInput();
    }

}
