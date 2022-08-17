using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class PrimMazeAlgorithm : IMazeAlgorithm
    {
        public Cell[,] GenerateGrid(int mazeWidth, int mazeHeight, GameObject wallPrefab, Transform transform)
        {
            Cell[,] cells = new Cell[mazeWidth, mazeHeight];
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    cells[x, y] = new Cell(x, y, 1, wallPrefab, transform);
                }
            }

            return cells;
        }

        public IEnumerator GenerateMaze(Cell[,] cells, float speed, GameObject wallPrefab)
        {
            //HashSet<Cell>[,] cellSets = new HashSet<Cell>[cells[]];
            Cell inital = cells[0, 0];
            inital.SetWall(false);
            inital.visited = true;
            List<Cell> walls = new List<Cell>();
            
            yield return new WaitForSeconds(0.0f);
        }
    }
}