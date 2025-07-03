using UnityEngine;
using OHGAR;

public class GameScene : MonoBehaviour
{
    public bool joystickInput = false;
    private Sokoban.Sokoban sokoban;
    public TitleCanvas titleCanvas;
    public Player player;

    /////// ���̽�ƽ�� ���� �������� �νĵǴ°� �ٲٱ�
    // �Է� ���� �ð�
    private float inputTime = 0.5f;
    // ���� �Էµ� ����
    private bool inputState = false;
    //���� �ð�
    private float prevTime = 0;


    private void Awake()
    {
        sokoban = FindFirstObjectByType<Sokoban.Sokoban>();
        titleCanvas = FindFirstObjectByType<TitleCanvas>();
        player = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        // Output ����ƽ�̶� �ؿ���ó�� ���(������ ���� ���� ���ص� ��)
        XRKJController.Output += SetJoyStickValue;  // event�Լ��� +=�� ��������Ʈ�� = �� ��밡��
    }

    private void Update()
    {
        if (!joystickInput)
        {
            KeyboardInput();
        }
    }

    // ���̽�ƽ �Է�
    public void SetJoyStickValue(Vector3 direction)
    {
        // Ÿ��Ʋ ĵ������ ���� �ִٸ� �Լ��� ���� (activeSelf : Ȱ��ȭ�� ����)
        if (titleCanvas != null && titleCanvas.gameObject.activeSelf) return;

        int horizontal = (int)direction.x;
        int vertical = (int)direction.z;

        // �������� ���� �� ���� �� �θ��� �÷��̾� �ٽ� ��������� ��
        if (player == null)
            player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetAnimation(horizontal, vertical);

        //�ѹ� �Է��� ���¶�� �ٽ� �Է¹��� �ð��� �Ǿ��� �� ���۵ǵ��� ó��
        if(inputState)
        {
            float elapsed = Time.time - prevTime;
            if (elapsed > inputTime)
            {
                inputState = false;
            }
            return;
        }

        // �̵�ó��
        // ������ �̵�
        if (vertical == 0 && horizontal == 1)
        {
            inputState = true;
            prevTime = Time.time;

            sokoban?.HandleInput(Direction.Right);
        }
        //�����̵�
        if (vertical == 0 && horizontal == -1)
        {
            inputState = true;
            prevTime = Time.time;

            sokoban?.HandleInput(Direction.Left);
        }
        //���� �̵�
        if (vertical == 1 && horizontal == 0)
        {
            inputState = true;
            prevTime = Time.time;

            sokoban?.HandleInput(Direction.Up);
        }
        //�Ʒ��� �̵�
        if (vertical == -1 && horizontal == 0)
        {
            inputState = true;
            prevTime = Time.time;

            sokoban?.HandleInput(Direction.Down);
        }


        print(direction);
    }

    public void KeyboardInput()
    {
        // Ÿ��Ʋ ĵ������ ���� �ִٸ� �Լ��� ���� (activeSelf : Ȱ��ȭ�� ����)
        if (titleCanvas != null && titleCanvas.gameObject.activeSelf) return;

        // GetAxis -1~1 ����, GetAxisRaw
        //int horizontal = (int)Input.GetAxis("Horizontal");
        //int vertical = (int)Input.GetAxis("Vertical");
        int horizontal = 0;
        int vertical = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) horizontal = -1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) horizontal = 1;
        if (Input.GetKeyDown(KeyCode.UpArrow)) vertical = 1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) vertical = -1;

        // ����, �¿츦 ����Ű�� Ű�� ���ÿ� �Էµǰ� �ִٸ� ���ϸ� ����Ű�� Ű�� ����
        if (vertical != 0 && horizontal != 0) vertical = 0;

        // �ش�Ǵ� �ִϸ��̼� ó��
        // �������� ���� �� ���� �� �θ��� �÷��̾� �ٽ� ��������� ��
        if (player == null)
            player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetAnimation(horizontal, vertical);

        // �̵�ó��
        // ������ �̵�
        if (vertical == 0 && horizontal == 1)
        {
            sokoban?.HandleInput(Direction.Right);
        }
        //�����̵�
        if (vertical == 0 && horizontal == -1)
        {
            sokoban?.HandleInput(Direction.Left);
        }
        //���� �̵�
        if (vertical == 1 && horizontal == 0)
        {
            sokoban?.HandleInput(Direction.Up);
        }
        //�Ʒ��� �̵�
        if (vertical == -1 && horizontal == 0)
        {
            sokoban?.HandleInput(Direction.Down);
        }
    }
}
