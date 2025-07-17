using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyHandle : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public WeaponManager weaponManager;
    private void Update()
    {
        int hotbarSize = inventoryManager.playerInventory.hotbarItems.Length;

        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseItemInHotBar(i);
            }
        }
    }

    private  void UseItemInHotBar(int index)
    {
        var slot = inventoryManager.playerInventory.hotbarItems[index];
        
        if(slot == null || slot.IsEmpty() || slot.GetItem() == null)
        {
            Debug.LogWarning($"Hotbar slot {index+1} is empty or invalid.");
            return;
        }

        var item = slot.GetItem();

        if (item.itemType == ItemType.Weapon)
        {
            weaponManager.EquipWeapon(item);
        }
        else
        {
            weaponManager.UnequipWeapon();
        }

        if (item is IUsableItem usable)
        {
            usable.UseItem(PlayerStatus.Instance, inventoryManager.playerInventory);
            inventoryManager.RefreshAllUI();
        }
    }
}
