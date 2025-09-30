using System.Collections;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    private TreeInstance treeInstance;

    private void Start()
    {
        treeInstance = GetComponent<TreeInstance>();
    }

    public Sprite GetIcon() => null;

    public InteractionType GetInteractionType() => InteractionType.Chop;

    public string GetItemAmount() => "";

    public string GetName() =>
        "Chop " + (treeInstance?.treeData?.treeName ?? "Unknown");

    public void Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.playerStateMachine.currentState is ChopState)
        {
            // Có thể show feedback nho nhỏ
            Debug.Log("Đang chặt, không thể thực hiện thêm.");
            return;
        }

        var equipManager = FindObjectOfType<EquipManager>();
        if (equipManager == null || !equipManager.HasItemEquipped(EquipType.Tool))
        {
            var feedback = FindObjectOfType<PlayerFeedbackUI>();
            feedback?.ShowFeedback(FeedbackType.NeedAxe);
            return;
        }

        // Chuyển state ChopState mới ngay lập tức
        var toolSlot = equipManager.GetEquippedSlot(EquipType.Tool);
        if (toolSlot != null && !toolSlot.IsEmpty())
        {
            toolSlot.ReduceDurability(0.2f); // trừ 5% durability
            InventoryManager.Instance.RefreshAllUI();
        }
        player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine, player, this));
        InventoryManager.Instance.RefreshAllUI();
    }

    public void SpawnDrops()
    {
        if (treeInstance == null) treeInstance = GetComponent<TreeInstance>();
        if (treeInstance == null || treeInstance.isChopped) return;

        treeInstance.isChopped = true;

        // Spawn stump
        if (!string.IsNullOrEmpty(treeInstance.treeData.stumpPoolID))
            ObjectPoolManager.Instance.SpawnFromPool(
                treeInstance.treeData.stumpPoolID,
                transform.position,
                transform.rotation
            );

        // Spawn logs
        if (!string.IsNullOrEmpty(treeInstance.treeData.logPoolID))
        {
            float offsetY = 0.5f;
            if (treeInstance.logDropGO != null && treeInstance.logDropGO.TryGetComponent<Collider>(out var logCol))
                offsetY = logCol.bounds.extents.y;

            Vector3 testPos = transform.position + transform.right * 1.5f + Vector3.up * 3f;
            Vector3 groundPos = GetGroundPosition(testPos, treeInstance.logDropGO);
            Vector3 spawnPos = groundPos + Vector3.up * offsetY;

            GameObject logDrop = ObjectPoolManager.Instance.SpawnFromPool(
                treeInstance.treeData.logPoolID,
                spawnPos,
                Quaternion.identity
            );

            if (logDrop != null && logDrop.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.down * 1f;
                StartCoroutine(DropAndSettle(rb));
            }
        }
    }

    public void HideTreeWithDelay(float delay = 0.2f)
    {
        StartCoroutine(HideTreeRoutine(delay));
    }

    private IEnumerator HideTreeRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    private IEnumerator DropAndSettle(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private Vector3 GetGroundPosition(Vector3 origin, GameObject prefab = null)
    {
        if (Physics.Raycast(origin, Vector3.down, out var hit, 10f, LayerMask.GetMask("Ground")))
        {
            Vector3 pos = hit.point;
            if (prefab != null && prefab.TryGetComponent<Collider>(out var col))
                pos.y += col.bounds.extents.y;
            return pos;
        }
        return origin;
    }
}
