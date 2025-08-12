using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Transform cursorTransform;
    public LayerMask interactableLayer;

    private IInteractable currentInteractable;
    public IInteractable CurrentInteractable => currentInteractable;

    private PlayerUIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<PlayerUIManager>();
        if (uiManager == null)
            Debug.LogError("Không tìm thấy PlayerUIManager trong scene!");
    }

    void Update()
    {
        Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
        RaycastHit hit;

        // Thêm bán kính "tương tác" → giúp dễ trúng object nhỏ
        float sphereRadius = 0.3f;

        if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactableLayer))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            var info = hit.transform.GetComponent<IInteractableInfo>();

            if (interactable != null && info != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    uiManager.ShowPrompt(info);
                }
                return;
            }
            if (info != null && info.GetInteractionType() == InteractionType.Placeable)
            {
                var buildable = hit.transform.GetComponent<BuildableObject>();
                if (buildable != null)
                {
                    // Gọi hiển thị crafting UI qua PlayerUIManager
                    uiManager.ShowCraftingInfo(buildable.GetBlueprint(), buildable);

                    // Nếu trước đó có prompt UI thì ẩn đi
                    currentInteractable = null;
                    uiManager.HidePrompt();
                    //uiManager.HideCraftingInfo();
                }
                return;
            }
        }

        //// Không trúng gì
        //currentInteractable = null;
        //uiManager.HidePrompt();



        currentInteractable = null;
        uiManager.HidePrompt();
        uiManager.HideCraftingInfo();
    }
}
