using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private int cellSize;

    [SerializeField] private int mazeSize;
    [SerializeField] private GameObject wallPrefab;
    private Cell[,] cells;
    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[mazeSize, mazeSize];
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                cells[x, y] = new Cell(x, y, cellSize, wallPrefab);
            }
        }
        StartCoroutine(GenerateMaze());
    }

    IEnumerator  GenerateMaze()
    {
        var visitedCells = new Stack<Cell>();
        cells[0, 0].visited = true;
        cells[0, 0].SetWall(false);
        visitedCells.Push(cells[0,0]);
        while (visitedCells.Count > 0)
        {
            Cell currCell = visitedCells.Pop();
            List<Cell> neighbors = GetNeighbors(currCell);
            if (neighbors.Count > 0)
            {
                visitedCells.Push(currCell);
                Cell neighbor = neighbors[Random.Range(0, neighbors.Count )];
                neighbor.visited = true;
                Vector2Int test =  new Vector2Int((currCell.x + neighbor.x) / 2, (currCell.y + neighbor.y) / 2);
                cells[test.x, test.y].SetWall(false);
                visitedCells.Push(neighbor);
            }

            yield return new WaitForSeconds(0.1f);

        }
        Debug.Log("finished Maze generation");
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> reti = new List<Cell>();
        if (cell.x > 1 && !cells[cell.x - 2, cell.y].visited) reti.Add(cells[cell.x - 2, cell.y]);
        if (cell.y > 1 && !cells[cell.x, cell.y - 2].visited) reti.Add(cells[cell.x , cell.y- 2]);
        if (cell.x < mazeSize - 2 && !cells[cell.x + 2, cell.y].visited) reti.Add(cells[cell.x + 2, cell.y]);
        if (cell.y < mazeSize - 2 && !cells[cell.x, cell.y + 2].visited) reti.Add(cells[cell.x, cell.y + 2]);
        return reti;
    }
}
