using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float weaponHitRadius;
    public Transform weaponHitPoint;
    public int damage;
    public LayerMask targetMask;

    private AnimationStateController animationController;
    private InputHandler inputHandler;

    private void Awake()
    {
        inputHandler= GetComponent<InputHandler>();
        animationController = GetComponent<AnimationStateController>();
    }
    public void HandleAtack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(weaponHitPoint.position, weaponHitRadius, targetMask);
        HashSet<IDamageable> uniqueDamageables = new();

        foreach (Collider col in hitColliders)
        {
            if (col.GetComponentInParent<IDamageable>() is IDamageable damageable)
            {
                if (!uniqueDamageables.Contains(damageable))
                {
                    damageable.TakeDamage(damage);
                    uniqueDamageables.Add(damageable);
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (weaponHitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(weaponHitPoint.position, weaponHitRadius);
        }
    }
}
