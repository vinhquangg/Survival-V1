using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour,IDamageable
{
    public static PlayerStatus Instance { get; private set; }

    //Player Health
    public StatsData healthData;
    // Player Stamina
    public StatsData hungerData;
    // Player Thirst
    public StatsData thirstData;

    [HideInInspector] public StatInstance health;
    [HideInInspector] public StatInstance hunger;
    [HideInInspector] public StatInstance thirst;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize Stat Instances
        health = new StatInstance(healthData);
        hunger = new StatInstance(hungerData);
        thirst = new StatInstance(thirstData);
    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltTime = Time.deltaTime;

        hunger.Reduce(hunger.data.decayRate * deltTime);
        thirst.Reduce(thirst.data.decayRate * deltTime);

        if(healthData.regenRate>0)
        {
            health.Restore(healthData.regenRate * deltTime);
        }

        if(hunger.IsEmpty() && hungerData.affectsHealth)
        {
            health.Reduce(hungerData.damePerSecondWhenEmpty * deltTime);
        }

        if(thirst.IsEmpty() && thirstData.affectsHealth)
        {
            health.Reduce(thirstData.damePerSecondWhenEmpty * deltTime);
        }

        if(health.IsEmpty())
        {
            Debug.Log("Player is dead!");
        }
    }

    public void TakeDamage(float amount)
    {
        health.Reduce(amount);
        Debug.Log($"Player take damage {amount} .Have {health.currentValue} left");

        if(health.IsEmpty())
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player is deadth");
    }
}
