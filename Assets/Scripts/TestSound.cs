using UnityEngine;
// 인공 코드

public class TestSound : MonoBehaviour
{
    public void PlayPushSound() // 푸쉬
    {
        SoundManager.Instance.PlayPush();
    }

    public void PlaySuccessSound() // 게임 성공
    {
        SoundManager.Instance.PlaySuccess();
    }

    public void PlayGameOverSound() // 게임 실패
    {
        SoundManager.Instance.PlayGameOver();
    }

    // 테스트용 버튼 콜백
    public void OnsoundtestbuttonClicked()
    {
        PlayPushSound();
    }


    public void PlayMoveSound() // 플레이어 이동하는 사운드
    {
        // 예: SoundManager.Instance.PlayMove();
    }

    public void PlayStageProgressSound() // 게임 진행 사운드
    {
        // 예: SoundManager.Instance.PlayProgress();
    }


}
