using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    private ItemEntity itemEntity;
    private InteractableObject InteractableObject;


    private void Awake()
    {
        itemEntity = GetComponent<ItemEntity>();
        if (itemEntity == null)
            Debug.LogError("Missing ItemEntity on ItemPickup");
        InteractableObject = GetComponent<InteractableObject>();
        if (InteractableObject == null)
        {
            Debug.LogError("ItemPickup requires an InteractableObject component.");
        }
    }

    public string GetItemName()
    {
        return InteractableObject.GetItemName();
    }

    public string GetItemType()
    {
        return InteractableObject.GetItemType();
    }

    public void Interact(GameObject interactor)
    {
        var inventory = interactor.GetComponentInChildren<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddItem(itemEntity.GetItemData(), itemEntity.GetQuantity());
            Destroy(gameObject);
        }
    }
}
