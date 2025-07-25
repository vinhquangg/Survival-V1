using UnityEngine;
using UnityEngine.AI;

public class FleeState : IAnimalState
{
    private AnimalStateMachine stateMachine;
    private Transform player;
    private NavMeshAgent agent;

    public FleeState(AnimalStateMachine machine)
    {
        stateMachine = machine;
        agent = machine.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Enter()
    {
        FleeFromPlayer();
    }

    public void Tick()
    {

        bool isRunning = agent.velocity.magnitude > 0.1f;
        stateMachine.baseAnimal.animator.SetBool("isRunning", isRunning);

        if (Vector3.Distance(stateMachine.transform.position, player.position) > 15f)
        {
            stateMachine.SwitchState(new WanderState(stateMachine));
        }
    }

    public void Exit() { }

    private void FleeFromPlayer()
    {
        Vector3 dir = (stateMachine.transform.position - player.position).normalized;
        Vector3 fleeTarget = stateMachine.transform.position + dir * 10f;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
