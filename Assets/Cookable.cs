using UnityEngine;

public class Cookable : MonoBehaviour, IInteractable, IInteractableInfo
{
    public string cookName = "Cooking Station";
    public Sprite cookIcon;

    public InteractionType GetInteractionType() => InteractionType.Cook;
    public string GetName() => cookName;
    public string GetItemAmount() => "";  // có thể return "Raw Meat x1" nếu đang nấu
    public Sprite GetIcon() => cookIcon;

    public void Interact(GameObject player)
    {
        Debug.Log("Start cooking...");
        // TODO: logic nấu
    }
}
