using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Attack")]
    public float weaponHitRadius;
    public Transform weaponHitPoint;
    public int damage;
    public LayerMask targetMask;

    [Header("Ranged Attack")]
    public Transform arrowSpawnPoint;
    [HideInInspector] public AmmoClass currentAmmo;       
    [HideInInspector] public GameObject currentArrow;     
    private float lastShootTime = 0f;
    [SerializeField] private float shootCooldown = 1f;

    [Header("Weapon Type")]
    [HideInInspector] public WeaponClass.WeaponType currentWeaponType = WeaponClass.WeaponType.Machete; 

    private AnimationStateController animationController;
    private InputHandler inputHandler;
    private PlayerController player;
    private PlayerInventory playerInventory;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        animationController = GetComponent<AnimationStateController>();
        player = GetComponent<PlayerController>();
        playerInventory = GetComponentInChildren<PlayerInventory>();
    }

    private void Start()
    {

        UpdateCurrentArrowType();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateCurrentArrowType;
        }
    }

    private void UpdateCurrentArrowType()
    {
        currentAmmo = FindFirstArrowInInventoryOrHotbar();
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
        if (currentAmmo == null)
        {
            Debug.LogWarning("⚠ Không có loại arrow nào được trang bị!");
            return;
        }

        if (!HasArrow(currentAmmo))
        {
            Debug.Log("❌ Hết mũi tên, không thể bắn!");
            return;
        }

        if (arrowSpawnPoint == null || currentArrow == null)
        {
            Debug.LogWarning("⚠ Arrow Spawn Point hoặc currentArrow chưa được gán!");
            return;
        }

        float shootSpeed = currentAmmo.speed;

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = arrowSpawnPoint.forward * shootSpeed;
        }

        currentArrow.transform.SetParent(null);

        Arrow arrowComp = currentArrow.GetComponent<Arrow>();
        if (arrowComp != null)
        {
            arrowComp.SetOwner(gameObject);
            arrowComp.arrowDamage = currentAmmo.damageMultiplier;
        }

        currentArrow = null;

        if (!ConsumeArrow(currentAmmo))
        {
            Debug.LogWarning("⚠ Lỗi trừ arrow sau khi bắn!");
        }
    }

    public bool HasArrow(AmmoClass arrowType)
    {
        if (playerInventory == null || arrowType == null) return false;
        return playerInventory.HasItem(arrowType, 1);
    }

    public bool ConsumeArrow(AmmoClass arrowType)
    {
        if (playerInventory == null || arrowType == null) return false;
        return playerInventory.RemoveItem(arrowType, 1);
    }

    private AmmoClass FindFirstArrowInInventoryOrHotbar()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.playerInventory == null)
            return null;

        PlayerInventory inv = InventoryManager.Instance.playerInventory;

        foreach (var slot in inv.hotbarItems)
        {
            if (slot != null && slot.GetItem() is AmmoClass ammo && slot.GetQuantity() > 0)
            {
                return ammo;
            }
        }
        foreach (var slot in inv.items)
        {
            if (slot != null && slot.GetItem() is AmmoClass ammo && slot.GetQuantity() > 0)
            {
                return ammo;
            }
        }

        return null; 
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
