//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CraftingManager : MonoBehaviour
//{
//    public GameObject craftingScreen;
//    public GameObject toolsScreen,survivalScreen;
//    [SerializeField] private BlueprintData blueprint;
//    public List<string> inventoryItemList = new List<string>();

//    //Category Buttons
//    public Button toolsBTN,survivalBTN;

//    //Crafting Buttons
//    public Button craftingBTN;

//    //Required Items
//    TextMeshProUGUI requiredItemText1;
//    TextMeshProUGUI requiredItemText2;
//    PlayerController PlayerController;
//    bool isOpen;

//    public static CraftingManager instance { get; set; }
//    private void Awake()
//    {
//        if(instance != null && instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        else
//        {
//            instance = this;
//        }
//    }
//    void Start()
//    {
//        isOpen = false;
//        PlayerController = FindObjectOfType<PlayerController>();
//        toolsBTN = craftingScreen.transform.Find("ToolsButton").GetComponent<Button>();
//        survivalBTN = craftingScreen.transform.Find("SurvivalButton").GetComponent<Button>();
//        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });
//        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });
//        // Set initial state of screens
//        InventoryManager.Instance.OnInventoryChanged += CheckCanCraft;
//        //Sword
//        requiredItemText1 = toolsScreen.transform.Find("Machete").transform.Find("RequireItem1").GetComponent<TextMeshProUGUI>();
//        requiredItemText2 = toolsScreen.transform.Find("Machete").transform.Find("RequireItem2").GetComponent<TextMeshProUGUI>();

//        craftingBTN = toolsScreen.transform.Find("Machete").transform.Find("Button").GetComponent<Button>();
//        // Survival Category
//        requiredItemText1 = toolsScreen.transform.Find("Campfire").transform.Find("RequireItem1").GetComponent<TextMeshProUGUI>();
//        requiredItemText2 = toolsScreen.transform.Find("Campfire").transform.Find("RequireItem2").GetComponent<TextMeshProUGUI>();

//        craftingBTN = toolsScreen.transform.Find("Campfire").transform.Find("Button").GetComponent<Button>();
//        // Add listener to the crafting button
//        craftingBTN.onClick.AddListener(delegate { CraftItem(); });

//    }

//    private void CraftItem()
//    {
//        var inventory = InventoryManager.Instance.playerInventory;

//        // check if player has enough items to craft
//        foreach (var req in blueprint.requirements)
//        {
//            if (!inventory.HasItem(req.item, req.amount))
//            {
//                Debug.Log("Không đủ nguyên liệu để chế tạo!");
//                return;
//            }
//        }

//        // subtract items from inventory
//        foreach (var req in blueprint.requirements)
//            inventory.RemoveItem(req.item, req.amount);

//        // Add item craft to inventory
//        InventoryManager.Instance.AddItem(blueprint.resultItem, blueprint.resultAmount);
//        InventoryManager.Instance.RefreshAllUI();

//        Debug.Log($"Đã chế: {blueprint.resultItem.itemName}");
//    }


//    void OpenToolsCategory()
//    {
//        craftingScreen.SetActive(false);
//        toolsScreen.SetActive(true);
//    }

//    void OpenSurvivalCategory()
//    {
//        craftingScreen.SetActive(false);
//        survivalScreen.SetActive(true);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.C))
//        {
//            ToggleCrafting();
//        }
//    }

//    private void ToggleCrafting()
//    {
//        isOpen = !isOpen;
//        if (isOpen)
//        {
//            CheckCanCraft(); 
//        }
//        craftingScreen.SetActive(isOpen);
//        toolsScreen.SetActive(false);
//        survivalScreen.SetActive(false);
//        PlayerController.inputHandler.DisablePlayerInput();
//        if (!isOpen)
//            PlayerController.inputHandler.EnablePlayerInput();

//        GameManager.instance?.SetCursorLock(!isOpen);
//        if (CameraTarget.Instance != null)
//            CameraTarget.Instance.allowCameraInput = !isOpen;

//    }

//    private void CheckCanCraft()
//    {
//        var inventory = InventoryManager.Instance.playerInventory;
//        bool canCraft = true;

//        // Update required items text
//        if (blueprint.requirements.Count > 0)
//        {
//            int have1 = inventory.GetTotalQuantity(blueprint.requirements[0].item);
//            int need1 = blueprint.requirements[0].amount;
//            requiredItemText1.text = $"{blueprint.requirements[0].item.itemName}: {have1} / {need1}";
//            if (have1 < need1) canCraft = false;
//        }

//        if (blueprint.requirements.Count > 1)
//        {
//            int have2 = inventory.GetTotalQuantity(blueprint.requirements[1].item);
//            int need2 = blueprint.requirements[1].amount;
//            requiredItemText2.text = $"{blueprint.requirements[1].item.itemName}: {have2} / {need2}";
//            if (have2 < need2) canCraft = false;
//        }

