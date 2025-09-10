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
    public void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;

        if (currentArrow != null)
        {
            // bật vật lý và tách khỏi bow
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = arrowSpawnPoint.forward * arrowSpeed;
            }

            currentArrow.transform.SetParent(null);
            currentArrow = null;
        }
        else
        {
            // fallback nếu không có arrow hiển thị
            GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position + arrowSpawnPoint.forward * 0.5f, arrowSpawnPoint.rotation);
            Rigidbody rb = arrowObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = arrowSpawnPoint.forward * arrowSpeed;
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
