using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    [SerializeField] private Volume globalVolume;   // gán Global Volume trong Inspector
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (globalVolume != null && globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            colorAdjustments = ca;
        }
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            // Map slider 0–1 -> EV -2 đến +2
            float ev = Mathf.Lerp(-2f, 2f, value);
            colorAdjustments.postExposure.Override(ev);

            PlayerPrefs.SetFloat("Brightness", value);
            PlayerPrefs.Save();
        }
    }

    public float GetBrightness(float defaultValue = 0.5f)
    {
        return PlayerPrefs.GetFloat("Brightness", defaultValue);
    }
}
