using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolState : IInterfaceMonsterState
{
    private MonsterStateMachine enemy;
    private Transform monsterTransform;
    private float patrolRadius = 10f;
    private float waitTime = 2f;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    public MonsterPatrolState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
        this.monsterTransform = enemy.transform;
    }
    public void EnterState()
    {
        enemy.baseMonster._navMeshAgent.isStopped = false;
        enemy.PlayAnimation("Chase_Monster");
        enemy.baseMonster.SetRandomPatrolDestination(patrolRadius);
    }

    public void ExitState()
    {
        enemy.baseMonster._navMeshAgent.ResetPath();
        isWaiting = false;
        waitCounter = 0f;
    }

    public void FixedUpdateState()
    {

    }

    public void UpdateState()
    {
        if(enemy.baseMonster.CanSeePlayer())
        {
            enemy.SwitchState(new MonsterChaseState(enemy));
            return;
        }
        NavMeshAgent agent = enemy.baseMonster._navMeshAgent;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitCounter = 0f;
            }
            else
            {
                waitCounter += Time.deltaTime;
                if (waitCounter >= waitTime)
                {
                    isWaiting = false;
                    enemy.baseMonster.SetRandomPatrolDestination(patrolRadius);
                }
            }
        }
    }

}
