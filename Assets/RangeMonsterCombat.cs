using UnityEngine;

public class RangeMonsterCombat : MonsterCombat
{
    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Bite Settings")]
    [SerializeField] private float maxBiteDistance = 3f; // khoảng cách tối đa bite

    [Header("Boss Skill Settings")]
    public GameObject shockwavePrefab;     // prefab shockwave (vfx + damage)
    public float fireballSpreadAngle = 15f; // độ lệch của 3 fireball
    public float skillCooldown = 8f;
    //private float skillTimer = 0f;
    public bool IsBoss => (monster is DragonMonster dragon) && dragon.isBoss;
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

    private void Update()
    {
        if (!IsBoss) return;

        //skillTimer -= Time.deltaTime;

        //if (skillTimer <= 0f && target != null)
        //{
        //    CastFireballSpread();
        //    skillTimer = skillCooldown;
        //}
    }

    protected override void Attack()
    {
        Debug.Log($"Attack() called - IsBoss = {IsBoss}");

        if (IsBoss)
        {
            Debug.Log("Boss fireball spread!");
            CastFireballSpread();

        }
        else
        {
            Debug.Log("Normal enemy single shot");
            ShootProjectile();

        }
    }


    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || target == null) return;

        Vector3 dir = (target.position - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + Vector3.up * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        SoundManager.Instance.PlaySFX(SoundManager.Instance.dragonHitSound);
        if (proj.TryGetComponent<FireballProjectile>(out var fireball))
        {
            fireball.Launch(dir, projectileSpeed);
        }
    }

    private void SpawnProjectile(Vector3 dir)
    {
        Vector3 spawnPos = firePoint.position + Vector3.up * 0.5f;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        SoundManager.Instance.PlaySFX(SoundManager.Instance.dragonHitSound);
        if (proj.TryGetComponent<FireballProjectile>(out var fireball))
        {
            fireball.Launch(dir, projectileSpeed);
        }
    }

    private void CastFireballSpread()
    {
        if (firePoint == null || projectilePrefab == null) return;

        Vector3 baseDir = (target != null)
            ? (target.position - firePoint.position).normalized
            : firePoint.forward;

        for (int i = -1; i <= 1; i++)
        {
            Quaternion spreadRot = Quaternion.Euler(0, i * fireballSpreadAngle, 0);
            Vector3 spreadDir = spreadRot * baseDir;

            SpawnProjectile(spreadDir);
        }

        Debug.Log("Boss cast Fireball Spread (cone)!");
    }


    private void CastShockwave()
    {
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }

        GameObject shockwave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.dragonCastspellSound);
        // nếu muốn truyền damage và radius từ boss sang shockwave
        if (shockwave.TryGetComponent<ShockwaveVFX>(out var shock))
        {
            shock.damage = attackDamage;   // gấp đôi damage thường
        }

        Debug.Log("Boss cast Shockwave!");
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
