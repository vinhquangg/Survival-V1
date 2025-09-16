using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolState : MonsterBaseState
{
    private Transform monsterTransform;
    private float patrolRadius = 10f;
    private float waitTime = 5f;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    public override void EnterState()
    {
        monster._navMeshAgent.isStopped = false;
        stateMachine.animator.SetBool("isPatrol", true);
        monster.PlayAnimation(MonsterAnimState.Patrol);
        monster.SetRandomPatrolDestination(patrolRadius);
    }

    public override void ExitState()
    {
        monster._navMeshAgent.ResetPath();
        isWaiting = false;
        waitCounter = 0f;
        stateMachine.animator.SetBool("isPatrol", false);
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {
        if (monster.CanSeePlayer())
        {
            stateMachine.SwitchState(new MonsterChaseState(stateMachine));
            return;
        }

        NavMeshAgent agent = monster._navMeshAgent;

        bool arrived = !agent.pathPending &&
                       agent.remainingDistance <= agent.stoppingDistance &&
                       agent.velocity.sqrMagnitude < 0.01f;

        if (arrived)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitCounter = 0f;

                stateMachine.animator.SetBool("isPatrol", false);
                stateMachine.animator.SetBool("isChase", false);
                monster.PlayAnimation(MonsterAnimState.Idle);
            }
            else
            {
                waitCounter += Time.deltaTime;
                if (waitCounter >= waitTime)
                {
                    isWaiting = false;
                    stateMachine.animator.SetBool("isPatrol", true);
                    monster.PlayAnimation(MonsterAnimState.Patrol);
                    monster.SetRandomPatrolDestination(patrolRadius);
                }
            }
        }
        else
        {
            if (agent.destination != null)
            {
                monster.RotateTowardsPatrolPoint(agent.destination, monster.combat.rotationSpeed);
            }
        }
    }




}
