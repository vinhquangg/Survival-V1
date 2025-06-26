using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Transform cursorTransform;
    public LayerMask interactableLayer;

    private IInteractable currentInteractable;
    public IInteractable CurrentInteractable => currentInteractable;

    void Update()
    {
        Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable?.HideUI();    
                    interactable.ShowUI();            
                    currentInteractable = interactable;
                }

                return;
            }
        }

        currentInteractable?.HideUI();
        currentInteractable = null;
    }
}
