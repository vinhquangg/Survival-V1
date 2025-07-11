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

    //private RectTransform panelRect;
    //private RectTransform canvasRect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            // Kiểm tra xem chuột có đang nằm trên slot không
            if (!IsPointerOverInventorySlot())
            {
                HideInfo();
            }
        }
    }

    private bool IsPointerOverInventorySlot()
    {
        var pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<InventorySlotUI>() != null)
            {
                return true;
            }
        }

        return false;
    }


    void Start()
    {
        Instance = this;
        //panelRect = itemInfoPanel.GetComponent<RectTransform>();
        //canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        itemInfoPanel.SetActive(false);
    }

    public void ShowInfo(ItemClass item, Vector2 Pos)
    {
        itemInfoPanel.SetActive(true);

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDesc;
        itemFuncText.text = item.itemFunc;

        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Pos, null, out localPoint);
        //panelRect.anchoredPosition = localPoint;
    }

    public void HideInfo()
    {
        itemInfoPanel.SetActive(false);
    }


}
