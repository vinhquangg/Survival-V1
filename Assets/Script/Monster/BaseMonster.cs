using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMonster : MonoBehaviour
{
    public MonsterStateMachine _stateMachine { get; protected set; }
    protected Animator animMonster { get; private set; }
    public Rigidbody _rigidbody { get; protected set; }
    public Transform player;
    public NavMeshAgent _navMeshAgent;
    public float hitDuration;
    public float detectedRange;
    public float attackRange;
    public float moveSpeed;
    public bool isKnockback = false;
    public float knockbackForce = 5f;
    public float patrolSpeed = 1f;
    public float currentHeal;
    private float graceTimer = 0f;
    private float graceTimeMax = 0.5f;
    protected virtual void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _stateMachine = GetComponent<MonsterStateMachine>();
        animMonster = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > detectedRange)
        {
            graceTimer = 0f;
            return false;
        }

        Vector3 origin = transform.position + Vector3.up * 1.2f;
        if (Physics.Raycast(origin, directionToPlayer.normalized, out RaycastHit hit, detectedRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                graceTimer = 0f;
                return true;
            }
        }

        graceTimer += Time.deltaTime;
        return graceTimer <= graceTimeMax;
    }

    //public virtual bool CanSeePlayer()
    //{
    //    if (player == null) return false;

    //    Vector3 directionToPlayer = player.position - transform.position;
    //    float distanceToPlayer = directionToPlayer.magnitude;

    //    if (distanceToPlayer < detectedRange)
    //    {
    //        return true;
    //        //Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
    //        //RaycastHit hit;

    //        //if (Physics.Raycast(ray, out hit, detectedRange))
    //        //{
    //        //    if (hit.transform == player)
    //        //    {
    //        //        return true;
    //        //    }
    //        //}
    //    }

    //    return false;
    //}

    //public virtual void SetDestination()
    //{
    //    if (player != null)
    //    {
    //        Vector3 targetVector = player.transform.position;
    //        _navMeshAgent.SetDestination(targetVector);
    //    }
    //}

    public virtual void SetRandomPatrolDestination(float patrolRadius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            _navMeshAgent.SetDestination(hit.position);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHeal -= damage;
        Debug.Log($"{gameObject.name} bị trúng, còn {currentHeal} máu");

        if (currentHeal <= 0)
        {
            Die();
        }
    }


    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} đã chết");
        gameObject.SetActive(false);
    }


}
