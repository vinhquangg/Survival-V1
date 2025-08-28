using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    [Header("References")]
    public EquipHolder equipHolder;
    public Animator playerAnimator;

    private Dictionary<EquipType, ItemClass> currentEquippedItems = new Dictionary<EquipType, ItemClass>();
    public AnimationStateController animController;
    public void EquipItem(ItemClass item)
    {
        EquipType type = item.GetEquipType();

        // Lưu lại item đang cầm
        currentEquippedItems[type] = item;

        equipHolder.DisplayItem(item);

        if (type == EquipType.Weapon && item is WeaponClass weapon)
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetInteger("WeaponType", (int)weapon.weaponType);
                playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 1f);
            }
        }

        // Nếu là tool thì bạn có thể xử lý animator tool tại đây nếu có
    }

    public void UnequipItem(EquipType type)
    {
        //if (type == EquipType.Weapon && animController != null && animController.IsAttacking)
        //{
        //    Debug.Log("[EquipManager] Đang tấn công, không thể cất vũ khí.");
        //    return;
        //}
        //if (type == EquipType.Tool && animController != null && animController.IsChopping)
        //{
        //    Debug.Log("[EquipManager] Đang chặt cây, không thể cất tool.");
        //    return;
        //}

        if (currentEquippedItems.TryGetValue(type, out var item))
        {
            equipHolder.HideItem(item);
            currentEquippedItems[type] = null;

            if (type == EquipType.Weapon && playerAnimator != null)
            {
                playerAnimator.SetInteger("WeaponType", 0);
                playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 0f);
            }
        }
    }


    public ItemClass GetEquippedItem(EquipType type)
    {
        currentEquippedItems.TryGetValue(type, out var item);
        return item;
    }

    public bool HasItemEquipped(EquipType type)
    {
        return currentEquippedItems.ContainsKey(type) && currentEquippedItems[type] != null;
    }
}
