using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine playerState;
    protected PlayerController player;

    public PlayerState(PlayerStateMachine playerState, PlayerController player)
    {
        this.playerState = playerState;
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
