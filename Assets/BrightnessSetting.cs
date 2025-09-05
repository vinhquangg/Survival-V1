using UnityEngine;
using UnityEngine.UI;

public class BrightnessSetting : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image previewImage;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Brightness"))
        {
            LoadBrightness();
        }
        else
        {
            SetBrightness();
        }

        // Khi slider thay đổi thì tự update
        brightnessSlider.onValueChanged.AddListener(delegate { SetBrightness(); });
    }

    public void SetBrightness()
    {
        float value = brightnessSlider.value;
        BrightnessManager.Instance.SetBrightness(value);

        // cập nhật ảnh preview
        if (previewImage != null)
        {
            float brightness = Mathf.Lerp(0.2f, 2f, value);
            previewImage.color = Color.white * brightness;
        }
    }

    public void LoadBrightness()
    {
        float saved = BrightnessManager.Instance.GetBrightness(0.5f);
        brightnessSlider.value = saved;
        BrightnessManager.Instance.SetBrightness(saved);

        if (previewImage != null)
        {
            float brightness = Mathf.Lerp(0.2f, 2f, saved);
            previewImage.color = Color.white * brightness;
        }
    }
}
