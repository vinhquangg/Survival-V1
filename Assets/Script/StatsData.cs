using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Stat Data", fileName = "New Data")]
public class StatsData : ScriptableObject
{
    [Header("Basic Stats")]
    public string statName;
    public float maxValue;
    public float regenRate;
    public float decayRate;
    public float currentValue;
    [Header("Effect if equal 0")]
    public bool affectsHealth = false;
    public float damePerSecondWhenEmpty = 0f;
}
