using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private SurvivalClass campFire;
    private float duration;
    private bool isBurning = false;

    public Sprite GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public InteractionType GetInteractionType()
    {
        throw new System.NotImplementedException();
    }

    public string GetItemAmount()
    {
        throw new System.NotImplementedException();
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public void Init(SurvivalClass survival)
    {
        campFire = survival;
        duration = campFire.duration;
    }

    public void Interact(GameObject interactor)
    {
        
    }
}
