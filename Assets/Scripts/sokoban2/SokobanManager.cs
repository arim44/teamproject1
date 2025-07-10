// 이 파일은 SoundManager를 활용해 소코반 게임에서 사운드를 재생하도록 수정된 SokobanManager 스크립트입니다.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RetroSokoban
{
    public class SokobanManager : MonoBehaviour
    {
        private const int Empty = 0;
        private const int Wall = 1;
        private const int Goal = 2;
        private const int Box = 3;
        private const int Player = 4;

        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HeartHealth _heartHealth;

        private Camera gameCamera;
        private GameObject subCamera;

        [SerializeField] private List<Vector2> cameraPositions = new List<Vector2>();

        private Player player;
        private Position playerPosition = new Position();

        private int[,]? currentBoard = null;
        private SpriteRenderer[,] sprites = null;
        private List<Sokoban_StageData> stages;

        private int totalStageCount = 0;
        private int currentStage = 1;
        private GameObject stageGameObject;
        private List<Position> goalPositions = new List<Position>(0);
        private int width;
        private int height;

        private void Awake()
        {
            subCamera = GameObject.FindGameObjectWithTag("SubCamera");
            _heartHealth = FindAnyObjectByType<HeartHealth>(FindObjectsInactive.Include);
        }

        public void SetScripts(UIManager uiManager, CountdownTimer countdownTimer)
        {
            _uiManager = uiManager;
            _countdownTimer = countdownTimer;
        }

        public void InitializeSokoban()
        {
            if (subCamera != null) gameCamera = subCamera.GetComponent<Camera>();
            stages = LoadJsonDate();
            SetStages(stages);
            HeartReset();
        }

        public void StartSokoban()
        {
            Setupstage(currentStage - 1);
        }

        private List<Sokoban_StageData> LoadJsonDate()
        {
            TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
            stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);
            return stages;
        }

        public void SetStages(List<Sokoban_StageData> stages)
        {
            this.stages = stages;
            totalStageCount = stages.Count;
        }

        public void Setupstage(int stage)
        {
            CreateStage(stage);
            SetSprits(stageGameObject);
            SetCameraPosition(stage);
            SetCurrentBoard(stage);
            FindGoalPositions();
            SetPlayerPosition();
            CountdownReset();
        }

        private void CreateStage(int stage)
        {
            if (stageGameObject != null) Destroy(stageGameObject);

            string stageText = $"Stage{stage + 1}";
            var stageObj = Resources.Load<GameObject>($"Prefabs/Stages/{stageText}");
            if (stageObj == null) return;

            height = stages[stage].Map.GetLength(0);
            width = stages[stage].Map.GetLength(1);
            sprites = new SpriteRenderer[height, width];

            stageGameObject = Instantiate(stageObj, Vector3.zero, Quaternion.identity);
        }

        private void SetSprits(GameObject stageGameObject)
        {
            Transform rootTransform = stageGameObject.transform;
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                Transform child = rootTransform.GetChild(i);
                var rc = child.name.Split(",");
                int.TryParse(rc[0], out int row);
                int.TryParse(rc[1], out int column);
                sprites[row, column] = child.GetComponent<SpriteRenderer>();
            }
        }

        private void SetCameraPosition(int stage)
        {
            if (gameCamera != null && cameraPositions.Count > 0)
            {
                Vector3 position = cameraPositions[stage];
                position.z = gameCamera.transform.position.z;
                gameCamera.transform.position = position;
            }
        }

        private void SetCurrentBoard(int stage)
        {
            currentBoard = new int[height, width];
            Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);
        }

        private void FindGoalPositions()
        {
            goalPositions.Clear();
            if (currentBoard == null) return;

            for (int r = 0; r < currentBoard.GetLength(0); r++)
            {
                for (int c = 0; c < currentBoard.GetLength(1); c++)
                {
                    if (currentBoard[r, c] == Goal)
                    {
                        Position position = new Position { X = c, Y = r };
                        goalPositions.Add(position);
                    }
                }
            }
        }

        private void SetPlayerPosition()
        {
            player = FindFirstObjectByType<Player>();
            FindPlayerPosition();
            player.SetPosition(playerPosition.X, (height - 1) - playerPosition.Y);
        }

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

        public void HandleInput(Direction direction)
        {
            FindPlayerPosition();
            InputMoveKey(direction);
            ClearGame();
        }

        public void InputMoveKey(Direction direction)
        {
            int dx = 0;
            int dy = 0;

            switch (direction)
            {
                case Direction.Up: dy = -1; break;
                case Direction.Down: dy = 1; break;
                case Direction.Left: dx = -1; break;
                case Direction.Right: dx = 1; break;
            }

            int px = playerPosition.X;
            int py = playerPosition.Y;

            int tx = px + dx;
            int ty = py + dy;

            int ntx = px + dx * 2;
            int nty = py + dy * 2;

            // 이동 범위 벗어나면 무시
            if (tx < 0 || tx >= width || ty < 0 || ty >= height)
                return;

            // 박스 밀기
            if (currentBoard[ty, tx] == Box)
            {
                if (ntx < 0 || ntx >= width || nty < 0 || nty >= height)
                    return;

                if (currentBoard[nty, ntx] == Empty || currentBoard[nty, ntx] == Goal)
                {
                    // 박스 이동
                    currentBoard[nty, ntx] = Box;
                    currentBoard[ty, tx] = Empty;
                    currentBoard[py, px] = Empty;
                    currentBoard[ty, tx] = Player;
                    playerPosition.X = tx;
                    playerPosition.Y = ty;
                    player.SetPosition(tx, (height - 1) - ty);

                    // 🔊 박스 밀기 사운드
                    SoundManager.Instance.PlayPush();
                }
            }
            else if (currentBoard[ty, tx] == Empty || currentBoard[ty, tx] == Goal)
            {
                // 일반 이동
                currentBoard[py, px] = Empty;
                currentBoard[ty, tx] = Player;
                playerPosition.X = tx;
                playerPosition.Y = ty;
                player.SetPosition(tx, (height - 1) - ty);

                // 🔊 이동 사운드
                SoundManager.Instance.PlayMove();
            }
        }


        public void ClearGame()
        {
            if (IsLevelCleared())
            {
                // 🔊 스테이지 클리어 사운드
                SoundManager.Instance.PlaySuccess();

                if (currentStage >= totalStageCount)
                {
                    print("모든스테이지를 클리어");
                    return;
                }
                else
                {
                    print("다음 스테이지로 이동하려면 y키를 입력");
                    _uiManager.OpenInfoNextStage();
                }
            }
        }

        private bool IsLevelCleared()
        {
            for (int i = 0; i < goalPositions.Count; i++)
            {
                int row = goalPositions[i].Y;
                int column = goalPositions[i].X;
                if (currentBoard[row, column] != Box)
                    return false;
            }
            return true;
        }

        public void ClickNextStageButton()
        {
            ++currentStage;
            Setupstage(currentStage - 1);
        }

        public void GameReset()
        {
            currentStage = 1;
            Setupstage(currentStage - 1);
        }

        public void TimeOver()
        {
            _uiManager.SetTimeOverActive();
        }

        private void HeartReset()
        {
            _heartHealth.SetHeartCount();
        }

        private void DeHeart()
        {
            var heartCount = _heartHealth.CalculateHeartCount();
            _uiManager.HideHeart(heartCount);
        }

        public void CountdownReset()
        {
            _countdownTimer.CountdownInitialized();
        }

        public void StageReset()
        {
            DeHeart();
            Setupstage(currentStage - 1);
        }

    } // 클래스 끝
} // 네임스페이스 끝