//        craftingBTN.gameObject.SetActive(canCraft);
//    }


//    private void OnDisable()
//    {
//        if (InventoryManager.Instance != null)
//            InventoryManager.Instance.OnInventoryChanged -= CheckCanCraft;
//    }

//}


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CraftingSlot
{
    public BlueprintData blueprint;
    public Button craftButton;
    public TextMeshProUGUI requiredItemText1;
    public TextMeshProUGUI requiredItemText2;
}

public class CraftingManager : MonoBehaviour
{
    public GameObject craftingScreen;
    public GameObject toolsScreen, survivalScreen;
    [SerializeField] private List<CraftingSlot> craftingSlots = new List<CraftingSlot>();

    public Button toolsBTN, survivalBTN;

    PlayerController PlayerController;
    bool isOpen;

    public static CraftingManager instance { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        isOpen = false;
        PlayerController = FindObjectOfType<PlayerController>();

        toolsBTN = craftingScreen.transform.Find("ToolsButton").GetComponent<Button>();
        survivalBTN = craftingScreen.transform.Find("SurvivalButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        InventoryManager.Instance.OnInventoryChanged += CheckCanCraft;

        foreach (var slot in craftingSlots)
        {
            slot.craftButton.onClick.AddListener(() => CraftItem(slot.blueprint));
        }
    }

    private void CraftItem(BlueprintData blueprint)
    {
    var inventory = InventoryManager.Instance.playerInventory;

    foreach (var req in blueprint.requirements)
    {
        if (!inventory.HasItem(req.item, req.amount))
        {
            Debug.Log("Không đủ nguyên liệu để chế tạo!");
            return;
        }
    }

    if (blueprint.craftingType == CraftingType.Immediate)
    {
        // Craft ngay lập tức
        foreach (var req in blueprint.requirements)
            inventory.RemoveItem(req.item, req.amount);

        InventoryManager.Instance.AddItem(blueprint.resultItem, blueprint.resultAmount);
        InventoryManager.Instance.RefreshAllUI();

        Debug.Log($"Đã chế: {blueprint.resultItem.itemName}");
    }
    else if (blueprint.craftingType == CraftingType.NeedResource)
    {
        // Chỉ trừ nguyên liệu khi đặt hoàn tất
        // Gọi hệ thống đặt vật thể (ghost preview)
        PlacementSystem.Instance.StartPlacement(blueprint,-1);
    }

        CheckCanCraft();
    }


    void OpenToolsCategory()
    {
        craftingScreen.SetActive(false);
        toolsScreen.SetActive(true);
        survivalScreen.SetActive(false);
    }

    void OpenSurvivalCategory()
    {
        craftingScreen.SetActive(false);
        survivalScreen.SetActive(true);
        toolsScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrafting();
        }
    }

    private void ToggleCrafting()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            CheckCanCraft();
        }

        craftingScreen.SetActive(isOpen);
        toolsScreen.SetActive(false);
        survivalScreen.SetActive(false);
        PlayerController.inputHandler.DisablePlayerInput();

        if (!isOpen)
            PlayerController.inputHandler.EnablePlayerInput();

        GameManager.instance?.SetCursorLock(!isOpen);

        if (CameraTarget.Instance != null)
            CameraTarget.Instance.allowCameraInput = !isOpen;
    }

    private void CheckCanCraft()
    {
        var inventory = InventoryManager.Instance.playerInventory;

        foreach (var slot in craftingSlots)
        {
            bool canCraft = true;
            var blueprint = slot.blueprint;

            if (blueprint.requirements.Count > 0)
            {
                int have1 = inventory.GetTotalQuantity(blueprint.requirements[0].item);
                int need1 = blueprint.requirements[0].amount;
                slot.requiredItemText1.text = $"{blueprint.requirements[0].item.itemName}: {have1} / {need1}";
                slot.requiredItemText1.gameObject.SetActive(true); // 🔹 bật hiển thị
                if (have1 < need1) canCraft = false;
            }
            else
            {
                slot.requiredItemText1.gameObject.SetActive(false); // 🔹 ẩn nếu không có requirement
            }

            if (blueprint.requirements.Count > 1)
            {
                int have2 = inventory.GetTotalQuantity(blueprint.requirements[1].item);
                int need2 = blueprint.requirements[1].amount;
                slot.requiredItemText2.text = $"{blueprint.requirements[1].item.itemName}: {have2} / {need2}";
                slot.requiredItemText2.gameObject.SetActive(true); // 🔹 bật hiển thị
                if (have2 < need2) canCraft = false;
            }
            else
            {
                slot.requiredItemText2.gameObject.SetActive(false); // 🔹 ẩn nếu không có requirement thứ 2
            }


            slot.craftButton.gameObject.SetActive(canCraft);
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= CheckCanCraft;
    }
}

