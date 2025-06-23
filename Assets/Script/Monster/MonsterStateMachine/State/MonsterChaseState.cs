using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : IInterfaceMonsterState
{
    public MonsterStateMachine enemy;   
    private float chaseSpeed;

    public MonsterChaseState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
        this.chaseSpeed = enemy.baseMonster.moveSpeed/*monsterData.chaseSpeed*/; 
    }
    public void EnterState()
    {
        enemy.animator.SetBool("isChase", true);
        enemy.PlayAnimation("Chase_Monster");
        Debug.Log("Monster is now chasing the player.");
    }

    public void ExitState()
    {
        enemy.animator.SetBool("isChase", false);
        Debug.Log($" Enemy stop chase.");
    }

    public void FixedUpdateState()
    {
        
    }

    public void UpdateState()
    {
        if (enemy.baseMonster.CanSeePlayer())
        {
            enemy.baseMonster._navMeshAgent.speed = chaseSpeed;
            enemy.baseMonster._navMeshAgent.SetDestination(enemy.baseMonster.player.position);
        }
        else
        {
            enemy.SwitchState(new MonsterIdleState(enemy));
            return;
        }
        //{
        //    enemy.SwitchState(new MonsterIdleState(enemy));
        //    return;
        //}
        //else
        //{
        //    enemy.baseMonster.SetDestination();
        //}
    }

}
