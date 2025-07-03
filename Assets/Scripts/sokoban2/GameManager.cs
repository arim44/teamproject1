using UnityEngine;
namespace Sokoban
{
    public enum GameMode
    {
        Start,
        Lobby,
        Main,
        End,
        //Shop    //상점이나 다른걸 만들면 여기에 모드 추가
    }

    public class GameManager : MonoBehaviour
    {
        private GameMode _gameMode = GameMode.Start;

        public bool Process()
        {
            //모드체크
            switch (_gameMode)
            {
                case GameMode.Start:
                    /// todo...
                    /// 초기세팅
                    /// Setup()
                    /// 게임모드 로비


                    //StartGame();    //게임시작
                    //                //1.5초 뒤에 직업선택
                    //Invoke(nameof(ChoosePlayerType), 1.5f);
                    break;
                case GameMode.Lobby:
                    //ProcessLobby();
                    break;
                case GameMode.Main:
                    // ProcessMain();
                    break;
                case GameMode.End:
                    //게임종료
                    //ExitGame();
                    break;
            }
            return _gameMode != GameMode.End; //종료되면 false를 반환하게
        }

        public void SetgameMode(GameMode gameMode)
        {

        }

    }
}

