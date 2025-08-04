using System.Collections;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    private TreeInstance treeInstance;

    private void Start()
    {
        treeInstance = GetComponent<TreeInstance>();
    }

    public Sprite GetIcon()
    {
        return null;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Chop;
    }

    public string GetItemAmount()
    {
        return "";
    }

    public string GetName()
    {
        if (treeInstance == null)
            treeInstance = GetComponent<TreeInstance>();

        return "Chop " + (treeInstance?.treeData?.treeName ?? "Unknown");
    }

    public void Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine, player, this));
        }
    }

    public void OnChopped()
    {
        Debug.Log("🌳 Cây bị chặt rồi!");

        if (treeInstance == null)
            treeInstance = GetComponent<TreeInstance>();

        if (treeInstance != null)
        {
            treeInstance.isChopped = true;

            // 🌱 Spawn gốc cây từ Pool
            if (!string.IsNullOrEmpty(treeInstance.treeData.stumpPoolID))
            {
                ObjectPoolManager.Instance.SpawnFromPool(
                    treeInstance.treeData.stumpPoolID,
                    transform.position,
                    transform.rotation
                );
            }

            // 🪵 Spawn LogDrop từ Pool
            if (!string.IsNullOrEmpty(treeInstance.treeData.logPoolID))
            {
                Vector3 spawnPos = GetGroundPosition(transform.position + Vector3.up * 3f);
                GameObject logDrop = ObjectPoolManager.Instance.SpawnFromPool(
                    treeInstance.treeData.logPoolID,
                    spawnPos,
                    Quaternion.identity
                );

                // ⚙️ Nếu có Rigidbody thì cho rơi nhẹ rồi đứng yên
                if (logDrop != null && logDrop.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.down * 1f;

                    treeInstance.StartCoroutine(DropAndSettle(rb));
                }
            }

            // ⏱ Trả cây về pool sau khi chặt
            StartCoroutine(ReturnToPoolWithDelay(0.6f));
        }
    }

    private IEnumerator ReturnToPoolWithDelay(float delay)
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

    private Vector3 GetGroundPosition(Vector3 origin)
    {
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }
        return origin;
    }
}
