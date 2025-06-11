using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTool", menuName = "Item/Tool")]
public class ToolClass : ItemClass
{
    public ToolType toolType;
    public float durability = 1f; 
    public ToolClass()
    {
        itemType = ItemType.Tool;
        itemName = "Tool";
        itemIcon = null; 
        toolType = ToolType.None;
    }

    public enum ToolType
    {
        None = 0,
        Hammer = 1,
        Axe = 2,
        Pickaxe = 3,
        Shovel = 4,
        FishingRod = 5
    }
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }

    public override float GetDurability()
    {
        return durability;
    }
}
