using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMazeAlgorithm
{
    public Cell[,] GenerateGrid(int mazeWidth, int mazeHeight, GameObject wallPrefab, Transform transform);

    public IEnumerator GenerateMaze(Cell[,] cells,float speed, GameObject wallPrefab);
}
