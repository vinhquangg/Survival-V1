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
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
        var slot = inventoryManager.playerInventory.hotbarItems[index];

        if (slot == null || slot.IsEmpty() || slot.GetItem() == null)
        {
            Debug.LogWarning($"Hotbar slot {index + 1} is empty or invalid.");
            return;
        }

        var interactable = SelectionManager.Instance.CurrentInteractable;
        if (interactable is Cookable)
        {
            TryCookFromHotbar(index, shiftPressed);
            return;
        }

        var item = slot.GetItem();

        // ✅ Nếu là item có thể đặt
        if (item.itemType == ItemType.Placable)
        {
            if (item.blueprint == null)
            {
                return;
            }

            Debug.Log($"[Hotkey] Bắt đầu chế độ đặt cho {item.itemName}");
            PlacementSystem.Instance.StartPlacement(item.blueprint, index);
            return; // Không chạy tiếp logic khác
        }
        else
        {
            // ⛔ Nếu đang ở chế độ đặt và chọn item khác → hủy chế độ đặt
            if (PlacementSystem.Instance != null)
                PlacementSystem.Instance.CancelPlacement();
        }

        EquipType equipType = item.GetEquipType();

        if (equipType != EquipType.None)
        {
            // 🔐 CHẶN nếu đang tấn công và cố gắng unequip vũ khí
            if (equipManager.HasItemEquipped(equipType) &&
                equipManager.GetEquippedItem(equipType) == item)
            {
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
                equipManager.EquipItem(item);
            }
        }
        else
        {
            if (equipManager.animController != null && equipManager.animController.IsAttacking)
            {
                Debug.LogWarning("[Hotkey] Không thể thay đổi trang bị khi đang attack.");
                return;
            }
            else if (item is IUsableItem usable)
            {
                usable.UseItem(PlayerStatus.Instance, inventoryManager.playerInventory);
                inventoryManager.RefreshAllUI();
            }

            equipManager.UnequipItem(EquipType.Weapon);
            equipManager.UnequipItem(EquipType.Tool);
        }
    }

    private void TryCookFromHotbar(int hotbarIndex, bool cookAll)
    {
        var selectionManager = FindObjectOfType<SelectionManager>();
        if (selectionManager.CurrentInteractable is Cookable cookable)
        {
            var campfire = cookable.GetComponent<Campfire>();
            if (campfire == null || !campfire.IsBurning) return;

            var slot = inventoryManager.playerInventory.hotbarItems[hotbarIndex];
            if (slot == null || slot.IsEmpty()) return;

            if (slot.GetItem() is Consumable consumable &&
                consumable.isMeat && consumable.meatState == AnimalMeat.Raw)
            {
                int quantityToCook = cookAll ? slot.GetQuantity() : Mathf.Min(2, slot.GetQuantity());

                // Trừ raw meat
                inventoryManager.playerInventory.RemoveItem(consumable, quantityToCook);

                // Spawn meat tại cookPoint
                for (int i = 0; i < quantityToCook; i++)
                {
                    Instantiate(consumable.dropPrefab, campfire.CookPoint.position, Quaternion.identity);
                }

                // Gọi cook
                cookable.Cook(PlayerStatus.Instance.gameObject);

                // Refresh UI
                inventoryManager.RefreshAllUI();

                Debug.Log($"Cooked {quantityToCook} raw meat from slot {hotbarIndex + 1}");
            }
        }
    }

}
