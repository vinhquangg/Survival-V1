using UnityEngine;
[System.Serializable]
public class StatInstance
{
    public StatsData data;
    public float currentValue;

    public StatInstance(StatsData data)
    {
        this.data = data;
        this.currentValue = data.currentValue;
    }

    public void Reduce(float amount)
    {
        currentValue = Mathf.Clamp(currentValue - amount, 0, data.maxValue);
    }

    public void Restore(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, data.maxValue);
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