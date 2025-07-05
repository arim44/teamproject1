using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;


//public enum Direction
//{
//    None,
//    Left,
//    Right,
//    Up,
//    Down
//}

namespace Sokoban
{
    // 사용자가 정의한 데이터는 [Serializable]직렬화를 사용해야만 에디터에서 확인
    [Serializable]
    public struct Position  // Position 구조체
    {
        // x, y 속성값
        //public int x { get; set; } = x;
        public int X;
        public int Y;
    }


    public class Sokoban : MonoBehaviour
    {
        //비어있는 장소를 가리키는 식별값
        private const int Empty = 0;
        //벽을 가리키는 식별 값
        private const int Wall = 1;
        //아이템이 들어갈 장소
        private const int Goal = 2;
        //이동시킬 박스
        private const int Box = 3;
        //플레이어의 식별 값
        private const int Player = 4;

        //제이슨 파일에서 가져올 꺼임
        private int[,]? currentBoard = null;
        //private Slot[,] currentBoard = null;
        // 생성된 스프라이트 리스트
        private SpriteRenderer[,] sprites = null;

        // 제이슨 파일에서 받아올 정보
        private List<Sokoban_StageData> stages;

        // 카메라 위치 리스트
        [SerializeField]
        // private List<Position> cameraPositions = new List<Position>();
        private List<Vector2> cameraPositions = new List<Vector2>();

        //전체 스테이지 수
        private int totalStageCount = 0;

        //현재 스테이지
        private int currentStage = 1;

        // 플레이어 위치
        private Position playerPosition = new Position();
        // 현재 스테이지의 아이템을 넣을 장소
        private List<Position> goalPositions = new List<Position>(0);

        //플레이어
        private Player player;

        // 현재 이동할 방향
        public Direction direction = Direction.None;

        // 카메라
        private Camera gameCamera;

        private GameObject stageGameObject;

        private int width;
        private int height;

        //방향 설정
        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        // 스테이지
        public void SetStages(List<Sokoban_StageData> stages)
        {
            this.stages = stages;
            this.totalStageCount = stages.Count;
        }

        //아이템을 저장할 공간을 순회하고 Box가 없으면 false
        private bool IsLevelCleared()
        {
            for (int i = 0; i < goalPositions.Count; i++)
            {
                int row = goalPositions[i].Y;
                int column = goalPositions[i].X;
                //currentBoard = new Slot[row, column];

                //if(!currentBoard.)
                //{

                //}

                if (currentBoard[row, column] != Box)
                    return false;
            }
            return true;
        }


        public void Setupstage(int stage)
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

            // 카메라 위치 설정
            // 카메라가 없고 카메라 포지션의 수가 0보다 크면
            if (gameCamera != null && cameraPositions.Count > 0)
            {

                Vector3 position = cameraPositions[stage];
                position.z = gameCamera.transform.position.z;
                // cameraPositions List를 <Position>이 아닌 <Vector2>로 해놓으면 사용가능
                gameCamera.transform.position = position;

                //cameraPositions List를 <Position> 일 경우
                //Vector3 position = Vector3.zero;
                //position.x = cameraPositions[stage].X;
                //position.y = cameraPositions[stage].Y;
                //gameCamera.transform.position = position;
            }

            //현재 보드에 배열을 할당
            currentBoard = new int[height, width];

            //스테이지 데이터를 CurrtentBoard에복사함
            Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);

            // 스테이지에서 아이템을 저장할 공간을 찾음
            FindGoalPositions();

            // 캐릭터의 위치를 찾음
            FindPlayerPosition();

            // Player 스크립트 찾아오기
            player = FindFirstObjectByType<Player>();

