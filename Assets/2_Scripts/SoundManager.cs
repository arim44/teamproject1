using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public GameSfxType _gameSfxType;

    public AudioClip backSound;
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

    private void PlaySfx(AudioClip clip)
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

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("���� Ŭ���� ��� �ֽ��ϴ�!");
        }
    }

    public void StopSound()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }        
    }


    // �������
    public void PlayBackSound() => PlaySound(backSound);
    // Ǫ�� ����
    public void PlayPush() => PlaySfx(pushSound);
    //���� Ŭ���� ����
    public void PlaySuccess() => PlaySfx(successSound);
    //���ӿ��� ����
    public void PlayGameOver() => PlaySfx(gameOverSound);
    // �������������޴� ����
    public void PlayNextStage() => PlaySfx(stageProgressSound);



    public void SetSfxType(GameSfxType sfx)
    {
        _gameSfxType = sfx;
    }

    public void PlaySfx()
    {
        switch(_gameSfxType)
        {
            case GameSfxType.SokobanSolved:
                PlayNextStage();
                break;
            case GameSfxType.GameClear:
                PlaySuccess();
                break;
            case GameSfxType.GameOver:
                PlayGameOver();
                break;
        }
    }
}
