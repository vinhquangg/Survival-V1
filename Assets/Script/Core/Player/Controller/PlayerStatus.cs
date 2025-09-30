using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    public static PlayerStatus Instance { get; private set; }

    [SerializeField] private DamageFeedback damageFeedback;
    private PlayerController playerControl;

    // ✅ Lưu mốc HP (theo bội số 10)
    private int lastHealthCheckpoint;

    public StatsData healthData;
    public StatsData hungerData;
    public StatsData thirstData;

    [HideInInspector] public StatInstance health;
    [HideInInspector] public StatInstance hunger;
    [HideInInspector] public StatInstance thirst;

    private void Awake()
    {
        Instance = this;
        health = new StatInstance(healthData);
        hunger = new StatInstance(hungerData);
        thirst = new StatInstance(thirstData);

        // Khởi tạo checkpoint ở bội số 10 gần nhất
        lastHealthCheckpoint = Mathf.FloorToInt(health.currentValue / 10f) * 10;
    }
    void Start()
    {
        playerControl = GetComponent<PlayerController>();
        if (damageFeedback == null)
        {
            StartCoroutine(FindFeedbackNextFrame());
        }
    }
    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        // Giảm Hunger/Thirst theo thời gian
        hunger.UpdateStat(deltaTime);
        thirst.UpdateStat(deltaTime);
        health.UpdateStat(deltaTime);

        ApplyHungerThirstDamage(deltaTime);

        if (health.IsEmpty())
            Die();
    }

    // ✅ Damage do Hunger/Thirst
    private void ApplyHungerThirstDamage(float deltaTime)
    {
        bool tookEnvDamage = false;
        float damageAmount = 0f;

        // Nếu hunger cạn
        if (hunger.currentValue <= 0f)
        {
            damageAmount += 2f * deltaTime; // ví dụ
            tookEnvDamage = true;
        }

        // Nếu thirst cạn
        if (thirst.currentValue <= 0f)
        {
            damageAmount += 2f * deltaTime; // ví dụ
            tookEnvDamage = true;
        }

        if (tookEnvDamage && damageAmount > 0f)
        {
            health.Reduce(damageAmount);

            // ✅ Check xem có vượt qua mốc 10 không
            int currentCheckpoint = Mathf.FloorToInt(health.currentValue / 10f) * 10;
            if (currentCheckpoint < lastHealthCheckpoint)
            {
                lastHealthCheckpoint = currentCheckpoint;

                if (damageFeedback != null)
                    damageFeedback.ShowDamage(); // nháy 1s

                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.playerHitSound);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        // Damage khác (enemy đánh chẳng hạn)
        health.Reduce(amount);
        if (damageFeedback != null)
            damageFeedback.ShowDamage();
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.playerHitSound);
        if (health.IsEmpty())
            Die();
    }

    private void Die()
    {
        if (playerControl.animationController.IsDead) return;
        playerControl.playerStateMachine.ChangeState(
            new DeadState(playerControl.playerStateMachine, playerControl)
        );
    }

    private IEnumerator FindFeedbackNextFrame()
    {
        yield return null; // chờ 1 frame để UI kịp spawn
        damageFeedback = FindObjectOfType<DamageFeedback>();

        if (damageFeedback != null)
            damageFeedback.HideDamage();
        else
            Debug.LogWarning("DamageFeedback not found in scene!");
    }
}
