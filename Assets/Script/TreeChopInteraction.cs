using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private string treeName;
    public Sprite GetIcon()
    {
        return null;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Chop;
    }

    public string GetItemAmount()
    {
        return "";
    }

    public string GetName()
    {
        return "Chop " + treeName;
    }

    public void Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine,player,this));
        }
    }


    public void OnChopped()
    {
        Debug.Log("🌳 Cây bị chặt rồi!");
        // Gợi ý: Animator cây, đổ gỗ, spawn item, play sound...
    }
}
