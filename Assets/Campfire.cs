using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private SurvivalClass campFire;
    private float duration;
    [Header("Data & UI")]
    [SerializeField] private SurvivalClass campfireData;
    [SerializeField] private Sprite icon;

    [Header("Effects & CookPoint")]
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private Transform cookPoint; // vị trí đặt meat
    [SerializeField] private int maxCookSlots = 3;

    private bool isBurning = false;
    public bool IsBurning => isBurning;
    public Transform CookPoint => cookPoint;
    public int MaxCookSlots => maxCookSlots;

    private void Update()
    {
        throw new System.NotImplementedException();
        if (isBurning && campfireData.duration > 0)
        {
            campfireData.duration -= Time.deltaTime;
            if (campfireData.duration <= 0f) StopFire();
        }
    }

    public string GetItemAmount()
    {
        throw new System.NotImplementedException();
    }
    // IInteractableInfo
    public Sprite GetIcon() => icon;
    public string GetName() => "Campfire";
    public InteractionType GetInteractionType() => InteractionType.Use;

    // IInteractable
    public void Interact(GameObject interactor)
    {
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
    }
}