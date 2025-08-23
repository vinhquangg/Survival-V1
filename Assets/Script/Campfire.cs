using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [Header("Data & UI")]
    [SerializeField] private SurvivalClass campfireData;
    [SerializeField] private Sprite icon;
    [SerializeField] private PlayerUIManager uiManager; // 🔹 UI manager

    [Header("Effects & CookPoint")]
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private Transform cookPoint;

    private bool isBurning = false;
    private bool isCooking = false;
    private string currentCookingName = "";
    public bool IsBurning => isBurning;
    public Transform CookPoint => cookPoint;

    private void Update()
    {
        if (isBurning && campfireData.duration > 0)
        {
            campfireData.duration -= Time.deltaTime;
            if (campfireData.duration <= 0f) StopFire();
        }
    }

    public Sprite GetIcon() => icon;
    public string GetName() => "Campfire";
    public string GetItemAmount() => "";

    public InteractionType GetInteractionType()
    {
        return isBurning ? InteractionType.Cook : InteractionType.Toggle;
    }

    public void Interact(GameObject interactor)
    {
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

    // 🔹 Gọi khi Cookable bắt đầu nấu
    public void StartCooking(string itemName)
    {
        isCooking = true;
        currentCookingName = itemName;
        uiManager?.ShowCookingUI(itemName);
    }

    // 🔹 Gọi khi Cookable nấu xong
    public void FinishCooking()
    {
        isCooking = false;
        uiManager?.ShowCookedUI(currentCookingName);
        currentCookingName = "";
    }
}
