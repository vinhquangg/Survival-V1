using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour
{
    [System.Serializable]
    public class EquipSlot
    {
        public EquipType type;
        public ItemClass itemData;
        public GameObject itemObject;
    }

    public List<EquipSlot> equipSlots = new List<EquipSlot>();

    private Dictionary<EquipType, GameObject> currentEquipped = new Dictionary<EquipType, GameObject>();

    public void DisplayItem(ItemClass item)
    {
        EquipType type = item.GetEquipType();
        if (currentEquipped.ContainsKey(type) && currentEquipped[type] != null)
        {
            currentEquipped[type].SetActive(false);
        }

        foreach (var slot in equipSlots)
        {
            if (slot.itemData == item)
            {
                currentEquipped[type] = slot.itemObject;
                currentEquipped[type].SetActive(true);
                return;
            }
        }

        Debug.LogWarning($"Item {item.itemName} không tìm thấy trong slot.");
    }

    public void HideItem(ItemClass item)
    {
        if (item == null) return;

        EquipType type = item.GetEquipType();

        // ✅ Nếu item không có type trang bị (ví dụ: Consumable, Misc...) thì bỏ qua
        if (type == EquipType.None) return;

        if (currentEquipped.TryGetValue(type, out var go) && go != null)
        {
            go.SetActive(false);
        }
    }


    public bool HasItemEquipped(EquipType type)
    {
        return currentEquipped.ContainsKey(type) && currentEquipped[type].activeSelf;
    }

    public ItemClass GetEquippedItem(EquipType type)
    {
        foreach (var slot in equipSlots)
        {
            if (slot.type == type && currentEquipped.ContainsKey(type) && currentEquipped[type] == slot.itemObject)
            {
                return slot.itemData;
            }
        }

        return null;
    }

}
