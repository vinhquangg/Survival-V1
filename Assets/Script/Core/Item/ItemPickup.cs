using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    private ItemEntity itemEntity;
    private InteractableObject interactableObject;

    [SerializeField] private GameObject tooltipUI;
    private TextMeshProUGUI interactionInfoText;
    private TextMeshProUGUI interactionTypeText;

    private void Awake()
    {
        itemEntity = GetComponent<ItemEntity>();
        if (itemEntity == null)
            Debug.LogError("Missing ItemEntity on ItemPickup");

        interactableObject = GetComponent<InteractableObject>();
        if (interactableObject == null)
            Debug.LogError("Missing InteractableObject on ItemPickup");

        if (tooltipUI == null)
            Debug.LogWarning("Tooltip UI chưa gán cho: " + gameObject.name);

        interactionInfoText = tooltipUI.transform.Find("interaction_Info")?.GetComponent<TextMeshProUGUI>();
        interactionTypeText = tooltipUI.transform.Find("interaction_type")?.GetComponent<TextMeshProUGUI>();

        if (interactionInfoText != null)
            interactionInfoText.text = interactableObject.GetItemName();

        if (interactionTypeText != null)
            interactionTypeText.text = interactableObject.GetItemType();

        HideUI();
    }

    public string GetItemName()
    {
        return interactableObject.GetItemName();
    }

    public string GetItemType()
    {
        return interactableObject.GetItemType();
    }

    //public void Interact(GameObject interactor)
    //{
    //    var inventory = interactor.GetComponentInChildren<InventoryManager>();
    //    if (inventory != null)
    //    {
    //        inventory.AddItem(itemEntity.GetItemData(), itemEntity.GetQuantity());
    //        Destroy(gameObject);
    //    }
    //}

    public void Interact(GameObject interactor)
    {
        var inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) return;

        bool added = inventoryManager.AddItem(itemEntity.GetItemData(), itemEntity.GetQuantity());

        if (added)
        {
            ObjectPoolManager.Instance?.ReturnToPool(gameObject);
        }
    }




    public GameObject GetItemUI()
    {
        return tooltipUI;
    }

    public void ShowUI()
    {
        if (tooltipUI != null)
            tooltipUI.SetActive(true);
    }

    public void HideUI()
    {
        if (tooltipUI != null)
            tooltipUI.SetActive(false);
    }

}
