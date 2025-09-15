using UnityEngine;

public class DragonMonster : BaseMonster
{
    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;     // Prefab đạn (fireball, arrow,...)
    public Transform firePoint;             // Vị trí bắn
    public float projectileSpeed = 10f;
    public float attackCooldown = 2f;

    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        lastAttackTime = -attackCooldown; // cho phép bắn ngay lần đầu
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
