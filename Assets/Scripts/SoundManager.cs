using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip pushSound;
    public AudioClip successSound;
    public AudioClip gameOverSound;

    // ���� �߰��� ����
    public AudioClip stageProgressSound;

    private AudioSource audioSource;

    void Awake()
    {
        // �̱��� ����
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
            Debug.LogWarning("���� Ŭ���� ��� �ֽ��ϴ�!");
        }
    }

    // ���� ����
    public void PlayPush() => PlaySound(pushSound);
    public void PlaySuccess() => PlaySound(successSound);
    public void PlayGameOver() => PlaySound(gameOverSound);

    // �� ����

    public void PlayStageProgress() => PlaySound(stageProgressSound);
}
