using UnityEngine;

public class RangeMonsterCombat : MonsterCombat
{
    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Bite Settings")]
    [SerializeField] private float maxBiteDistance = 3f; // khoảng cách tối đa bite

    // Property để AttackState check
    public float biteRange
    {
        get
        {
            if (target == null) return 0f;

            // Vector từ monster tới player, bỏ chiều cao
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            return toTarget.magnitude;
        }
    }

    protected override void Attack()
    {
        if (animator != null)
        {
            // Trigger animation ranged attack nếu muốn
            // animator.SetTrigger("Attack");
        }

        ShootProjectile();
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || target == null) return;

        Vector3 dir = (target.position - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + Vector3.up * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));

        if (proj.TryGetComponent<FireballProjectile>(out var fireball))
        {
            fireball.Launch(dir, projectileSpeed);
        }
    }

    public bool CanBite()
    {
        return biteRange <= maxBiteDistance;
    }

    public void Bite()
    {
        if (!CanBite() || target == null) return;

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(attackDamage);
        }
    }
}
