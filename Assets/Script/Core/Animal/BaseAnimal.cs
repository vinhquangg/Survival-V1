using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAnimal : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }
    public Animator animator { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        agent= GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public abstract void Init();
    public abstract void Tick();
}
