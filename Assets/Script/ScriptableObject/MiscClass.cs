using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewMisc", menuName = "Item/Misc")]
public class MiscClass : ItemClass
{
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return this; }
    public override Consumable GetConsumable() { return null; }
    public override WeaponClass GetWeapon() { return null; }
    public override float GetDurability() => -1f;
}
