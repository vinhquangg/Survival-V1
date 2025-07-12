using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public static ItemInfo Instance;
    public GameObject itemInfoPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemFuncText;

    private float hideDelay = 0.15f;
    private float hideTimer = -1f;
    private bool isHoveringSlot = false;
    //private RectTransform panelRect;
    //private RectTransform canvasRect;

    void Start()
    {
        Instance = this;
        itemInfoPanel.SetActive(false);
    }

    void Update()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                HideInfo();
            }
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverInventorySlot())
        {
            HideInfo();
            hideTimer = -1f;
        }
    }

    public void SetHovering(bool hovering)
    {
        isHoveringSlot = hovering;
    }


    private bool IsPointerOverInventorySlot()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<InventorySlotUI>() != null)
                return true;
        }

        return false;
    }

    public void ShowInfo(ItemClass item, Vector2 Pos)
    {
        itemInfoPanel.SetActive(true);

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDesc;
        itemFuncText.text = item.itemFunc;
    }

    public void HideInfo()
    {
        itemInfoPanel.SetActive(false);
    }

    public void StartHideTimer()
    {
        hideTimer = hideDelay;
    }

    public void CancelHideTimer()
    {
        hideTimer = -1f;
    }


}
