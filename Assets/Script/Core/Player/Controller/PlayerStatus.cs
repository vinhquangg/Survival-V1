using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStatus : MonoBehaviour,IDamageable
{
    public static PlayerStatus Instance { get; private set; }
    [SerializeField] private DamageFeedback damageFeedback;
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

        if (damageFeedback == null)
        {
            damageFeedback = FindObjectOfType<DamageFeedback>();
            damageFeedback?.HideDamage();
        }

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

        if (TemperatureManager.Instance != null)
        {
            ApplyTemperatureEffect(TemperatureManager.Instance.currentTemperature, deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        health.Reduce(amount);
        Debug.Log($"Player take damage {amount} .Have {health.currentValue} left");
        if (damageFeedback != null)
        {
            damageFeedback.ShowDamage();
        }

        if (health.IsEmpty())
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

    private void ApplyTemperatureEffect(float currentTemperature, float deltaTime)
    {
        // TRỜI LẠNH
        if (currentTemperature <= 22) // bạn có thể chỉnh ngưỡng
        {
            // Tăng tốc độ giảm Hunger
            hunger.Reduce(1f * deltaTime);

            // Quá lạnh → giảm Health
            if (currentTemperature <= 5)
            {
                health.Reduce(1f * deltaTime);
            }
        }

        // TRỜI NÓNG
        else if (currentTemperature >= 35)
        {
            // Tăng tốc giảm Thirst
            thirst.Reduce(0.8f * deltaTime);

            // Quá nóng → giảm Health
            if (currentTemperature >= 40)
            {
                health.Reduce(1f * deltaTime);
            }
        }
    }
}
