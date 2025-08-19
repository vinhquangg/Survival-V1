using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource soundFXPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>Phát âm thanh tại một vị trí trong world</summary>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null || soundFXPrefab == null) return;

        AudioSource source = Instantiate(soundFXPrefab, position, Quaternion.identity);
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    /// <summary>Phát âm thanh gắn vào 1 actor (theo transform)</summary>
    public void PlayAtActor(AudioClip clip, Transform actor, float volume = 1f)
    {
        if (clip == null || actor == null) return;
        PlayAtPosition(clip, actor.position, volume);
    }
    public void StopSound()
    {
        soundFXPrefab.Stop();
    }
}
