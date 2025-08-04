using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    private TreeInstance treeInstance;
    //[SerializeField] private string treeName;
    //[SerializeField] private GameObject stumpPrefab;
    //[SerializeField] private string stumpPoolTag;

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
            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine,player,this));
        }
    }


    public void OnChopped()
    {
        Debug.Log("🌳 Cây bị chặt rồi!");

        var treeInstance = GetComponent<TreeInstance>();
        if (treeInstance != null)
        {
            treeInstance.isChopped = true;
            if (!string.IsNullOrEmpty(treeInstance.treeData.stumpPoolID))
            {
                ObjectPoolManager.Instance.SpawnFromPool(treeInstance.treeData.stumpPoolID, transform.position, transform.rotation);
            }

            treeInstance.ShowLogDropAfterDelay(0.5f); 

            StartCoroutine(ReturnToPoolWithDelay(0.6f)); 
        }
    }

    private IEnumerator ReturnToPoolWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }



}
