using UnityEngine;

public class BuildableObject : MonoBehaviour
{
    private SurvivalClass data;
    private PlayerInventory playerInventory;
    private int[] currentMaterials;
    private Renderer[] renderers;
    private bool isBuilt = false;
    private ParticleSystem[] buildVFX;
    public int LastHotkeyIndex { get; set; }
    public bool IsBuilt => isBuilt;

    public void Init(SurvivalClass survivalData, PlayerInventory inventory)
    {
        data = survivalData;
        playerInventory = inventory;
        currentMaterials = new int[data.requiredItems.Length];
        renderers = GetComponentsInChildren<Renderer>();
        buildVFX = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var vfx in buildVFX)
        {
            vfx.gameObject.SetActive(false);
        }
        SetMaterial(data.previewMaterial);
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

        for (int i = 0; i < data.requiredItems.Length; i++)
        {
            var req = data.requiredItems[i];

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
            SetMaterial(allComplete ? data.originalMaterial : data.validMaterial);

            if (allComplete)
                CompleteBuild();
        }
        else
        {
            SetMaterial(data.invalidMaterial);
        }
    }

    private void CompleteBuild()
    {
        isBuilt = true;
        SetMaterial(data.originalMaterial);

        foreach (var vfx in buildVFX)
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }
    }

    public bool IsSameBlueprint(SurvivalClass otherData)
    {
        return data == otherData;
    }

    private void SetMaterial(Material mat)
    {
        if (mat == null) return;
        foreach (var r in renderers)
            r.material = mat;
    }
}
