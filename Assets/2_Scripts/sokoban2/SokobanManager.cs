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

            // 하트 초기화
            HeartReset();
        }

        // 소코반 시작
        public void StartSokoban()
        {
            // 현재 스테이지를 구성
            Setupstage(currentStage - 1);
        }


        /// <summary>
        /// Json에서 데이터 로드
        /// </summary>
        private List<Sokoban_StageData> LoadJsonDate()
        {
            // json파일 가져오기 (확장자 없이 파일 이름만 써도 됨)
            TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
            stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);

            return stages;
        }

        /// <summary>
        /// 스테이지 데이터 할당하기 (제이슨에서 읽은후 호출)
        /// </summary>
        /// <param name="stages"></param>
        public void SetStages(List<Sokoban_StageData> stages)
        {
            this.stages = stages;
            totalStageCount = stages.Count;
        }

        /// <summary>
        /// 스테이지들 만들기
        /// </summary>
        /// <param name="stage"></param>
        public void Setupstage(int stage)
        {
            // 스테이지 생성
            CreateStage(stage);

            // 스프라이트 설정
            SetSprits(stageGameObject);

            // 카메라 위치 설정
            SetCameraPosition(stage);

            // 현재 보드 설정
            SetCurrentBoard(stage);

            // Goal 위치 스테이지에서 아이템을 저장할 공간을 찾음
            FindGoalPositions();

            // 플레이어 위치 설정
            SetPlayerPosition();

            // 카운트다운 재시작
            CountdownReset();
        }

        /// <summary>
        /// 스테이지 생성 후 스프라이트 삽입
        /// </summary>
        /// <param name="stage"></param>
        private void CreateStage(int stage)
        {
            // 기존 스테이지가 남아있다면 삭제
            if (stageGameObject != null) Destroy(stageGameObject);

            //현재 스테이지 이름을 구함
            string stageText = $"Stage{stage + 1}";
            // 리소스를 로드
            var stageObj = Resources.Load<GameObject>($"Prefabs/Stages/{stageText}");
            if (stageObj == null) return;

            // 지정한 스테이지의 행과 열의 값을 받음(배열의 크기값을 구하고)
            height = stages[stage].Map.GetLength(0);
            width = stages[stage].Map.GetLength(1);
            // 스프라이트 배열을 생성
            sprites = new SpriteRenderer[height, width];

            // 스테이지 생성
            stageGameObject = Instantiate(stageObj, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 생성된 스테이지를 받아서 스프라이트 삽입
        /// </summary>
        private void SetSprits(GameObject stageGameObject)
        {
            // 스테이지 부모 오브젝트의 트랜스폼
            Transform rootTransform = stageGameObject.transform;

            // rootTransform 오브젝트 밑에 있는 스테이지들 수만큼 반복
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                // 위에서부터 순서대로 child에 넣음 
                Transform child = rootTransform.GetChild(i);
                var rc = child.name.Split(",");
                //  rc name 입력받음
                int.TryParse(rc[0], out int row);
                int.TryParse(rc[1], out int column);

                // 스프라이트 배열에 요소를 채움(row,column 위치에 각각 스프라이트 넣음)
                sprites[row, column] = child.GetComponent<SpriteRenderer>();
            }
        }


        /// <summary>
        /// 카메라 위치 설정
        /// </summary>
        private void SetCameraPosition(int stage)
        {
            // 카메라가 없고 카메라 포지션의 수가 0보다 크면
            if (gameCamera != null && cameraPositions.Count > 0)
            {
                Vector3 position = cameraPositions[stage];
                position.z = gameCamera.transform.position.z;
                // cameraPositions List를 <Position>이 아닌 <Vector2>로 해놓으면 사용가능
                gameCamera.transform.position = position;
            }
        }

        /// <summary>
        /// currentBoard(현재 보드) 설정
        /// </summary>
        private void SetCurrentBoard(int stage)
        {
            //현재 보드에 배열을 할당
            currentBoard = new int[height, width];

            //스테이지 데이터를 CurrtentBoard에복사함
            Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);
        }

        /// <summary>
        /// 보드판에서 Goal의 위치찾아서 리스트에 저장
        /// </summary>
        private void FindGoalPositions()
        {
            //저장소의 내용을 삭제
            goalPositions.Clear();

            if (currentBoard == null) return;

            // 보드판 순회
            for (int r = 0; r < currentBoard.GetLength(0); r++)
            {
                for (int c = 0; c < currentBoard.GetLength(1); c++)
                {
                    if (currentBoard[r, c] == Goal)
                    {
                        //아이템을 넣을 수 있는 공간인 경우 위치를 저장
                        // 메모리를 할당하면서 저장
                        Position position = new Position { X = c, Y = r };
                        goalPositions.Add(position);
                    }
                }
            }
        }

        /// <summary>
        /// 플레이어 위치 설정
        /// </summary>
        private void SetPlayerPosition()
        {
            // Player 스크립트 찾아오기
            player = FindFirstObjectByType<Player>();

            // 캐릭터의 위치를 찾음
            FindPlayerPosition();

            // 플레이어의 위치를 이동시킴
            player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y);
        }

        /// <summary>
        /// 2차원 배열에서 캐릭터의 위치를 찾음
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

        // 입력 키 조정
        public void HandleInput(Direction direction)
        {
            //플레이어의 위치를 찾음
            FindPlayerPosition();

            //키 입력을 받아서 처리하는 내용
            InputMoveKey(direction);

            // 골자리가 비어있으면 다시 그리기
            IsGoalEmpty();

            //게임이 클리어 되었을 때 처리
            ClearGame();
        }

        // 입력 키 조정
        public void InputMoveKey(Direction direction)
        {
            //입력된 키값을 받아옴
            //키 입력을 받아서 처리하는 내용
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
