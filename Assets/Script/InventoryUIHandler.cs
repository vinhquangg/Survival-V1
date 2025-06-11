using System.Collections.Generic;
using UnityEngine;

public class InventoryUIHandler : MonoBehaviour
{
    public GameObject slotHolder;
    public InventoryManager inventoryManager;
    private GameObject[] slots;

    public InventoryArea area;
    public GameObject[] Init()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slotHolder.transform.GetChild(i).gameObject;

            var ui = slot.GetComponent<InventorySlotUI>();
            ui.slotIndex = i;
            ui.inventoryArea = area;
            ui.inventoryManager = inventoryManager;

            slots[i] = slot;
        }
        return slots;
    }


    public void RefreshUI(SlotClass[] dataSlots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var uiRef = slots[i].GetComponent<SlotUIRef>();
            if (uiRef == null) continue;

            if (i < dataSlots.Length && dataSlots[i] != null && !dataSlots[i].IsEmpty())
            {
                var slot = dataSlots[i];
                var item = slot.GetItem();

                uiRef.iconImage.sprite = item.itemIcon;
                uiRef.iconImage.enabled = true;

                if (slot.IsTool())
                {
                    uiRef.durabilitySlider.gameObject.SetActive(true);
                    uiRef.durabilitySlider.value = slot.GetDurability();

                    uiRef.amountText.text = "";
                    uiRef.amountText.gameObject.SetActive(false);
                }
                else
                {
                    uiRef.durabilitySlider.gameObject.SetActive(false);
                    uiRef.amountText.text = slot.GetQuantity().ToString();
                    uiRef.amountText.gameObject.SetActive(true);
                }
            }
            else
            {
                uiRef.iconImage.sprite = null;
                uiRef.iconImage.enabled = false;
                uiRef.amountText.text = "";
                uiRef.amountText.gameObject.SetActive(true);
                uiRef.durabilitySlider.gameObject.SetActive(false);
            }
        }
    }

    public int GetSlotCount()
    {
        return slotHolder.transform.childCount;
    }
}
