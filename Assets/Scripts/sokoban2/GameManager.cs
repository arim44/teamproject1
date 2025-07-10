using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace RetroSokoban
{
    public class GameManager : MonoBehaviour
    {
        // 스크립트 연결
        [SerializeField] UIManager _uiManager;
        [SerializeField] SokobanManager _sokobanManager;
        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private AmbientModeSwitcher _ambientModeSwitcher;
        [SerializeField] private MaterialChanger[] _materialChangers;

        // 게임모드 변수
        private GameMode _gameMode = GameMode.Start;

        // 현재 이동할 방향
        public Direction direction = Direction.None;


        private void Awake()
        {
            _uiManager = FindFirstObjectByType<UIManager>();
            _sokobanManager = FindFirstObjectByType<SokobanManager>();
            _countdownTimer = FindFirstObjectByType<CountdownTimer>();
            _ambientModeSwitcher = FindFirstObjectByType<AmbientModeSwitcher>();
            _materialChangers = FindObjectsByType<MaterialChanger>(FindObjectsSortMode.None);
        }

        private void Start()
        {
            // 초기 세팅
            Setup();
            _gameMode = GameMode.Start; //게임모드를 Start로 설정             
        }

        // 레트로코반 전체 프로세스
        public bool Process()
        {
            //모드체크
            switch (_gameMode)
            {
                case GameMode.Start:
                    // 인트로 실행
                    _uiManager.SetUIMode(_gameMode);
                    // 3초뒤 호출
                    Invoke(nameof(ProcessStart), 3f);
                    break;
                case GameMode.Main:
                    ProcessMain();
                    /// todo...
                    // VR화면내 Main 로직
                    break;
                //case GameMode.Sokoban:
                //    // ProcessSokoban();
                //    break;
                case GameMode.End:
                    ProcessEnd();
                    break;
                case GameMode.Exit:
                    ProcessExit();
                    break;
            }
            return _gameMode != GameMode.Exit; //종료되면 false를 반환하게
        }

        // 게임모드 설정
        public void SetgameMode(GameMode gameMode)
        {
            //게임모드를 세팅한다
            print($"[{_gameMode}] -> [{gameMode}]");

            //이전 모드의 UI를 꺼버리고
            //_uIGame.SetUIActive(_gameMode, false);

            _gameMode = gameMode;

            //현재 모드에 맞는 게임 로직을 실행
            Process();
        }

        private void Setup()
        {
            _uiManager.SetManager(this, _sokobanManager);   //가지고 있는거 넘겨주기
            _sokobanManager.SetScripts(_uiManager, _countdownTimer);
            _countdownTimer.SetManager(_uiManager, _sokobanManager);
        }

        // 방향 설정
        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        //스타트모드 프로세스
        private void ProcessStart()
        {
            // 스크린 끄기
            FadeAllOut();
            // 3초뒤 메인모드로 변경
            SetgameMode(GameMode.Main);         //main 프로세스 시작

        }

        // 메인 모드 프로세스
        private void ProcessMain()
        {
            // 소코반 초기세팅
            _sokobanManager?.InitializeSokoban();

            // 인VR, mainUi 활성
            _uiManager.SetUIMode(_gameMode);
        }

        private void ProcessEnd()
        {
            //하트가 0이 되면 vr게임오버 UI활성
            //기획변경 소코반 내에서만 하트 사용
        }

        // 프로그램 종료
        private void ProcessExit()
        {
            print("프로그램 종료");
            Application.Quit(); //프로그램 종료

            //에디터용 코드
#if UNITY_EDITOR
            print("프로그램 종료");
            UnityEditor.EditorApplication.isPlaying = false;    //에디터의 플레이버튼 끔
#endif
        }

        // 스튜디오의 스위치 켰을때
        public void OnLightSwitchOn()
        {
            if (_materialChangers != null)
            {
                FadeAllIn();
            }

            // 노티스 4 활성
            _uiManager.OpenNotice(4);

            // 1초후 스카이박스로 모드변경
            Invoke("SetAmientMode", 1.5f);

            // 소코반 까만화면 비활성
            _uiManager?.OffExitSokoban();
        }

        /// <summary>
        /// 모드변경, 스카이박스
        /// </summary>
        private void SetAmientMode()
        {
            _ambientModeSwitcher.InitializeAmbientMode(true, Color.gray);
        }

        //스크린 전부 페이드인
        public void FadeAllIn()
        {
            foreach (var changer in _materialChangers)
            {
                changer.FadeIn();
            }
        }
        private void FadeAllOut()
        {
            foreach (var changer in _materialChangers)
            {
                changer.FadeOut();
            }
        }


    }   // GameManager 클래스
}

