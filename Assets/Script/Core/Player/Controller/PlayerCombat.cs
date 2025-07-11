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
        Collider[] hitTargets = Physics.OverlapSphere(weaponHitPoint.position, weaponHitRadius, targetMask);
        Debug.Log($"🔍 Found {hitTargets.Length} targets");

        foreach (var target in hitTargets)
        {
            Debug.Log($"🎯 Hit {target.name}");

            // 👇 Dùng GetComponentInParent thay vì TryGetComponent
            if (target.GetComponentInParent<IDamageable>() is IDamageable damageable)
            {
                Debug.Log($"✅ Gọi TakeDamage() trên {target.name}");
                damageable.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning($"❌ Không tìm thấy IDamageable trên {target.name} hoặc cha của nó");
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
