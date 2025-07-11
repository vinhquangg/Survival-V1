using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetItemName();
    string GetItemType();
    GameObject GetItemUI();
    void Interact(GameObject interactor);
    void ShowUI();         
    void HideUI();
}
