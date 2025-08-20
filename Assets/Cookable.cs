using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string cookName = "Campfire";

    private Campfire campfire;

    private void Start()
    {
        campfire = GetComponent<Campfire>(); // 🔗 tham chiếu Campfire cùng object
    }

    public Sprite GetIcon() => icon;
    public string GetName() => cookName;
    public string GetItemAmount() => "";

    public InteractionType GetInteractionType()
    {
        // Chỉ hiển thị Cook khi lửa đang cháy
        return (campfire != null && campfire.IsBurning) ? InteractionType.Cook : InteractionType.None;
    }

    public void Interact(GameObject interactor)
    {
        //var playerInv = interactor.GetComponentInChildren<PlayerInventory>();
        //if (playerInv != null)
        //    Cook(playerInv); // Gọi luôn hàm cook chung
    }

    public void Cook(PlayerInventory playerInv, int quantityToCook = -1)
    {
        if (campfire == null || !campfire.IsBurning) return;
        if (playerInv == null) return;

        List<SlotClass> rawMeatSlots = playerInv.GetAllRawMeatSlots();
        if (rawMeatSlots.Count == 0) return;

        foreach (var slot in rawMeatSlots)
        {
            if (!(slot.GetItem() is Consumable c) || !c.isMeat || c.meatState != AnimalMeat.Raw)
                continue;

            // Lấy số lượng thực từ ItemEntity
            int qty = slot.GetQuantity();
            if (quantityToCook > 0) qty = Mathf.Min(qty, quantityToCook);

            slot.SubQuantity(qty);
            if (slot.GetQuantity() <= 0) playerInv.ClearSlot(slot);

            // Spawn raw meat prefab với quantity
            var rawMeatObj = GameObject.Instantiate(c.dropPrefab, campfire.CookPoint.position, Quaternion.identity);
            var itemEntity = rawMeatObj.GetComponent<ItemEntity>();
            if (itemEntity != null)
            {
                itemEntity.Initialize(c, qty); // 🔹 quantity lấy từ ItemEntity
            }

            // Coroutine cook tự động
            StartCoroutine(CookAfterDelay(rawMeatObj, 10f, c.cookedPrefab, qty));
        }

        FindObjectOfType<PlayerUIManager>()?.ShowPrompt(this);
        Debug.Log("🍖 Started cooking raw meat...");
    }

    private IEnumerator CookAfterDelay(GameObject rawMeatObj, float delay, GameObject cookedPrefab, int qty)
    {
        yield return new WaitForSeconds(delay);

        if (rawMeatObj != null)
        {
            Vector3 pos = rawMeatObj.transform.position;
            Destroy(rawMeatObj);

            // Spawn cooked meat prefab với quantity = qty
            var cookedObj = GameObject.Instantiate(cookedPrefab, pos, Quaternion.identity);
            var itemEntity = cookedObj.GetComponent<ItemEntity>();
            if (itemEntity != null)
            {
                // giữ nguyên số lượng
                itemEntity.Initialize(itemEntity.GetItemData(), qty);
            }
        }
    }



}
