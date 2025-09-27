using UnityEngine;

public class PlacementSystem : MonoBehaviour, IPlayerDependent
{
    public static PlacementSystem Instance;
    public Transform playerCamera;
    public float previewDistance = 3f;
    public LayerMask groundMask;
    public LayerMask placementBlockMask;

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

        Transform cam = playerCamera != null ? playerCamera : Camera.main.transform;

        // Tắt toàn bộ collider trước khi raycast
        SetAllCollidersEnabled(false);

        // Raycast xuống mặt đất
        Vector3 rayOrigin = cam.position + cam.forward * previewDistance + Vector3.up * 50f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lowestLocal = GetLowestPointLocal(previewObject);
            previewObject.transform.position = hit.point - previewObject.transform.TransformVector(lowestLocal);
            previewObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            previewObject.transform.position = cam.position + cam.forward * previewDistance;
            previewObject.transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
        }

        // Bật lại collider sau khi raycast xong
        SetAllCollidersEnabled(true);

        // Chunk preview
        if (ChunkManager.Instance != null)
            previewChunk = ChunkManager.Instance.GetOrCreateChunk(previewObject.transform.position);

        // Đổi material preview
        bool canPlace = CanPlaceHere();
        bool hasAllItems = HasAllRequiredItems();

        if (!canPlace)
            SetPreviewMaterial(invalidMat);
        else if (canPlace && hasAllItems)
            SetPreviewMaterial(validMat);
        else
            SetPreviewMaterial(originalMat);

        // Đặt hoặc hủy
        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceObject();
        else if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    /// <summary>
    /// Lấy điểm thấp nhất của collider prefab trong local space
    /// </summary>
    private Vector3 GetLowestPointLocal(GameObject obj)
    {
        Collider[] cols = obj.GetComponentsInChildren<Collider>();
        Vector3 lowest = Vector3.zero;
        float minY = float.MaxValue;

        foreach (var c in cols)
        {
            Vector3 localMin = obj.transform.InverseTransformPoint(c.bounds.min);
            if (localMin.y < minY)
            {
                minY = localMin.y;
                lowest = localMin;
            }
        }
        return lowest;
    }

    private void SetAllCollidersEnabled(bool enabled)
    {
        if (previewObject == null) return;

        Collider parentCol = previewObject.GetComponent<Collider>();
        if (parentCol != null)
            parentCol.enabled = enabled;

        Collider[] childColliders = previewObject.GetComponentsInChildren<Collider>(true); // true = include inactive
        foreach (var col in childColliders)
        {
            // Tránh tắt lại collider của cha lần nữa
            if (col != parentCol)
                col.enabled = enabled;
        }
    }


    public void SetPlayer(PlayerController player)
    {
        if (player != null)
        {
            playerCamera = player.transform;
        }
    }

    private bool CanPlaceHere()
    {
        Collider col = previewObject.GetComponent<Collider>();
        if (col == null) return true;

        Collider[] hits = Physics.OverlapBox(
            col.bounds.center,
            col.bounds.extents,
            previewObject.transform.rotation,
            placementBlockMask
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






//using UnityEngine;

//public class PlacementSystem : MonoBehaviour, IPlayerDependent
//{
//    public static PlacementSystem Instance;
//    public Transform playerCamera;
//    public float previewDistance = 3f;
//    public LayerMask groundMask;
//    public LayerMask placementBlockMask;
//    //public float raycastHeight = 5f;     
//    //public float placementOffset = 0.02f;
//    private GameObject previewObject;
//    private BlueprintData currentBlueprint;
//    private InventoryManager inventoryManager;
//    private GameObject previewChunk;
//    private PlayerController player;
//    private Material previewMat;
//    private Material validMat;
//    private Material invalidMat;
//    private Material originalMat;

//    private int currentHotkeyIndex = -1;

//    private void Awake()
//    {
//        Instance = this;
//    }

//    private void Start()
//    {
//        inventoryManager = InventoryManager.Instance;
//    }

//    public void StartPlacement(BlueprintData blueprint, int hotkeyIndex)
//    {
//        if (blueprint == null) return;

//        currentBlueprint = blueprint;
//        currentHotkeyIndex = hotkeyIndex;

//        // Lấy prefab và materials nếu là SurvivalClass
//        GameObject prefabToUse = null;
//        if (blueprint.resultItem is SurvivalClass survival && survival.prefabToPlace != null)
//        {
//            prefabToUse = survival.prefabToPlace;
//            previewMat = survival.previewMaterial;
//            validMat = survival.validMaterial;
//            invalidMat = survival.invalidMaterial;
//            originalMat = survival.originalMaterial;
//        }
//        else
//        {
//            prefabToUse = blueprint.resultItem.dropPrefab;
//            previewMat = validMat = invalidMat = originalMat = null;
//        }

//        if (prefabToUse == null)
//        {
//            Debug.LogWarning($"Blueprint {blueprint.name} không có prefab để đặt");
//            return;
//        }

//        if (previewObject != null)
//            Destroy(previewObject);

//        previewObject = Instantiate(prefabToUse);
//        SetPreviewMaterial(previewMat);
//    }

//    private void Update()
//    {
//        if (previewObject == null || player == null) return;

//        // 🔹 Lấy vị trí và hướng từ player
//        Transform playerTr = PlayerManager.Instance.GetPlayerTransform();
//        if (playerTr == null) return;

//        Vector3 playerPos = playerTr.position;
//        Vector3 playerFwd = playerTr.forward;

//        // 🔹 Điểm xuất phát của raycast: ngay trước mặt player, cao hơn một chút
//        Vector3 castOrigin = playerPos + playerFwd * previewDistance + Vector3.up * 1f;

//        // Tạm tắt collider của preview để không chặn ray
//        Collider previewCol = previewObject.GetComponent<Collider>();
//        if (previewCol) previewCol.enabled = false;

//        // 🔹 Bắn ray thẳng xuống mặt đất
//        if (Physics.Raycast(castOrigin, Vector3.down, out RaycastHit hit, 20f, groundMask))
//        {
//            // Tính offset đáy của object theo normal
//            float bottomOffset = GetBottomOffsetAlongNormal(previewObject, hit.normal);

//            // Đặt object lên đúng vị trí mặt đất
//            previewObject.transform.position = hit.point + hit.normal * bottomOffset;

//            // Xoay object theo hướng player, nhưng bám dốc
//            Vector3 forwardProjected = Vector3.ProjectOnPlane(playerFwd, hit.normal);
//            previewObject.transform.rotation = Quaternion.LookRotation(forwardProjected, hit.normal);
//        }
//        else
//        {
//            // Không bắn trúng gì → fallback đặt cách trước mặt player
//            Vector3 fallback = playerPos + playerFwd * previewDistance;
//            previewObject.transform.position = fallback;
//            previewObject.transform.rotation = Quaternion.LookRotation(playerFwd, Vector3.up);
//        }

//        // Bật lại collider
//        if (previewCol) previewCol.enabled = true;



//        // Chunk preview
//        if (ChunkManager.Instance != null)
//            previewChunk = ChunkManager.Instance.GetOrCreateChunk(previewObject.transform.position);

//        // Đổi vật liệu preview
//        bool canPlace = CanPlaceHere();
//        bool hasAllItems = HasAllRequiredItems();

//        if (!canPlace)
//            SetPreviewMaterial(invalidMat);
//        else if (canPlace && hasAllItems)
//            SetPreviewMaterial(validMat);
//        else
//            SetPreviewMaterial(originalMat);

//        // Đặt hoặc hủy
//        if (Input.GetMouseButtonDown(0) && canPlace)
//            PlaceObject();
//        else if (Input.GetMouseButtonDown(1))
//            CancelPlacement();
//    }

//    /// <summary>
//    /// Tính khoảng cách từ pivot của prefab xuống đáy collider theo hướng pháp tuyến mặt đất
//    /// </summary>
//    private float GetBottomOffsetAlongNormal(GameObject obj, Vector3 normal)
//    {
//        Collider[] cols = obj.GetComponentsInChildren<Collider>();
//        if (cols.Length == 0) return 0f;

//        Vector3 pivot = obj.transform.position;
//        float minProj = float.MaxValue;

//        foreach (var c in cols)
//        {
//            Bounds b = c.bounds;
//            // Lấy 8 đỉnh của collider
//            Vector3[] pts =
//            {
//            b.min,
//            new Vector3(b.min.x, b.min.y, b.max.z),
//            new Vector3(b.min.x, b.max.y, b.min.z),
//            new Vector3(b.max.x, b.min.y, b.min.z),
//            new Vector3(b.max.x, b.max.y, b.min.z),
//            new Vector3(b.max.x, b.min.y, b.max.z),
//            new Vector3(b.min.x, b.max.y, b.max.z),
//            b.max
//        };

//            foreach (var p in pts)
//            {
//                float proj = Vector3.Dot(p - pivot, normal);
//                if (proj < minProj) minProj = proj;
//            }
//        }

//        // Kết quả là khoảng cách cần cộng theo hướng normal để đáy chạm mặt đất
//        return -minProj + 0.01f; // thêm 0.01 để tránh lún
//    }


//    // Lấy điểm Y thấp nhất của tất cả collider trong previewObject
//    private float GetLowestPointY(GameObject obj)
//    {
//        Collider[] cols = obj.GetComponentsInChildren<Collider>();
//        float minY = float.MaxValue;

//        foreach (var col in cols)
//        {
//            minY = Mathf.Min(minY, col.bounds.min.y);
//        }

//        return minY;
//    }


//    public void SetPlayer(PlayerController p)
//    {
//        player = p;   // <── lưu player
//        if (player != null)
//        {
//            // camera của player góc nhìn thứ 3
//            playerCamera = player.playerCamera != null ? player.playerCamera : player.transform;
//        }
//    }

//    private bool CanPlaceHere()
//    {
//        Collider col = previewObject.GetComponent<Collider>();
//        if (col == null) return true;

//        Collider[] hits = Physics.OverlapBox(
//            col.bounds.center,
//            col.bounds.extents,
//            previewObject.transform.rotation,
//            placementBlockMask
//        );

//        foreach (var h in hits)
//        {
//            if (h.gameObject != previewObject)
//                return false;
//        }
//        return true;
//    }

//    private bool HasAllRequiredItems()
//    {
//        foreach (var req in currentBlueprint.requirements)
//        {
//            if (inventoryManager.playerInventory.GetTotalQuantity(req.item) < req.amount)
//                return false;
//        }
//        return true;
//    }

//    private void PlaceObject()
//    {
//        if (currentBlueprint == null || previewObject == null) return;

//        if (currentBlueprint.resultItem is SurvivalClass survival)
//        {
//            // Xóa blueprint cũ cùng hotkey nếu chưa build
//            var existingBlueprints = FindObjectsOfType<BuildableObject>();
//            foreach (var bp in existingBlueprints)
//            {
//                if (bp.IsSameBlueprint(currentBlueprint) &&
//                    bp.LastHotkeyIndex == currentHotkeyIndex &&
//                    !bp.IsBuilt &&
//                    !bp.HasAnyMaterials())
//                {
//                    Destroy(bp.gameObject);
//                    Debug.Log($"Xóa blueprint {currentBlueprint.name} của hotkey {currentHotkeyIndex + 1}");
//                }
//            }
//        }

//        GameObject prefabToPlace = (currentBlueprint.resultItem is SurvivalClass s && s.prefabToPlace != null)
//            ? s.prefabToPlace
//            : currentBlueprint.resultItem.dropPrefab;

//        GameObject placedObj = Instantiate(prefabToPlace, previewObject.transform.position, previewObject.transform.rotation);

//        BuildableObject buildScript = placedObj.GetComponent<BuildableObject>();
//        if (buildScript != null)
//        {
//            buildScript.Init(currentBlueprint, inventoryManager.playerInventory);
//            buildScript.LastHotkeyIndex = currentHotkeyIndex;

//            if (previewChunk != null)
//            {
//                buildScript.currentChunk = previewChunk;
//                placedObj.transform.SetParent(previewChunk.transform);
//            }

//            // Chỉ đổi material trên defaultObject qua method SetMaterial
//            if (currentBlueprint.resultItem is SurvivalClass sData && sData.previewMaterial != null)
//            {
//                buildScript.SetMaterial(sData.previewMaterial);
//            }
//        }

//        CancelPlacement();
//    }

//    public void CancelPlacement()
//    {
//        if (previewObject != null)
//            Destroy(previewObject);

//        previewObject = null;
//        currentBlueprint = null;
//        currentHotkeyIndex = -1;
//    }

//    private void SetPreviewMaterial(Material mat)
//    {
//        if (mat == null) return;
//        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
//        {
//            var mats = r.materials;
//            for (int i = 0; i < mats.Length; i++)
//                mats[i] = mat;
//            r.materials = mats;
//        }
//    }
//}
