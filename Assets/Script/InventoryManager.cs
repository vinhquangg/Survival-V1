using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public InventoryUIHandler inventoryUI;
    public InventoryUIHandler hotbarUI;
    public ItemDropper dropper;
    public event Action OnInventoryChanged;
    public PlayerInventory playerInventory;
    public GameObject pickupNotification; 
    public TextMeshProUGUI pickupText;
    public Image pickupImage;
    public float notifyDuration = 5f;
    private Coroutine notifyCoroutine;
    private PlayerController PlayerController;
    private bool isInventoryOpen = false;

    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        inventoryUI.inventoryManager = this;
        hotbarUI.inventoryManager = this;

        inventoryUI.Init();
        hotbarUI.Init();
        playerInventory.Init(inventoryUI.GetSlotCount(), hotbarUI.GetSlotCount());

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple InventoryManager instances found! Using the first one.");
            Destroy(gameObject);
        }

    }

    void Start()
    {
        RefreshAllUI();
        PlayerController = FindObjectOfType<PlayerController>();
        dropper.playerTransform = PlayerController.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public bool AddItem(ItemClass item, int quantity = 1)
    {
        bool added = playerInventory.AddItem(item, quantity);
        if (added)
        {
            RefreshAllUI();
            ShowPickupNotification($"+{quantity} {item.itemName}",item);
        }
        return added;
    }


    private void ShowPickupNotification(string message, ItemClass item)
    {
        if (pickupNotification == null || pickupText == null || pickupImage == null) return;

        if (notifyCoroutine != null)
            StopCoroutine(notifyCoroutine);

        pickupText.text = message;
        if (item != null && item.itemIcon != null)
            pickupImage.sprite = item.itemIcon;
        notifyCoroutine = StartCoroutine(ShowAndHideNotification());
    }

    private IEnumerator ShowAndHideNotification()
    {
        pickupNotification.SetActive(true);
        yield return new WaitForSeconds(notifyDuration);
        pickupNotification.SetActive(false);
    }


    //public bool RemoveItem(ItemClass item)
    //{
    //    bool removed = playerInventory.RemoveItem(item);
    //    if (removed) RefreshAllUI();
    //    return removed;
    //}

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        PlayerController.enabled = !isInventoryOpen;
        GameManager.instance?.SetCursorLock(!isInventoryOpen);

        if (CameraTarget.Instance != null)
            CameraTarget.Instance.allowCameraInput = !isInventoryOpen;

        if (isInventoryOpen)
        {
            RefreshAllUI();
        }
    }

    public SlotClass GetSlot(InventoryArea area, int index)
    {
        var container = area == InventoryArea.Hotbar ? playerInventory.hotbarItems : playerInventory.items;
        if (index >= 0 && index < container.Length)
            return container[index];
        return null;
    }

    public void TryMergeOrSwap(InventoryArea fromArea, int fromIndex, InventoryArea toArea, int toIndex)
    {
        var fromContainer = fromArea == InventoryArea.Hotbar ? playerInventory.hotbarItems : playerInventory.items;
        var toContainer = toArea == InventoryArea.Hotbar ? playerInventory.hotbarItems : playerInventory.items;

        var fromSlot = fromContainer[fromIndex];
        var toSlot = toContainer[toIndex];

        if (fromSlot == null || fromSlot.IsEmpty()) return;

        if (toSlot != null && !toSlot.IsEmpty()
            && fromSlot.GetItem() == toSlot.GetItem()
            && fromSlot.GetItem().isStack)
        {
            int currentTo = toSlot.GetQuantity();
            int currentFrom = fromSlot.GetQuantity();
            int max = fromSlot.GetItem().maxStack;
            int spaceLeft = max - currentTo;

            if (spaceLeft > 0)
            {
                int amountToMove = Mathf.Min(spaceLeft, currentFrom);
                toSlot.AddQuantity(amountToMove);
                fromSlot.SubQuantity(amountToMove);

                if (fromSlot.GetQuantity() <= 0)
                    fromContainer[fromIndex] = null;

                RefreshAllUI();
                return;
            }
        }

        // swap items if they are not stackable or no space left
        var temp = fromContainer[fromIndex];
        fromContainer[fromIndex] = toContainer[toIndex];
        toContainer[toIndex] = temp;

        RefreshAllUI();
    }

    public void SplitItem(InventoryArea area, int fromIndex)
    {
        var container = area == InventoryArea.Hotbar ? playerInventory.hotbarItems : playerInventory.items;
        var fromSlot = container[fromIndex];

        if (fromSlot == null || fromSlot.IsEmpty()) return;
        if (!fromSlot.GetItem().isStack || fromSlot.GetQuantity() <= 1) return;

        for (int i = 0; i < container.Length; i++)
        {
            if (container[i] == null || container[i].IsEmpty())
            {
                int half = fromSlot.GetQuantity() / 2;
                fromSlot.SubQuantity(half);
                container[i] = new SlotClass(fromSlot.GetItem(), half);
                RefreshAllUI();
                Debug.Log($"Split {half} items from slot {fromIndex} to slot {i} in {area}");
                return;
            }
        }

        Debug.LogWarning("No empty slot to split item!");
    }

    public void DropItemToWorld(InventoryArea area, int index)
    {
        var container = area == InventoryArea.Hotbar ? playerInventory.hotbarItems : playerInventory.items;
        if (container == null || index < 0 || index >= container.Length) return;

        var slot = container[index];
        if (slot == null || slot.IsEmpty()) return;

        dropper.Drop(slot.GetItem(), slot.GetQuantity(), slot.GetDurability());
        container[index] = null;
        RefreshAllUI();
    }

    public void RefreshAllUI()
    {
        inventoryUI.RefreshUI(playerInventory.items);
        hotbarUI.RefreshUI(playerInventory.hotbarItems);
        OnInventoryChanged?.Invoke();
    }
}
