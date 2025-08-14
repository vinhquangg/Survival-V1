using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance;
    public Transform playerCamera;
    public float previewDistance = 3f;
    public LayerMask placementCheckMask;

    private GameObject previewObject;
    private BlueprintData currentBlueprint;
    private InventoryManager inventoryManager;
    private GameObject previewChunk;

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

    public void StartPlacement(BlueprintData blueprint, int hotkeyIndex)
    {
        if (blueprint == null) return;

        currentBlueprint = blueprint;
        currentHotkeyIndex = hotkeyIndex;

        // Lấy prefab và materials nếu là SurvivalClass
        GameObject prefabToUse = null;
        if (blueprint.resultItem is SurvivalClass survival && survival.prefabToPlace != null)
        {
            prefabToUse = survival.prefabToPlace;
            previewMat = survival.previewMaterial;
            validMat = survival.validMaterial;
            invalidMat = survival.invalidMaterial;
            originalMat = survival.originalMaterial;
        }
        else
        {
            prefabToUse = blueprint.resultItem.dropPrefab;
            previewMat = validMat = invalidMat = originalMat = null;
        }

        if (prefabToUse == null)
        {
            Debug.LogWarning($"Blueprint {blueprint.name} không có prefab để đặt");
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

        // Update vị trí preview
        Vector3 targetPos = playerCamera.position + playerCamera.forward * previewDistance;
        previewObject.transform.position = targetPos;
        previewObject.transform.rotation = Quaternion.LookRotation(playerCamera.forward);

        if (ChunkManager.Instance != null)
        {
            previewChunk = ChunkManager.Instance.GetOrCreateChunk(targetPos);
        }

        bool canPlace = CanPlaceHere();
        bool hasAllItems = HasAllRequiredItems();

        if (!canPlace)
        {
            SetPreviewMaterial(invalidMat);
        }
        else if (canPlace && hasAllItems)
        {
            SetPreviewMaterial(validMat);
        }
        else
        {
            SetPreviewMaterial(originalMat);
        }


        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceObject();
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

    private bool HasAllRequiredItems()
    {
        foreach (var req in currentBlueprint.requirements)
        {
            if (inventoryManager.playerInventory.GetTotalQuantity(req.item) < req.amount)
                return false;
        }
        return true;
    }

    private void PlaceObject()
    {
        if (currentBlueprint == null || previewObject == null) return;

        if (currentBlueprint.resultItem is SurvivalClass survival)
        {
            // Xóa blueprint cũ cùng hotkey nếu chưa build
            var existingBlueprints = FindObjectsOfType<BuildableObject>();
            foreach (var bp in existingBlueprints)
            {
                if (bp.IsSameBlueprint(currentBlueprint) &&
                    bp.LastHotkeyIndex == currentHotkeyIndex &&
                    !bp.IsBuilt &&
                    !bp.HasAnyMaterials())
                {
                    Destroy(bp.gameObject);
                    Debug.Log($"Xóa blueprint {currentBlueprint.name} của hotkey {currentHotkeyIndex + 1}");
                }
            }
        }

        GameObject prefabToPlace = (currentBlueprint.resultItem is SurvivalClass s && s.prefabToPlace != null)
            ? s.prefabToPlace
            : currentBlueprint.resultItem.dropPrefab;

        GameObject placedObj = Instantiate(prefabToPlace, previewObject.transform.position, previewObject.transform.rotation);

        BuildableObject buildScript = placedObj.GetComponent<BuildableObject>();
        if (buildScript != null)
        {
            buildScript.Init(currentBlueprint, inventoryManager.playerInventory);
            buildScript.LastHotkeyIndex = currentHotkeyIndex;

            if (previewChunk != null)
            {
                buildScript.currentChunk = previewChunk;
                placedObj.transform.SetParent(previewChunk.transform);
            }

            // Chỉ đổi material trên defaultObject qua method SetMaterial
            if (currentBlueprint.resultItem is SurvivalClass sData && sData.previewMaterial != null)
            {
                buildScript.SetMaterial(sData.previewMaterial);
            }
        }

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        currentBlueprint = null;
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
