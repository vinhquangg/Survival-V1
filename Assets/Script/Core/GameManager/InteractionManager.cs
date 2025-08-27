using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private SelectionManager SelectionManager;
    private InputHandler inputHandler;

    void Start()
    {
        GameObject selectionOBJ = GameObject.FindGameObjectWithTag("SelectionManager");
        if (selectionOBJ != null)
            SelectionManager = selectionOBJ.GetComponent<SelectionManager>();
        else
            Debug.LogError("SelectionManager not found in the scene. Please ensure it is tagged correctly.");

        inputHandler = FindObjectOfType<InputHandler>();
        if (inputHandler == null)
            Debug.LogError("InputHandler not found in scene!");
    }

    void Update()
    {
        if (SelectionManager == null)
            return;

        if (SelectionManager.CurrentInteractable != null && inputHandler.IsInteractPressed())
        {
            // Không cần ép kiểu BuildableObject mà gọi thẳng Interact()
            // vì BuildableObject đã implement Interact()
            SelectionManager.CurrentInteractable.Interact(transform.root.gameObject);

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.pickupItemSound);
        }
    }
}
