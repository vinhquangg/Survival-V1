<<<<<<< HEAD
<<<<<<< HEAD
﻿﻿using UnityEngine;
﻿using System;
using UnityEngine;
=======
﻿using UnityEngine;
>>>>>>> parent of 88062a8 (new)

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    public string cookName = "Cooking Station";
    public Sprite cookIcon;
<<<<<<< HEAD
    [SerializeField] private Sprite icon;
    [SerializeField] private string cookName = "Campfire";
=======
﻿using UnityEngine;

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    public string cookName = "Cooking Station";
    public Sprite cookIcon;
>>>>>>> parent of 1f79ee6 (make cooked meat)

    public InteractionType GetInteractionType() => InteractionType.Cook;
    public string GetName() => cookName;
    public string GetItemAmount() => "";  // có thể return "Raw Meat x1" nếu đang nấu
<<<<<<< HEAD
=======
    public Sprite GetIcon() => cookIcon;
>>>>>>> parent of 1f79ee6 (make cooked meat)

    public void Interact(GameObject player)
    {
        Debug.Log("Start cooking...");
        // TODO: logic nấu
    }
<<<<<<< HEAD

    public void Interact(GameObject interactor)
=======

    public InteractionType GetInteractionType() => InteractionType.Cook;
    public string GetName() => cookName;
    public string GetItemAmount() => "";  // có thể return "Raw Meat x1" nếu đang nấu
    public Sprite GetIcon() => cookIcon;

    public void Interact(GameObject player)
>>>>>>> parent of 88062a8 (new)
    {
        Debug.Log("Start cooking...");
        // TODO: logic nấu
    }
<<<<<<< HEAD

    public void Cook(GameObject interactor)
    {
        if (campfire == null || !campfire.IsBurning)
        {
            Debug.Log("❌ Can't cook, campfire is not burning!");
            return;
        }

        PlayerInventory playerInv = interactor.GetComponent<PlayerInventory>();
        if (playerInv == null)
        {
            Debug.LogError("PlayerInventory not found on interactor!");
            return;
        }

        // Tìm raw meat trong hotbar
        SlotClass meatSlot = playerInv.FindRawMeatInHotbar();
        int meatIndex = playerInv.FindRawMeatInHotbarIndex();
        if (meatSlot == null || meatIndex < 0)
        {
            Debug.Log("❌ No raw meat to cook!");
            return;
        }

        // Xác định số lượng cook được
        int cookQty = Mathf.Min(meatSlot.GetQuantity(), campfire.MaxCookSlots);

        // Trừ raw meat
        meatSlot.SubQuantity(cookQty);
        if (meatSlot.GetQuantity() <= 0)
            playerInv.hotbarItems[meatIndex] = null;

        // Spawn cooked meat lên cookPoint
        for (int i = 0; i < cookQty; i++)
        {
            // Giả sử có prefab của cooked meat
            GameObject cookedMeatPrefab = meatSlot.GetItem().dropPrefab;
            if (cookedMeatPrefab != null && campfire.CookPoint != null)
            {
                Vector3 spawnPos = campfire.CookPoint.position + new Vector3(i * 0.3f, 0, 0);
                GameObject.Instantiate(cookedMeatPrefab, spawnPos, Quaternion.identity);
            }
        }

        Debug.Log($"🍖 Cooked {cookQty} raw meat!");
    }



}
=======
}
>>>>>>> parent of 1f79ee6 (make cooked meat)
=======
}
>>>>>>> parent of 88062a8 (new)
