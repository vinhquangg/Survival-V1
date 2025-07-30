using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewConsumable", menuName = "Item/Consumable")]
public class Consumable : ItemClass,IUsableItem
{
    public float statAdded;
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override WeaponClass GetWeapon() { return null; }
    public override Consumable GetConsumable() { return this; }
    public override float GetDurability() => -1f;

    public void UseItem(PlayerStatus status, PlayerInventory inventory)
    {
        if (status == null)
        {
            Debug.LogWarning("❗ PlayerStatus is null when using Consumable.");
            return;
        }

        //if (/*status.health.currentValue >= status.health.data.maxValue ||*/ status.hunger.currentValue >= status.hunger.data.maxValue)
        //{
        //    Debug.Log("✅ Máu đã đầy hoặc không còn đói, không thể dùng vật phẩm.");
        //    return;
        //}

        if (status.hunger.IsFull())
        {
            Debug.Log("❗ No quá rồi!");
            return;
        }


        Debug.Log($"[Consumable] Hunger: +{statAdded}");
        status.hunger.Restore(statAdded);
        inventory.RemoveItem(this, 1);
    }


}
