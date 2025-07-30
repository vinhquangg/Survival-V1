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

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
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
        }

        currentInteractable = null;
        uiManager.HidePrompt();
    }
}
