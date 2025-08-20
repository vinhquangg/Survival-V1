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

    public void ShowPrompt(IInteractableInfo info)
    {
        if (info.GetInteractionType() == InteractionType.Pickup)
        {
            if (interactUI != null)
                interactUI.SetActive(true);

            nameText.text = info.GetName();
            itemAmount.gameObject.SetActive(true);
            iconImage.gameObject.SetActive(true);

            itemAmount.text = info.GetItemAmount();
            iconImage.sprite = info.GetIcon();
        }
        else if(info.GetInteractionType() ==  InteractionType.Chop)
        {
            if (interactUI != null)
                interactUI.SetActive(true);

<<<<<<< HEAD
            case InteractionType.Cook:
<<<<<<< HEAD
=======
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);
                itemAmount.text = info.GetItemAmount(); 
                iconImage.sprite = info.GetIcon();
                break;

            case InteractionType.TakeCooked:
>>>>>>> parent of 1f79ee6 (make cooked meat)
                itemAmount.gameObject.SetActive(true);
                iconImage.gameObject.SetActive(true);
                itemAmount.text = info.GetItemAmount(); 
                iconImage.sprite = info.GetIcon();
                break;
=======
            nameText.text = info.GetName();
            itemAmount.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(false);
>>>>>>> parent of 7862a86 (make cooked meat)

            itemAmount.text = info.GetItemAmount();
            iconImage.sprite = info.GetIcon();
        }
        else
        {
            HidePrompt();
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
