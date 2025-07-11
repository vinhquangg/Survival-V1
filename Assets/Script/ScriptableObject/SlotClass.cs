using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SlotClass
{
    [SerializeField] private ItemClass item;
    [SerializeField] private int quantity;
    private float durability;
    
    public SlotClass()
    {
        item = null;
        quantity = 0;
        durability = 0;
    }

    public SlotClass(ItemClass _item, int _quantity)
    {
        item= _item;
        quantity= _quantity;

        if (item is ToolClass tool)
            durability = tool.durability; 
        else
            durability = -1f; 
    }

    public ItemClass GetItem() { return item; }
    public int GetQuantity() { return quantity; }
    public float GetDurability() => durability;
    // Setters
    public void AddQuantity(int value) => quantity += value;
    public void SubQuantity(int value) => quantity -= value;
    public void SetDurability(float value) => durability = Mathf.Clamp01(value);
    public bool IsTool() => item != null && item.itemType == ItemType.Tool;
    public bool IsEmpty() 
    {
        return this.item == null || this.quantity <=0;
    }
    public void AddItem(ItemClass newItem, int quantity)
    {
        this.item = newItem;
        this.quantity = quantity;
    }
}
