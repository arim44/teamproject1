using UnityEngine;
namespace Sokoban
{
    public enum GameMode
    {
        Start,
        Lobby,
        Main,
        End,
        //Shop    //�����̳� �ٸ��� ����� ���⿡ ��� �߰�
    }

    public class GameManager : MonoBehaviour
    {
        private GameMode _gameMode = GameMode.Start;

        public bool Process()
        {
            //���üũ
            switch (_gameMode)
            {
                case GameMode.Start:
                    /// todo...
                    /// �ʱ⼼��
                    /// Setup()
                    /// ���Ӹ�� �κ�


                    //StartGame();    //���ӽ���
                    //                //1.5�� �ڿ� ��������
                    //Invoke(nameof(ChoosePlayerType), 1.5f);
                    break;
                case GameMode.Lobby:
                    //ProcessLobby();
                    break;
                case GameMode.Main:
                    // ProcessMain();
                    break;
                case GameMode.End:
                    //��������
                    //ExitGame();
                    break;
            }
            return _gameMode != GameMode.End; //����Ǹ� false�� ��ȯ�ϰ�
        }

        public void SetgameMode(GameMode gameMode)
        {

        }

    }
}

