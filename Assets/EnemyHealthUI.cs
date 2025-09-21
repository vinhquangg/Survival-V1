using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private BaseMonster monster;
    private Transform followPoint;
    private Camera mainCam;

    public void Setup(BaseMonster monster, Transform healthUIPoint)
    {
        this.monster = monster;
        this.followPoint = healthUIPoint;

        // Lắng nghe event máu
        monster.OnHealthChanged += UpdateHealthUI;

        // Set giá trị ban đầu
        UpdateHealthUI(monster.currentHeal, monster.stats.maxHealth);

        // Cache camera
        mainCam = Camera.main;
    }

    private void UpdateHealthUI(float current, float max)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    private void LateUpdate()
    {
        if (followPoint == null || mainCam == null) return;

        // Luôn đi theo HealthUIPoint
        transform.position = followPoint.position;
        // Quay về phía camera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }

    private void OnDestroy()
    {
        if (monster != null)
            monster.OnHealthChanged -= UpdateHealthUI;
    }
}
