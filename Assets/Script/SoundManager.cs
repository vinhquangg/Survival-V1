using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    public AudioSource dropItemSound;
    public AudioSource pickupItemSound;
    public AudioSource noitifySound;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayDropSound()
    {
        if(dropItemSound != null && dropItemSound.clip != null)
        {
            dropItemSound.PlayOneShot(dropItemSound.clip);
        }
    }

    public void playPickupItem()
    {
        if (pickupItemSound != null && pickupItemSound.clip != null)
        {
            pickupItemSound.PlayOneShot(pickupItemSound.clip);
        }
    }

    public void PlayNoitify()
    {
        if (noitifySound != null && noitifySound.clip != null)
        {
            noitifySound.PlayOneShot(noitifySound.clip);
        }
    }
    //[Header("Audio Sources (Prefabs)")]
    //[SerializeField] private AudioSource musicSource;
    //[SerializeField] private AudioSource ambientSource;
    //[SerializeField] private AudioSource sfxSource;
    //[SerializeField] private AudioSource uiSource;

    //[Header("Audio Mixer")]
    //public AudioMixer mainMixer; // kéo AudioMixer vào đây trong Inspector
    //public AudioMixerGroup sfxGroup;

    //[Header("Default Clips")]
    //public AudioClip backgroundClip;
    //public AudioClip ambientClip;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else Destroy(gameObject);
    //}

    //private void Start()
    //{
    //    // play music background
    //    if (backgroundClip != null)
    //    {
    //        musicSource.clip = backgroundClip;
    //        musicSource.loop = true;
    //        musicSource.Play();
    //    }

    //    // play ambient loop
    //    if (ambientClip != null)
    //    {
    //        ambientSource.clip = ambientClip;
    //        ambientSource.loop = true;
    //        ambientSource.Play();
    //    }
    //}

    //// 🔊 Play SFX (ngắn)
    //public void PlaySFX(AudioClip clip, float volume = 1f)
    //{
    //    if (clip == null) return;
    //    sfxSource.PlayOneShot(clip, volume);
    //}

    //// 🔊 Play UI sound
    //public void PlayUI(AudioClip clip, float volume = 1f)
    //{
    //    if (clip == null) return;
    //    uiSource.PlayOneShot(clip, volume);
    //}

    //// 🔊 Play SFX tại vị trí
    //public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    //{
    //    if (clip == null) return;
    //    AudioSource source = Instantiate(sfxSource, position, Quaternion.identity);
    //    source.clip = clip;
    //    source.volume = volume;
    //    source.Play();
    //    Destroy(source.gameObject, clip.length);
    //}
}
