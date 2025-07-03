using Sokoban;
using UnityEngine;

public class StartVRGame : MonoBehaviour
{
    //To do..
    // 게임매니저에서 게임이 끝날때까지 계속 돌림
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        bool running = true;

        while (running)
        {
            running = gameManager.Process(); //프로세스가 end가 되면 false반환해서 와일문 종료
        }
        ExitGame();
    }

    private void ExitGame()
    {
        print("프로그램 종료");
        Application.Quit(); //프로그램 종료

        //에디터용 코드
#if UNITY_EDITOR
        print("프로그램 종료");
        UnityEditor.EditorApplication.isPlaying = false;    //에디터의 플레이버튼 끔
#endif
    }
}
