using System.Collections.Generic;
using UnityEngine;


namespace RetroSokoban
{
    public class GameManager : MonoBehaviour
    {
        // ���Ӹ�� ����
        private GameMode _gameMode = GameMode.Start;

        // ���� �̵��� ����
        public Direction direction = Direction.None;

        private SokobanManager sokobanManager;


        private void Awake()
        {
            sokobanManager = FindFirstObjectByType<SokobanManager>();
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
            }
            return _gameMode != GameMode.End; //����Ǹ� false�� ��ȯ�ϰ�
        }

        // ���Ӹ�� ����
        public void SetgameMode(GameMode gameMode)
        {
            _gameMode = gameMode;
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
            //    _gameMode = GameMode.Main;
            //}
            //

            // ��ŸƮ����? ���ο���?
            // ���ڹ� �ʱ⼼��    
            sokobanManager.InitializeSokoban();
        }

        // ���� ��� ���μ���
        private void ProcessMain()
        {
            // �ΰ��� ����

            // ���ڹ� ����
            sokobanManager.StartSokoban();

            // ���� UIȰ��(��ü ������ ������� ����ĳ��Ʈ ��), ��� ��
            // ���� ��ư Ŭ�� �� ���� ��Ȱ��, �������
            // �ܼ�Ʈ�� �ȳ�(��ġ ǥ�� �Ǵ� �ܼ�Ʈ ��(��ƼŬ �Ǵ� ����Ʈ���� ó��)

            // �ڵ� �ű� �ൿ ó��
            // ��ü ���� ����
            // ���ڹ� ���� ����

            // �����ư Ŭ�� ��
            // �ص��� ����
            //    _gameMode = GameMode.End;

        }
        
    }   // GameManager Ŭ����
}

