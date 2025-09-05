using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lake : MonoBehaviour,IInteractable,IInteractableInfo
{
    [Header("Drink Settings")]
    [SerializeField] private string waterName = "Fresh Water";
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private float drinkAmount = 10f;
    [SerializeField] private bool isDirty = false;

    [SerializeField] private float interactCooldown = 1f;
    private float _nextTime;
    [SerializeField] private bool playerInRange = false; 
    public string GetName() => waterName;
    public string GetItemAmount() => isDirty ? "Dirty" : "Clean";
    public Sprite GetIcon() => waterIcon;
    public InteractionType GetInteractionType() => InteractionType.Drink;

    public void Interact(GameObject interactor)
    {
        if (Time.time < _nextTime||!playerInRange) return;

        if (PlayerStatus.Instance != null && PlayerStatus.Instance.thirst != null)
        {
            PlayerStatus.Instance.thirst.Restore(drinkAmount);
            Debug.Log($"[Drink] {PlayerStatus.Instance.thirst.data.statName}: +{drinkAmount}");
        }

        // Nếu nước bẩn → gây hiệu ứng phụ (ví dụ trừ máu)
        if (isDirty && PlayerStatus.Instance != null)
        {
            PlayerStatus.Instance.health.Reduce(2f);
        }

        _nextTime = Time.time + interactCooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool IsPlayerInRange() => playerInRange;

}
