using System;
using UnityEngine;
[System.Serializable]
public class StatInstance
{
    public StatsData data;
    public float currentValue;
    public Action<float> OnStatChanged;
    public StatInstance(StatsData data)
    {
        this.data = data;
        this.currentValue = data.currentValue;
    }

    public void Reduce(float amount)
    {
 
        currentValue = Mathf.Clamp(currentValue - amount, 0, data.maxValue);
        OnStatChanged?.Invoke(currentValue); 
        
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
        if (data.decayRate > 0)
        {
            Reduce(data.decayRate * deltaTime);
        }

        if (data.regenRate > 0)
        {
            Restore(data.regenRate * deltaTime);
        }

        if (IsEmpty() && data.affectsHealth)
        {
            PlayerStatus.Instance?.health?.Reduce(data.damePerSecondWhenEmpty * deltaTime);
        }
    }


    public bool IsFull()
    {
        return Mathf.Approximately(currentValue, data.maxValue) || currentValue >= data.maxValue;
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