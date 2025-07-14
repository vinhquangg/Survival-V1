using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewConsumable", menuName = "Item/Consumable")]
public class Consumable : ItemClass,IUsableItem
{
    public float headthAdded;
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override Consumable GetConsumable() { return this; }
    public override float GetDurability() => -1f;

    public void UseItem(PlayerStatus status, PlayerInventory inventory)
    {
        if (status == null)
        {
            Debug.LogWarning("❗ PlayerStatus is null when using Consumable.");
            return;
        }

        if (status.health.currentValue >= status.health.data.maxValue)
        {
            Debug.Log("✅ Máu đã đầy, không thể dùng vật phẩm hồi máu.");
            return;
        }

        Debug.Log($"[Consumable] Heal: +{headthAdded}");
        status.health.Restore(headthAdded);
        inventory.RemoveItem(this, 1);
    }


}
