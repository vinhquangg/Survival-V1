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
    public int LastHotkeyIndex { get; set; }
    public bool IsBuilt => isBuilt;

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

        if (anyAdded)
        {
            InventoryManager.Instance.RefreshAllUI();

            OnMaterialChanged?.Invoke();

            if (bluePrint.resultItem is SurvivalClass survival && survival.previewMaterial != null)
            {
                SetMaterial(allComplete ? survival.originalMaterial : survival.validMaterial);
            }

            if (allComplete)
                CompleteBuild();
        }
        else
        {
            if (bluePrint.resultItem is SurvivalClass survival && survival.invalidMaterial != null)
                SetMaterial(survival.invalidMaterial);
        }
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
