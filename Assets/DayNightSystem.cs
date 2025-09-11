using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public Light directionLight;
    public float dayDurationInSeconds = 24.0f; // Thời gian một ngày (giây)
    public int currentHour;

    float currentTimeOfDay = 0.0f;
    public List<SkyBoxTimeMapping> timeMappings;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentTimeOfDay += (Time.deltaTime / dayDurationInSeconds); // Chuyển đổi thời gian thành giờ
        currentTimeOfDay %= 1;
        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24f);

        directionLight.transform.rotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0f);

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
                break;
            }

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
