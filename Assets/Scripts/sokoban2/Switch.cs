using RetroSokoban;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>(FindObjectsInactive.Include);
    }

    // 스위치 레버 움직임
    public void SetLeverRotation(float xRotation)
    {
        // 현재 회전값 가져옴
        Vector3 newRotation = transform.eulerAngles;
        // 로테이션 x 값 설정
        newRotation.x = xRotation;

        // 새 회전 적용
        transform.eulerAngles = newRotation;

        //회전값에 따른 디버그 출력
        if (xRotation == 30f) Debug.Log("스위치 꺼짐");
        else if (xRotation == -30f) Debug.Log("스위치 켜짐");

        _gameManager.OnLightSwitchOn();
    }
}
