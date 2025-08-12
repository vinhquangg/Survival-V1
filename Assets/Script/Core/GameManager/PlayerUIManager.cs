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
    /// <summary>
    /// Hiển thị prompt với thông tin từ object
    /// </summary>
    public void ShowPrompt(IInteractableInfo info)
    {
        if (interactUI != null)
            interactUI.SetActive(true);

        nameText.text = info.GetName();

        if (info.GetInteractionType() == InteractionType.Pickup)
        {
            itemAmount.gameObject.SetActive(true);
            iconImage.gameObject.SetActive(true);

            itemAmount.text = info.GetItemAmount();
            iconImage.sprite = info.GetIcon();
        }
        else
        {
            itemAmount.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Ẩn prompt
    /// </summary>
    public void HidePrompt()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="buildable"></param>
    public void ShowCraftingInfo(BlueprintData blueprint, BuildableObject buildable)
    {
        if (craftingUI != null)
            craftingUI.SetActive(true);

        if (blueprint != null)
            craftingNameText.text = blueprint.name;

        if (buildable != null)
            craftingItemAmount.text = buildable.GetItemAmount().ToString();

        if (blueprint != null && blueprint.resultItem.itemIcon != null)
            craftingIconImage.sprite = blueprint.resultItem.itemIcon;
    }
    /// <summary>
    /// 
    /// </summary>
    public void HideCraftingInfo()
    {
        if (craftingUI != null)
            craftingUI.SetActive(false);
    }


}
