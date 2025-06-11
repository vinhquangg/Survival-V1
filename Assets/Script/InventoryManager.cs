using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public SlotClass[] items;
    public SlotClass[] itemstoremove;
    public SlotClass[] hotbarItems;
    public GameObject inventoryPanel;
    public InventoryUIHandler inventoryUI;
    public InventoryUIHandler hotbarUI;
    public ItemDropper dropper;
    private PlayerController PlayerController;
    private bool isInventoryOpen = false;

    void Awake()
    {
        inventoryUI.inventoryManager = this;
        hotbarUI.inventoryManager = this;

        inventoryUI.Init();
        hotbarUI.Init();

        items = new SlotClass[inventoryUI.GetSlotCount()];
        hotbarItems = new SlotClass[hotbarUI.GetSlotCount()];
    }


    void Start()
    {
        RefreshAllUI();
        PlayerController = FindObjectOfType<PlayerController>();
        dropper.playerTransform = PlayerController.transform;
    }


    public bool AddItem(ItemClass item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;

        if (item.isStack)
        {
            if (TryStackItem(InventoryArea.Inventory, item, ref quantity)) return true;
            if (TryStackItem(InventoryArea.Hotbar, item, ref quantity)) return true;
        }

        if (TryAddToEmptySlot(InventoryArea.Hotbar, item, ref quantity)) return true;
        if (TryAddToEmptySlot(InventoryArea.Inventory, item, ref quantity)) return true;

        Debug.LogWarning("Không thể thêm item, inventory & hotbar đầy.");
        return false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private bool TryStackItem(InventoryArea area, ItemClass item, ref int quantity)
    {
        var container = GetContainer(area);

        for (int i = 0; i < container.Length; i++)
        {
            var slot = container[i];
            if (slot != null &&
                slot.GetItem() == item &&
                slot.GetQuantity() < item.maxStack)
            {
                int canAdd = Mathf.Min(quantity, item.maxStack - slot.GetQuantity());
                slot.AddQuantity(canAdd);
                quantity -= canAdd;

                if (quantity <= 0)
                {
                    RefreshAllUI();
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryAddToEmptySlot(InventoryArea area, ItemClass item, ref int quantity)
    {
        var container = GetContainer(area);

        for (int i = 0; i < container.Length; i++)
        {
            if (container[i] == null || container[i].IsEmpty())
            {
                int toAdd = Mathf.Min(quantity, item.maxStack);
                container[i] = new SlotClass(item, toAdd);
                quantity -= toAdd;

                if (quantity <= 0)
                {
                    RefreshAllUI();
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(ItemClass item)
    {
        SlotClass temp = Contains(item);

        if( temp != null)
        {
            if(temp.GetQuantity() > 1)
            {
                temp.SubQuantity(1);
            }
            else
            {
                int slotToRemoveIndex = 0; 
                for(int i=0;i<items.Length;i++)
                {
                    if (items[i].GetItem()==item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].IsEmpty();
            }
        }
        else
        {
            return false;
        }
        RefreshAllUI();
        return true;

    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        inventoryPanel.SetActive(isInventoryOpen);

        PlayerController.enabled = !isInventoryOpen;

        GameManager.instance?.SetCursorLock(!isInventoryOpen);

        if (isInventoryOpen)
        {
            RefreshAllUI();
        }
    }

    public SlotClass Contains(ItemClass item)
    {
        foreach (SlotClass slot in items)
        {
            if (slot != null && slot.GetItem() == item)
            {
                return slot;
            }
        }
        return null;
    }

    public void TryMergeOrSwap(InventoryArea fromArea, int fromIndex, InventoryArea toArea, int toIndex)
    {
        var fromContainer = (fromArea == InventoryArea.Hotbar) ? hotbarItems : items;
        var toContainer = (toArea == InventoryArea.Hotbar) ? hotbarItems : items;

        var fromSlot = fromContainer[fromIndex];
        var toSlot = toContainer[toIndex];

        if (fromSlot == null || fromSlot.IsEmpty()) return;

        if (toSlot != null && !toSlot.IsEmpty()
            && fromSlot.GetItem() == toSlot.GetItem()
            && fromSlot.GetItem().isStack)
        {
            int currentTo = toSlot.GetQuantity();
            int currentFrom = fromSlot.GetQuantity();
            int max = fromSlot.GetItem().maxStack;
            int spaceLeft = max - currentTo;

            if (spaceLeft > 0)
            {
                int amountToMove = Mathf.Min(spaceLeft, currentFrom);
                toSlot.AddQuantity(amountToMove);
                fromSlot.SubQuantity(amountToMove);

                if (fromSlot.GetQuantity() <= 0)
                    fromContainer[fromIndex] = null;

                RefreshAllUI();
                return;
            }
        }

        var temp = fromContainer[fromIndex];
        fromContainer[fromIndex] = toContainer[toIndex];
        toContainer[toIndex] = temp;

        RefreshAllUI();
    }

    public void SplitItem(InventoryArea area, int fromIndex)
    {
        var container = area == InventoryArea.Hotbar ? hotbarItems : items;
        var fromSlot = container[fromIndex];

        if (fromSlot == null || fromSlot.IsEmpty()) return;
        if (!fromSlot.GetItem().isStack || fromSlot.GetQuantity() <= 1) return;

        for (int i = 0; i < container.Length; i++)
        {
            if (container[i] == null || container[i].IsEmpty())
            {
                int half = fromSlot.GetQuantity() / 2;
                fromSlot.SubQuantity(half);
                container[i] = new SlotClass(fromSlot.GetItem(), half);
                RefreshAllUI();
                Debug.Log($"Split {half} items from slot {fromIndex} to slot {i} in {area}");
                return;
            }
        }

        Debug.LogWarning("No empty slot to split item!");
    }

    private SlotClass[] GetContainer(InventoryArea area)
    {
        return area == InventoryArea.Hotbar ? hotbarItems : items;
    }

    public SlotClass GetSlot(InventoryArea area, int index)
    {
        var container = GetContainer(area);
        if (index >= 0 && index < container.Length)
        {
            return container[index];
        }
        return null;
    }

    public void DropItemToWorld(InventoryArea area, int index)
    {
        var container = GetContainer(area);
        if (container == null || index < 0 || index >= container.Length) return;

        var slot = container[index];
        if (slot == null || slot.IsEmpty()) return;

        dropper.Drop(slot.GetItem(), slot.GetQuantity(), slot.GetDurability());
        container[index] = null;
        RefreshAllUI();
    }

    public void RefreshAllUI()
    {
        inventoryUI.RefreshUI(items);
        hotbarUI.RefreshUI(hotbarItems);
    }
}
