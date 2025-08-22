using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [Header("Data & UI")]
    [SerializeField] private SurvivalClass campfireData;
    [SerializeField] private Sprite icon;

    [Header("Effects & CookPoint")]
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private Transform cookPoint; // vị trí đặt meat
    //[SerializeField] private int maxCookSlots = 3;

    private bool isBurning = false;

    public bool IsBurning => isBurning;
    public Transform CookPoint => cookPoint;
    //public int MaxCookSlots => maxCookSlots;

    private void Update()
    {
        if (isBurning && campfireData.duration > 0)
        {
            campfireData.duration -= Time.deltaTime;
            if (campfireData.duration <= 0f) StopFire();
        }
    }

    // IInteractableInfo
    public Sprite GetIcon() => icon;
    public string GetName() => "Campfire";
    public string GetItemAmount() => "";
    public InteractionType GetInteractionType() => InteractionType.Use;

    // IInteractable
    public void Interact(GameObject interactor)
    {
        // Chỉ bật/tắt lửa, không gọi Cook
        if (!isBurning)
            StartFire();
        else
            StopFire();
    }


    public void StartFire()
    {
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
