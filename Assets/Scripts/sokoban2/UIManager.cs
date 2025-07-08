using ArcadeMiniGames;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

namespace RetroSokoban
{
    public class UIManager : MonoBehaviour
    {
        [Header("Util")]
        [SerializeField] private TMP_Text txtNotice;    //알림 텍스트(공지)
        [SerializeField] private GameObject objStart;   //스타트UI 오브젝트(게임모드가 Start일때 켠다)
        [SerializeField] private GameObject objMain;    //메인UI 오브젝트(게임모드가 Main일때 켠다)
        [SerializeField] private GameObject objSokoban; //소코반UI 오브젝트(게임모드가 고민중 일때 켠다)

        [Header("Start")]
        // 인트로 1
        [SerializeField] private GameObject objIntro;   //인트로패널 => 필요없으면 지우기

        [Header("Main")]
        // 메뉴, 공지, 시작버튼
        [SerializeField] private Button btnStartRetrokoban;      //레트로코반 시작버튼
        [SerializeField] private GameObject pnlStartMenu;        //레트로코반 메뉴 화면
        [SerializeField] private GameObject[] pnlNotice;         //공지 화면들
        [SerializeField] private GameObject pnlRetrokobanClaer;  //레트로코반클리어 화면

        [Header("Sokoban")]
        // 소코반내 카운트 다운, 하트이미지, 시작버튼
        [SerializeField] private TitleCanvas titleCanvas;   //타이틀 캔버스
        [SerializeField] private TMP_Text txtCountDown;     //소코반내 카운트다운 텍스트
        [SerializeField] private GameObject[] hpHearts;     // 하트

        //타임오버 화면, 게임오버 화면, 클리어 화면, 종료화면(검정)
        [SerializeField] private GameObject pnlTimeOver;    //타임오버 화면
        [SerializeField] private GameObject pnlGameOver;    //게임오버 화면
        [SerializeField] private GameObject pnlGameClear;   //게임클리어 화면
        [SerializeField] private GameObject pnlGameExit;    //게임종료 화면

        //시작버튼, 메뉴버튼, 리플레이버튼, 넥스트 버튼
        [SerializeField] private Button btnStartSokoban;    //소코반 시작버튼
        [SerializeField] private Button btnMenuSokoban;     //소코반 메뉴버튼
        [SerializeField] private Button btnReplaySokoban;   //소코반 리플레이 버튼
        [SerializeField] private Button btnNextstageSokoban;     //소코반 다음스테이지 버튼


        private GameManager _gameManager;       // 게임매니저
        private SokobanManager _skobanManager;  // 소코반 매니저

        private int pnlNoticeCount = 0;


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
                    Debug.Log("SetUIMode :Main");
                    SetUIActive(GameMode.Main, true);
                    break;

                case GameMode.Sokoban:
                    Debug.Log("SetUIMode :Sokoban");
                    SetUIActive(GameMode.Sokoban, true);
                    break;
            }
        }

        // 배열에 넣은 하트 번호받아서 끄기
        public void HideHeart(int heartNumber)
        {
            // 비활성만 한거라 파괴한게 아니어서 널이 될리 없음.... 다시 생각
            if (hpHearts != null)
            {
                //hpHearts[heartNumber].SetActive(false);
                Destroy(hpHearts[heartNumber]);
            }
            else Debug.LogWarning("하트가 없습니다");
        }

        // 타이틀 캔버스
        public void SetActive(bool active)
        {
            titleCanvas?.SetActive(active);
            Invoke(nameof(InvokeShowNotice), 1.5f);
        }

        private void InvokeShowNotice()
        {
            pnlNotice[0].SetActive(true);
        }

        //타임오버시ui 켜기
        public void SetTimeOverActive()
        {
            pnlTimeOver.SetActive(true);
        }


        ////////////// 버튼 관련 기능 //////////////
        
        // 메인내 버튼
        // 레트로코반 시작 버튼
        public void OnStartRetrokobanClicked()
        {
            pnlStartMenu.SetActive(false);
            OpenNotice(0);
        }

        private void OpenNotice(int number)
        {
            pnlNotice[number].SetActive(true);
        }

        // next 버튼 클릭
        public void OnNextButtonClicked()
        {
            // 현재 공지 끄기
            pnlNotice[pnlNoticeCount].SetActive(false);
            pnlNoticeCount += 1;
            //다음 공지 켜기
            OpenNotice(pnlNoticeCount);
        }

        public void OnCloseNoticeClicked()
        {
            pnlNotice[pnlNoticeCount].SetActive(false);
            pnlNoticeCount += 1;

            //// pnlNotice 안에 있는 공지 다끔 => 적용이 안됨
            //for (int i = 0; i < pnlNoticeCount; i++)
            //{
            //    pnlNotice[i].SetActive(false);
            //}            
        }
       
        // 소코반 게임내 버튼
        // 인게임 UI에서 뒤로가기 버튼을 클릭
        public void OnMenuClicked()
        {
            // 타이틀캔버스 활성
            titleCanvas?.SetActive(true);
            // 스테이지 초기화
            _skobanManager.GameReset();
        }

        // 인게임 타이틀내 시작버튼 클릭
        public void OnPlayClicked()
        {
            // 스테이지 초기화
            _skobanManager.GameReset();
            // 타이틀캔버스 비활성
            titleCanvas?.SetActive(false);
        }

        // 리플레이 버튼 클릭
        public void OnRePlayClicked()
        {    
            if(hpHearts != null)
            {
                // 하트차감, 스테이지 리셋, 타이머리셋
                _skobanManager.StageReset();
                pnlTimeOver.SetActive(false);
            }
            else
            {
                // 하트배열에 게임오버화면
                pnlGameOver.SetActive(true);
            }
        }


    }
}
