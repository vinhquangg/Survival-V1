using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Vector3 start = weaponHitPoint.position;
        Vector3 end = start + transform.forward; 
        float radius = weaponHitRadius;
        Collider[] hitTargets = Physics.OverlapCapsule(start, end, radius, targetMask);
        Debug.Log($"🔍 Found {hitTargets.Length} targets");

        foreach (var target in hitTargets)
        {
            Debug.Log($"🎯 Hit {target.name}");

            if (target.GetComponentInParent<IDamageable>() is IDamageable damageable)
            {
                Debug.Log($"✅ Gọi TakeDamage() trên {target.name}");
                damageable.TakeDamage(damage);
            }
            else
            {
                //Debug.LogWarning($"❌ Không tìm thấy IDamageable trên {target.name} hoặc cha của nó");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponHitPoint != null)
        {
            Vector3 start = weaponHitPoint.position;
            Vector3 end = start + transform.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(start, weaponHitRadius);
            Gizmos.DrawWireSphere(end, weaponHitRadius);
            Gizmos.DrawLine(start + Vector3.up * weaponHitRadius, end + Vector3.up * weaponHitRadius);
            Gizmos.DrawLine(start - Vector3.up * weaponHitRadius, end - Vector3.up * weaponHitRadius);
            Gizmos.DrawLine(start + Vector3.right * weaponHitRadius, end + Vector3.right * weaponHitRadius);
            Gizmos.DrawLine(start - Vector3.right * weaponHitRadius, end - Vector3.right * weaponHitRadius);
        }
    }

}
