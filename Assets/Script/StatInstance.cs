using System;
using UnityEngine;
[System.Serializable]
public class StatInstance
{
    public StatsData data;
    public float currentValue;
    public Action<float> OnStatChanged;
    private float decayTimer = 0f;
    private float fullRegenTimer = 0f;
    private bool canRegenHealth = false;
    public StatInstance(StatsData data)
    {
        this.data = data;
        this.currentValue = data.currentValue;
    }

    public void Reduce(float amount)
    {
        float old = currentValue;
        currentValue = Mathf.Clamp(currentValue - amount, 0, data.maxValue);
        OnStatChanged?.Invoke(currentValue);

        Debug.Log($"[Reduce] {data.statName}: {old:F6} -> {currentValue:F6} (-{amount})");
    }


    public void Restore(float amount)
    {
        float oldValue = currentValue;
        currentValue = Mathf.Clamp(currentValue + amount, 0, data.maxValue);
        if (Mathf.Abs(currentValue - oldValue) > 0.01f)
        {
            OnStatChanged?.Invoke(currentValue); 
        }
    }

    public void UpdateStat(float deltaTime)
    {
        decayTimer += deltaTime;

        if (data.decayRate > 0 && decayTimer >= data.decayInterval)
        {
            Reduce(data.decayRate); 
            decayTimer = 0f;
        }

        if (data.regenRate > 0)
        {
            Restore(data.regenRate * deltaTime); 
        }

        if (IsFull() && data.healthRegenRateWhenFull > 0)
        {
            if (!canRegenHealth)
            {
                canRegenHealth = true;
                fullRegenTimer = data.healthRegenDurationWhenFull;
            }

            if (canRegenHealth && fullRegenTimer > 0f)
            {
                PlayerStatus.Instance?.health?.Restore(data.healthRegenRateWhenFull * deltaTime);
                fullRegenTimer -= deltaTime;
            }
            else
            {
                canRegenHealth = false;
            }
        }
        else
        {
            // nếu không còn full thì reset trạng thái regen
            canRegenHealth = false;
            fullRegenTimer = 0f;
        }


        if (IsEmpty() && data.affectsHealth)
        {
            PlayerStatus.Instance?.health?.Reduce(data.damePerSecondWhenEmpty * deltaTime);
        }
    }



    public bool IsFull()
    {
        return currentValue >= data.maxValue - 0.01f;
    }




    public bool IsEmpty()
    {
        return currentValue <= 0;
    }

    public float GetPercent()
    {
        return currentValue / data.maxValue;
    }
}