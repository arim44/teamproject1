using RetroSokoban;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>(FindObjectsInactive.Include);
    }

    // ����ġ ���� ������
    public void SetLeverRotation(float xRotation)
    {
        // ���� ȸ���� ������
        Vector3 newRotation = transform.eulerAngles;
        // �����̼� x �� ����
        newRotation.x = xRotation;

        // �� ȸ�� ����
        transform.eulerAngles = newRotation;

        //ȸ������ ���� ����� ���
        if (xRotation == 30f) Debug.Log("����ġ ����");
        else if (xRotation == -30f) Debug.Log("����ġ ����");

        _gameManager.OnLightSwitchOn();
    }
}
