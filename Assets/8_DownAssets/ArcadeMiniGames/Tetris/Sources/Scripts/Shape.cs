using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeMiniGames
{
    public class Shape : MonoBehaviour
    {
        //public int[,] grid;
        public Pos[] cells;
        private Pos centre;// = new Pos { x = 5, y = 16 };

        private float fallDelayRemaining;

        //public Material material;
        public GameObject block;
        [HideInInspector]
        public List<GameObject> blocks;

        private TetrisController tetrisController;
        private InputController inputController;

        public bool Check(GameObject[,] playGrid)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (!IsEmpty(playGrid, cells[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public void Instantiate(int x, int y, TetrisController tetrisController, InputController inputController)
        {
            transform.localPosition = new Vector3(x, y, 0);
            Pos move = new Pos { x = x - centre.x, y = y - centre.y };
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] += move;

                var newblock = Instantiate(block, transform);// new Vector3(cells[i].x, cells[i].y, 0), Quaternion.identity, transform);
                newblock.transform.localPosition = new Vector3(cells[i].x - x, cells[i].y - y, 0);
                newblock.transform.localRotation = Quaternion.identity;
                //newblock.GetComponent<MeshRenderer>().sharedMaterial = material;
                //newblock.transform.parent = transform;
                blocks.Add(newblock);
            }
            centre += move;

            this.tetrisController = tetrisController;
            this.inputController = inputController;

            fallDelayRemaining = tetrisController.fallDelay * 2;
        }

        public void Reset(int x, int y)
        {
            Pos move = new Pos { x = x - centre.x, y = y - centre.y };
            centre += move;
            transform.localPosition = new Vector3(centre.x, centre.y, 0);
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] += move;
                //blocks[i].transform.localPosition = new Vector3(cells[i].x, cells[i].y, 0);
            }
            fallDelayRemaining = tetrisController.fallDelay * 2;
        }

        public bool Tick(GameObject[,] playGrid)
        {
            if (inputController.LeftStart)
            {
                //fallDelay = maxfallDelay;
                Move(playGrid, Pos.Left);
            }
            if (inputController.RightStart)
            {
                //fallDelay = maxfallDelay;
                Move(playGrid, Pos.Right);
            }

            if (inputController.AStart)
            {
                TurnLeft(playGrid);
            }
            else if (inputController.BStart)
            {
                TurnRight(playGrid);
            }

            fallDelayRemaining -= Time.deltaTime;
            if (inputController.DownStart)
            {
                fallDelayRemaining = 0;
            }
            else if (inputController.Down)
            {
                fallDelayRemaining -= Time.deltaTime * 10;
            }
            if (fallDelayRemaining <= 0)
            {
                fallDelayRemaining = tetrisController.fallDelay;
                if (!Move(playGrid, Pos.Down))
                {
                    AddToGrid(playGrid);
                    return false;
                }
            }
            return true;
        }

        private void TurnRight(GameObject[,] playGrid)
        {
            var newCells = new Pos[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = cells[i];
                newCells[i].x = (cells[i].y - centre.y) + centre.x;
                newCells[i].y = (centre.x - cells[i].x) + centre.y;
                if (!IsEmpty(playGrid, newCells[i]))
                {
                    return;
                }
            }
            cells = newCells;
            for (int i = 0; i < cells.Length; i++)
            {
                blocks[i].transform.localPosition = new Vector3(cells[i].x - centre.x, cells[i].y - centre.y, 0);
            }
        }

        private void TurnLeft(GameObject[,] playGrid)
        {
            var newCells = new Pos[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = cells[i];
                newCells[i].x = (centre.y - cells[i].y) + centre.x;
                newCells[i].y = (cells[i].x - centre.x) + centre.y;
                if (!IsEmpty(playGrid, newCells[i]))
                {
                    return;
                }
            }
            cells = newCells;
            for (int i = 0; i < cells.Length; i++)
            {
                blocks[i].transform.localPosition = new Vector3(cells[i].x - centre.x, cells[i].y - centre.y, 0);
            }
        }

        private bool Move(GameObject[,] playGrid, Pos direction)
        {
            var newCells = new Pos[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = cells[i] + direction;
                if (!IsEmpty(playGrid, newCells[i]))
                {
                    return false;
                }
            }
            cells = newCells;
            centre += direction;
            transform.localPosition = new Vector3(centre.x, centre.y, 0);
            return true;
        }

        private bool IsEmpty(GameObject[,] playGrid, Pos cell)
        {
            if (cell.x < 0 || cell.x >= playGrid.GetLength(0) || cell.y < 0 || cell.y >= playGrid.GetLength(1)) //it can go out of the top
            {
                return false;
            }
            //if (cell.y >= playGrid.GetLength(1)) //it can go out of the top
            //{
            //    return true;
            //}
            return !playGrid[cell.x, cell.y];
        }

        private void AddToGrid(GameObject[,] playGrid)
        {
            var rows = new List<int>();
            for (int i = 0; i < cells.Length; i++)
            {
                Pos cell = cells[i];
                //if (cell.y < playGrid.GetLength(1))
                {
                    if (rows.Count == 0)
                    {
                        rows.Add(cell.y);
                    }
                    else if (!rows.Contains(cell.y))
                    {
                        //rows.Add(cell.y);
                        bool added = false;
                        for (int j = 0; j < rows.Count; j++)
                        {
                            if (cell.y < rows[j])
                            {
                                rows.Insert(j, cell.y);
                                added = true;
                                break;
                            }
                            //else if (j == rows.Count-1)
                            //{
                            //    rows.Add(cell.y);
                            //    break;
                            //}
                        }
                        if (!added)
                        {
                            rows.Add(cell.y);
                        }
                    }
                    if (playGrid[cell.x, cell.y])
                    {
                        Debug.LogError("overlapping blocks");
                    }
                    playGrid[cell.x, cell.y] = blocks[i];//true;
                    blocks[i].transform.parent = transform.parent;//null;
                }
            }
            //GameController.AddScore(10, transform.position);// centre.x, centre.y);
            tetrisController.CheckLines(rows);

            //GameController.AddToGrid(blocks);
        }
    }
}