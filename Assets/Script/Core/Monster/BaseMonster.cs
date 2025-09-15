using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMonster : MonoBehaviour,IDamageable
{
    public MonsterStateMachine _stateMachine { get; protected set; }
    protected Animator animMonster { get; private set; }
    public Rigidbody _rigidbody { get; protected set; }
    public MonsterCombat combat { get; protected set; }

    [Header("Monster Settings")]
    public Transform player;
    public NavMeshAgent _navMeshAgent;
    public float hitDuration;
    public float detectedRange;
    //public float attackRange;
    public float moveSpeed;
    public bool isKnockback = false;
    public float knockbackForce = 5f;
    public float patrolSpeed = 1f;
    public float currentHeal;
    private float graceTimer = 0f;
    private float graceTimeMax = 0.5f;

    [Header("Animation Settings")]
    public MonsterAnimationData animData;  // SO chứa mapping anim theo enum
    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _stateMachine = GetComponent<MonsterStateMachine>();
        _rigidbody = GetComponent<Rigidbody>();

        animMonster = GetComponent<Animator>();
        if (animMonster == null)
        {
            Debug.LogError($"{name}: Animator not found!");
        }
    }
    protected virtual void Start()
    {
        //_navMeshAgent = GetComponent<NavMeshAgent>();
        //_stateMachine = GetComponent<MonsterStateMachine>();
        //animMonster = GetComponent<Animator>();
        //_rigidbody = GetComponent<Rigidbody>();
        player = GetPlayer();


        combat = GetComponent<MonsterCombat>();
        if (combat != null)
        {
            combat.target = player;
        }
    }

    protected Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        return player;
    }

    public virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.animationController.IsDead) return false;

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

    public void OnAttackAnimationFinished()
    {
        if (_stateMachine._currentState is MonsterAttackState attackState)
        {
            attackState.OnAttackAnimationFinished();
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

    public void PlayAnimation(MonsterAnimState state)
    {
        if (animData == null || animData.mappings == null)
        {
            Debug.LogWarning($"No animData on {gameObject.name}");
            return;
        }

        foreach (var map in animData.mappings)
        {
            if (map.state == state)
            {
                animMonster.Play(map.animName);
                return;
            }
        }

        Debug.LogWarning($"Animation for {state} not found on {gameObject.name}");
    }



}
