using UnityEngine;

public class BoatInteractable : MonoBehaviour, IInteractable, IInteractableInfo
{
    [Header("Data")]
    public ToolClass boatData;

    [Header("Effects")]
    [SerializeField] private GameObject builtVFX;

    private BuildableObject buildable;

    private void Awake()
    {
        buildable = GetComponent<BuildableObject>();
        if (buildable == null)
            Debug.LogError("BoatInteractable yêu cầu BuildableObject cùng GO!");
    }

    public bool IsBuilt => buildable != null && buildable.IsBuilt;
    public ToolClass BoatData => boatData;

    public Sprite GetIcon() => boatData != null ? boatData.itemIcon : null;
    public string GetName() => boatData != null ? boatData.itemName : "Boat";
    public string GetItemAmount() => buildable != null ? buildable.GetItemAmount() : "";

    public InteractionType GetInteractionType() => InteractionType.Boat;

    public void Interact(GameObject interactor)
    {
        if (buildable != null)
        {
            buildable.AddMaterialByPlayerInput(); // fill vật liệu
        }
    }
}
