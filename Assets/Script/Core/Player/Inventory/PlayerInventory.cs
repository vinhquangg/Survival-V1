using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public SlotClass[] items;
    public SlotClass[] hotbarItems;

    public void Init(int inventorySize, int hotbarSize)
    {
        items = new SlotClass[inventorySize];
        hotbarItems = new SlotClass[hotbarSize];
    }

    public bool AddItem(ItemClass item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        int originalQuantity = quantity;

        if (item.isStack)
        {
            if (TryStackItem(items, item, ref quantity)) return true;
            if (TryStackItem(hotbarItems, item, ref quantity)) return true;
        }

        if (TryAddToEmptySlot(hotbarItems, item, ref quantity)) return true;
        if (TryAddToEmptySlot(items, item, ref quantity)) return true;

        if (quantity < originalQuantity)
        {
            return true;
        }

        Debug.LogWarning("Inventory và hotbar đầy.");
        return false;
    }

    public bool HasItem(ItemClass item, int amount)
    {
        int total = 0;

        // Check item in inventory first
        foreach (var slot in items)
        {
            if (slot != null && slot.HasSameItem(item))
            {
                total += slot.GetQuantity();
                if (total >= amount) return true;
            }
        }

        // check item in hotbar
        foreach (var slot in hotbarItems)
        {
            if (slot != null && slot.GetItem() == item)
            {
                total += slot.GetQuantity();
                if (total >= amount) return true;
            }
        }

        return false;
    }


    public bool RemoveItem(ItemClass item, int amount)
    {
        int removed = 0;

        // remove from inventory first
        for (int i = 0; i < items.Length; i++)
        {
            var slot = items[i];
            if (slot != null && slot.HasSameItem(item))
            {
                int qty = slot.GetQuantity();
                int toRemove = Mathf.Min(qty, amount - removed);
                slot.SubQuantity(toRemove);
                removed += toRemove;

                if (slot.GetQuantity() <= 0)
                    items[i] = null;

                if (removed >= amount) return true;
            }
        }

        // remove from hotbar if not enough removed from main inventory
        for (int i = 0; i < hotbarItems.Length; i++)
        {
            var slot = hotbarItems[i];
            if (slot != null && slot.GetItem() == item)
            {
                int qty = slot.GetQuantity();
                int toRemove = Mathf.Min(qty, amount - removed);
                slot.SubQuantity(toRemove);
                removed += toRemove;

                if (slot.GetQuantity() <= 0)
                    hotbarItems[i] = null;

                if (removed >= amount) return true;
            }
        }

        return removed >= amount;
    }

    public List<SlotClass> GetAllRawMeatSlots()
    {
        List<SlotClass> result = new();
        foreach (var slot in hotbarItems)
            if (slot != null && slot.GetItem() is Consumable c && c.isMeat && c.meatState == AnimalMeat.Raw)
                result.Add(slot);
        foreach (var slot in items)
            if (slot != null && slot.GetItem() is Consumable c && c.isMeat && c.meatState == AnimalMeat.Raw)
                result.Add(slot);
        return result;
    }

    public void ClearSlot(SlotClass slot)
    {
        for (int i = 0; i < hotbarItems.Length; i++)
            if (hotbarItems[i] == slot) hotbarItems[i] = null;
        for (int i = 0; i < items.Length; i++)
            if (items[i] == slot) items[i] = null;
    }


    public SlotClass Contains(ItemClass item)
    {
        foreach (SlotClass slot in items)
        {
            if (slot != null && slot.GetItem().itemName == item.itemName)
                return slot;
        }
        return null;
    }

    private bool TryStackItem(SlotClass[] container, ItemClass item, ref int quantity)
    {
        if (container == null) return false;  // tránh container null

        foreach (SlotClass slot in container)
        {
            // 👉 thêm check slot.GetItem() != null
            if (slot != null && slot.GetItem() != null &&
                slot.GetItem().itemName == item.itemName &&
                slot.GetQuantity() < item.maxStack)
            {
                int canAdd = Mathf.Min(quantity, item.maxStack - slot.GetQuantity());
                slot.AddQuantity(canAdd);
                quantity -= canAdd;

                if (quantity <= 0)
                    return true;
            }
        }
        return false;
    }


    private bool TryAddToEmptySlot(SlotClass[] container, ItemClass item, ref int quantity)
    {
        for (int i = 0; i < container.Length; i++)
        {
            if (container[i] == null || container[i].IsEmpty())
            {
                int toAdd = Mathf.Min(quantity, item.maxStack);
                container[i] = new SlotClass(item, toAdd);
                quantity -= toAdd;
                return quantity <= 0;
            }
        }
        return false;
    }

    public int GetTotalQuantity(ItemClass item)
    {
        int total = 0;

        foreach (var slot in items)
            if (slot != null && slot.GetItem() == item)
                total += slot.GetQuantity();

        foreach (var slot in hotbarItems)
            if (slot != null && slot.GetItem() == item)
                total += slot.GetQuantity();

        return total;
    }

    public void UseItemFromSlot(InventoryArea area, int index)
    {
        SlotClass[] container = area == InventoryArea.Hotbar ? hotbarItems : items;

        if (index < 0 || index >= container.Length) return;

        var slot = container[index];
        if (slot == null || slot.IsEmpty()) return;

        ItemClass item = slot.GetItem();

        if (item is IUsableItem usable)
        {
            usable.UseItem(PlayerStatus.Instance, this);
        }
        else
        {
            Debug.LogWarning($"Item {item.itemName} is not usable.");
        }
    }

    public bool HasPlacableItem()
    {
        foreach (var slot in items)
        {
            if (slot != null && slot.GetItem() != null && slot.GetItem().itemType == ItemType.Placable)
            {
                return true;
            }
        }

        foreach (var slot in hotbarItems)
        {
            if (slot != null && slot.GetItem() != null && slot.GetItem().itemType == ItemType.Placable)
            {
                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        for (int i = 0; i < items.Length; i++)
            items[i] = null;

        for (int i = 0; i < hotbarItems.Length; i++)
            hotbarItems[i] = null;
    }

}
