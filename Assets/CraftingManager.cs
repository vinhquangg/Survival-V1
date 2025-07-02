using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public GameObject craftingScreen;
    public GameObject toolsScreen;

    public List<string> inventoryItemList = new List<string>();

    //Category Buttons
    public Button toolsBTN;

    //Crafting Buttons
    public Button craftingBTN;

    //Required Items
    TextMeshProUGUI requiredItemText1;
    TextMeshProUGUI requiredItemText2;
    PlayerController PlayerController;
    bool isOpen;

    public static CraftingManager instance { get; set; }
    private void Awake()
    {
        if(instance != null && instance != this)
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
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        //Sword
        requiredItemText1 = toolsScreen.transform.Find("Sword").transform.Find("RequireItem1").GetComponent<TextMeshProUGUI>();
        requiredItemText2 = toolsScreen.transform.Find("Sword").transform.Find("RequireItem2").GetComponent<TextMeshProUGUI>();

        craftingBTN = toolsScreen.transform.Find("Sword").transform.Find("Button").GetComponent<Button>();
        craftingBTN.onClick.AddListener(delegate { CraftItem(); });
    }

    private void CraftItem()
    {
        //Add Item Into Inventory

        //Remove Resource Item From Inventory

    }

    void OpenToolsCategory()
    {
        craftingScreen.SetActive(false);
        toolsScreen.SetActive(true);
    }

    // Update is called once per frame
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

        craftingScreen.SetActive(isOpen);
        toolsScreen .SetActive(false);
        PlayerController.enabled = !isOpen;
        GameManager.instance?.SetCursorLock(!isOpen);
        if (CameraTarget.Instance != null)
            CameraTarget.Instance.allowCameraInput = !isOpen;

    }
}
