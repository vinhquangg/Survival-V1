using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    public AudioSource dropItemSound;
    public AudioSource pickupItemSound;
    public AudioSource noitifySound;
    public AudioSource backgroundSound;
    public AudioSource menuSound;
    public AudioSource equipSound;
    public AudioSource unequipSound;
    public AudioSource craftSound;
    public AudioSource animalDeadSound;
    public AudioSource animalHitSound;
    public AudioSource inventoryOpenSound;

    [Header("Foot Step Player")]
    public AudioSource footstepSource;


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

    public void PlayBackGroundSound()
    {
        if (backgroundSound != null && backgroundSound.clip != null)
        {
            noitifySound.Play();
        }
    }

    public void PlayFootstep(AudioClip clip, float pitch = 1f)
    {
        if (footstepSource == null) return;

        if (!footstepSource.isPlaying) // <--- chỉ play nếu đang rảnh
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


}
