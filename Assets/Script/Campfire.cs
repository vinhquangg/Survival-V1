using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable, IInteractableInfo
{
    [Header("Data & UI")]
    [HideInInspector] public SurvivalClass campfireData;
    [SerializeField] private PlayerUIManager uiManager;

    [Header("Effects & CookPoint")]
    [SerializeField] private GameObject fireVFX;
    [SerializeField] private Transform cookPoint;

    private bool isBurning = false;
    private bool isCooking = false;

    //Item đang nấu
    private string currentCookingName = "";
    private int currentCookingQty = 0;
    private Sprite currentCookingIcon;

    //Item đã nấu xong (giữ lại cho player nhìn/nhặt)
    private string cookedItemName = "";
    private int cookedItemQty = 0;
    private Sprite cookedItemIcon;
    private SphereCollider triggerCollider;
    public bool IsBurning => isBurning;
    public bool IsCooking => isCooking;
    public string CurrentCookingName => currentCookingName;
    public Sprite CurrentCookingIcon => currentCookingIcon;
    public int CurrentCookingQty => currentCookingQty;
    public Transform CookPoint => cookPoint;

    public string CookedItemName => cookedItemName;
    public int CookedItemQty => cookedItemQty;
    public Sprite CookedItemIcon => cookedItemIcon;

    private void Start()
    {
        uiManager = FindAnyObjectByType<PlayerUIManager>();

        triggerCollider = GetComponentInChildren<SphereCollider>();
        if (triggerCollider == null || !triggerCollider.isTrigger)
        {
            Debug.LogWarning("⚠ Campfire cần SphereCollider dạng Trigger để hoạt động đúng!");
        }
    }

    private void Update()
    {
        if (isBurning && campfireData.duration > 0)
        {
            campfireData.duration -= Time.deltaTime;
            if (campfireData.duration <= 0f) StopFire();
        }
    }

    public Sprite GetIcon() => campfireData.itemIcon;
    public string GetName() => campfireData.itemName;
    public string GetItemAmount() => "";

    public InteractionType GetInteractionType() => InteractionType.Toggle;

    public void Interact(GameObject interactor)
    {
        if (!isBurning) StartFire();
        else StopFire();
    }

    public void StartFire()
    {
        isBurning = true;
        if (fireVFX != null) fireVFX.SetActive(true);

        if (triggerCollider != null)
        {
            Vector3 worldCenter = triggerCollider.transform.TransformPoint(triggerCollider.center);
            Collider[] colliders = Physics.OverlapSphere(worldCenter, triggerCollider.radius);

            foreach (var col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    TemperatureManager.Instance.SetNearFire(true);
                }
            }
        }

        Debug.Log("🔥 Campfire is burning!");
    }

    public void StopFire()
    {
        isBurning = false;
        if (fireVFX != null) fireVFX.SetActive(false);
        TemperatureManager.Instance.SetNearFire(false);
        Debug.Log("❌ Campfire stopped!");
    }

    //Bắt đầu nấu
    public void StartCooking(string itemName, Sprite icon, int qty)
    {
        isCooking = true;
        currentCookingName = itemName;
        currentCookingIcon = icon;
        currentCookingQty = qty;

        uiManager?.ShowCookingUI(itemName, icon, qty);
    }

    //Nấu xong
    public void FinishCooking()
    {
        isCooking = false;

        // Lưu item đã nấu xong
        cookedItemName = currentCookingName;
        cookedItemIcon = currentCookingIcon;
        cookedItemQty = currentCookingQty;

        // Báo cho UI
        uiManager?.ShowCookedUI(cookedItemName, cookedItemIcon, cookedItemQty);


        // Reset slot cooking
        currentCookingName = "";
        currentCookingIcon = null;
        currentCookingQty = 0;
    }

    //Khi player nhặt món ăn xong (nếu bạn muốn clear)
    public void CollectCookedItem()
    {
        cookedItemName = "";
        cookedItemIcon = null;
        cookedItemQty = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBurning) return;

        if (other.CompareTag("Player"))
        {
            TemperatureManager.Instance.SetNearFire(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TemperatureManager.Instance.SetNearFire(false);
        }
    }
}
