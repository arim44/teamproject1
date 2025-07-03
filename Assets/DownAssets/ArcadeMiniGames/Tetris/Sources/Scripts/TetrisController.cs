using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArcadeMiniGames
{
    public class TetrisController : BaseController
    {
        public Shape[] shapes;
        private Shape shape;
        private List<Shape> nextShapes;
        public int bagSize = 3;

        private Shape nextShapeDisplay;

        private Shape holdShape;
        private bool canSwitch = true;

        public int GridX = 10; //recommended between 6 and 13
        public const int GridY = 21; //1 more for when rotating a tall one
        private GameObject[,] grid;// = new GameObject[GridX, GridY];
        public Transform GridBG;
        public Transform GridBorder;

        private int levelCounter;
        public int clearsToLevelUp = 10;
        public Number level;

        public float fallDelay { get; private set; } = 1;
        public float startFallDelay = 1;
        public float fallDelayMultiplier = 0.8f;

        private bool isClearingLines;

        public AudioClip LandSound;
        public AudioClip ScoreSound;

        void Awake()
        {
            if (bagSize < 1) { bagSize = 1; }

            float gridOffset = (GridX - 10) / 2.0f;
            foreach (Transform child in PlayField.transform)
            {
                var pos = child.localPosition;
                pos.x += gridOffset;
                child.localPosition = pos;
            }
            PlayField.transform.localPosition -= new Vector3(gridOffset, 0, 0);

            grid = new GameObject[GridX, GridY];

            GridBG.localScale = new Vector3(GridX, GridY - 1, 1);
            GridBG.localPosition = new Vector3((GridX - 1) * 0.5f, (GridY - 2) * 0.5f, GridBG.localPosition.z);
            GridBorder.localScale = new Vector3(GridBG.localScale.x + 0.4f, GridBG.localScale.y + 0.4f, 1);
            GridBorder.localPosition = new Vector3(GridBG.localPosition.x, GridBG.localPosition.y, GridBorder.localPosition.z);
        }

        protected override void StartGame()
        {
            nextShapes = new List<Shape>();
            CreateShapesBag();
            SpawnShape();

            level.Value = 1;
            SetTetrisTimeScale();
        }

        private IEnumerator ClearLines(List<int> rows)
        {
            isClearingLines = true;
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < rows.Count; i++)
            {
                if (i > 0 && rows[i] < rows[i - 1])
                {
                    Debug.LogError("not in order");
                }
                MoveAllDown(rows[i] - i); //-i because they drop a row with every cleared row
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.4f);
            isClearingLines = false;
        }

        private bool MoveAllDown(int startRow)
        {
            bool movedSomething = false;
            for (int y = startRow; y < GridY - 1; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    if (GridX != grid.GetLength(0) ||
                        GridY != grid.GetLength(1))
                    {
                        Debug.LogError("grid altered");
                    }
                    else if (y == startRow && grid[x, y]) 
                    {
                        Debug.LogError("should have been destroyed");
                    }
                    else if (grid[x, y + 1]) 
                    {
                        grid[x, y] = grid[x, y + 1];
                        grid[x, y].transform.localPosition += Vector3.down;
                        movedSomething = true;
                    }
                    else
                    {
                        grid[x, y] = null;
                    }
                }
            }
            for (int x = 0; x < GridX; x++)
            {
                grid[x, GridY - 1] = null;
            }
            return movedSomething;
        }

        protected override void PlayUpdate()
        {
            if (isClearingLines)
            {
                return;
            }

            if (shape && canSwitch && inputController.XStart)
            {
                Shape newshape = null;
                if (holdShape)
                {
                    newshape = holdShape;
                    newshape.Reset(GridX/2, 18);
                }
                holdShape = shape;
                holdShape.Reset(-5, 10);
                shape = newshape;
                canSwitch = false;
            }
            if (!shape)
            {
                if (LandSound)
                {
                    AudioSource.PlayClipAtPoint(LandSound, Music.transform.position, 1.0f);
                }
                canSwitch = true;
                SpawnShape();
            }
            if (!shape.Tick(grid))
            {
                Destroy(shape.gameObject);
            }
        }

        private void SpawnShape()
        {
            shape = Instantiate(nextShapes[0], PlayField.transform);
            shape.Instantiate(GridX/2, 18, this, inputController);
            nextShapes.RemoveAt(0);
            if (!shape.Check(grid))
            {
                StartCoroutine(GameOver());
            }
            else
            {
                if (nextShapeDisplay)
                {
                    Destroy(nextShapeDisplay.gameObject);
                }

                if (nextShapes.Count == 0)
                {
                    CreateShapesBag();
                }

                nextShapeDisplay = Instantiate(nextShapes[0], PlayField.transform);
                nextShapeDisplay.Instantiate(GridX+4, 10, this, null);
            }
        }

        private void CreateShapesBag()
        {
            var newShapes = new List<Shape>(shapes.Length * 2);
            for (int i = 0; i < bagSize; i++)
            {
                newShapes.AddRange(shapes);
            }
            while (newShapes.Count > 0)
            {
                var index = UnityEngine.Random.Range(0, newShapes.Count);
                nextShapes.Add(newShapes[index]);
                newShapes.RemoveAt(index);
            }
        }

        public void CheckLines(List<int> rows)
        {
            int clears = 0;
            List<int> clearedRows = new List<int>();
            foreach (var row in rows)
            {
                bool full = true;
                for (int x = 0; x < GridX; x++)
                {
                    if (!grid[x, row])
                    {
                        full = false;
                        break;
                    }
                }
                if (full)
                {
                    clears++;
                    for (int x = 0; x < GridX; x++)
                    {
                        Destroy(grid[x, row]);
                    }
                    clearedRows.Add(row);
                }
            }
            if (clears > 0)
            {
                StartCoroutine(ClearLines(clearedRows));

                if (clears == 1)
                {
                    AddScore(100);
                }
                else if (clears == 2)
                {
                    AddScore(300);
                }
                else if (clears == 3)
                {
                    AddScore(500);
                }
                else //if (clears >= 4)
                {
                    AddScore(clears * 200);
                }

                levelCounter += clears;
                if (levelCounter > clearsToLevelUp)
                {
                    levelCounter -= clearsToLevelUp;
                    level.Value++;
                    SetTetrisTimeScale();
                }
            }
        }

        private void SetTetrisTimeScale()
        {
            //fallDelay = Mathf.Pow(0.8f - ((level.Value - 1) * 0.007f), level.Value - 1);
            fallDelay = startFallDelay * Mathf.Pow(fallDelayMultiplier, level.Value-1);
            //Debug.Log("fallDelay: " + fallDelay);
        }

        public void AddScore(int points)
        {
            Score.Value += Mathf.RoundToInt(points * (level.Value * 0.5f + 0.5f));
            if (ScoreSound)
            {
                AudioSource.PlayClipAtPoint(ScoreSound, Music.transform.position, 0.5f);
            }
        }

        protected override void OnGameOver()
        {
            for (int y = 0; y < GridY; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    if (grid[x, y])
                    {
                        Destroy(grid[x, y]);
                    }
                }
            }

            if (shape)
            {
                Destroy(shape.gameObject);
            }
            if (holdShape)
            {
                Destroy(holdShape.gameObject);
            }
            if (nextShapeDisplay)
            {
                Destroy(nextShapeDisplay.gameObject);
            }
        }
    }
}
