using UnityEngine;

public class BearCombat : MonsterCombat
{
    [Header("Bear Attack Settings")]
    public float biteRange;   // Cắn gần
    public float clawRange;   // Vung vuốt xa hơn 1 chút
    public float roarRange;     // Hú dọa, gây hiệu ứng

    // Kiểm tra có thể cắn không
    public bool CanBite()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= biteRange;
    }

    // Kiểm tra có thể vung vuốt không
    public bool CanClaw()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= clawRange;
    }

    // Kiểm tra có thể gầm không
    //public bool CanRoar()
    //{
    //    if (target == null) return false;
    //    return Vector3.Distance(transform.position, target.position) <= roarRange;
    //}

    // Attack chỉ để FSM gọi, thực tế deal damage qua Animation Event
    protected override void Attack()
    {
        DealDamageToTarget();
        Debug.Log("BearCombat.Attack() được gọi - Damage thực tế sẽ do Animation Event gọi DealDamageToTarget()");
    }

    // --- Hàm gây damage (gọi từ Animation Event) ---
    public void DealDamageToTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(attackDamage);

                if (distance <= biteRange)
                {
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.animalBearAttackSound);
                }
                else if (distance <= clawRange)
                {
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.animalBearClawSound);
                }
            }
        }
    }
}
