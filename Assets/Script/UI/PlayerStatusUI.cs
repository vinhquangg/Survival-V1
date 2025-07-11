using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStatus == null) return;

        // Update Health Bar
        if (healthData != null)
        {
            healthData.UpdateBar(playerStatus.health.GetPercent());
        }
        // Update Calories Bar
        if (caloriesData != null)
        {
            caloriesData.UpdateBar(playerStatus.hunger.GetPercent());
        }
        // Update Thirst Bar
        if (thirstData != null)
        {
            thirstData.UpdateBar(playerStatus.thirst.GetPercent());
        }
    }
}
