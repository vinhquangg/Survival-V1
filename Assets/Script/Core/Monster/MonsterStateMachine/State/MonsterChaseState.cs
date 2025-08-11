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
        // Nếu không còn thấy player → về idle
        if (!enemy.baseMonster.CanSeePlayer())
        {
            enemy.SwitchState(new MonsterIdleState(enemy));
            return;
        }

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.baseMonster.player.position);

        if (distanceToPlayer <= enemy.baseMonster.combat.attackRange)
        {
            enemy.SwitchState(new MonsterAttackState(enemy));
            return;
        }
        enemy.baseMonster._navMeshAgent.speed = chaseSpeed;
        enemy.baseMonster._navMeshAgent.SetDestination(enemy.baseMonster.player.position);
    }

}
