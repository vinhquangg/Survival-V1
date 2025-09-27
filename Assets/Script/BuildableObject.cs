using System.Collections;
using System.Linq;
using UnityEngine;

public class BuildableObject : MonoBehaviour, IInteractableInfo, IHasBlueprint, IInteractable
{
    BlueprintData bluePrint;
    private PlayerInventory playerInventory;
    private int[] currentMaterials;
    private Renderer[] renderers;
    private bool isBuilt = false;
    private ParticleSystem[] buildVFX;
    public delegate void OnMaterialChangedHandler();
    public event OnMaterialChangedHandler OnMaterialChanged;
    [SerializeField] private GameObject defaultObject; 
    [HideInInspector] public GameObject currentChunk;
    public BlueprintData sceneBlueprint;
    public int LastHotkeyIndex { get; set; }
    public bool IsBuilt => isBuilt;

    void Awake()
    {
        if (sceneBlueprint != null)
            TryInit();
    }

    private void TryInit()
    {
        if (bluePrint != null && playerInventory != null)
            return;

        if (sceneBlueprint == null)
        {
            Debug.LogWarning($"{name} chưa có sceneBlueprint!");
            return;
        }

        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            StartCoroutine(DelayedInit()); // chờ PlayerInventory xuất hiện
        }
        else
        {
            Init(sceneBlueprint, playerInventory);
        }
    }

    IEnumerator DelayedInit()
    {
        while (FindObjectOfType<PlayerInventory>() == null)
            yield return null;

        playerInventory = FindObjectOfType<PlayerInventory>();
        Init(sceneBlueprint, playerInventory);
    }


    //void Start()
    //{
    //    if (bluePrint == null && sceneBlueprint != null)
    //    {
    //        if (playerInventory == null)
    //            playerInventory = FindObjectOfType<PlayerInventory>();

    //        if (playerInventory != null)
    //            Init(sceneBlueprint, playerInventory);
    //        else
    //            Debug.LogWarning($"BuildableObject {name} chưa có PlayerInventory để Init!");
    //    }
    //}


    public void Init(BlueprintData blueprintData, PlayerInventory inventory)
    {
        bluePrint = blueprintData;
        playerInventory = inventory;

        currentMaterials = new int[bluePrint.requirements.Count];

        // Chỉ lấy Renderer từ defaultObject, nếu defaultObject null thì tạo mảng rỗng
        if (defaultObject != null)
            renderers = defaultObject.GetComponentsInChildren<Renderer>()
                       .Where(r => r is MeshRenderer || r is SkinnedMeshRenderer)
                       .ToArray();
        else
            renderers = new Renderer[0];


        buildVFX = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var vfx in buildVFX)
            vfx.gameObject.SetActive(false);

        if (bluePrint.resultItem is SurvivalClass survival && survival.previewMaterial != null)
            SetMaterial(survival.previewMaterial);

        isBuilt = false;
    }

    public void AddMaterialByPlayerInput()
    {
        if (!isBuilt)
            TryAddMaterial();
    }

    private void TryAddMaterial()
    {
        if (bluePrint == null)
        {
            Debug.LogWarning($"BuildableObject {name} chưa có blueprint!");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogWarning($"BuildableObject {name} chưa gán playerInventory!");
            return;
        }

        bool anyAdded = false;
        bool allComplete = true;

        for (int i = 0; i < bluePrint.requirements.Count; i++)
        {
            var req = bluePrint.requirements[i];

            if (currentMaterials[i] < req.amount && playerInventory.HasItem(req.item, 1))
            {
                playerInventory.RemoveItem(req.item, 1);
                currentMaterials[i]++;
                anyAdded = true;
            }

            if (currentMaterials[i] < req.amount)
                allComplete = false;
        }

        if (!anyAdded)
        {
            // 🔹 Nếu là Tools/khác Survival → log luôn
            if (!(bluePrint.resultItem is SurvivalClass))
            {
                Debug.Log($"BuildableObject {name}: thiếu nguyên liệu để thêm!");
            }
            // 🔹 Nếu là Survival → đổi sang invalidMaterial nếu có
            else if (bluePrint.resultItem is SurvivalClass survival && survival.invalidMaterial != null)
            {
                SetMaterial(survival.invalidMaterial);
            }
            return;
        }

        InventoryManager.Instance.RefreshAllUI();
        OnMaterialChanged?.Invoke();

        // 🔹 Nếu là Survival → đổi preview/valid/original
        if (bluePrint.resultItem is SurvivalClass survivalMat && survivalMat.previewMaterial != null)
        {
            SetMaterial(allComplete ? survivalMat.originalMaterial : survivalMat.validMaterial);
        }

        if (allComplete)
            CompleteBuild();
    }



    private void CompleteBuild()
    {
        isBuilt = true;

        if (bluePrint.resultItem is SurvivalClass survival && survival.originalMaterial != null)
            SetMaterial(survival.originalMaterial);

        foreach (var vfx in buildVFX)
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }
        OnMaterialChanged?.Invoke();

        // 🔑 Nếu là Campfire thì tắt lửa ngay khi build xong
        Campfire campfire = GetComponent<Campfire>();
        if (campfire != null)
        {
            campfire.StopFire();
        }

        if (PlayerStatus.Instance != null)
        {
            PlayerStatus.Instance.thirst.Reduce(20f);  
            PlayerStatus.Instance.hunger.Reduce(20f); 
            Debug.Log("Player mất 5 máu và 10 thức ăn khi build xong!");
        }

    }


    public bool HasAnyMaterials()
    {
        if (currentMaterials == null || currentMaterials.Length == 0)
            return false;

        for (int i = 0; i < currentMaterials.Length; i++)
        {
            if (currentMaterials[i] > 0)
                return true;
        }
        return false;
    }

    public bool IsSameBlueprint(BlueprintData otherData)
    {
        return bluePrint == otherData;
    }

    public void SetMaterial(Material mat)
    {
        if (defaultObject != null)
        {
            var defaultRenderer = defaultObject.GetComponent<Renderer>();
            if (defaultRenderer != null)
            {
                defaultRenderer.material = mat;
            }
        }
    }


    public string GetName()
    {
        return bluePrint != null ? bluePrint.resultItem.itemName : "Unknown";
    }

    public string GetItemAmount()
    {
        if (bluePrint == null) return "";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < bluePrint.requirements.Count; i++)
        {
            var req = bluePrint.requirements[i];
            sb.Append($"{currentMaterials[i]}/{req.amount} {req.item.itemName}");
            if (i < bluePrint.requirements.Count - 1)
                sb.Append("\n");
        }
        return sb.ToString();
    }

    public Sprite GetIcon()
    {
        return bluePrint != null ? bluePrint.resultItem.itemIcon : null;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Placeable;
    }

    public BlueprintData GetBlueprint()
    {
        return bluePrint;
    }

    public void Interact(GameObject interactor)
    {
         AddMaterialByPlayerInput();
    }
}
