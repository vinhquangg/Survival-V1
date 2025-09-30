using UnityEngine;
using UnityEngine.SceneManagement;

public class BoatInteractable : MonoBehaviour, IInteractable, IInteractableInfo
{
    [Header("Data")]
    public ToolClass boatData;
    //[SerializeField] private GameObject builtVFX;
    [SerializeField] private GameObject escapeCompleteText;
    private BuildableObject buildable;

    private void Awake()
    {
        buildable = GetComponent<BuildableObject>();
        if (buildable == null)
            Debug.LogError("BoatInteractable yêu cầu BuildableObject cùng GO!");

        // Ẩn text ban đầu (nếu gán sẵn từ Inspector)
        if (escapeCompleteText != null)
            escapeCompleteText.SetActive(false);
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

            if (buildable.IsBuilt)
            {
                // Hiển thị text Escape Complete
                if (escapeCompleteText != null)
                    escapeCompleteText.SetActive(true);
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
