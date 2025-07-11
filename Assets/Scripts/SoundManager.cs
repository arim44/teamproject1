using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip pushSound;
    public AudioClip successSound;
    public AudioClip gameOverSound;

    // 새로 추가된 사운드
    public AudioClip stageProgressSound;

    private AudioSource audioSource;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("사운드 클립이 비어 있습니다!");
        }
    }

    // 기존 사운드
    public void PlayPush() => PlaySound(pushSound);
    public void PlaySuccess() => PlaySound(successSound);
    public void PlayGameOver() => PlaySound(gameOverSound);

    // 새 사운드

    public void PlayStageProgress() => PlaySound(stageProgressSound);
}
