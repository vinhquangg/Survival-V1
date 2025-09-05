using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip dropItemSound;
    public AudioClip pickupItemSound;
    public AudioClip noitifySound;
    public AudioClip backgroundSound;
    public AudioClip menuSound;
    public AudioClip equipSound;
    public AudioClip unequipSound;
    public AudioClip craftSound;
    public AudioClip animalDeadSound;
    public AudioClip animalHitSound;
    public AudioClip inventoryOpenSound;

    [Header("Foot Step Player")]
    public AudioSource footstepSource;
    public AudioSource sfxSource;
    public AudioSource backGround;


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
}
