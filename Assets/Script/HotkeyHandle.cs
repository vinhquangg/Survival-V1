using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyHandle : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public EquipManager equipManager;
    private void Update()
    {
        int hotbarSize = inventoryManager.playerInventory.hotbarItems.Length;

        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseItemInHotBar(i);
            }
        }
    }
    private void UseItemInHotBar(int index)
    {
        var slot = inventoryManager.playerInventory.hotbarItems[index];

        if (slot == null || slot.IsEmpty() || slot.GetItem() == null)
        {
            Debug.LogWarning($"Hotbar slot {index + 1} is empty or invalid.");
            return;
        }

        var item = slot.GetItem();
        EquipType equipType = item.GetEquipType();

        if (equipType != EquipType.None)
        {
            // 🔐 CHẶN nếu đang tấn công và cố gắng unequip vũ khí
            if (equipManager.HasItemEquipped(equipType) &&
                equipManager.GetEquippedItem(equipType) == item)
            {
                // Nếu là Weapon và đang tấn công thì KHÔNG cho unequip
                if (equipType == EquipType.Weapon &&
                    equipManager.animController != null &&
                    equipManager.animController.IsAttacking)
                {
                    Debug.LogWarning("[Hotkey] Không thể cất vũ khí khi đang attack.");
                    return;
                }

                equipManager.UnequipItem(equipType);
            }
            else
            {
                // Nếu đang cầm vũ khí A → bấm chọn vũ khí B, vẫn cho đổi (nếu bạn muốn chặn cái này nữa thì bổ sung thêm)
                equipManager.EquipItem(item);
            }
        }
        else
        {
            // Nếu là consumable, vẫn cần chặn không cho Unequip khi đang attack
            if (equipManager.animController != null && equipManager.animController.IsAttacking)
            {
                Debug.LogWarning("[Hotkey] Không thể thay đổi trang bị khi đang attack.");
                return;
            }
            if (item is IUsableItem usable)
            {
                usable.UseItem(PlayerStatus.Instance, inventoryManager.playerInventory);
                inventoryManager.RefreshAllUI();
            }
            // Nếu là tool, có thể xử lý logic sử dụng tool tại đây
            equipManager.UnequipItem(EquipType.Weapon);
            equipManager.UnequipItem(EquipType.Tool);
        }
    }


}
