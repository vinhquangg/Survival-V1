using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private SelectionManager SelectionManager;

    void Start()
    {
        GameObject selectionOBJ = GameObject.FindGameObjectWithTag("SelectionManager");
        if (selectionOBJ != null)
            SelectionManager = selectionOBJ.GetComponent<SelectionManager>();
        else
            Debug.LogError("SelectionManager not found in the scene. Please ensure it is tagged correctly.");
    }

    void Update()
    {
        if(SelectionManager == null)
            return;
        if (SelectionManager != null && Input.GetKeyDown(KeyCode.E))
        {
            SelectionManager.CurrentInteractable?.Interact(transform.root.gameObject);
        }
    }
}
