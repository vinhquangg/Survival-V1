using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    [SerializeField] private string treeName;
    [SerializeField] private GameObject stumpPrefab;
    [SerializeField] private string stumpPoolTag;
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
        return "Chop " + treeName;
    }

    public void Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine,player,this));
        }
    }


    public void OnChopped()
    {
        Debug.Log("🌳 Cây bị chặt rồi!");

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        // 🟢 Spawn từ pool thay vì Instantiate
        if (!string.IsNullOrEmpty(stumpPoolTag))
        {
            ObjectPoolManager.Instance.SpawnFromPool(stumpPoolTag, pos, rot);
        }

        // Cuối cùng, tắt cây
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }


}
