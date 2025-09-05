using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStatus : MonoBehaviour,IDamageable
{
    public static PlayerStatus Instance { get; private set; }
    private PlayerController playerControl;

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

        health = new StatInstance(healthData);
        healthData.currentValue = health.currentValue;

        hunger = new StatInstance(hungerData);
        hungerData.currentValue = hunger.currentValue;

        thirst = new StatInstance(thirstData);
        thirstData.currentValue = thirst.currentValue;


    }

    // Start is called before the first frame update
    void Start()
    {
        playerControl = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        hunger.UpdateStat(deltaTime);
        thirst.UpdateStat(deltaTime);
        health.UpdateStat(deltaTime);

        //if (health.IsEmpty())
        //{
        //    Die();
        //}
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
        if (playerControl.animationController.IsDead) return;

        playerControl.playerStateMachine.ChangeState(
            new DeadState(playerControl.playerStateMachine, playerControl)
        );
    }
}
