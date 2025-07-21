using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Prompt UI")]
    public GameObject promptUI;                 
    public TextMeshProUGUI nameText;            
    public TextMeshProUGUI itemAmount;   
    public Image iconImage;

    /// <summary>
    /// Hiển thị prompt với thông tin từ object
    /// </summary>
    public void ShowPrompt(IInteractableInfo info)
    {
        if (promptUI != null)
            promptUI.SetActive(true);

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
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}
