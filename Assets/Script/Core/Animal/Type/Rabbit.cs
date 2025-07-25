using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : BaseAnimal
{
    private AnimalStateMachine stateMachine;

    protected override void Awake()
    {
        base.Awake();
        stateMachine = GetComponent<AnimalStateMachine>();
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        
    }

    public override void Tick()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distanceToPlayer < 8f)
        {
            stateMachine.SwitchState(new FleeState(stateMachine));
        }
    }
}
