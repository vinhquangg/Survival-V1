using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : MonoBehaviour
{
    private IAnimalState currentState;
    public BaseAnimal baseAnimal { get; private set; }

    private void Awake()
    {
        baseAnimal = GetComponent<BaseAnimal>();
    }

    private void Start()
    {
        currentState = new WanderState(this);
        currentState.Enter();
    }

    private void Update()
    {
        currentState?.Tick();
        baseAnimal.Tick();
    }

    public void SwitchState(IAnimalState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
}
