using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMonster : MonoBehaviour, IDamageable, IPoolable
{
    public MonsterStateMachine _stateMachine { get; protected set; }
    protected Animator animMonster { get; private set; }
    public Rigidbody _rigidbody { get; protected set; }
    public MonsterCombat combat { get; protected set; }
    public float currentHeal { get; protected set; }

    public System.Action<float, float > OnHealthChanged; // (current, max)
    public System.Action<BaseMonster> OnDeath;
    public System.Action<BaseMonster> OnReturnedToPool;

    [Header("Monster Settings")]
    public MonsterStatsSO stats;

    public Transform healthUIPoint;      
    public GameObject healthUIPrefab;    
    private EnemyHealthUI healthUI;      

    public Transform player;
    public NavMeshAgent _navMeshAgent;
    public PatrolType patrolType = PatrolType.Random;

    public DropTableData dropTable;

    [Header("Animation Settings")]
    public MonsterAnimationData animData;  // SO chứa mapping anim theo enum
    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        animMonster = GetComponent<Animator>();
        _stateMachine = GetComponent<MonsterStateMachine>();

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
        player = PlayerManager.Instance.GetPlayerTransform();

        combat = GetComponent<MonsterCombat>();
        if (combat != null)
        {
            combat.target = player;
            combat.SetupFromStats(stats);
        }
        if (healthUIPrefab != null && healthUIPoint != null)
        {
            GameObject uiGO = Instantiate(healthUIPrefab, healthUIPoint.position, Quaternion.identity);
            healthUI = uiGO.GetComponentInChildren<EnemyHealthUI>();
            healthUI.Setup(this, healthUIPoint);
            uiGO.transform.SetParent(transform);
        }

        currentHeal = stats.maxHealth;

        OnHealthChanged?.Invoke(currentHeal, stats.maxHealth);
        //if (_navMeshAgent != null)
        //{
        //    _navMeshAgent.updateRotation = false;
        //}

        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _navMeshAgent.avoidancePriority = Random.Range(20, 80);
    }

    //protected Transform GetPlayer()
    //{
    //    if (player == null)
    //    {
    //        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //        if (playerObj != null)
    //        {
    //            player = playerObj.transform;
    //        }
    //    }
    //    return player;
    //}

    public virtual bool CanSeePlayer()
    {
        if (player == null) return false;

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.animationController.IsDead) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > stats.detectedRange)
        {
            stats.graceTimer = 0f;
            return false;
        }

        Vector3 origin = transform.position + Vector3.up * 1.2f;
        if (Physics.Raycast(origin, directionToPlayer.normalized, out RaycastHit hit, stats.detectedRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                stats.graceTimer = 0f;
                return true;
            }
        }

        stats.graceTimer += Time.deltaTime;
        return stats.graceTimer <= stats.graceTimeMax;
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

    public void RotateTowardsPatrolPoint(Vector3 patrolPoint, float rotationSpeed)
    {
        Vector3 direction = patrolPoint - transform.position;
        direction.y = 0f; // chỉ quay theo trục Y
        if (direction.sqrMagnitude < 0.01f) return; // tránh quay khi quá gần

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        currentHeal = Mathf.Clamp(currentHeal, 0, stats.maxHealth);

        Debug.Log($"{gameObject.name} bị trúng, còn {currentHeal} máu");

        OnHealthChanged?.Invoke(currentHeal, stats.maxHealth);

        if (currentHeal <= 0)
        {
            Die();
        }
    }

    public virtual void DropItems()
    {
        if (dropTable == null) return;

        foreach (var drop in dropTable.drops)
        {
            for (int i = 0; i < drop.spawnCount; i++)
            {
                if (Random.value <= drop.chance)
                {
                    Vector3 pos = transform.position + drop.offset + Random.insideUnitSphere * 0.2f;
                    GameObject obj = ObjectPoolManager.Instance.SpawnFromPool(drop.poolID, pos, Quaternion.identity);

                    // Thiết lập ItemEntity luôn
                    ItemEntity itemEntity = obj.GetComponent<ItemEntity>();
                    if (itemEntity != null && drop.quantity > 0)
                    {
                        itemEntity.Initialize(itemEntity.GetItemData(), drop.quantity);
                    }
                }
            }

        }
    }

    protected virtual void Die()
    {
        //DropItems();
        // Chuyển sang DeadState để chạy animation và pool
        Debug.Log($"{name} DIE() called, invoking OnDeath");
        OnDeath?.Invoke(this);

        if (_stateMachine != null)
        {
            _stateMachine.SwitchState(new MonsterDeadState(_stateMachine));
        }
        else
        {
            // fallback nếu không có stateMachine
            ReturnPool();
        }

        Debug.Log($"{gameObject.name} đã chết");
        //healthUIPrefab.SetActive(false);
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

    public void OnSpawned()
    {
        currentHeal = stats.maxHealth;

        if (_navMeshAgent != null)
            _navMeshAgent.enabled = true;

        if (healthUI != null)
            healthUI.gameObject.SetActive(true);

        gameObject.SetActive(true);
        _stateMachine?.ResetToIlde();
    }

    public void OnReturned()
    {
        OnDeath = null;
    }

    public void ReturnPool()
    {
        Debug.Log($"{name} DIE() called, invoking OnDeath");
        OnDeath?.Invoke(this);

        if (_navMeshAgent != null)
        {
            if (_navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
            }
            _navMeshAgent.enabled = false;
        }


        // 2️⃣ Tắt Collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        foreach (var childCol in GetComponentsInChildren<Collider>())
            childCol.enabled = false;

        // 3️⃣ Tắt Rigidbody / reset velocity
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true;
        }

        // 4️⃣ Tắt Animator để giảm update
        if (animMonster != null)
        {
            animMonster.Rebind();       // reset tất cả parameter
            animMonster.Update(0f);
            animMonster.enabled = false;
        }

        // 5️⃣ Pool Health UI
        if (healthUI != null)
            healthUI.gameObject.SetActive(false);

        // 6️⃣ Tắt tất cả Renderer
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        // 7️⃣ Reset các biến state
        OnDeath = null;
        currentHeal = stats.maxHealth;

        // 8️⃣ Return object về pool
        ObjectPoolManager.Instance.ReturnToPool(gameObject);


    }


}
