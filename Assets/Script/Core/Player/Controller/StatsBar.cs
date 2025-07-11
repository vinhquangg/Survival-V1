using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI amount;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("StatsBar requires a Slider component.");
        }
        amount = GetComponentInChildren<TextMeshProUGUI>();
        if (amount == null)
        {
            Debug.LogError("StatsBar requires a TextMeshProUGUI component.");
        }
    }

    public void UpdateBar(float current, float max)
    {
        float percent = Mathf.Clamp01(current / max);
        slider.value = percent;
        amount.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}"; 
    }


}
