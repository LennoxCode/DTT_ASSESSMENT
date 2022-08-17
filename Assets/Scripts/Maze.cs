using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private int cellSize;

    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] public float speed;
    
    // Start is called before the first frame update
    private Cell[,] cells;

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
    public void GenerateGrid2()
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
                cells[x, y] = new Cell(x, y, cellSize, wallPrefab, transform, true);
            }
        }
    }
    public void ButtonPressed()
    {
        StopAllCoroutines();
        GenerateGrid2();
        StartCoroutine(GenerateMazePrim());
        
    }
    private IEnumerator GenerateMaze()
    {
        Cell initialCell = cells[0, 0];
        var visitedCells = new Stack<Cell>();
        cells[0, 0].visited = true;
        cells[0, 0].SetWall(false);
        visitedCells.Push(cells[0,0]);
        GameObject cursor = Instantiate(wallPrefab, 
                new Vector3(initialCell.x, initialCell.y, 10) + transform.position, 
                Quaternion.identity, 
                transform)
            ;
        cursor.GetComponent<SpriteRenderer>().color = Color.magenta;
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
                    cursor.transform.position = new Vector3(neighbor.x, neighbor.y, -5) + transform.position;
                    if(speed > 0)yield return new WaitForSeconds(speed);
                    
                }
               
                visitedCells.Push(neighbor);
                
            }

            

        }
        yield return new WaitForSeconds(0.0f);
        Destroy(cursor);
        Debug.Log("finished Maze generation");
    }

    private IEnumerator GenerateMazePrim()
    {
        System.Random rng = new System.Random();
        Cell initialCell = cells[0, 0];
        //cells[0, 0].visited = true;
        cells[0, 0].SetWall(false);
        List<Cell> frontier = new List<Cell>();
        frontier.AddRange(GetNeighbors(initialCell));
        while (frontier.Count > 0)
        {
            Cell currCell = frontier[Random.Range(0, frontier.Count )];
            frontier.Remove(currCell);
            Cell neighborInMaze = GetNeighbors(currCell).OrderBy(a => rng.Next()).ToList().Find(cell => !cell.isWall);
            Vector2Int wallOffset =  new Vector2Int((currCell.x + neighborInMaze.x) / 2, (currCell.y + neighborInMaze.y) / 2);
            currCell.SetWall(false);
            cells[wallOffset.x, wallOffset.y].SetWall(false);
            frontier.AddRange(GetNeighbors(currCell).FindAll(cell => cell.isWall && !frontier.Contains(cell)));
            if(speed > 0)yield return new WaitForSeconds(speed);

        }
        yield return new WaitForSeconds(0.0f);
        Debug.Log("finished Maze generation");
        
    }
    private List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        if (cell.x > 1 && !cells[cell.x - 2, cell.y].visited) neighbors.Add(cells[cell.x - 2, cell.y]);
        if (cell.y > 1 && !cells[cell.x, cell.y - 2].visited) neighbors.Add(cells[cell.x , cell.y- 2]);
        if (cell.x < mazeWidth - 2 && !cells[cell.x + 2, cell.y].visited) neighbors.Add(cells[cell.x + 2, cell.y]);
        if (cell.y < mazeHeight - 2 && !cells[cell.x, cell.y + 2].visited) neighbors.Add(cells[cell.x, cell.y + 2]);
        return neighbors;
    }
    private List<Cell> getWalls(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        if (cell.x > 0 && !cells[cell.x - 1, cell.y].visited) neighbors.Add(cells[cell.x - 1, cell.y]);
        if (cell.y > 0 && !cells[cell.x, cell.y - 1].visited) neighbors.Add(cells[cell.x , cell.y- 1]);
        if (cell.x < mazeWidth - 1 && !cells[cell.x + 1, cell.y].visited) neighbors.Add(cells[cell.x + 1, cell.y]);
        if (cell.y < mazeHeight - 1 && !cells[cell.x, cell.y + 1].visited) neighbors.Add(cells[cell.x, cell.y + 1]);
        return neighbors;
    }
    public void SetWidth(int newWidth)
    {
        mazeWidth = newWidth;
    }

    public void SetHeight(int newHeight)
    {
        mazeHeight = newHeight;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
