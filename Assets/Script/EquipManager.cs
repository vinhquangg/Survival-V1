using System.Collections.Generic;
using UnityEngine;
using static WeaponClass;

public class EquipManager : MonoBehaviour
{
    [Header("References")]
    public EquipHolder equipHolderR;
    public EquipHolder equipHolderL;
    public Animator playerAnimator;

    private Dictionary<EquipType, ItemClass> currentEquippedItems = new Dictionary<EquipType, ItemClass>();
    private Dictionary<EquipType, SlotClass> equippedSlots = new Dictionary<EquipType, SlotClass>();

    public AnimationStateController animController;
    public void EquipItem(ItemClass item)
    {
        EquipType type = item.GetEquipType();

        // Lưu lại item đang cầm
        currentEquippedItems[type] = item;

        // Hiển thị item theo tay
        if (type == EquipType.Weapon && item is WeaponClass weapon)
        {
            if (weapon.weaponType == WeaponType.Bow) // nếu là cung thì tay trái
            {
                equipHolderL.DisplayItem(item);
                equipHolderR.HideItem(null); // ẩn bên tay phải
            }
            else
            {
                equipHolderR.DisplayItem(item); // mặc định vũ khí cận chiến tay phải
                equipHolderL.HideItem(null);
            }

            if (playerAnimator != null)
            {
                playerAnimator.SetInteger("WeaponType", (int)weapon.weaponType);
                playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 1f);
            }
        }
        else if (type == EquipType.Tool) // nếu là tool thì có thể vẫn cầm tay phải
        {
            equipHolderR.DisplayItem(item);
            equipHolderL.HideItem(null);
        }
    }

    public void EquipItem(SlotClass slot)
    {
        if (slot == null || slot.IsEmpty()) return;

        ItemClass item = slot.GetItem();
        if (item == null) return;

        EquipType type = item.GetEquipType();

        // lưu slot runtime
        equippedSlots[type] = slot;

        // gọi lại hàm cũ để xử lý hiển thị + animation
        EquipItem(item);
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
        ///////////
        //if (currentEquippedItems.TryGetValue(type, out var item))
        //{
        //    if (item is WeaponClass weapon && weapon.weaponType == WeaponType.Bow)
        //        equipHolderL.HideItem(item);
        //    else
        //        equipHolderR.HideItem(item);

        //    currentEquippedItems[type] = null;

        //    if (type == EquipType.Weapon && playerAnimator != null)
        //    {
        //        playerAnimator.SetInteger("WeaponType", 0);
        //        playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 0f);
        //    }
        //}


        if (currentEquippedItems.TryGetValue(type, out var item))
        {
            if (item is WeaponClass weapon && weapon.weaponType == WeaponType.Bow)
                equipHolderL.HideItem(item);
            else
                equipHolderR.HideItem(item);

            currentEquippedItems[type] = null;
            equippedSlots[type] = null; // clear slot runtime

            if (type == EquipType.Weapon && playerAnimator != null)
            {
                playerAnimator.SetInteger("WeaponType", 0);
                playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 0f);
            }
        }
    }

    public SlotClass GetEquippedSlot(EquipType type)
    {
        equippedSlots.TryGetValue(type, out var slot);
        return slot;
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
