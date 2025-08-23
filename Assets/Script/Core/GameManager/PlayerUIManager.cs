using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

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
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);
                itemAmount.text = info.GetItemAmount();
                iconImage.sprite = info.GetIcon();
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

    public void ShowCookingUI(string itemName)
    {
        if (cookedUI != null) cookedUI.SetActive(false);
        if (interactUI != null) interactUI.SetActive(true);
        nameText.text = $"Cooking: {itemName}";
    }

    public void ShowCookedUI(string itemName)
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (cookedUI != null) cookedUI.SetActive(true);
        cookedNameText.text = $"{itemName} Ready!";
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
