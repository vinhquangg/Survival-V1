using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager Instance { get; private set; }

    // Base Temperature (°C)
    [Header("Modifiers (°C)")]
    [SerializeField] private int baseTemperature = 37;         
    [SerializeField] private int NIGHT_MODIFIER = -10;         
    [SerializeField] private int Fire_MODIFIER = 12;           

    //Smoothing speeds (°C per second) 
    [Header("Smoothing (°C/sec)")]
    [Tooltip("Tốc độ tăng nhiệt khi vào fire (°C/s)")]
    [SerializeField] private float heatRate = 8f;              
    [Tooltip("Tốc độ giảm nhiệt khi rời fire (°C/s)")]
    [SerializeField] private float coolRate = 2.5f;            

    // Effect Threshold
    [Header("Thresholds")]
    [SerializeField] private int DANGER_BELOW = 27;            
    [SerializeField] private int DANGER_ABOVE = 38;            
    [Tooltip("Hysteresis margin (°C) to avoid flicker on warnings")]
    [SerializeField] private float hysteresis = 1.0f;          

    // Internal
    public float currentTemperature { get; private set; }      
    private float targetTemperature;

    // Modifiers tracking
    private bool isNight = false;
    private bool isNearFire = false;

    // UI
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI temperatureWarningText;

    // Internal for hysteresis state
    private bool warningColdActive = false;
    private bool warningHotActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize
        currentTemperature = baseTemperature;
        targetTemperature = baseTemperature;
    }

    private void Start()
    {
        UpdateUIActiveStates();
        UpdateTemperatureText();
    }

    private void Update()
    {
        
        ComputeTargetTemperature();

        float rate = (targetTemperature > currentTemperature) ? heatRate : coolRate;

        currentTemperature = Mathf.MoveTowards(currentTemperature, targetTemperature, rate * Time.deltaTime);

        UpdateTemperatureText();

        ApplyEffectWithHysteresis();
    }

    private void ComputeTargetTemperature()
    {
        targetTemperature = baseTemperature;
        if (isNight) targetTemperature += NIGHT_MODIFIER;
        if (isNearFire) targetTemperature += Fire_MODIFIER;
    }

    private void UpdateUIActiveStates()
    {

        bool shouldShowUI = isNight || isNearFire;
        if (temperatureText != null) temperatureText.gameObject.SetActive(shouldShowUI);
        if (temperatureWarningText != null) temperatureWarningText.gameObject.SetActive(shouldShowUI);
    }

    //public void RecalculateTemperature()
    //{
        
    //    ComputeTargetTemperature();
    //    UpdateUIActiveStates();
    //    UpdateTemperatureText();
    //}

    private void UpdateTemperatureText()
    {
        if (temperatureText != null)
        {
            temperatureText.text = $"Temp: {Mathf.RoundToInt(currentTemperature)}ºC";
        }
    }

    private void ApplyEffectWithHysteresis()
    {
        if (temperatureWarningText == null) return;

        // cold check (with hysteresis)
        if (!warningColdActive && currentTemperature <= DANGER_BELOW - hysteresis)
        {
            warningColdActive = true;
            warningHotActive = false;
            temperatureWarningText.text = "Danger! Too Cold";
        }
        else if (warningColdActive && currentTemperature > DANGER_BELOW + hysteresis)
        {
            warningColdActive = false;
            temperatureWarningText.text = "";
        }

        // hot check (with hysteresis)
        if (!warningHotActive && currentTemperature >= DANGER_ABOVE + hysteresis)
        {
            warningHotActive = true;
            warningColdActive = false;
            temperatureWarningText.text = "Danger! Too Hot";
        }
        else if (warningHotActive && currentTemperature < DANGER_ABOVE - hysteresis)
        {
            warningHotActive = false;
            temperatureWarningText.text = "";
        }
    }

    public void SetNight(bool isNightNow)
    {
        isNight = isNightNow;
        UpdateUIActiveStates();

        ComputeTargetTemperature();
    }

    public void SetNearFire(bool nearFire)
    {
        isNearFire = nearFire;

        UpdateUIActiveStates();

        ComputeTargetTemperature();
    }

    //public void ForceSetTemperature(float temp)
    //{
    //    currentTemperature = temp;
    //    targetTemperature = temp;
    //    UpdateTemperatureText();
    //}
}
