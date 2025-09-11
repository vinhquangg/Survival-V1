using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager Instance { get; private set; }
    //MODIFIER
    private const int NIGHT_MODIFIER = -15;
    //private const int Day_MODIFIER = 8;
    private const int Fire_MODIFIER = 10;
    //EFFECCT THRESHOLD
    private const int DANGER_BELOW = 25;
    private const int DANGER_ABOVE = 50;

    private int baseTemperature = 37;
    public int currentTemperature { get; private set; }
    //Modifiers tracking
    private bool isNight = false;
    private bool isNearFire = false;
    //UI
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI temperatureWarningText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RecalculateTemperature()
    {
        currentTemperature = baseTemperature;
        if(isNight)
            currentTemperature += NIGHT_MODIFIER;
        if (isNearFire)
            currentTemperature += Fire_MODIFIER;

        temperatureText.text = $"Temp: {currentTemperature}ºC" ;

        ApplyEffect();
    }

    private void ApplyEffect()
    {
        if(currentTemperature <= DANGER_BELOW)
        {
            temperatureWarningText.text = "Danger! Too Cold";
        }
        if(currentTemperature >= DANGER_ABOVE)
        {
            temperatureWarningText.text = "Danger! Too Hot";
        }
    }

    public void SetNight(bool isNightNow)
    {
        isNight = isNightNow;

        // Chỉ hiển thị UI khi night
        temperatureText.gameObject.SetActive(isNightNow);
        temperatureWarningText.gameObject.SetActive(isNightNow);

        RecalculateTemperature();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
