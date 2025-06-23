using UnityEngine;

public class MonsterIdleState : IInterfaceMonsterState
{
    private MonsterStateMachine enemy;
    private float idleDuration = 10f;
    private float idleTimer;

    public MonsterIdleState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        //enemy._rigidbody.velocity = Vector3.zero;
        enemy.PlayAnimation("Idle_Monster");
        enemy.baseMonster._navMeshAgent.ResetPath();
        idleTimer = 0f;
    }

    public void ExitState()
    {

    }

    public void FixedUpdateState()
    {
    }

    public void UpdateState()
    {
        if (enemy.baseMonster.CanSeePlayer())
        {
            enemy.SwitchState(new MonsterChaseState(enemy));
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            enemy.SwitchState(new MonsterPatrolState(enemy));
        }
    }
}
