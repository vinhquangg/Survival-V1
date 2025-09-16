using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonMonster : BaseMonster
{
    //[Header("Ranged Attack Settings")]
    //public GameObject projectilePrefab;     // Prefab đạn (fireball, arrow,...)
    //public Transform firePoint;             // Vị trí bắn
    //public float projectileSpeed = 10f;
    //public float attackCooldown = 2f;

    public float patrolZoneRadius = 10f;
    public int patrolPointCount = 6;
    public List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPatrolIndex = 0;
   // private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        GeneratePatrolPoints();
        //lastAttackTime = -attackCooldown; // cho phép bắn ngay lần đầu
    }

    //public bool CanAttack()
    //{
    //    return Time.time - lastAttackTime >= attackCooldown;
    //}

    //public void PerformRangedAttack()
    //{
    //    if (projectilePrefab == null || firePoint == null) return;

    //    lastAttackTime = Time.time;

    //    // Spawn projectile
    //    GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

    //    // Hướng về player
    //    Vector3 dir = (player.position - firePoint.position).normalized;

    //    if (proj.TryGetComponent<Rigidbody>(out Rigidbody rb))
    //    {
    //        rb.velocity = dir * projectileSpeed;
    //    }

    //    //// Nếu projectile có script riêng (vd: DragonProjectile)
    //    //if (proj.TryGetComponent<ProjectileBase>(out ProjectileBase p))
    //    //{
    //    //    p.Init(dir, this);
    //    //}

    //    Debug.Log("DragonMonster bắn đạn!");
    //}

    public override void SetRandomPatrolDestination(float patrolRadius)
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("SwampMonster: Không có điểm tuần tra.");
            return;
        }

        _navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex]);
        Debug.Log("SwampMonster patrol to point: " + patrolPoints[currentPatrolIndex]);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        int attempts = 0;

        while (patrolPoints.Count < patrolPointCount && attempts < patrolPointCount * 5)
        {
            Vector3 randomOffset = Random.insideUnitSphere * patrolZoneRadius;
            randomOffset.y = 0;
            Vector3 point = transform.position + randomOffset;

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                patrolPoints.Add(hit.position);
            }

            attempts++;
        }

        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("SwampMonster: Không tìm được điểm tuần tra nào!");
        }
    }
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} đã chết - chuyển sang DeadState");
        if (_stateMachine != null)
        {
            _stateMachine.SwitchState(new MonsterDeadState(_stateMachine));
        }
        else
        {
            base.Die();
        }
    }
}
