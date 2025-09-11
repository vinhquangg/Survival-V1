using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float weaponHitRadius;
    public Transform weaponHitPoint;
    public int damage;
    public LayerMask targetMask;

    public Transform arrowSpawnPoint;
    public GameObject arrowPrefab;
    public float arrowSpeed = 40f;
    private float lastShootTime = 0f;
    [SerializeField] private float shootCooldown = 1f;
    [HideInInspector] public GameObject currentArrow { get; set; } // arrow hiển thị khi kéo

    [HideInInspector] public WeaponClass.WeaponType currentWeaponType = WeaponClass.WeaponType.Machete; // default

    private AnimationStateController animationController;
    private InputHandler inputHandler;
    private PlayerController player;
    private void Awake()
    {
        inputHandler= GetComponent<InputHandler>();
        animationController = GetComponent<AnimationStateController>();
        player = GetComponent<PlayerController>();
    }
    public void HandleAtack()
    {
        // melee logic
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

    public bool CanShoot()
    {
        return Time.time - lastShootTime >= shootCooldown;
    }

    public void MarkShootTime()
    {
        lastShootTime = Time.time;
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;

        if (currentArrow != null)
        {
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = arrowSpawnPoint.forward * arrowSpeed;
            }

            // Gỡ khỏi cung
            currentArrow.transform.SetParent(null);

            // Gán owner cho arrow
            Arrow arrowComp = currentArrow.GetComponent<Arrow>();
            if (arrowComp != null)
            {
                arrowComp.SetOwner(gameObject); // gameObject ở đây là Player
            }

            currentArrow = null;
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
