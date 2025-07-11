using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler,IPointerEnterHandler
{
    public InventoryManager inventoryManager;
    public InventoryArea inventoryArea; 
    private Canvas canvas;
    private GameObject dragIcon;

    public int slotIndex { get; set; }
    private Image iconImage;
    private TextMeshProUGUI amountText;
    private Slider durabilitySlider;

    void Start()
    {
        if (inventoryManager == null)
            inventoryManager = GetComponentInParent<InventoryManager>();
        canvas = GetComponentInParent<Canvas>();
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        durabilitySlider = transform.GetChild(2).GetComponent<Slider>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = inventoryManager.GetSlot(inventoryArea, slotIndex);
        if (slot == null || slot.IsEmpty()) return;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform);
        dragIcon.transform.SetAsLastSibling();

        var image = dragIcon.AddComponent<Image>();
        image.sprite = slot.GetItem().itemIcon;
        image.raycastTarget = false;

        var rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);
        rt.pivot = new Vector2(0.5f, 0.5f);

        if (iconImage != null) iconImage.enabled = false;
        if (amountText != null) amountText.gameObject.SetActive(false);
        if (durabilitySlider != null) durabilitySlider.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);


        bool isOutUI = eventData.pointerEnter == null || eventData.pointerEnter.GetComponentInParent<InventoryUIHandler>()==null;

        if (isOutUI && eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryManager.DropItemToWorld(inventoryArea, slotIndex);
        }
        else
        {
            inventoryManager.RefreshAllUI();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var sourceSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (sourceSlot != null && sourceSlot != this)
        {
            inventoryManager.TryMergeOrSwap(
                sourceSlot.inventoryArea, sourceSlot.slotIndex,
                inventoryArea, slotIndex
            );
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryManager.SplitItem(inventoryArea, slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var slot = inventoryManager.GetSlot(inventoryArea, slotIndex);
        if (slot != null && !slot.IsEmpty() && eventData.pointerEnter == gameObject)
        {
            ItemClass item = slot.GetItem();
            ItemInfo.Instance.ShowInfo(slot.GetItem(), eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemInfo.Instance.HideInfo();
    }

}
