using UnityEngine;
// �ΰ� �ڵ�

public class TestSound : MonoBehaviour
{
    public void PlayPushSound() // Ǫ��
    {
        SoundManager.Instance.PlayPush();
    }

    public void PlaySuccessSound() // ���� ����
    {
        SoundManager.Instance.PlaySuccess();
    }

    public void PlayGameOverSound() // ���� ����
    {
        SoundManager.Instance.PlayGameOver();
    }

    // �׽�Ʈ�� ��ư �ݹ�
    public void OnsoundtestbuttonClicked()
    {
        PlayPushSound();
    }


    public void PlayMoveSound() // �÷��̾� �̵��ϴ� ����
    {
        // ��: SoundManager.Instance.PlayMove();
    }

    public void PlayStageProgressSound() // ���� ���� ����
    {
        // ��: SoundManager.Instance.PlayProgress();
    }


}
