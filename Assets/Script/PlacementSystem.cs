using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance;
    public Transform playerCamera;
    public float previewDistance = 3f;
    public LayerMask placementCheckMask;

    private GameObject previewObject;
    private ItemClass currentItem;
    private InventoryManager inventoryManager;

    // Material của item hiện tại
    private Material previewMat;
    private Material validMat;
    private Material invalidMat;
    private Material originalMat;

    private int currentHotkeyIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void StartPlacement(ItemClass item, int hotkeyIndex)
    {
        if (item == null) return;

        currentItem = item;
        currentHotkeyIndex = hotkeyIndex;

        // Lấy prefab và material từ SurvivalClass
        GameObject prefabToUse = null;
        if (item is SurvivalClass survival && survival.prefabToPlace != null)
        {
            prefabToUse = survival.prefabToPlace;
            previewMat = survival.previewMaterial;
            validMat = survival.validMaterial;
            invalidMat = survival.invalidMaterial;
            originalMat = survival.originalMaterial;
        }
        else
        {
            prefabToUse = item.dropPrefab;
            previewMat = validMat = invalidMat = originalMat = null;
        }

        if (prefabToUse == null)
        {
            Debug.LogWarning($"Item {item.itemName} không có prefab để đặt");
            return;
        }

        if (previewObject != null)
            Destroy(previewObject);

        previewObject = Instantiate(prefabToUse);
        SetPreviewMaterial(previewMat);
    }

    private void Update()
    {
        if (previewObject == null) return;

        // Cập nhật vị trí preview
        Vector3 targetPos = playerCamera.position + playerCamera.forward * previewDistance;
        previewObject.transform.position = targetPos;
        previewObject.transform.rotation = Quaternion.LookRotation(playerCamera.forward);

        // Check hợp lệ
        bool canPlace = CanPlaceHere();
        bool hasAllItems = currentItem is SurvivalClass s && HasAllRequiredItems(s);

        // Đổi màu theo trạng thái
        if (!canPlace)
            SetPreviewMaterial(invalidMat);
        else if (hasAllItems)
            SetPreviewMaterial(originalMat);
        else
            SetPreviewMaterial(validMat);

        // Chuột trái → đặt
        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceObject();

        // Chuột phải → hủy
        else if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private bool CanPlaceHere()
    {
        Collider col = previewObject.GetComponent<Collider>();
        if (col == null) return true;

        Collider[] hits = Physics.OverlapBox(
            col.bounds.center,
            col.bounds.extents,
            previewObject.transform.rotation,
            placementCheckMask
        );

        foreach (var h in hits)
        {
            if (h.gameObject != previewObject)
                return false;
        }
        return true;
    }

    private bool HasAllRequiredItems(SurvivalClass itemData)
    {
        foreach (var req in itemData.requiredItems)
        {
            if (inventoryManager.playerInventory.GetTotalQuantity(req.item) < req.amount)
                return false;
        }
        return true;
    }

    private void PlaceObject()
    {
        if (currentItem == null || previewObject == null) return;

        if (currentItem is SurvivalClass survival)
        {
            // Xóa blueprint cũ cùng hotkey nếu chưa build
            var existingBlueprints = FindObjectsOfType<BuildableObject>();
            foreach (var bp in existingBlueprints)
            {
                if (bp.IsSameBlueprint(survival) &&
                    bp.LastHotkeyIndex == currentHotkeyIndex &&
                    !bp.IsBuilt)
                {
                    Destroy(bp.gameObject);
                    Debug.Log($"Xóa blueprint {survival.itemName} của hotkey {currentHotkeyIndex + 1}");
                }
            }
        }

        // Spawn object thật
        GameObject prefabToPlace = (currentItem is SurvivalClass s && s.prefabToPlace != null)
            ? s.prefabToPlace
            : currentItem.dropPrefab;

        GameObject placedObj = Instantiate(prefabToPlace, previewObject.transform.position, previewObject.transform.rotation);

        // Nếu là SurvivalClass → set màu preview ban đầu
        if (currentItem is SurvivalClass sData && sData.previewMaterial != null)
        {
            foreach (var r in placedObj.GetComponentsInChildren<Renderer>())
                r.material = sData.previewMaterial;

            BuildableObject buildScript = placedObj.GetComponent<BuildableObject>();
            if (buildScript != null)
            {
                buildScript.Init(sData, inventoryManager.playerInventory);
                buildScript.LastHotkeyIndex = currentHotkeyIndex;
            }
        }

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        currentItem = null;
        currentHotkeyIndex = -1;
    }

    private void SetPreviewMaterial(Material mat)
    {
        if (mat == null) return;
        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
                mats[i] = mat;
            r.materials = mats;
        }
    }
}
