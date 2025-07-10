using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip pushSound;
    public AudioClip successSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    void Awake()
    {
        // ΩÃ±€≈Ê ∆–≈œ
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
        audioSource.PlayOneShot(clip);
    }

    public void PlayPush() => PlaySound(pushSound);
    public void PlaySuccess() => PlaySound(successSound);
    public void PlayGameOver() => PlaySound(gameOverSound);
}
