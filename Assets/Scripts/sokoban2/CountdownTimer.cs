using UnityEngine;

namespace RetroSokoban
{
    public class CountdownTimer : MonoBehaviour
    {
        // 매 스테이지마다 정해진 시간 받아서 카운트 다운
        // 카운트 다운 끝나고 실패화면의 다시시작 버튼을 누르면 하트 차감
        // ui매니저의 타이머텍스트에 출력

        [SerializeField] private UIManager _uiManager;
        [SerializeField] private SokobanManager _sokobanManager;

        [SerializeField] private float startTime = 180f;    //초기 카운트다운(초) 3분

        private float currentTime;          // 현재시간
        private bool isCounting = false;    //카운팅여부


        private void Update()
        {
            if (isCounting)
            {
                currentTime -= Time.deltaTime;  // 현재 시간에서 흐르는 시간 뺌

                // 현재시간이 0보다 작으면
                if (currentTime < 0)
                {
                    // 현재 시간 0
                    currentTime = 0;
                    isCounting = false;
                    print("카운트다운 종료");

                    // 타임오버로직
                    _sokobanManager.TimeOver();
                }
                // 시간계속 출력
                UPdateTimerText();
            }
        }

        // 게임매니저에서 넣음
        public void SetManager(UIManager uiManager, SokobanManager sokobanManager)
        {
            _uiManager = uiManager;
            _sokobanManager = sokobanManager;
        }

        public void CountdownInitialized()
        {
            currentTime = startTime;
            isCounting = true;
        }

        private void UPdateTimerText()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);   //현재시간을 60을 나눈 몫
            int seconds = Mathf.FloorToInt(currentTime % 60);   //현재시간을 60으로 나눈 나머지

            _uiManager.SetCountDown(minutes, seconds);
        }
    }
}
