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

        if (nameText != null)
            nameText.text = info.GetName();

        if (itemAmount != null)
            itemAmount.text = info.GetItemAmount();

        if (iconImage != null)
            iconImage.sprite = info.GetIcon();
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