            // 플레이어의 위치를 이동시킴
            player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y);
        }


        // 2차원 배열에서 캐릭터의 위치를 찾음
        private void FindPlayerPosition()
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

        private void SetPosition(int x, int y)
        {
            // 2 칸씩 띄어서 놓을 수 있음
            int currX = x * 2;
            int currY = y;

            Console.SetCursorPosition(currX, currY);
        }


        // 입력 키 조정
        public void HandleInput(Direction direction)
        {
            //플레이어의 위치를 찾음
            FindPlayerPosition();

            //입력된 키값을 받아옴
            //키 입력을 받아서 처리하는 내용
            switch (direction)
            {
                // x-1 캐릭터의 왼쪽                
                case Direction.Left:
                    // 캐릭터의 왼쪽이 비어있거나 아이템을 놓을 수 있는 장소Goal 라면 이동처리
                    if (currentBoard[playerPosition.Y, playerPosition.X - 1] == Empty ||
                        currentBoard[playerPosition.Y, playerPosition.X - 1] == Goal)
                    {
                        // 배열의 값을 갱신하는 코드 임
                        // 캐릭터를 이동
                        currentBoard[playerPosition.Y, playerPosition.X - 1] = Player;

                        //캐릭터가있던 자리에 비어있는 식별코드를 넣음
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // 플레이어의 위치를 이동시킴
                        player.SetPosition(playerPosition.X - 1, (height - 1) - playerPosition.Y);
                    }
                    //캐릭터의 왼쪽에 박스가 있다면 처리
                    else if (currentBoard[playerPosition.Y, playerPosition.X - 1] == Box)
                    {
                        //박스옆 -2 이 무엇인지 봐야함(캐릭터의 옆의 옆자리)
                        if (currentBoard[playerPosition.Y, playerPosition.X - 2] == Empty ||
                            currentBoard[playerPosition.Y, playerPosition.X - 2] == Goal)
                        {
                            // 캐릭터를 이동시킬 수 있다면 배열을 갱신
                            // 캐릭터의 옆에 박스를 옮기고
                            currentBoard[playerPosition.Y, playerPosition.X - 2] = Box;
                            // 박스 자리에 캐릭터를 옮기고
                            currentBoard[playerPosition.Y, playerPosition.X - 1] = Player;
                            // 캐릭터 자리는 비어놓음
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // 커서의 위치를 이동하고 플레이어를 출력(2칸짜리)

                            // Box 처리
                            sprites[playerPosition.Y, playerPosition.X - 2] = sprites[playerPosition.Y, playerPosition.X - 1];
                            sprites[playerPosition.Y, playerPosition.X - 2].transform.position = new Vector3(playerPosition.X - 2, (height - 1) - playerPosition.Y);
                            sprites[playerPosition.Y, playerPosition.X - 1] = null;

                            // 플레이어 위치 설정
                            player.transform.position = new Vector3(playerPosition.X - 1, (height - 1) - playerPosition.Y);
                        }
                    }
                    break;
                case Direction.Right:
                    // 캐릭터의 오른쪽이 비어있거나 아이템을 놓을 수 있는 장소Goal 라면 이동처리
                    if (currentBoard[playerPosition.Y, playerPosition.X + 1] == Empty ||
                        currentBoard[playerPosition.Y, playerPosition.X + 1] == Goal)
                    {
                        // 배열의 값을 갱신하는 코드 임
                        // 캐릭터를 이동
                        currentBoard[playerPosition.Y, playerPosition.X + 1] = Player;

                        //캐릭터가있던 자리에 비어있는 식별코드를 넣음
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // 커서의 위치를 이동하고 플레이어를 출력
                        // 플레이어의 위치를 이동시킴
                        player.SetPosition(playerPosition.X + 1, (height - 1) - playerPosition.Y);

                    }
                    //캐릭터의 오른쪽에 박스가 있다면 처리
                    else if (currentBoard[playerPosition.Y, playerPosition.X + 1] == Box)
                    {
                        //박스옆 -2 이 무엇인지 봐야함(캐릭터의 옆의 옆자리)
                        if (currentBoard[playerPosition.Y, playerPosition.X + 2] == Empty ||
                            currentBoard[playerPosition.Y, playerPosition.X + 2] == Goal)
                        {
                            // 캐릭터를 이동시킬 수 있다면 배열을 갱신
                            // 캐릭터의 옆에 박스를 옮기고
                            currentBoard[playerPosition.Y, playerPosition.X + 2] = Box;
                            // 박스 자리에 캐릭터를 옮기고
                            currentBoard[playerPosition.Y, playerPosition.X + 1] = Player;
                            // 캐릭터 자리는 비어놓음
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // 커서의 위치를 이동하고 플레이어를 출력(2칸짜리)
                            // Box 처리
                            sprites[playerPosition.Y, playerPosition.X + 2] =
                                 sprites[playerPosition.Y, playerPosition.X + 1];
                            sprites[playerPosition.Y, playerPosition.X + 2].transform.position =
                                 new Vector3(playerPosition.X + 2, (height - 1) - playerPosition.Y);

                            sprites[playerPosition.Y, playerPosition.X + 1] = null;

                            // 플레이어 위치 설정
                            player.transform.position = new Vector3(playerPosition.X + 1, (height - 1) - playerPosition.Y);
                        }
                    }
                    break;
                case Direction.Up:
                    // 캐릭터의 위쪽이 비어있거나 아이템을 놓을 수 있는 장소Goal 라면 이동처리
                    if (currentBoard[playerPosition.Y - 1, playerPosition.X] == Empty ||
                        currentBoard[playerPosition.Y - 1, playerPosition.X] == Goal)
                    {
                        // 배열의 값을 갱신하는 코드 임
                        // 캐릭터를 이동
                        currentBoard[playerPosition.Y - 1, playerPosition.X] = Player;

                        //캐릭터가있던 자리에 비어있는 식별코드를 넣음
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // 플레이어의 위치를 이동시킴
                        player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y + 1);

                    }
                    //캐릭터의 위쪽에 박스가 있다면 처리
                    else if (currentBoard[playerPosition.Y - 1, playerPosition.X] == Box)
                    {
                        //박스옆 -2 이 무엇인지 봐야함(캐릭터의 위의 위자리)
                        if (currentBoard[playerPosition.Y - 2, playerPosition.X] == Empty ||
                            currentBoard[playerPosition.Y - 2, playerPosition.X] == Goal)
                        {
                            // 캐릭터를 이동시킬 수 있다면 배열을 갱신
                            // 캐릭터의 위에 박스를 옮기고
                            currentBoard[playerPosition.Y - 2, playerPosition.X] = Box;
                            // 박스 자리에 캐릭터를 옮기고
                            currentBoard[playerPosition.Y - 1, playerPosition.X] = Player;
                            // 캐릭터 자리는 비어놓음
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // 커서의 위치를 이동하고 플레이어를 출력(2칸짜리)
                            // Box 처리
                            sprites[playerPosition.Y - 2, playerPosition.X] =
                                 sprites[playerPosition.Y - 1, playerPosition.X];
                            sprites[playerPosition.Y - 2, playerPosition.X].transform.position =
                                 new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + 2);

                            sprites[playerPosition.Y - 1, playerPosition.X] = null;

                            // 플레이어 위치 설정
                            player.transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y + 1);

                        }
                    }
                    break;
                case Direction.Down:
                    // 캐릭터의 아래가 비어있거나 아이템을 놓을 수 있는 장소Goal 라면 이동처리
                    if (currentBoard[playerPosition.Y + 1, playerPosition.X] == Empty ||
                        currentBoard[playerPosition.Y + 1, playerPosition.X] == Goal)
                    {
                        // 배열의 값을 갱신하는 코드 임
                        // 캐릭터를 이동
                        currentBoard[playerPosition.Y + 1, playerPosition.X] = Player;

                        //캐릭터가있던 자리에 비어있는 식별코드를 넣음
                        currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                        // 플레이어의 위치를 이동시킴
                        player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y - 1);

                    }
                    //캐릭터의 아래에 박스가 있다면 처리
                    else if (currentBoard[playerPosition.Y + 1, playerPosition.X] == Box)
                    {
                        //박스옆 -2 이 무엇인지 봐야함(캐릭터의 아래의 아래자리)
                        if (currentBoard[playerPosition.Y + 2, playerPosition.X] == Empty ||
                            currentBoard[playerPosition.Y + 2, playerPosition.X] == Goal)
                        {
                            // 캐릭터를 이동시킬 수 있다면 배열을 갱신
                            // 캐릭터의 아래에 박스를 옮기고
                            currentBoard[playerPosition.Y + 2, playerPosition.X] = Box;
                            // 박스 자리에 캐릭터를 옮기고
                            currentBoard[playerPosition.Y + 1, playerPosition.X] = Player;
                            // 캐릭터 자리는 비어놓음
                            currentBoard[playerPosition.Y, playerPosition.X] = Empty;

                            // 커서의 위치를 이동하고 플레이어를 출력(2칸짜리)
                            // Box 처리
                            sprites[playerPosition.Y + 2, playerPosition.X] = sprites[playerPosition.Y + 1, playerPosition.X];
                            sprites[playerPosition.Y + 2, playerPosition.X].transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y - 2);

                            sprites[playerPosition.Y + 1, playerPosition.X] = null;

                            // 플레이어 위치 설정
                            player.transform.position = new Vector3(playerPosition.X, (height - 1) - playerPosition.Y - 1);
                        }
                    }
                    break;
            }
            IsGoalEmpty();

            //게임이 클리어 되었을 때 처리
            // 골이 비어있지 않으면
            if (IsLevelCleared())
            {
                if (currentStage >= totalStageCount)
                {
                    // UI가 나와야 함
                    print("모든스테이지를 클리어");
                    print("게임을 종료합니다!");
                    return;
                }
                else
                {
                    // UI가 나와야 함
                    print("다음 스테이지를 플레이 하시겠습니까?");
                    print("다음 스테이지로 이동하려면 y키를 입력");

                    ++currentStage;
                    Setupstage(currentStage - 1);


                    // 아래의 코드는 버튼을 선택하는 UI로 구성한 이후에 처리해야 함
                    //입력받기 위해 대기
                    //var key = Console.ReadKey(true).KeyChar;

                    ////입력한 키 값을 소문자로 변경처리
                    //char input = char.ToLower(key);
                    //if (input == 'y')
                    //{
                    //    ++currentStage;
                    //    Setupstage(currentStage - 1);
                    //}
                }
            }
        }

        /// <summary>
        /// 보드판에서 Goal의 위치찾아서 리스트에 저장
        /// </summary>
        private void FindGoalPositions()
        {
            // 저장소의 내용을 삭제
            goalPositions.Clear();

            if (currentBoard == null) return;

            //보드판 순회
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
        /// 골 들어갔다 나오면 다시 그려주기
        /// </summary>
        private void IsGoalEmpty()
        {
            // 스테이지 추가 모드에서는 순회하는 범위도 변경되어야 함
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

        public void Start()
        {
            // subCamera 태그값 지정해서 찾아오기
            var subCamera = GameObject.FindGameObjectWithTag("SubCamera");
            if (subCamera != null) gameCamera = subCamera.GetComponent<Camera>();

            // 신에 배치된 subCamera 이름을 갖는 게임오브젝트를 찾아 카메라로 얻어오는 방식
            //subCamera = GameObject.Find("SubCamera");
            //if (subCamera != null) gameCamera = subCamera.GetComponent<Camera>();


            // 전체 스테이지를 로드
            // 확장자 없이 파일 이름만 써도 됨 json파일 가져오기
            TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
            stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);

            totalStageCount = stages.Count;

            // 현재 스테이지를 구성
            Setupstage(currentStage - 1);
        }

        public void GameReset()
        {
            // 현재 스테이지 번호
            currentStage = 1;
            // 현재 스테이지를 구성
            Setupstage(currentStage - 1);
        }

        public void Update()
        {
            // 입력처리를 받음
            //HandleInput();
        }

    }
}
