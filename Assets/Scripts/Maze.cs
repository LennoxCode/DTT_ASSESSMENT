using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private int cellSize;

    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] public float speed;
    private Cell[,] cells;
    // Start is called before the first frame update
  

    public void GenerateGrid()
    {
        if (cells != null)
        {
            foreach (var cell in cells)
            {
                cell.DestroyCell();
            }
        }
        cells = new Cell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                cells[x, y] = new Cell(x, y, cellSize, wallPrefab, transform);
            }
        }
    }

    public void ButtonPressed()
    {
        StopAllCoroutines();
        GenerateGrid();
        StartCoroutine(GenerateMaze());
        
    }
    private IEnumerator GenerateMaze()
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
                Vector2Int wallOffset =  new Vector2Int((currCell.x + neighbor.x) / 2, (currCell.y + neighbor.y) / 2);
                if (cells[wallOffset.x, wallOffset.y].isWall)
                {
                    cells[wallOffset.x, wallOffset.y].SetWall(false);
                    if(speed > 0)yield return new WaitForSeconds(speed);
                }
               
                visitedCells.Push(neighbor);
                
            }

            

        }
        yield return new WaitForSeconds(0.0f);
        Debug.Log("finished Maze generation");
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> reti = new List<Cell>();
        if (cell.x > 1 && !cells[cell.x - 2, cell.y].visited) reti.Add(cells[cell.x - 2, cell.y]);
        if (cell.y > 1 && !cells[cell.x, cell.y - 2].visited) reti.Add(cells[cell.x , cell.y- 2]);
        if (cell.x < mazeWidth - 2 && !cells[cell.x + 2, cell.y].visited) reti.Add(cells[cell.x + 2, cell.y]);
        if (cell.y < mazeHeight - 2 && !cells[cell.x, cell.y + 2].visited) reti.Add(cells[cell.x, cell.y + 2]);
        return reti;
    }

    public void SetWidth(int newWidth)
    {
        mazeWidth = newWidth;
    }

    public void SetHeight(int newHeight)
    {
        mazeHeight = newHeight;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
}
