using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterCombat : MonsterCombat
{
    private bool isAttacking = false;
    protected override void Attack()
    {
        if(animator == null) return;
            //animator.SetTrigger("isAttack");
        DealDamageToTarget();
    }

    public void DealDamageToTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }
}
