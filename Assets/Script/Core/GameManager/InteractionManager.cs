using System.Collections;
using System.Collections.Generic;
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
        if(SelectionManager == null)
            return;
        if (SelectionManager != null && inputHandler.IsInteractPressed())
        {
            SelectionManager.CurrentInteractable?.Interact(transform.root.gameObject);
        }
    }
}
