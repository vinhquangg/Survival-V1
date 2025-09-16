using UnityEngine;

public class MonsterIdleState : MonsterBaseState
{
    private float idleDuration = 10f;
    private float idleTimer;

    public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        //enemy._rigidbody.velocity = Vector3.zero;
        monster.PlayAnimation(MonsterAnimState.Idle);
        monster._navMeshAgent.ResetPath();
        idleTimer = 0f;
    }

    public override void ExitState()
    {

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

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            stateMachine.SwitchState(new MonsterPatrolState(stateMachine));
        }
    }
}
