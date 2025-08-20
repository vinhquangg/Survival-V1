<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections;
=======
using System.Collections;
>>>>>>> parent of 1f79ee6 (make cooked meat)
=======
using System.Collections;
>>>>>>> parent of 88062a8 (new)
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private SurvivalClass campFire;
    private float duration;
<<<<<<< HEAD
<<<<<<< HEAD
    [Header("Data & UI")]
    [SerializeField] private SurvivalClass campfireData;
    [SerializeField] private Sprite icon;

    [Header("Effects & CookPoint")]
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private Transform cookPoint; // vị trí đặt meat
    [SerializeField] private int maxCookSlots = 3;

=======
>>>>>>> parent of 88062a8 (new)
    private bool isBurning = false;

    public Sprite GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public InteractionType GetInteractionType()
    {
        throw new System.NotImplementedException();
    }

=======
    private bool isBurning = false;

    public Sprite GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public InteractionType GetInteractionType()
    {
        throw new System.NotImplementedException();
    }

>>>>>>> parent of 1f79ee6 (make cooked meat)
    public string GetItemAmount()
    {
        throw new System.NotImplementedException();
    }
<<<<<<< HEAD
<<<<<<< HEAD
    // IInteractableInfo
    public Sprite GetIcon() => icon;
    public string GetName() => "Campfire";
    public InteractionType GetInteractionType() => InteractionType.Use;
=======

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public void Init(SurvivalClass survival)
    {
        campFire = survival;
        duration = campFire.duration;
    }
>>>>>>> parent of 1f79ee6 (make cooked meat)

    public void Interact(GameObject interactor)
    {
<<<<<<< HEAD
        if (!isBurning)
        {
            StartFire();
            return;
        }

        // Nếu đang cháy, tìm Cookable và gọi Cook
        Cookable cookable = GetComponent<Cookable>();
        if (cookable != null)
        {
            cookable.Cook(interactor);
        }
    }

    public void StartFire()
    {
        duration = campFire.duration;
        isBurning = true;
        if (fireVFX != null) fireVFX.SetActive(true);
        Debug.Log("🔥 Campfire is burning!");
    }

    public void StopFire()
    {

        isBurning = false;
        if (fireVFX != null) fireVFX.SetActive(false);
        Debug.Log("❌ Campfire stopped!");
=======
        
>>>>>>> parent of 1f79ee6 (make cooked meat)
    }
}
=======

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
>>>>>>> parent of 88062a8 (new)
