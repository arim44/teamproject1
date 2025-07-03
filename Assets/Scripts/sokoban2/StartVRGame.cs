using Sokoban;
using UnityEngine;

public class StartVRGame : MonoBehaviour
{
    //To do..
    // ���ӸŴ������� ������ ���������� ��� ����
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
            running = gameManager.Process(); //���μ����� end�� �Ǹ� false��ȯ�ؼ� ���Ϲ� ����
        }
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
