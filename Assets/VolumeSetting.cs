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

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
        }
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
