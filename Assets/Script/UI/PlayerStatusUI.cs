using UnityEngine;

public class PlayerStatusUI : MonoBehaviour
{
    public StatsBar healthData;
    public StatsBar caloriesData;
    public StatsBar thirstData;

    private PlayerStatus playerStatus;

    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        if (playerStatus == null) return;

        playerStatus.health.OnStatChanged += UpdateHealth;
        playerStatus.hunger.OnStatChanged += UpdateCalories;
        playerStatus.thirst.OnStatChanged += UpdateThirst;

        UpdateHealth(playerStatus.health.currentValue);
        UpdateCalories(playerStatus.hunger.currentValue);
        UpdateThirst(playerStatus.thirst.currentValue);
    }

    void UpdateHealth(float value)
    {
        if (healthData != null)
            healthData.UpdateBar(value, playerStatus.health.data.maxValue);
    }

    void UpdateCalories(float value)
    {
        if (caloriesData != null)
            caloriesData.UpdateBar(value, playerStatus.hunger.data.maxValue);
    }

    void UpdateThirst(float value)
    {
        if (thirstData != null)
            thirstData.UpdateBar(value, playerStatus.thirst.data.maxValue);
    }
}
