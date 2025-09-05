using UnityEngine;

public class FootStepManager : MonoBehaviour
{
    public static FootStepManager Instance { get; private set; }
    public TerrainGroundDetector terrainGroundDetector;
    private AudioClip currentClip;
    //private AudioSource footstepSource;
    private Transform actor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //footstepSource = gameObject.AddComponent<AudioSource>();
        //footstepSource.loop = true;
        //footstepSource.spatialBlend = 1f;  // 3D sound
        //footstepSource.volume = 1f;
    }

    public void AttachToActor(Transform actorTransform)
    {
        actor = actorTransform;

        //footstepSource.transform.parent = actor;
        //footstepSource.transform.localPosition = Vector3.zero;
    }

    //public void PlayFootstep(Vector3 position, bool isRunning)
    //{
    //    if (terrainGroundDetector == null || actor == null) return;

    //    GroundData gd = terrainGroundDetector.GetGroundData(position);
    //    if (gd == null || gd.footstepClips.Length == 0) return;

    //    // Chọn clip ngẫu nhiên từ ground hiện tại
    //    AudioClip clip = gd.footstepClips[Random.Range(0, gd.footstepClips.Length)];

    //    // Tính pitch cơ bản theo ground
    //    float basePitch = 1f;
    //    switch (gd.groundType)
    //    {
    //        case GroundType.Sand:
    //            basePitch = 1.2f;
    //            break;
    //        case GroundType.Grass:
    //            basePitch = 1f;
    //            break;
    //            // thêm các loại khác nếu cần
    //    }

    //    //// Nếu chạy → tăng thêm pitch
    //    //footstepSource.pitch = isRunning ? basePitch * 1.3f : basePitch;
    //    //// Chỉ thay clip nếu khác clip hiện tại
    //    //if (clip != currentClip)
    //    //{
    //    //    footstepSource.Stop();
    //    //    footstepSource.clip = clip;
    //    //    footstepSource.Play();
    //    //    currentClip = clip;
    //    //}
    //    //else if (!footstepSource.isPlaying)
    //    //{
    //    //    footstepSource.Play();
    //    //}
    //}

    public void PlayFootstep(Vector3 position, bool isRunning)
    {
        if (terrainGroundDetector == null)
        {
            Debug.LogWarning("❌ FootStepManager: terrainGroundDetector chưa gán!");
            return;
        }
        if (actor == null)
        {
            Debug.LogWarning("❌ FootStepManager: actor chưa attach!");
            return;
        }
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("❌ FootStepManager: SoundManager chưa tồn tại!");
            return;
        }

        GroundData gd = terrainGroundDetector.GetGroundData(position);
        if (gd == null || gd.footstepClips.Length == 0) return;

        AudioClip clip = gd.footstepClips[Random.Range(0, gd.footstepClips.Length)];
        float basePitch = 1f;
        switch (gd.groundType)
        {
            case GroundType.Sand: basePitch = 1.2f; break;
            case GroundType.Grass: basePitch = 1f; break;
        }

        float finalPitch = isRunning ? basePitch * 1.3f : basePitch;
        SoundManager.Instance.PlayFootstep(clip, finalPitch);
    }


    public void StopFootstep()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopFootstep();
    }


    //public void StopFootstep()
    //{
    //    if (footstepSource.isPlaying)
    //        footstepSource.Stop();
    //}
}