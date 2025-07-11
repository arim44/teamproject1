using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public GameSfxType _gameSfxType;

    public AudioClip backSound;
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

    private void PlaySfx(AudioClip clip)
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

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("사운드 클립이 비어 있습니다!");
        }
    }

    public void StopSound()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }        
    }


    // 배경음악
    public void PlayBackSound() => PlaySound(backSound);
    // 푸쉬 사운드
    public void PlayPush() => PlaySfx(pushSound);
    //게임 클리어 사운드
    public void PlaySuccess() => PlaySfx(successSound);
    //게임오버 사운드
    public void PlayGameOver() => PlaySfx(gameOverSound);
    // 다음스테이지메뉴 사운드
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
