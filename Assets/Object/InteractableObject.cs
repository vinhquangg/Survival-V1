using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string itemName;
    public ObjectType objectType;
    public string GetItemName()
    {
        return itemName;
    }
    public string GetItemType()
    {
        return objectType.ToString();
    }
}
