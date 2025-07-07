using System.Collections.Generic;
using UnityEngine;


namespace RetroSokoban
{
    public class GameManager : MonoBehaviour
    {
        // ��ũ��Ʈ ����
        [SerializeField] UIManager _uiManager;
        [SerializeField] SokobanManager _sokobanManager;
        [SerializeField] private CountdownTimer _countdownTimer;

        // ���Ӹ�� ����
        private GameMode _gameMode = GameMode.Start;

        // ���� �̵��� ����
        public Direction direction = Direction.None;


        private void Awake()
        {
            _uiManager = FindFirstObjectByType<UIManager>();
            _sokobanManager = FindFirstObjectByType<SokobanManager>();
            _countdownTimer = FindFirstObjectByType<CountdownTimer>();
        }

        private void Start()
        {
            // �ʱ� ����
            Setup();
        }

        // ��Ʈ���ڹ� ��ü ���μ���
        public bool Process()
        {
            //���üũ
            switch (_gameMode)
            {
                case GameMode.Start:
                    // ��Ʈ�� �ΰ� �ִϸ��̼� 3���� ��Ȱ�� IEnumerator LoadIntro()
                    // �ƴ� �ٷ� �ΰ� �ִϸ��̼Ǹ� ȣ���ϰ� �ؿ��� ���ʵ�

                    //3�� �ڿ� 
                    //Invoke(nameof(ProcessStart), 3.0f);
                    // ����ȭ��UI
                    ProcessStart();
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
            //// �ΰ�ִϸ��̼� ��Ȱ��

            //// => UI���� ó��
            //// ����ȭ�� UI Ȱ��
            //uiManager.startUIPnl.SetActive(true);


            //// UI�Ŵ������� ���ӽ��� ��ư  true �� 
            //bool IsClickStartButton = uiManager.ClickStartButton();

            //if (IsClickStartButton)
            //{
            //    // ����ȭ�� UI ��Ȱ��
            //    startUI.SetActive(false);
            //    // ���θ�� ����
            //    SetgameMode(GameMode.Main);
            //}
            //

            // ��ŸƮ����? ���ο���?            

            SetgameMode(GameMode.Main);
        }

        // ���� ��� ���μ���
        private void ProcessMain()
        {
            // �ΰ��� ����
            // ���ڹ� �ʱ⼼��
            _sokobanManager?.InitializeSokoban();
            // ���ڹ� ����
            //sokobanManager?.StartSokoban();

            // ���� UIȰ��(��ü ������ ������� ����ĳ��Ʈ ��), ��� ��
            // ���� ��ư Ŭ�� �� ���� ��Ȱ��, �������
            // �ܼ�Ʈ�� �ȳ�(��ġ ǥ�� �Ǵ� �ܼ�Ʈ ��(��ƼŬ �Ǵ� ����Ʈ���� ó��)

            // �ڵ� �ű� �ൿ ó��
            // ��ü ���� ����
            // ���ڹ� ���� ����

        }

        private void ProcessEnd()
        {
            //��Ʈ�� 0�� �Ǹ� vr���ӿ��� UIȰ��
        }

        // �����ư Ŭ�� ��  //SetgameMode(GameMode.Exit);
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

    }   // GameManager Ŭ����
}

