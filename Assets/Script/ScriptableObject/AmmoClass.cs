using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Arrow", menuName = "Item/Arrow")]
public class AmmoClass : ItemClass
{
    [Header("Ammo Settings")]
    public int damageMultiplier;
    public GameObject projectilePrefab;    
    public float speed;

    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }
    public override SurvivalClass GetSurvival() { return null; }
    public override WeaponClass GetWeapon() { return null ; }
    public override AmmoClass GetAmmo() { return this; }

    public override float GetDurability() => -1f; 
}
