using UnityEngine;
// 인공 코드

public class TestSound : MonoBehaviour
{
    public void PlayPushSound()
    {
        SoundManager.Instance.PlayPush();
    }

    public void PlaySuccessSound()
    {
        SoundManager.Instance.PlaySuccess();
    }

    public void PlayGameOverSound()
    {
        SoundManager.Instance.PlayGameOver();
    }

    // 테스트용 버튼 콜백
    public void OnsoundtestbuttonClicked()
    {
        PlayPushSound();
    }
}
