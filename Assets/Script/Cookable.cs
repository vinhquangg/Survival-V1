using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string cookName;
    private Campfire campfire;

    private void Start()
    {
        campfire = GetComponent<Campfire>(); // 🔗 tham chiếu Campfire
    }

    public Sprite GetIcon() => icon;
    public string GetName() => cookName;
    public string GetItemAmount() => "";

    public InteractionType GetInteractionType()
    {
        if (campfire != null && campfire.IsBurning)
            return InteractionType.Cook;
        else
            return InteractionType.None;
    }

    public void Interact(GameObject interactor)
    {
        var playerInv = interactor.GetComponentInChildren<PlayerInventory>();
        if (playerInv != null)
            Cook(playerInv);
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

            int qty = slot.GetQuantity();
            if (quantityToCook > 0) qty = Mathf.Min(qty, quantityToCook);

            slot.SubQuantity(qty);
            if (slot.GetQuantity() <= 0) playerInv.ClearSlot(slot);

            var rawMeatObj = GameObject.Instantiate(c.dropPrefab, campfire.CookPoint.position, Quaternion.identity);
            var itemEntity = rawMeatObj.GetComponent<ItemEntity>();
            if (itemEntity != null) itemEntity.Initialize(c, qty);

            // 🔹 Notify campfire bắt đầu nấu
            campfire.StartCooking(c.GetName(), c.itemIcon, qty);

            StartCoroutine(CookAfterDelay(rawMeatObj, 10f, c.cookedPrefab, qty));
        }

        Debug.Log("🍖 Started cooking raw meat...");
    }

    private IEnumerator CookAfterDelay(GameObject rawMeatObj, float delay, GameObject cookedPrefab, int qty)
    {
        if (rawMeatObj == null) yield break;

        var col = rawMeatObj.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(delay);

        if (rawMeatObj != null)
        {
            Vector3 pos = rawMeatObj.transform.position;
            Destroy(rawMeatObj);

            var cookedObj = GameObject.Instantiate(cookedPrefab, pos, Quaternion.identity);
            var cookedEntity = cookedObj.GetComponent<ItemEntity>();
            if (cookedEntity != null) cookedEntity.Initialize(cookedEntity.GetItemData(), qty);

            var cookedCol = cookedObj.GetComponent<Collider>();
            if (cookedCol != null) cookedCol.enabled = true;

            // 🔹 Notify campfire cook xong
            campfire.FinishCooking();
        }
    }
}
