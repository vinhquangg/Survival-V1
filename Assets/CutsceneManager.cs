using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;
    public CutsceneSoundHandler soundHandler;
    public string gameplaySceneName = "GameScene"; // tên scene gameplay, set trong Inspector

    private void Start()
    {
        StartCutscene();
    }

    public void StartCutscene()
    {
        if (soundHandler != null)
            soundHandler.OnCutsceneStart();

        director.Play();

        director.stopped += OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        if (soundHandler != null)
            soundHandler.OnCutsceneEnd();

        // Load scene gameplay
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void SkipCutscene()
    {
        director.time = director.duration;
        director.Evaluate(); // đảm bảo timeline chạy tới cuối
        OnCutsceneFinished(director);
    }
}
