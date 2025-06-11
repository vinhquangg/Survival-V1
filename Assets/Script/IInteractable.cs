using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetItemName();
    string GetItemType();

    void Interact(GameObject interactor);
}
