using UnityEngine;
// �ΰ� �ڵ�

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

    // �׽�Ʈ�� ��ư �ݹ�
    public void OnsoundtestbuttonClicked()
    {
        PlayPushSound();
    }
}
