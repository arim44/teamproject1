using Sokoban;
using UnityEngine;

public class StartVRGame : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        StartRetrokoban();
    }

    // ��Ʈ���ڹ� ����!!(VR ���α׷� ����)
    private void StartRetrokoban()
    {
        bool running = true;

        while (running)
        {
            running = gameManager.Process(); //���μ����� end�� �Ǹ� false��ȯ�ؼ� ���Ϲ� ����
        }

        // ��Ʈ���ڹ� ����
        ExitGame();
    }

    private void ExitGame()
    {
        print("���α׷� ����");
        Application.Quit(); //���α׷� ����

        //�����Ϳ� �ڵ�
#if UNITY_EDITOR
        print("���α׷� ����");
        UnityEditor.EditorApplication.isPlaying = false;    //�������� �÷��̹�ư ��
#endif
    }
}
