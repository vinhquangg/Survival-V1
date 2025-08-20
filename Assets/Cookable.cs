﻿using UnityEngine;
﻿using System;
using UnityEngine;

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    public Sprite cookIcon;
    [SerializeField] private Sprite icon;
    [SerializeField] private string cookName = "Campfire";

    private Campfire campfire;

    private void Start()
    {
        campfire = GetComponent<Campfire>(); // 🔗 tham chiếu Campfire cùng object
    }

    public Sprite GetIcon() => icon;
    public string GetName() => cookName;
    public string GetItemAmount() => "";  // có thể return "Raw Meat x1" nếu đang nấu

    public InteractionType GetInteractionType()
    {
        // Chỉ hiển thị Cook khi lửa đang cháy
        return (campfire != null && campfire.IsBurning) ? InteractionType.Cook : InteractionType.None;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("Start cooking...");
        // TODO: logic nấu
        if (campfire != null && campfire.IsBurning)
        {
            Debug.Log("🍖 Cooking is start!");
            // TODO: gọi hệ thống cooking (inventory → lấy item → nướng → spawn item chín)
        }
        else
        {
            Debug.Log("❌ Can't cook, campfire is not burning!");
        }
    }

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