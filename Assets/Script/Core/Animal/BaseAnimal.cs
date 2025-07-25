using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseAnimal : MonoBehaviour, IDamageable
{
    public NavMeshAgent agent;
    public Animator animator;
    public float maxHealth = 50f;
    public float currentHealth;
    private bool isDead = false;
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public abstract void Init();
    public abstract void Tick();

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return; 

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} bị trúng, còn {currentHealth} máu");

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
        else
        {
            var stateMachine = GetComponent<AnimalStateMachine>();
            if (stateMachine != null)
            {
                stateMachine.SwitchState(new FleeState(stateMachine));
            }
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} đã chết");
        agent.isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;

        StartCoroutine(DisableAfterDelay(2f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); 
    }

}
