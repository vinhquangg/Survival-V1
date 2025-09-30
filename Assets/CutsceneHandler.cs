using UnityEngine;

public class CutsceneSoundHandler : MonoBehaviour
{
    private float savedVolume;
    private bool savedMuted;

    public void OnCutsceneStart()
    {
        if (SoundManager.Instance != null)
        {
            savedVolume = SoundManager.Instance.Volume;
            savedMuted = SoundManager.Instance.Muted;
            SoundManager.Instance.gameObject.SetActive(false);
        }
    }

    public void OnCutsceneEnd()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.gameObject.SetActive(true);
            SoundManager.Instance.Volume = savedVolume;
            SoundManager.Instance.Muted = savedMuted;
            SoundManager.Instance.RefreshAllAudioSources();
        }
    }
}
