using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("Master"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
        }

        // SFX
        if (PlayerPrefs.HasKey("SFX"))
            LoadSFXVolume();
        else
            SetSFXVolume();
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        SoundManager.Instance.SetVolume("Master", volume);
    }

    public void LoadVolume()
    {
        float savedVolume = SoundManager.Instance.GetVolume("Master", 0.75f);
        masterSlider.value = savedVolume;
        SoundManager.Instance.SetVolume("Master", savedVolume);
    }
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        SoundManager.Instance.SetVolume("SFX", volume);
    }

    public void LoadSFXVolume()
    {
        float savedVolume = SoundManager.Instance.GetVolume("SFX", 0.75f);
        sfxSlider.value = savedVolume;
        SoundManager.Instance.SetVolume("SFX", savedVolume);
    }
}
