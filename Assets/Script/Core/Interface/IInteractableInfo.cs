using UnityEngine;

public interface IInteractableInfo
{
    string GetName();            
    string GetItemAmount();     
    Sprite GetIcon();            
    InteractionType GetInteractionType(); 
}
