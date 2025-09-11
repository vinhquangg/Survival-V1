using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public Light directionLight;
    public float dayDurationInSeconds; // Thời gian một ngày (giây)
    public int currentHour;
    public List<SkyBoxTimeMapping> timeMappings;
    public TextMeshProUGUI timeUI;
    public int isNightStartHour = 18;
    public int isNightEndHour = 6;


    float currentTimeOfDay = 0.25f;
    float blendedValue = 0.0f;
    bool lockNextDayTrigger = false;

    
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        currentTimeOfDay += (Time.deltaTime / dayDurationInSeconds); // Chuyển đổi thời gian thành giờ
        currentTimeOfDay %= 1;
        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24f);
        timeUI.text = $"{currentHour}:00";
        directionLight.transform.rotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0f);
        if (currentHour >= isNightStartHour || currentHour < isNightEndHour)
        {
            TemperatureManager.Instance.SetNight(true);
        }
        else
        {
            TemperatureManager.Instance.SetNight(false);
        }
        ChangeSkyBoxForCurrentHour();
    }

    public void ChangeSkyBoxForCurrentHour()
    {
        Material currentSkyBox = null;
        foreach (SkyBoxTimeMapping mapping in timeMappings)
        {
            if (currentHour == mapping.hour)
            {
                currentSkyBox = mapping.skyboxMaterial;

                if(currentSkyBox.shader != null)
                {
                    if(currentSkyBox.shader.name == "Custom/SkyboxTransition")
                    {
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);

                        currentSkyBox.SetFloat("_TransitionFactor", blendedValue);
                    }
                    else
                    {
                        blendedValue = 0f;
                    }
                }
                break;
            }

        }

        if(currentHour == 0 && lockNextDayTrigger == false)
        {
            TimeManager.Instance.NextDay();
            lockNextDayTrigger = true;
        }

        if(currentHour != 0)
        {
            lockNextDayTrigger = false;
        }

        if (currentSkyBox != null)
        {
            RenderSettings.skybox = currentSkyBox;
        }
    }
}

[Serializable]
public class SkyBoxTimeMapping
{
    public string phaseName;
    public int hour;
    public Material skyboxMaterial;
}
