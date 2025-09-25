using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable, IInteractableInfo, IPoolable
{
    private ItemEntity itemEntity;
    private InteractableObject interactableObject;

    [SerializeField] private GameObject tooltipUI;
    private TextMeshProUGUI interactionInfoText;
    private TextMeshProUGUI interactionTypeText;
    private bool isPickedUp = false;
    private void Awake()
    {
        itemEntity = GetComponent<ItemEntity>();
        if (itemEntity == null)
            Debug.LogError("Missing ItemEntity on ItemPickup");

        interactableObject = GetComponent<InteractableObject>();
        if (interactableObject == null)
            Debug.LogError("Missing InteractableObject on ItemPickup");
    }

    //public string GetItemName()
    //{
    //    return interactableObject.GetItemName();
    //}

    //public string GetItemType()
    //{
    //    return interactableObject.GetItemType();
    //}

    public void Interact(GameObject interactor)
    {
        var inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) return;
        if (isPickedUp) return;
        var itemData = itemEntity.GetItemData();

        if(itemData.itemType == ItemType.Consumable)
        {
            var consumable = itemData as Consumable;
            
            if (consumable != null && !(consumable.isMeat && consumable.meatState != AnimalMeat.Cooked))
            {
                var playerStatus = interactor.GetComponent<PlayerStatus>();
                var feedback = GameObject.FindObjectOfType<PlayerFeedbackUI>();
                if (playerStatus != null && playerStatus.hunger.IsFull())
                {

                    if (feedback != null)
                        feedback.ShowFeedback(FeedbackType.Full);
                    return;
                }
            }            
        }
        Debug.Log($"▶️ Nhặt: {itemEntity.GetItemData().name} - SL: {itemEntity.GetQuantity()}");

        bool added = inventoryManager.AddItem(itemEntity.GetItemData(), itemEntity.GetQuantity());

        if (added)
        {
            isPickedUp = true;
            GameObject root = gameObject.GetComponent<PoolableObject>() != null ? gameObject : gameObject.transform.root.gameObject;
            ObjectPoolManager.Instance?.ReturnToPool(root);
        }
    }


    public GameObject GetItemUI()
    {
        return tooltipUI;
    }

    public string GetName()
    {
        return interactableObject.GetItemName();
    }

    public Sprite GetIcon()
    {
       return itemEntity.GetItemIcon();
    }
    public string GetItemAmount()
    {
        return "x" + itemEntity.GetQuantity().ToString();
    }

    public InteractionType GetInteractionType() => InteractionType.Pickup;

    public void OnSpawned()
    {
        gameObject.SetActive(!isPickedUp);
    }

    public void OnReturned()
    {
        gameObject.SetActive(false);
    }
}
