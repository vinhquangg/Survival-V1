using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : IInterfaceMonsterState
{
    private MonsterStateMachine enemy;
    public MonsterIdleState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
    }
    public void EnterState()
    {
        enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        enemy.baseMonster._navMeshAgent.ResetPath();
        enemy.PlayAnimation("Idle_Monster");
    }

    public void ExitState()
    {

    }

    public void FixedUpdateState()
    {
        
    }

    public void UpdateState()
    {
        if(enemy.baseMonster.CanSeePlayer())
        {
            enemy.SwitchState(new MonsterChaseState(enemy));
        }
    }


}
