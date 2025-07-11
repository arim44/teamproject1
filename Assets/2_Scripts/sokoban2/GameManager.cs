using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace RetroSokoban
{
    public class GameManager : MonoBehaviour
    {
        // ��ũ��Ʈ ����
        [SerializeField] UIManager _uiManager;
        [SerializeField] SokobanManager _sokobanManager;
        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private AmbientModeSwitcher _ambientModeSwitcher;
        [SerializeField] private MaterialChanger[] _materialChangers;

        // ���Ӹ�� ����
        private GameMode _gameMode = GameMode.Start;

        // ���� �̵��� ����
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
            // �ʱ� ����
            Setup();
            _gameMode = GameMode.Start; //���Ӹ�带 Start�� ����             
        }

        // ��Ʈ���ڹ� ��ü ���μ���
        public bool Process()
        {
            //���üũ
            switch (_gameMode)
            {
                case GameMode.Start:
                    // ��Ʈ�� ����
                    _uiManager.SetUIMode(_gameMode);
                    // 3�ʵ� ȣ��
                    Invoke(nameof(ProcessStart), 3f);
                    break;
                case GameMode.Main:
                    ProcessMain();
                    /// todo...
                    // VRȭ�鳻 Main ����
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
            return _gameMode != GameMode.Exit; //����Ǹ� false�� ��ȯ�ϰ�
        }

        // ���Ӹ�� ����
        public void SetgameMode(GameMode gameMode)
        {
            //���Ӹ�带 �����Ѵ�
            print($"[{_gameMode}] -> [{gameMode}]");

            //���� ����� UI�� ��������
            //_uIGame.SetUIActive(_gameMode, false);

            _gameMode = gameMode;

            //���� ��忡 �´� ���� ������ ����
            Process();
        }

        private void Setup()
        {
            _uiManager.SetManager(this, _sokobanManager);   //������ �ִ°� �Ѱ��ֱ�
            _sokobanManager.SetScripts(_uiManager, _countdownTimer);
            _countdownTimer.SetManager(_uiManager, _sokobanManager);
        }

        // ���� ����
        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        //��ŸƮ��� ���μ���
        private void ProcessStart()
        {
            // ��ũ�� ����
            FadeAllOut();
            // 3�ʵ� ���θ��� ����
            SetgameMode(GameMode.Main);         //main ���μ��� ����

        }

        // ���� ��� ���μ���
        private void ProcessMain()
        {
            // ���ڹ� �ʱ⼼��
            _sokobanManager?.InitializeSokoban();

            // ��VR, mainUi Ȱ��
            _uiManager.SetUIMode(_gameMode);
        }

        private void ProcessEnd()
        {
            //��Ʈ�� 0�� �Ǹ� vr���ӿ��� UIȰ��
            //��ȹ���� ���ڹ� �������� ��Ʈ ���
        }

        // ���α׷� ����
        private void ProcessExit()
        {
            print("���α׷� ����");
            Application.Quit(); //���α׷� ����

            //�����Ϳ� �ڵ�
#if UNITY_EDITOR
            print("���α׷� ����");
            UnityEditor.EditorApplication.isPlaying = false;    //�������� �÷��̹�ư ��
#endif
        }

        // ��Ʃ����� ����ġ ������
        public void OnLightSwitchOn()
        {
            if (_materialChangers != null)
            {
                FadeAllIn();
            }

            // ��Ƽ�� 4 Ȱ��
            _uiManager.OpenNotice(4);

            // 1���� ��ī�̹ڽ��� ��庯��
            Invoke("SetAmientMode", 1.5f);

            // ���ڹ� �ȭ�� ��Ȱ��
            _uiManager?.OffExitSokoban();
        }

        /// <summary>
        /// ��庯��, ��ī�̹ڽ�
        /// </summary>
        private void SetAmientMode()
        {
            _ambientModeSwitcher.InitializeAmbientMode(true, Color.gray);
        }

        //��ũ�� ���� ���̵���
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


    }   // GameManager Ŭ����
}

