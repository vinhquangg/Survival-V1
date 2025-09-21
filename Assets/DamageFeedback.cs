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

    private void Update()
    {
        if (damageOverlay != null && damageOverlay.color.a > 0)
        {
            damageOverlay.color = Color.Lerp(damageOverlay.color, clearColor, fadeSpeed * Time.deltaTime);
        }
    }

    public void ShowDamage()
    {
        if (damageOverlay != null)
        {
            damageOverlay.color = damageColor;
        }
    }
    public void HideDamage() 
    {
        if (damageOverlay != null)
        {
            damageOverlay.color = clearColor;
        }
    }
}
