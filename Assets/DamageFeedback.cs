using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFeedback : MonoBehaviour
{
    [Header("UI Overlay")]
    [SerializeField] private Image damageOverlay;
    [SerializeField] private Color damageColor = new Color(1, 0, 0, 0.35f);
    [SerializeField] private float fadeSpeed = 3f;

    private Color clearColor = new Color(1, 0, 0, 0);
    private Coroutine currentRoutine;
    private void Update()
    {
        if (damageOverlay != null && damageOverlay.color.a > 0)
        {
            damageOverlay.color = Color.Lerp(damageOverlay.color, clearColor, fadeSpeed * Time.deltaTime);
        }
    }

    public void ShowDamage()
    {
        if (damageOverlay == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FlashDamage());
    }

    private IEnumerator FlashDamage()
    {
        // Bật ngay overlay
        damageOverlay.color = damageColor;

        // Giữ nguyên màu trong 1 giây
        yield return new WaitForSeconds(1f);

        // Sau đó giảm alpha về 0 theo fadeSpeed
        while (damageOverlay.color.a > 0.01f)
        {
            damageOverlay.color = Color.Lerp(damageOverlay.color, clearColor, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        damageOverlay.color = clearColor;
        currentRoutine = null;
    }

    public void HideDamage()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        damageOverlay.color = clearColor;
    }
}
