using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("StatsBar requires a Slider component.");
        }
    }

    public void UpdateBar(float percent)
    {
        slider.value = Mathf.Clamp01(percent);
    }
}
