using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace RetroSokoban
{
    public class SokobanManager : MonoBehaviour
    {
        //비어있는 장소를 가리키는 식별값
        private const int Empty = 0;
        //벽을 가리키는 식별 값
        private const int Wall = 1;
        //아이템이 들어갈 장소
        private const int Goal = 2;
        //이동시킬 박스
        private const int Box = 3;
        //이동시킬 박스
        private const int Player = 4;

        // 스크립트 연결
        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HeartHealth _heartHealth;

        /// ====== 카메라 관련 ======
        // 카메라
        private Camera gameCamera;
        // sub카메라 게임오브젝트
        private GameObject subCamera;

        // 카메라 위치 리스트
        [SerializeField]
        private List<Vector2> cameraPositions = new List<Vector2>();

        /// ====== 플레이어 관련 ======
        //플레이어
        private Player player;
        // 플레이어 위치
        private Position playerPosition = new Position();

        /// ====== 데이터 관련 ======
        // 제이슨 파일에서 가져오기
        private int[,]? currentBoard = null;

        // 생성된 스프라이트 리스트
        private SpriteRenderer[,] sprites = null;

        // 제이슨 파일에서 받아올 정보
        private List<Sokoban_StageData> stages;

        /// ====== 스테이지 관련 ======
        //전체 스테이지 수
        private int totalStageCount = 0;

        //현재 스테이지
        [SerializeField] private int currentStage = 1;
        //클리어 스테이지
        [SerializeField] private int clearStage = 3;

        // 스테이지 오브젝트
        private GameObject stageGameObject;

        // 현재 스테이지의 아이템을 넣을 장소
        private List<Position> goalPositions = new List<Position>(0);

        // 지정한 스테이지의 행과 열값
        private int width;
        private int height;


        private void Awake()
        {
            // subCamera 태그값 찾아오기
            subCamera = GameObject.FindGameObjectWithTag("SubCamera");
            _heartHealth = FindAnyObjectByType<HeartHealth>(FindObjectsInactive.Include);
        }

        // 스크립트 가져오기, 게임매니저에서 보냄
        public void SetScripts(UIManager uiManager, CountdownTimer countdownTimer)
        {
            _uiManager = uiManager;
            _countdownTimer = countdownTimer;
        }


        // 소코반 초기세팅
        public void InitializeSokoban()
        {
            // 카메라 받아오기
            if (subCamera != null) gameCamera = subCamera.GetComponent<Camera>();

            // 전체 스테이지를 로드
            stages = LoadJsonDate();
            // 스테이지 데이터 할당
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
                // 게임클리어 사운드 재생
                SoundManager.Instance.PlaySuccess();

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
            //게임오버 사운드 재생
            SoundManager.Instance.PlayGameOver();

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
