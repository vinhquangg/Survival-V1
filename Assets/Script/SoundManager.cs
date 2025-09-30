using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip backgroundSound;
    public AudioClip menuSound;
    [Header("Inventory and Interact Sound")]
    public AudioClip dropItemSound;
    public AudioClip noitifySound;
    public AudioClip equipSound;
    public AudioClip unequipSound;
    public AudioClip craftSound;
    public AudioClip pickupItemSound;
    public AudioClip inventoryOpenSound;
    public AudioClip choppingSound;
    public AudioClip drinkingSound;
    [Header("Monster")]
    public AudioClip playerHitSound;
    public AudioClip playerDeadSound;
    [Header("Animal")]
    public AudioClip animalDeadSound;
    public AudioClip animalHitSound;
    public AudioClip animalBearHitSound;
    public AudioClip animalBearAttackSound;
    public AudioClip animalBearClawSound;
    public AudioClip animalBearDeadSound;
    [Header("Monster")]
    public AudioClip swampHitSound;
    public AudioClip swampDeadSound;
    public AudioClip dragonHitSound;
    public AudioClip dragonDeadSound;
    public AudioClip dragonCastspellSound;
    [Header("Foot Step Player")]
    public AudioSource footstepSource;
    public AudioSource sfxSource;
    public AudioSource backGround;
    public float Volume { get; set; } = 0.5f;   // default
    public bool Muted { get; set; } = false;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //public void PlayDropSound()
    //{
    //    if(dropItemSound != null && dropItemSound.clip != null)
    //    {
    //        dropItemSound.PlayOneShot(dropItemSound.clip);
    //    }
    //} 

    //public void playPickupItem()
    //{
    //    if (pickupItemSound != null && pickupItemSound.clip != null)
    //    {
    //        pickupItemSound.PlayOneShot(pickupItemSound.clip);
    //    }
    //}

    //public void PlayNoitify()
    //{
    //    if (noitifySound != null && noitifySound.clip != null)
    //    {
    //        noitifySound.PlayOneShot(noitifySound.clip);
    //    }
    //}

    //public void PlayBackGroundSound()
    //{
    //    if (backgroundSound != null && backgroundSound.clip != null)
    //    {
    //        noitifySound.Play();
    //    }
    //}

    public void PlayFootstep(AudioClip clip, float pitch = 1f)
    {
        if (footstepSource == null) return;

        if (!footstepSource.isPlaying) 
        {
            footstepSource.clip = clip;
            footstepSource.pitch = pitch;
            footstepSource.Play();
        }
    }

    public void StopFootstep()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetVolume(string parameterName, float value)
    {
        mainMixer.SetFloat(parameterName, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(parameterName, value);
    }

    public float GetVolume(string parameterName, float defaultValue = 0.75f)
    {
        return PlayerPrefs.GetFloat(parameterName, defaultValue);
    }

    public void RefreshAllAudioSources()
    {
        float actualVolume = Muted ? 0f : Volume;
        if (backGround != null) backGround.volume = actualVolume;
        if (sfxSource != null) sfxSource.volume = actualVolume;
        if (footstepSource != null) footstepSource.volume = actualVolume;
    }
}
