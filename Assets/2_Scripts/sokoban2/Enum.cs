
public enum GameMode
{
    Start,
    //Lobby,
    Main,
    Sokoban,
    End,
    Exit
}

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down
}

public enum GameSfxType
{
    Start,              // 시작
    SokobanSolved,      //단계 클리어
    GameClear,          //게임 클리어
    GameOver            //게임 오버
}