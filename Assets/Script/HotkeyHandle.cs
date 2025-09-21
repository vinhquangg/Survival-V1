using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyHandle : MonoBehaviour, IPlayerDependent
{
    public InventoryManager inventoryManager;
    public EquipManager equipManager;
    public HotbarSelector hotbarSelector;

    private int currentHotkeyIndex = -1;

    private void Update()
    {
        int hotbarSize = inventoryManager.playerInventory.hotbarItems.Length;

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
        if (equipManager.animController != null && (equipManager.animController.IsAttacking ||equipManager.animController.IsChopping))
        {
            Debug.Log("[HotkeyHandle] Đang attack → không cho đổi item");
            return;
        }


        if (hotbarSelector != null)
            hotbarSelector.SelectSlot(index);

        var slot = inventoryManager.playerInventory.hotbarItems[index];
        if (slot == null || slot.IsEmpty() || slot.GetItem() == null)
        {
            if (currentHotkeyIndex != -1)
            {
                UnequipAll();
            }
            currentHotkeyIndex = -1;
            return;
        }

        var item = slot.GetItem();

        if (currentHotkeyIndex != -1 && index != currentHotkeyIndex)
        {
            UnequipAll();
            currentHotkeyIndex = -1;
        }


        if (item == null)
        {
            return;
        }

        if (index == currentHotkeyIndex)
        {
            UnequipAll();
            currentHotkeyIndex = -1;

            return;
        }


        if (item is Consumable c && c.isMeat && c.meatState == AnimalMeat.Raw)
        {
            TryCookFromHotbar(index);
            return;
        }


        if (item.itemType == ItemType.Placable && item.blueprint != null)
        {
            PlacementSystem.Instance.StartPlacement(item.blueprint, index);
            currentHotkeyIndex = index;
            return;
        }
        else if (PlacementSystem.Instance != null)
        {
            PlacementSystem.Instance.CancelPlacement();
        }

        EquipType equipType = item.GetEquipType();
        if (equipType != EquipType.None)
        {
            UnequipAll();

            equipManager.EquipItem(item);
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.equipSound);

            currentHotkeyIndex = index;
            return;
        }

        if (item is IUsableItem usable)
        {
            usable.UseItem(PlayerStatus.Instance, inventoryManager.playerInventory);
            inventoryManager.RefreshAllUI();
            currentHotkeyIndex = -1;
            return;
        }

        currentHotkeyIndex = -1;
    }

    private void UnequipAll()
    {
        equipManager.UnequipItem(EquipType.Weapon);
        equipManager.UnequipItem(EquipType.Tool);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.unequipSound);
    }

    private void TryCookFromHotbar(int hotbarIndex)
    {
        var slot = inventoryManager.playerInventory.hotbarItems[hotbarIndex];
        if (slot == null || slot.IsEmpty()) return;

        if (!(slot.GetItem() is Consumable c) || !c.isMeat || c.meatState != AnimalMeat.Raw) return;

        var interactable = SelectionManager.Instance.CurrentInteractable;
        if (interactable == null)
        {
            var feedback = GameObject.FindObjectOfType<PlayerFeedbackUI>();
            if (feedback != null)
                feedback.ShowFeedback(FeedbackType.RawMeat);
            return;
        }

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

        int cookQty = slot.GetQuantity();
        cookable.Cook(inventoryManager.playerInventory, cookQty);

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

    public void SetPlayer(PlayerController player)
    {
        equipManager = player.GetComponent<EquipManager>();
        if (hotbarSelector == null)
            hotbarSelector = FindObjectOfType<HotbarSelector>();

    }
}
