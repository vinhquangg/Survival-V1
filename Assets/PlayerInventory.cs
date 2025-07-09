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
            // ✅ Thêm được ít nhất một phần → không log
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
            if (slot != null && slot.GetItem().itemName == item.itemName)
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
            if (slot != null && slot.GetItem().itemName == item.itemName)
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
        foreach (SlotClass slot in container)
        {
            if (slot != null && slot.GetItem().itemName == item.itemName && slot.GetQuantity() < item.maxStack)
            {
                int canAdd = Mathf.Min(quantity, item.maxStack - slot.GetQuantity());
                slot.AddQuantity(canAdd);
                quantity -= canAdd;

                if (quantity <= 0) return true;
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

    public void Clear()
    {
        for (int i = 0; i < items.Length; i++)
            items[i] = null;

        for (int i = 0; i < hotbarItems.Length; i++)
            hotbarItems[i] = null;
    }

}
