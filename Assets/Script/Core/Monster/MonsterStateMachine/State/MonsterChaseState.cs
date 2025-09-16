using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterBaseState
{ 
    private float chaseSpeed;

    public MonsterChaseState(MonsterStateMachine stateMachine): base(stateMachine) { }

    public override void EnterState()
    {
        chaseSpeed = monster.moveSpeed;
        stateMachine.animator.SetBool("isChase", true);
        monster.PlayAnimation(MonsterAnimState.Chase);
        Debug.Log("Monster is now chasing the player.");
    }

    public override void ExitState()
    {
        stateMachine.animator.SetBool("isChase", false);
        Debug.Log($" Enemy stop chase.");
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void UpdateState()
    {
        // Nếu không còn thấy player → về idle
        if (!monster.CanSeePlayer())
        {
            stateMachine.SwitchState(new MonsterIdleState(stateMachine));
            return;
        }

        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, monster.player.position);

        if (distanceToPlayer <= monster.combat.attackRange)
        {
            stateMachine.SwitchState(new MonsterAttackState(stateMachine));
            return;
        }
        monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);
        monster._navMeshAgent.speed = chaseSpeed;
        monster._navMeshAgent.SetDestination(monster.player.position);
    }

}
