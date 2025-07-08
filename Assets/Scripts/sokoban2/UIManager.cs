using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroSokoban
{
    public class UIManager : MonoBehaviour
    {
        [Header("Util")]
        [SerializeField] TMP_Text txtNotice;    //알림 텍스트(공지)
        [SerializeField] GameObject objStart;   //스타트UI 오브젝트(게임모드가 Start일때 켠다)
        [SerializeField] GameObject objMain;    //메인UI 오브젝트(게임모드가 Main일때 켠다)
        [SerializeField] GameObject objSokoban; //소코반UI 오브젝트(게임모드가 고민중 일때 켠다)

        [Header("Start")]
        // 인트로 1
        [SerializeField] GameObject objIntro;   //인트로패널 => 필요없으면 지우기

        [Header("Main")]
        // 메뉴, 공지, 시작버튼
        [SerializeField] Button btnStartRetrokoban;      //레트로코반 시작버튼

        [Header("Sokoban")]
        // 소코반내 카운트 다운, 하트이미지, 시작버튼
        [SerializeField] private TitleCanvas titleCanvas;
        [SerializeField] TMP_Text txtCountDown;     //소코반내 카운트다운 텍스트
        [SerializeField] Button btnStartSokoban;    //소코반 시작버튼
        [SerializeField] GameObject[] hpHearts;      // 하트

        private GameManager _gameManager;       // 게임매니저
        private SokobanManager _skobanManager;  // 소코반 매니저


        private void Awake()
        {
            titleCanvas = FindAnyObjectByType<TitleCanvas>(FindObjectsInactive.Include);
        }

        // 스크립트 가져오기
        public void SetManager(GameManager gameManager, SokobanManager skobanManager)
        {
            _gameManager = gameManager;
            _skobanManager = skobanManager;
        }

        /// <summary>
        /// UI로 공지 출력
        /// </summary>
        /// <param name="message"></param>
        public void SetNotice(string message)
        {
            txtNotice.text = message;
        }
        /// <summary>
        /// 소코반 내 카운트다운 텍스트 세팅
        /// </summary>
        public void SetCountDown(int minutes, int seconds)
        {
            txtCountDown.text = $"{minutes} : {seconds}";
            //txtCountDown.text = string.Format("{{0:D2}:{1:D2}", minutes, seconds);
        }
        /// <summary>
        /// 게임모드에 따라 활성화
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="value"></param>
        public void SetUIActive(GameMode mode, bool value)
        {
            //받은 모드에 따라서 활성화
            switch (mode)
            {
                case GameMode.Start:
                    objStart.SetActive(value);
                    break;

                case GameMode.Main:
                    objMain.SetActive(value);
                    break;

                case GameMode.Sokoban:
                    objSokoban.SetActive(value);
                    break;
            }
        }

        /// <summary>
        /// 게임모드에 따라 UI모드를 변경한다
        /// </summary>
        /// <param name="mode"></param>
        public void SetUIMode(GameMode mode)
        {
            //처음에 다 끄고 모드에 맞을때만 켜기
            SetUIActive(GameMode.Start, false);
            SetUIActive(GameMode.Main, false);
            SetUIActive(GameMode.Sokoban, false);


            //받은 모드에 따라서 배경 바꿈
            switch (mode)
            {
                case GameMode.Start:
                    Debug.Log("SetUIMode :Start");
                    SetUIActive(GameMode.Start, true);
                    break;

                case GameMode.Main:
                    Debug.Log("SetUIMode :Lobby");
                    SetUIActive(GameMode.Main, true);
                    break;

                case GameMode.Sokoban:
                    SetUIActive(GameMode.Sokoban, true);
                    break;
            }
        }

        // 배열에 넣은 하트 번호받아서 끄기
        public void HideHeart(int heartNumber)
        {
            hpHearts[heartNumber].SetActive(false);
        }

        // 버튼 관련 기능

        // 인게임 UI에서 뒤로가기 버튼을 클릭
        public void OnBackClicked()
        {
            titleCanvas?.SetActive(true);
            //gameObject.SetActive(false);            
        }

        // 인게임 타이틀내 시작버튼 클릭
        public void OnPlayClicked()
        {
            titleCanvas?.SetActive(false);
        }


    }
}
