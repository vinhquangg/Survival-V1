using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBaseState : IInterfaceMonsterState
{
    protected MonsterStateMachine stateMachine;
    protected BaseMonster monster;

    public MonsterBaseState(MonsterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.monster = stateMachine.baseMonster;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void FixedUpdateState();
    public abstract void UpdateState();
}
