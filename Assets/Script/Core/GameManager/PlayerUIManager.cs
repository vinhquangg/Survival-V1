using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Interact UI")]
    public GameObject interactUI;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI itemAmount;
    public Image iconImage;

    [Header("Crafting UI")]
    public GameObject craftingUI;
    public TextMeshProUGUI craftingNameText;
    public TextMeshProUGUI craftingItemAmount;
    public Image craftingIconImage;

    [Header("Cooked UI")]
    public GameObject cookedUI;
    public TextMeshProUGUI cookedNameText;
    public TextMeshProUGUI cookedItemAmount;
    public Image cookedIconImage;


    private Coroutine cookedUICoroutine;
    public void ShowPrompt(IInteractableInfo info)
    {
        if (interactUI == null || info == null)
            return;

        interactUI.SetActive(true);

        // Reset mặc định
        nameText.text = info.GetName();
        itemAmount.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);

        switch (info.GetInteractionType())
        {
            case InteractionType.Pickup:
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);

                itemAmount.text = info.GetItemAmount();
                iconImage.sprite = info.GetIcon();
                break;

            case InteractionType.Chop:
                break;

            case InteractionType.Cook:
                cookedItemAmount.gameObject.SetActive(true);
                cookedIconImage.gameObject.SetActive(true);
                cookedItemAmount.text = info.GetItemAmount();
                cookedIconImage.sprite = info.GetIcon();
                break;

            case InteractionType.TakeCooked:
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);
                itemAmount.text = "Cooked Meat Ready";
                iconImage.sprite = info.GetIcon();
                break;

            case InteractionType.Toggle:
                nameText.text = info.GetName(); // Campfire
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);
                itemAmount.text = (info as Campfire).IsBurning ? "Off" : "On"; // hiển thị text
                iconImage.sprite = info.GetIcon();
                break;


            default:
                HidePrompt();
                break;
        }
    }


    public void ShowCraftingInfo(BlueprintData blueprint, BuildableObject buildable)
    {
        if (craftingUI != null)
            craftingUI.SetActive(true);

        if (blueprint != null)
            craftingNameText.text = blueprint.name;

        if (buildable != null)
            craftingItemAmount.text = buildable.GetItemAmount();

        if (blueprint != null && blueprint.resultItem.itemIcon != null)
            craftingIconImage.sprite = blueprint.resultItem.itemIcon;
    }

    public void ShowCookingUI(string itemName, Sprite icon, int quantity)
    {
        if (cookedUI != null) cookedUI.SetActive(false);
        if (interactUI != null) interactUI.SetActive(true);

        nameText.text = $"Cooking";
        itemAmount.gameObject.SetActive(true);
        itemAmount.text = "x" + quantity;

        if (iconImage != null && icon != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = icon;
        }
    }


    //public void ShowCookedUI(string itemName)
    //{
    //    if (interactUI != null) interactUI.SetActive(false);
    //    if (cookedUI != null) cookedUI.SetActive(true);
    //    cookedNameText.text = $"{itemName} Ready!";
    //}

    public void ShowCookedUI(string itemName, Sprite icon, int quantity)
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (cookedUI != null) cookedUI.SetActive(true);

        // Tên món ăn
        cookedNameText.text = $"{itemName} Ready!";

        // Số lượng
        if (cookedItemAmount != null)
        {
            cookedItemAmount.gameObject.SetActive(true);
            cookedItemAmount.text = "x" + quantity;
        }

        // Icon đúng món đã nấu
        if (cookedIconImage != null && icon != null)
        {
            cookedIconImage.gameObject.SetActive(true);
            cookedIconImage.sprite = icon;
        }

        if (cookedUICoroutine != null) StopCoroutine(cookedUICoroutine);
        cookedUICoroutine = StartCoroutine(HideCookedUIDelay(3f));
    }

    private IEnumerator HideCookedUIDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideCookedUI();
    }

    public void HideCookedUI()
    {
        if (cookedUI != null)
            cookedUI.SetActive(false);
    }


    public void HidePrompt()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    public void HideCraftingInfo()
    {
        if (craftingUI != null)
            craftingUI.SetActive(false);
    }

}
