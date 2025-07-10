using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace RetroSokoban
{
    public class SokobanManager : MonoBehaviour
    {
        //����ִ� ��Ҹ� ����Ű�� �ĺ���
        private const int Empty = 0;
        //���� ����Ű�� �ĺ� ��
        private const int Wall = 1;
        //�������� �� ���
        private const int Goal = 2;
        //�̵���ų �ڽ�
        private const int Box = 3;
        //�÷��̾��� �ĺ� ��
        private const int Player = 4;

        // ��ũ��Ʈ ����
        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HeartHealth _heartHealth;

        /// ====== ī�޶� ���� ======
        // ī�޶�
        private Camera gameCamera;
        // subī�޶� ���ӿ�����Ʈ
        private GameObject subCamera;

        // ī�޶� ��ġ ����Ʈ
        [SerializeField]
        private List<Vector2> cameraPositions = new List<Vector2>();

        /// ====== �÷��̾� ���� ======
        //�÷��̾�
        private Player player;
        // �÷��̾� ��ġ
        private Position playerPosition = new Position();

        /// ====== ������ ���� ======
        //���̽� ���Ͽ��� ������ ����
        private int[,]? currentBoard = null;
        //private Slot[,] currentBoard = null;

        // ������ ��������Ʈ ����Ʈ
        private SpriteRenderer[,] sprites = null;

        // ���̽� ���Ͽ��� �޾ƿ� ����
        private List<Sokoban_StageData> stages;

        /// ====== �������� ���� ======
        //��ü �������� ��
        private int totalStageCount = 0;

        //���� ��������
        [SerializeField] private int currentStage = 1;
        [SerializeField] private int clearStage = 3;    //클리어 스테이지

        // �������� ������Ʈ
        private GameObject stageGameObject;

        // ���� ���������� �������� ���� ���
        private List<Position> goalPositions = new List<Position>(0);

        // ������ ���������� ��� ����
        private int width;
        private int height;


        private void Awake()
        {
            // subCamera �±װ� �����ؼ� ã�ƿ���
            subCamera = GameObject.FindGameObjectWithTag("SubCamera");
            _heartHealth = FindAnyObjectByType<HeartHealth>(FindObjectsInactive.Include);
        }

        // ��ũ��Ʈ ��������, ���ӸŴ������� ����
        public void SetScripts(UIManager uiManager, CountdownTimer countdownTimer)
        {
            _uiManager = uiManager;
            _countdownTimer = countdownTimer;
        }


        // ���ڹ� �ʱ⼼��
        public void InitializeSokoban()
        {
            // �̰� ���ڹ� ��忡��???
            if (subCamera != null) gameCamera = subCamera.GetComponent<Camera>();

            // ��ü ���������� �ε�
            stages = LoadJsonDate();
            // �������� ������ �Ҵ�
            SetStages(stages);

            //��Ʈ �ʱ�ȭ
            HeartReset();

            // ���� ���������� ����(�ؿ� �ִ°� ���� �ø�) ���۹�ư ������ �������� ����
            //Setupstage(currentStage - 1);
        }

        // ���ڹ� ����
        public void StartSokoban()
        {
            // ���� ���������� ����
            Setupstage(currentStage - 1);
        }


        /// <summary>
        /// Json���� ������ �ε�
        /// </summary>
        private List<Sokoban_StageData> LoadJsonDate()
        {
            // Ȯ���� ���� ���� �̸��� �ᵵ �� json���� ��������
            TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
            stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);

            return stages;
        }

        /// <summary>
        /// �������� ������ �Ҵ��ϱ� (���̽����� ������ ȣ��)
        /// </summary>
        /// <param name="stages"></param>
        public void SetStages(List<Sokoban_StageData> stages)
        {
            this.stages = stages;
            totalStageCount = stages.Count;
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        /// <param name="stage"></param>
        public void Setupstage(int stage)
        {
            // �������� ����
            CreateStage(stage);

            // ��������Ʈ ����
            SetSprits(stageGameObject);

            // ī�޶� ��ġ ����
            SetCameraPosition(stage);

            // ���� ���� ����
            SetCurrentBoard(stage);

            // Goal ��ġ ������������ �������� ������ ������ ã��
            FindGoalPositions();

            // �÷��̾� ��ġ ����
            SetPlayerPosition();

            // ī��Ʈ�ٿ� �����
            CountdownReset();
        }

        /// <summary>
        /// �������� ���� �� ��������Ʈ ����
        /// </summary>
        /// <param name="stage"></param>
        private void CreateStage(int stage)
        {
            // ���� ���������� �����ִٸ� ����
            if (stageGameObject != null) Destroy(stageGameObject);

            //���� �������� �̸��� ����
            string stageText = $"Stage{stage + 1}";
            // ���ҽ��� �ε�
            var stageObj = Resources.Load<GameObject>($"Prefabs/Stages/{stageText}");
            if (stageObj == null) return;

            // ������ ���������� ��� ���� ���� ����(�迭�� ũ�Ⱚ�� ���ϰ�)
            height = stages[stage].Map.GetLength(0);
            width = stages[stage].Map.GetLength(1);
            // ��������Ʈ �迭�� ����
            sprites = new SpriteRenderer[height, width];

            // �������� ����
            stageGameObject = Instantiate(stageObj, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// ������ ���������� �޾Ƽ� ��������Ʈ ����
        /// </summary>
        private void SetSprits(GameObject stageGameObject)
        {
            // �������� �θ� ������Ʈ�� Ʈ������
            Transform rootTransform = stageGameObject.transform;

            // rootTransform ������Ʈ �ؿ� �ִ� ���������� ����ŭ �ݺ�
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                // ���������� ������� child�� ���� 
                Transform child = rootTransform.GetChild(i);
                var rc = child.name.Split(",");
                //  rc name �Է¹���
                int.TryParse(rc[0], out int row);
                int.TryParse(rc[1], out int column);

                // ��������Ʈ �迭�� ��Ҹ� ä��(row,column ��ġ�� ���� ��������Ʈ ����)
                sprites[row, column] = child.GetComponent<SpriteRenderer>();
            }
        }


        /// <summary>
        /// ī�޶� ��ġ ����
        /// </summary>
        private void SetCameraPosition(int stage)
        {
            // ī�޶� ���� ī�޶� �������� ���� 0���� ũ��
            if (gameCamera != null && cameraPositions.Count > 0)
            {
                try
                {
                    Vector3 position = cameraPositions[stage];
                    position.z = gameCamera.transform.position.z;
                    // cameraPositions List�� <Position>�� �ƴ� <Vector2>�� �س����� ��밡��
                    gameCamera.transform.position = position;
                }
                catch( Exception e )
                {
                    print(e);
                }
            }
        }

        /// <summary>
        /// currentBoard(���� ����) ����
        /// </summary>
        private void SetCurrentBoard(int stage)
        {
            //���� ���忡 �迭�� �Ҵ�
            currentBoard = new int[height, width];

            //�������� �����͸� CurrtentBoard��������
            Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);
        }

        /// <summary>
        /// �����ǿ��� Goal�� ��ġã�Ƽ� ����Ʈ�� ����
        /// </summary>
        private void FindGoalPositions()
        {
            // ������� ������ ����
            goalPositions.Clear();

            if (currentBoard == null) return;

            //������ ��ȸ
            for (int r = 0; r < currentBoard.GetLength(0); r++)
            {
                for (int c = 0; c < currentBoard.GetLength(1); c++)
                {
                    if (currentBoard[r, c] == Goal)
                    {
                        //�������� ���� �� �ִ� ������ ��� ��ġ�� ����
                        // �޸𸮸� �Ҵ��ϸ鼭 ����
                        Position position = new Position { X = c, Y = r };
                        goalPositions.Add(position);
                    }
                }
            }
        }

        /// <summary>
        /// �÷��̾� ��ġ ����
        /// </summary>
        private void SetPlayerPosition()
        {
            // Player ��ũ��Ʈ ã�ƿ���
            player = FindFirstObjectByType<Player>();

            // ĳ������ ��ġ�� ã��
            FindPlayerPosition();

            // �÷��̾��� ��ġ�� �̵���Ŵ
            player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y);
        }

        /// <summary>
        /// 2���� �迭���� ĳ������ ��ġ�� ã��
        /// </summary>
        public void FindPlayerPosition()
        {
            for (int r = 0; r < currentBoard.GetLength(0); r++)
            {
                for (int c = 0; c < currentBoard.GetLength(1); c++)
                {
                    if (currentBoard[r, c] == Player)
                    {
                        playerPosition.Y = r;
                        playerPosition.X = c;
                        return;
                    }
                }
            }
        }

        // �Է� Ű ����
        public void HandleInput(Direction direction)
        {
            //�÷��̾��� ��ġ�� ã��
            FindPlayerPosition();

            //Ű �Է��� �޾Ƽ� ó���ϴ� ����
            InputMoveKey(direction);

            // ���ڸ��� ��������� �ٽ� �׸���
            IsGoalEmpty();

            //������ Ŭ���� �Ǿ��� �� ó��
            ClearGame();
        }

        // �Է� Ű ����
        public void InputMoveKey(Direction direction)
        {
            //�Էµ� Ű���� �޾ƿ�
            //Ű �Է��� �޾Ƽ� ó���ϴ� ����
            switch (direction)
            {
                // x-1 ĳ������ ����                
                case Direction.Left:
                    // ĳ������ ������ ����ְų� �������� ���� �� �ִ� ���Goal ��� �̵�ó��
                    if (currentBoard[playerPosition.Y, playerPosition.X - 1] == Empty ||
                        currentBoard[playerPosition.Y, playerPosition.X - 1] == Goal)
                    {
                        // �迭�� ���� �����ϴ� �ڵ� ��
                        // ĳ���͸� �̵�
                        currentBoard[playerPosition.Y, playerPosition.X - 1] = Player;

                        //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // �÷��̾��� ��ġ�� �̵���Ŵ
                        player.SetPosition(playerPosition.X - 1, (height - 1) - playerPosition.Y);
                    }
                    //ĳ������ ���ʿ� �ڽ��� �ִٸ� ó��
                    else if (currentBoard[playerPosition.Y, playerPosition.X - 1] == Box)
                    {
                        //�ڽ��� -2 �� �������� ������(ĳ������ ���� ���ڸ�)
                        if (currentBoard[playerPosition.Y, playerPosition.X - 2] == Empty ||
                            currentBoard[playerPosition.Y, playerPosition.X - 2] == Goal)
                        {
                            // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                            // ĳ������ ���� �ڽ��� �ű��
                            currentBoard[playerPosition.Y, playerPosition.X - 2] = Box;
                            // �ڽ� �ڸ��� ĳ���͸� �ű��
                            currentBoard[playerPosition.Y, playerPosition.X - 1] = Player;
                            // ĳ���� �ڸ��� ������
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)

                            // Box ó��
                            sprites[playerPosition.Y, playerPosition.X - 2] = sprites[playerPosition.Y, playerPosition.X - 1];
                            sprites[playerPosition.Y, playerPosition.X - 2].transform.position = new Vector3(playerPosition.X - 2, (height - 1) - playerPosition.Y);
                            sprites[playerPosition.Y, playerPosition.X - 1] = null;

                            // �÷��̾� ��ġ ����
                            player.transform.position = new Vector3(playerPosition.X - 1, (height - 1) - playerPosition.Y);
                        }
                    }
                    break;
                case Direction.Right:
                    // ĳ������ �������� ����ְų� �������� ���� �� �ִ� ���Goal ��� �̵�ó��
                    if (currentBoard[playerPosition.Y, playerPosition.X + 1] == Empty ||
                        currentBoard[playerPosition.Y, playerPosition.X + 1] == Goal)
                    {
                        // �迭�� ���� �����ϴ� �ڵ� ��
                        // ĳ���͸� �̵�
                        currentBoard[playerPosition.Y, playerPosition.X + 1] = Player;

                        //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���
                        // �÷��̾��� ��ġ�� �̵���Ŵ
                        player.SetPosition(playerPosition.X + 1, (height - 1) - playerPosition.Y);

                    }
                    //ĳ������ �����ʿ� �ڽ��� �ִٸ� ó��
                    else if (currentBoard[playerPosition.Y, playerPosition.X + 1] == Box)
                    {
                        //�ڽ��� -2 �� �������� ������(ĳ������ ���� ���ڸ�)
                        if (currentBoard[playerPosition.Y, playerPosition.X + 2] == Empty ||
                            currentBoard[playerPosition.Y, playerPosition.X + 2] == Goal)
                        {
                            // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                            // ĳ������ ���� �ڽ��� �ű��
                            currentBoard[playerPosition.Y, playerPosition.X + 2] = Box;
                            // �ڽ� �ڸ��� ĳ���͸� �ű��
                            currentBoard[playerPosition.Y, playerPosition.X + 1] = Player;
                            // ĳ���� �ڸ��� ������
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)
                            // Box ó��
                            sprites[playerPosition.Y, playerPosition.X + 2] =
                                 sprites[playerPosition.Y, playerPosition.X + 1];
                            sprites[playerPosition.Y, playerPosition.X + 2].transform.position =
                                 new Vector3(playerPosition.X + 2, (height - 1) - playerPosition.Y);

                            sprites[playerPosition.Y, playerPosition.X + 1] = null;

                            // �÷��̾� ��ġ ����
                            player.transform.position = new Vector3(playerPosition.X + 1, (height - 1) - playerPosition.Y);
                        }
                    }
                    break;
                case Direction.Up:
                    // ĳ������ ������ ����ְų� �������� ���� �� �ִ� ���Goal ��� �̵�ó��
                    if (currentBoard[playerPosition.Y - 1, playerPosition.X] == Empty ||
                        currentBoard[playerPosition.Y - 1, playerPosition.X] == Goal)
                    {
                        // �迭�� ���� �����ϴ� �ڵ� ��
                        // ĳ���͸� �̵�
                        currentBoard[playerPosition.Y - 1, playerPosition.X] = Player;

                        //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // �÷��̾��� ��ġ�� �̵���Ŵ
                        player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y + 1);

                    }
                    //ĳ������ ���ʿ� �ڽ��� �ִٸ� ó��
                    else if (currentBoard[playerPosition.Y - 1, playerPosition.X] == Box)
                    {
                        //�ڽ��� -2 �� �������� ������(ĳ������ ���� ���ڸ�)
                        if (currentBoard[playerPosition.Y - 2, playerPosition.X] == Empty ||
                            currentBoard[playerPosition.Y - 2, playerPosition.X] == Goal)
                        {
                            // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                            // ĳ������ ���� �ڽ��� �ű��
                            currentBoard[playerPosition.Y - 2, playerPosition.X] = Box;
                            // �ڽ� �ڸ��� ĳ���͸� �ű��
                            currentBoard[playerPosition.Y - 1, playerPosition.X] = Player;
                            // ĳ���� �ڸ��� ������
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)
                            // Box ó��
                            sprites[playerPosition.Y - 2, playerPosition.X] =
                                 sprites[playerPosition.Y - 1, playerPosition.X];
                            sprites[playerPosition.Y - 2, playerPosition.X].transform.position =
                                 new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + 2);

                            sprites[playerPosition.Y - 1, playerPosition.X] = null;

                            // �÷��̾� ��ġ ����
                            player.transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + 1);

                        }
                    }
                    break;
                case Direction.Down:
                    // ĳ������ �Ʒ��� ����ְų� �������� ���� �� �ִ� ���Goal ��� �̵�ó��
                    if (currentBoard[playerPosition.Y + 1, playerPosition.X] == Empty ||
                        currentBoard[playerPosition.Y + 1, playerPosition.X] == Goal)
                    {
                        // �迭�� ���� �����ϴ� �ڵ� ��
                        // ĳ���͸� �̵�
                        currentBoard[playerPosition.Y + 1, playerPosition.X] = Player;

                        //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // �÷��̾��� ��ġ�� �̵���Ŵ
                        player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y - 1);

                    }
                    //ĳ������ �Ʒ��� �ڽ��� �ִٸ� ó��
                    else if (currentBoard[playerPosition.Y + 1, playerPosition.X] == Box)
                    {
                        //�ڽ��� -2 �� �������� ������(ĳ������ �Ʒ��� �Ʒ��ڸ�)
                        if (currentBoard[playerPosition.Y + 2, playerPosition.X] == Empty ||
                            currentBoard[playerPosition.Y + 2, playerPosition.X] == Goal)
                        {
                            // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                            // ĳ������ �Ʒ��� �ڽ��� �ű��
                            currentBoard[playerPosition.Y + 2, playerPosition.X] = Box;
                            // �ڽ� �ڸ��� ĳ���͸� �ű��
                            currentBoard[playerPosition.Y + 1, playerPosition.X] = Player;
                            // ĳ���� �ڸ��� ������
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)
                            // Box ó��
                            sprites[playerPosition.Y + 2, playerPosition.X] = sprites[playerPosition.Y + 1, playerPosition.X];
                            sprites[playerPosition.Y + 2, playerPosition.X].transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y - 2);

                            sprites[playerPosition.Y + 1, playerPosition.X] = null;

                            // �÷��̾� ��ġ ����
                            player.transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y - 1);
                        }
                    }
                    break;
            }
        }


        /// <summary>
        /// �� ���� ������ �ٽ� �׷��ֱ�
        /// </summary>
        public void IsGoalEmpty()
        {
            // �������� �߰� ��忡���� ��ȸ�ϴ� ������ ����Ǿ�� ��
            for (int i = 0; i < goalPositions.Count; i++)
            {
                int row = goalPositions[i].Y;
                int column = goalPositions[i].X;
                if (currentBoard[row, column] == Empty)
                {
                    currentBoard[row, column] = Goal;
                }
            }
        }

        // �������� Ŭ����� ó��
        public void ClearGame()
        {            
            // ��� �� �ڽ��� ä�������� �˻� �� ä������ true��ȯ
            if (IsLevelCleared())
            {
                //카운트다운 정지
                StopCountdown();

                if (currentStage >= totalStageCount)
                {
                    // Ŭ���� UI
                    _uiManager.SetClearSokobanUI(true);

                    print("��罺�������� Ŭ����");
                    print("������ �����մϴ�!");
                    return;
                }
                else
                {
                    // ���� üũ �� Ŭ����ȭ�� �Ǵ� ������������ ����
                    CheckAndShowClearUI();

                    // ������������ UI
                    print("���� ���������� �÷��� �Ͻðڽ��ϱ�?");
                    print("���� ���������� �̵��Ϸ��� yŰ�� �Է�");
                }
            }
        }

        //Ŭ���� ���� �Լ�
        private void CheckAndShowClearUI()
        {
            // 2�������� Ŭ���� ��
            if (currentStage == clearStage)
            {
                // Ŭ���� UI
                _uiManager.SetClearSokobanUI(true);
            }
            else
            {
                _uiManager.OnNextStageClicked();
                //_uiManager.SetInfoNextStageUI(true);

                // To do...
                // �������������̵�Ű(R, A��ư) �Է¹ޱ� => ��ưó��
                // ���� �������� �̵�
               // ClickNextStageButton();
            }
        }

        /// <summary>
        /// Goal�� Box���� ���� Ȯ��
        /// </summary>
        /// <returns></returns>
        // �������� ������ ������ ��ȸ�ϰ� Box�� ������ false
        private bool IsLevelCleared()
        {
            for (int i = 0; i < goalPositions.Count; i++)
            {
                int row = goalPositions[i].Y;
                int column = goalPositions[i].X;
                // �� box�� ������ ���� �� �ְ�
                if (currentBoard[row, column] != Box)
                    return false;
            }
            return true;
        }


        // �������������̵�Ű(R) �Է¹ޱ� => UI�Ŵ����� �ű��
        public void ClickNextStageButton()
        {
            // vr���� A��ư

            // ������ ó��
            ++currentStage;
            Setupstage(currentStage - 1);
        }

        // ���ڹ� �������� �� �̵�ó��
        /// <summary>
        /// ��,�� �̵�ó��
        /// </summary>
        public void MoveHorizontal(int moveNumber1, int moveNumber2)
        {
            if (currentBoard[playerPosition.Y, playerPosition.X + moveNumber1] == Empty ||
                        currentBoard[playerPosition.Y, playerPosition.X + moveNumber1] == Goal)
            {
                // �迭�� ���� �����ϴ� �ڵ� ��
                // ĳ���͸� �̵�
                currentBoard[playerPosition.Y, playerPosition.X + moveNumber1] = Player;

                //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                // �÷��̾��� ��ġ�� �̵���Ŵ
                player.SetPosition(playerPosition.X + moveNumber1, (height - 1) - playerPosition.Y);
            }
            //ĳ������ ���ʿ� �ڽ��� �ִٸ� ó��
            else if (currentBoard[playerPosition.Y, playerPosition.X + moveNumber1] == Box)
            {
                //�ڽ��� -2 �� �������� ������(ĳ������ ���� ���ڸ�)
                if (currentBoard[playerPosition.Y, playerPosition.X + moveNumber2] == Empty ||
                    currentBoard[playerPosition.Y, playerPosition.X + moveNumber2] == Goal)
                {
                    // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                    // ĳ������ ���� �ڽ��� �ű��
                    currentBoard[playerPosition.Y, playerPosition.X + moveNumber2] = Box;
                    // �ڽ� �ڸ��� ĳ���͸� �ű��
                    currentBoard[playerPosition.Y, playerPosition.X + moveNumber1] = Player;
                    // ĳ���� �ڸ��� ������
                    currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                    // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)

                    // Box ó��
                    sprites[playerPosition.Y, playerPosition.X + moveNumber2] = sprites[playerPosition.Y, playerPosition.X + moveNumber1];
                    sprites[playerPosition.Y, playerPosition.X + moveNumber2].transform.position = new Vector3(playerPosition.X + moveNumber2, (height - 1) - playerPosition.Y);
                    sprites[playerPosition.Y, playerPosition.X + moveNumber1] = null;

                    // �÷��̾� ��ġ ����
                    player.transform.position = new Vector3(playerPosition.X + moveNumber1, (height - 1) - playerPosition.Y);
                }
            }
        }

        /// <summary>
        /// ��, �Ʒ� �̵�ó��
        /// </summary>
        public void MoveVertical(int moveNumber1, int moveNumber2)
        {
            // ĳ������ ������ ����ְų� �������� ���� �� �ִ� ���Goal ��� �̵�ó��
            if (currentBoard[playerPosition.Y + moveNumber1, playerPosition.X] == Empty ||
                currentBoard[playerPosition.Y + moveNumber1, playerPosition.X] == Goal)
            {
                // �迭�� ���� �����ϴ� �ڵ� ��
                // ĳ���͸� �̵�
                currentBoard[playerPosition.Y + moveNumber1, playerPosition.X] = Player;

                //ĳ���Ͱ��ִ� �ڸ��� ����ִ� �ĺ��ڵ带 ����
                currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                // �÷��̾��� ��ġ�� �̵���Ŵ
                player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y + moveNumber1);

            }
            //ĳ������ ���ʿ� �ڽ��� �ִٸ� ó��
            else if (currentBoard[playerPosition.Y + moveNumber1, playerPosition.X] == Box)
            {
                //�ڽ��� -2 �� �������� ������(ĳ������ ���� ���ڸ�)
                if (currentBoard[playerPosition.Y + moveNumber2, playerPosition.X] == Empty ||
                    currentBoard[playerPosition.Y + moveNumber2, playerPosition.X] == Goal)
                {
                    // ĳ���͸� �̵���ų �� �ִٸ� �迭�� ����
                    // ĳ������ ���� �ڽ��� �ű��
                    currentBoard[playerPosition.Y + moveNumber2, playerPosition.X] = Box;
                    // �ڽ� �ڸ��� ĳ���͸� �ű��
                    currentBoard[playerPosition.Y + moveNumber1, playerPosition.X] = Player;
                    // ĳ���� �ڸ��� ������
                    currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                    // Ŀ���� ��ġ�� �̵��ϰ� �÷��̾ ���(2ĭ¥��)
                    // Box ó��
                    sprites[playerPosition.Y + moveNumber2, playerPosition.X] =
                         sprites[playerPosition.Y + moveNumber1, playerPosition.X];
                    sprites[playerPosition.Y + moveNumber2, playerPosition.X].transform.position =
                         new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + moveNumber2);

                    sprites[playerPosition.Y + moveNumber1, playerPosition.X] = null;

                    // �÷��̾� ��ġ ����
                    player.transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + moveNumber1);
                }
            }
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        public void GameReset()
        {
            //��Ʈ �ʱ�ȭ
            HeartReset();

            // ���� �������� ��ȣ
            currentStage = 1;
            // ���� ���������� ����
            Setupstage(currentStage - 1);
        }

        //ī��Ʈ �ٿ� ����
        // ���ӿ�������
        public void TimeOver()
        {
            // Ÿ�ӿ��� UI Ȱ��
            _uiManager.SetTimeOverUI(true);
        }

        // ������(Ÿ�ӿ�����) Ÿ�ӿ��� �Ǵ� ���ӿ��� â ����
        public void LoseSokoban()
        {
            int heartCount = _heartHealth.GetHeartCount();
            if (heartCount <= 0)
            {
                // ��Ʈ������ 0�̸� ���ӿ��� â
                _uiManager.SetGameOverUI(true);
            }
            else
            {   // ��Ʈ�� ������ Ÿ�� ���� â
                _uiManager.SetTimeOverUI(true);
            }
        }

        private void HeartReset()
        {
            // ��Ʈ���� �ʱ�ȭ
            _heartHealth.SetHeartCount();

            //��Ʈ Ȱ��
            _uiManager.ShowAllHeart();
        }

        private void DeHeart()
        {
            var heartCount = _heartHealth.CalculateHeartCount();
            // ��Ʈ ��Ȱ��
            _uiManager.HideHeart(heartCount);
        }

        // ī��Ʈ�ٿ� �����
        public void CountdownReset()
        {
            _countdownTimer.CountdownInitialized();
        }

        public void StopCountdown()
        {
            //ī��Ʈ�ٿ� ���߱�
            _countdownTimer.StopCountdown();
        }


        //���罺������ ���÷���(���÷��� ��ư Ŭ��)
        public void StageReset()
        {
            //��Ʈ����
            DeHeart();

            // ���� �������� ��ȣ
            // currentStage = currentStage

            // ���� ���������� ����
            Setupstage(currentStage - 1);
        }

    } // ���ڹ� �Ŵ��� Ŭ����
}
