using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyNamespace;
using UnityEngine;
using Random = UnityEngine.Random;


public class Maze : MonoBehaviour
{
    [SerializeField] private int cellSize;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float speed;

    public static Action<Vector2Int> NewMazeEvent;
    private Cell[,] cells;

    private void ClearGrid()
    {
        if (cells != null)
        {
            foreach (var cell in cells)
            {
                cell.DestroyCell();
            }
        }
    }
    public void GenerateGrid()
    {
       // ClearGrid();
        
        if (cells != null)
        {
            var oldCells = cells;
            cells = new Cell[mazeWidth, mazeHeight];
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    if (x < oldCells.GetLength(0) && y < oldCells.GetLength(1))
                    {
                        cells[x,y] = oldCells[x, y];
                        cells[x,y].Reset();
                    }
                    else cells[x, y] = new Cell(x, y, cellSize, wallPrefab, transform);
                }
            }

        }
        else
        {
            cells = new Cell[mazeWidth, mazeHeight];
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    cells[x, y] = new Cell(x, y, cellSize, wallPrefab, transform);
                }
            }
        }
        NewMazeEvent?.Invoke(new Vector2Int(mazeWidth ,mazeHeight));
    }
    private void GenerateGridWallsOnly()
    {
        ClearGrid();
        cells = new Cell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                cells[x, y] = new Cell(x, y, cellSize, wallPrefab, transform, true);
            }
        }
    }
    public void RunGeneration(MazeAlgorithm algorithm)
    {
        StopAllCoroutines();
       // ClearGrid();
        switch (algorithm)
        {
            case MazeAlgorithm.DepthFirst:
                GenerateGrid();
                StartCoroutine(GenerateMaze());
                break;
            case MazeAlgorithm.Prim:
                GenerateGridWallsOnly();
                StartCoroutine(GenerateMazePrim());
                break;
            case MazeAlgorithm.Kruskal:
                GenerateGrid();
                StartCoroutine(GenerateMazeKruskal());
                break; 
        }
        
       
        
    }
    private IEnumerator GenerateMaze()
    {
        var initialCell = cells[0, 0];
        var visitedCells = new Stack<Cell>();
        cells[0, 0].visited = true;
        cells[0, 0].SetWall(false);
        visitedCells.Push(cells[0,0]);
        //initiating an additional sprite renderer to show where the algorithm is modifying walls
        var transform1 = transform;
        var cursor = Instantiate(wallPrefab, 
                new Vector3(initialCell.x, initialCell.y, 10) + transform1.position, 
                Quaternion.identity, 
                transform1)
            ;
        cursor.GetComponent<SpriteRenderer>().color = Color.magenta;
        while (visitedCells.Count > 0)
        {
            var currCell = visitedCells.Pop();
            var neighbors = GetNeighbors(currCell);

            if (neighbors.Count <= 0) continue;
            visitedCells.Push(currCell);
            var neighbor = neighbors[Random.Range(0, neighbors.Count )];
            neighbor.visited = true;
            var wallOffset =  new Vector2Int((currCell.x + neighbor.x) / 2, (currCell.y + neighbor.y) / 2);
            if (cells[wallOffset.x, wallOffset.y].isWall)
            {
                cells[wallOffset.x, wallOffset.y].SetWall(false);
                cursor.transform.position = new Vector3(neighbor.x, neighbor.y, -5) + transform.position;
                if(speed > 0)yield return new WaitForSeconds(speed);
                    
            }
            visitedCells.Push(neighbor);
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

    private IEnumerator GenerateMazeKruskal()
    {
        HashSet<Cell>[,] cellSets = new HashSet<Cell>[mazeWidth, mazeHeight];
        List<Cell> walls = new List<Cell>();
        int setCount = 0;
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (!cells[x, y].isWall)
                {
                    cellSets[x, y] = new HashSet<Cell> {cells[x, y]};
                    setCount++;
                }
                else if(x % 2 != 0 ^ y % 2 != 0)
                {
                    walls.Add((cells[x, y]));
                }
               
            }
        }
        while (setCount > 1)
        {
            Cell currWall = walls[Random.Range(0, walls.Count)];
            walls.Remove(currWall);
            if (currWall.x % 2 != 0 && currWall.y % 2 == 0)
            {
                HashSet <Cell> set1 = cellSets[currWall.x - 1, currWall.y];
                HashSet <Cell> set2 = cellSets[currWall.x + 1, currWall.y];
                if (!set1.SetEquals(set2))
                {
                    
                    currWall.SetWall(false);
                    set1.UnionWith(set2);
                    foreach (var cell in set1)
                    {
                        cellSets[cell.x, cell.y] = set1;
                    }
                    setCount--;
                }
            }
            else
            {
                HashSet <Cell> set1 = cellSets[currWall.x, currWall.y - 1];
                HashSet <Cell> set2 = cellSets[currWall.x, currWall.y + 1];
                if (!set1.SetEquals(set2))
                {
                    currWall.SetWall(false);
                    set1.UnionWith(set2);
                    foreach (var cell in set1)
                    {
                        cellSets[cell.x, cell.y] = set1;
                    }
                    //Debug.LogError(set1.Count);
                    setCount--;
                }
                
            }
            if(speed > 0)yield return new WaitForSeconds(speed);
        }
        yield return new WaitForSeconds(0.0f);
        Debug.Log("finished Maze generation");
    }

    private IEnumerator GenerateMazeEller()
    {
        List<HashSet<Cell>> cellSets = new List<HashSet<Cell>>();
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                cellSets.Add(new HashSet<Cell> {cells[x, y]});
            }
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
