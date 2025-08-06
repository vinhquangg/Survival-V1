using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Item/Weapon")]
public class WeaponClass : ItemClass
{
    public WeaponType weaponType;
    public float durability = 1f;
    public WeaponClass()
    {
        itemType = ItemType.Tool;
        itemName = "Weapon";
        itemIcon = null;
        weaponType = WeaponType.None;
    }

    public enum WeaponType
    {
        None,
        Machete,
        Sword
    }
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }
    public override SurvivalClass GetSurvival() { return null; }
    public override WeaponClass GetWeapon() { return this; }

    public override float GetDurability()
    {
        return durability;
    }

    public override EquipType GetEquipType()
    {
        return EquipType.Weapon; 
    }
}
