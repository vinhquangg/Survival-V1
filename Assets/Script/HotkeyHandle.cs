using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyHandle : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public EquipManager equipManager;
    public HotbarSelector hotbarSelector;
    private void Update()
    {
        int hotbarSize = inventoryManager.playerInventory.hotbarItems.Length;

        // Hotkey cho các slot
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                HandleHotbarSlot(i);
            }
        }
    }

    private void HandleHotbarSlot(int index)
    {
        if (hotbarSelector != null)
            hotbarSelector.SelectSlot(index);

        var slot = inventoryManager.playerInventory.hotbarItems[index];
        if (slot == null || slot.IsEmpty() || slot.GetItem() == null) return;

        // Nếu là raw meat → cook tại campfire gần nhất
        if (slot.GetItem() is Consumable c && c.isMeat && c.meatState == AnimalMeat.Raw)
        {
            TryCookFromHotbar(index);
            return;
        }

        // Item có thể đặt
        var item = slot.GetItem();
        if (item.itemType == ItemType.Placable && item.blueprint != null)
        {
            PlacementSystem.Instance.StartPlacement(item.blueprint, index);
            return;
        }
        else if (PlacementSystem.Instance != null)
        {
            PlacementSystem.Instance.CancelPlacement();
        }

        // Equip item nếu có
        EquipType equipType = item.GetEquipType();
        if (equipType != EquipType.None)
        {
            if (equipManager.HasItemEquipped(equipType) && equipManager.GetEquippedItem(equipType) == item)
            {
                if (equipType == EquipType.Weapon && equipManager.animController != null && equipManager.animController.IsAttacking)
                    return;

                equipManager.UnequipItem(equipType);
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.unequipSound);
            }
            else
            {
                equipManager.EquipItem(item);
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.equipSound);
            }
        }
        else if (item is IUsableItem usable)
        {
            // 🔹 Chỉ gọi UseItem với item bình thường, không phải raw meat
            usable.UseItem(PlayerStatus.Instance, inventoryManager.playerInventory);
            inventoryManager.RefreshAllUI();
        }
    }

    private void TryCookFromHotbar(int hotbarIndex)
    {
        var slot = inventoryManager.playerInventory.hotbarItems[hotbarIndex];
        if (slot == null || slot.IsEmpty()) return;

        if (!(slot.GetItem() is Consumable c) || !c.isMeat || c.meatState != AnimalMeat.Raw) return;

        // 🔹 Lấy object đang được nhìn vào
        var interactable = SelectionManager.Instance.CurrentInteractable;
        if (interactable == null)
        {
            var feedback = GameObject.FindObjectOfType<PlayerFeedbackUI>();
            if (feedback != null)
                feedback.ShowFeedback(FeedbackType.RawMeat); // hoặc tạo FeedbackType.NearCampfire
            return;
        }

        // 🔹 Kiểm tra có phải campfire không
        Campfire campfire = (interactable as MonoBehaviour)?.GetComponent<Campfire>();
        if (campfire == null || !campfire.IsBurning)
        {
            var feedback = GameObject.FindObjectOfType<PlayerFeedbackUI>();
            if (feedback != null)
                feedback.ShowFeedback(FeedbackType.RawMeat);
            return;
        }

        Cookable cookable = campfire.GetComponent<Cookable>();
        if (cookable == null) return;

        // Cook tất cả số lượng raw meat trong slot
        int cookQty = slot.GetQuantity();
        cookable.Cook(inventoryManager.playerInventory, cookQty);

        // 🔹 Hiển thị prompt cook xong
        var uiManager = FindObjectOfType<PlayerUIManager>();
        if (uiManager != null)
            uiManager.ShowPrompt(cookable);

        inventoryManager.RefreshAllUI();
    }




    private Campfire FindNearestBurningCampfire()
    {
        float maxDistance = 3f;
        Campfire nearest = null;
        float minDist = float.MaxValue;

        foreach (var campfire in GameObject.FindObjectsOfType<Campfire>())
        {
            if (!campfire.IsBurning) continue;
            float dist = Vector3.Distance(campfire.transform.position, PlayerStatus.Instance.transform.position);
            if (dist < minDist && dist <= maxDistance)
            {
                minDist = dist;
                nearest = campfire;
            }
        }
        return nearest;
    }
}