using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass:ScriptableObject
{
    [Header ("Item")]
    public ItemType itemType = ItemType.None;
    public string itemName;
    public Sprite itemIcon;
    public string itemDesc;
    public string itemFunc;
    public bool isStack;
    public int maxStack;
    public GameObject dropPrefab;
    public abstract ItemClass GetItem();
    public abstract ToolClass GetTool();
    public abstract MiscClass GetMisc();
    public abstract Consumable GetConsumable();

    public abstract float GetDurability();
}
