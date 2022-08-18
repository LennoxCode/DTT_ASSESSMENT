using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class PrimMazeAlgorithm
    {
        public BaseCell[,] GenerateGrid(int mazeWidth, int mazeHeight, GameObject wallPrefab, Transform transform)
        {
            BaseCell[,] cells = new Cell[mazeWidth, mazeHeight];
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                   // cells[x, y] = new Cell(x, y, 1, wallPrefab, transform);
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
            List<BaseCell> walls = new List<BaseCell>();
            
            yield return new WaitForSeconds(0.0f);
        }
    }
}