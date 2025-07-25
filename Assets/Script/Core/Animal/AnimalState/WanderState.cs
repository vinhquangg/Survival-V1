using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : IAnimalState
{
    private AnimalStateMachine stateMachine;
    private NavMeshAgent agent;
    private float timer;
    private float wanderInterval = 3f;

    public WanderState(AnimalStateMachine machine)
    {
        stateMachine = machine;
        agent = stateMachine.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        Wander();
    }
    public void Tick()
    {
        timer += Time.deltaTime;
        bool isRunning = agent.velocity.magnitude > 0.1f;
        stateMachine.baseAnimal.animator.SetBool("isRunning", isRunning);

        if (timer >= wanderInterval)
        {
            Wander();
            timer = 0;
        }
    }

    public void Exit() { }

    private void Wander()
    {
        Vector3 randomPos = stateMachine.transform.position + Random.insideUnitSphere * 8f;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
