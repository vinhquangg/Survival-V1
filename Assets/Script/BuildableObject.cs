using UnityEngine;

public class BuildableObject : MonoBehaviour,IInteractableInfo,IHasBlueprint
{
    BlueprintData bluePrint;
    private PlayerInventory playerInventory;
    private int[] currentMaterials;
    private Renderer[] renderers;
    private bool isBuilt = false;
    private ParticleSystem[] buildVFX;
    public int LastHotkeyIndex { get; set; }
    public bool IsBuilt => isBuilt;

    public void Init(BlueprintData blueprintData, PlayerInventory inventory)
    {
        bluePrint = blueprintData;
        playerInventory = inventory;

        currentMaterials = new int[bluePrint.requirements.Count];
        renderers = GetComponentsInChildren<Renderer>();
        buildVFX = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var vfx in buildVFX)
            vfx.gameObject.SetActive(false);
        if(bluePrint.resultItem is SurvivalClass survival && survival.previewMaterial != null)
            SetMaterial(survival.previewMaterial);
    }

    private void OnMouseDown()
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

            if (currentMaterials[i] < req.amount &&
                playerInventory.HasItem(req.item, 1))
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
            if(bluePrint.resultItem is SurvivalClass survival && survival.previewMaterial != null)
                SetMaterial(allComplete ? survival.originalMaterial : survival.validMaterial);

            if (allComplete)
                CompleteBuild();
        }
        else
        {
            if(bluePrint.resultItem is SurvivalClass survival && survival.invalidMaterial != null)
                SetMaterial(survival.invalidMaterial);
        }
    }

    private void CompleteBuild()
    {
        isBuilt = true;
        if (bluePrint.resultItem is SurvivalClass survival && survival.invalidMaterial != null)
            SetMaterial(survival.invalidMaterial);

        foreach (var vfx in buildVFX)
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }
    }

    public bool IsSameBlueprint(BlueprintData otherData)
    {
        return bluePrint == otherData;
    }

    private void SetMaterial(Material mat)
    {
        if (mat == null) return;
        foreach (var r in renderers)
            r.material = mat;
    }

    public string GetName()
    {
        return bluePrint != null ? bluePrint.resultItem.itemName : "Unknown";
    }

    public string GetItemAmount()
    {
        if (bluePrint == null) return "";

        // Ví dụ hiển thị số nguyên liệu đã có / cần
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
}
