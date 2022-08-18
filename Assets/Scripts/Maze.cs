using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// this class represents the model for maze generation. It holds values for all relevant maze options like the size
/// of the maze and a private reference to a two dimensional array of cell objects which represent on point in the grid
/// This class provided all the necessary functions and operations to generate maze 
/// </summary>
public class Maze : MonoBehaviour
{
    [SerializeField] private int cellSize;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float speed;
    [SerializeField] private GameObject cursor;
    public static Action<Vector2Int> NewMazeEvent;
    public Action OnMazeGenFinished;
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
    /// <summary>
    /// generates a new grid of the size given my the class variables. if another grid already exists the already
    /// present cells are reused instead of destroyed because the instantiation of the GameObjects is very expensive.
    /// </summary>
    public void GenerateGrid()
    {

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
    /// <summary>
    /// this function also generates a grid of the given size. The main difference is that this grid exclusively
    /// consists of walls because the Prim algorithm demands a grid of walls 
    /// </summary>
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
    /// <summary>
    /// a public wrapper function which enables the controller to select grid options without further worrying
    /// about the individual implementations of each algorithm and the preparation needed to run it. 
    /// </summary>
    /// <param name="algorithm">An enum which represents any of the possible options for maze generation</param>
    public void RunGeneration(MazeAlgorithm algorithm)
    {
        // stopping old maze generation if it is already running
        StopAllCoroutines();
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
    
    /// <summary>
    /// <c>GenerateMaze</c> uses the depth first algorithm to generate a perfect maze.   
    /// </summary>
    /// <remarks>
    /// This algorithm is primitive
    /// compared to the other ones. It holds a stack which references each cell which should be visited.
    /// In each iteration the stack is popped and one of the neighbors is randomly selected and the wall is deleted if
    /// it exists. This is done until no unvisited cells are left.
    /// </remarks>
    /// <returns> WaitForSeconds which is changed in the interfaceController and sets the delay after each step of the
    /// algorithm. if the timer is zero there is no delay. using a coroutine for the implementation also
    /// opens the door for parallel execution of the algorithm because multiple coroutines can be started
    /// and are thread safe which enables concurrency 
    /// is applied
    /// </returns>
    private IEnumerator GenerateMaze()
    {
        var initialCell = cells[0, 0];
        var frontier = new Stack<Cell>();
        initialCell.visited = true;
        initialCell.SetWall(false);
        frontier.Push(initialCell);
        //initiating an additional sprite renderer to show where the algorithm is modifying walls
        SetCursor(new Vector2Int(initialCell.x, initialCell.y));
        while (frontier.Count > 0)
        {
            var currCell = frontier.Pop();
            var neighbors = GetNeighbors(currCell);

            if (neighbors.Count <= 0) continue;
            frontier.Push(currCell);
            var neighbor = neighbors[Random.Range(0, neighbors.Count )];
            neighbor.visited = true;
            // the position of the wall can be easily calculated by just calculating the median
            var wallPos =  new Vector2Int((currCell.x + neighbor.x) / 2, (currCell.y + neighbor.y) / 2);
            // theoretically it is redundant to see if a wall is present. however this enables me to only 
            // use the delay if the a wall got deleted which speeds up the animation for large mazes.
            if (cells[wallPos.x, wallPos.y].isWall)
            {
                cells[wallPos.x, wallPos.y].SetWall(false);
              
                // only return if speed is greater zero because WaitForSeconds(0) still waits for next frame
                if (speed > 0)
                {
                    SetCursor(new Vector2Int(neighbor.x, neighbor.y));
                    SetCursor(wallPos);
                    yield return new WaitForSeconds(speed);
                }
                    
            }
            frontier.Push(neighbor);
        }
        OnMazeGenFinished?.Invoke();
        yield return new WaitForSeconds(0.0f);
        Debug.Log("finished Maze generation");
    }
    /// <summary>
    /// <c>GenerateMazePrim</c> uses the random prim algorithm to generate a perfect maze.   
    /// </summary>
    /// <remarks>
    /// This algorithm works by first creating a maze full of walls.
    /// in the initial step the first cell is selected, and is turned into part of the maze. All neighbors of this cell
    /// are added to the frontier set which is a list of cells
    /// in each iteration a random frontier wall cell is chosen, and in addition a random neighbor of this cell
    /// which is part of the maze is selected and the wall between them is destroyed.
    /// the algorithm terminates when the frontier is empty.
    /// </remarks>
    /// <returns> WaitForSeconds which sets the delay after each step of the algorithm. 
    /// </returns>
    private IEnumerator GenerateMazePrim()
    {
        var rng = new System.Random();
        var initialCell = cells[0, 0];
        cells[0, 0].SetWall(false);
        
        var frontier = new List<Cell>();
        frontier.AddRange(GetNeighbors(initialCell));
        while (frontier.Count > 0)
        {
            var currCell = frontier[Random.Range(0, frontier.Count )];
            frontier.Remove(currCell);
            //finding random neighbor which is already a cell
            var neighborInMaze = GetNeighbors(currCell).OrderBy(
                a => rng.Next())
                .ToList().Find(cell => !cell.isWall);
            var wallPos =  new Vector2Int((currCell.x + neighborInMaze.x) / 2, (currCell.y + neighborInMaze.y) / 2);
            currCell.SetWall(false);
            cells[wallPos.x, wallPos.y].SetWall(false);
            
            frontier.AddRange(GetNeighbors(currCell).FindAll(cell => cell.isWall && !frontier.Contains(cell)));
            if (!(speed > 0)) continue;
            SetCursor(new Vector2Int(currCell.x, currCell.y));
            SetCursor(wallPos);
            yield return new WaitForSeconds(speed);
            

        }
        OnMazeGenFinished?.Invoke();
        yield return new WaitForSeconds(0.0f);
        Debug.Log("finished Maze generation");
    }
    /// <summary>
    /// <c>GenerateMazeKruskal</c> uses Kruskal's algorithm to generate a perfect maze.   
    /// </summary>
    /// <remarks>
    /// This algorithm works by first creating a normal maze, and putting each cell into a separate <c> HashSet </c>,
    /// and keeping a list of all none corner walls. 
    /// in each iteration a random wall is chosen, and the algorithm looks if the cell divided by it belong to
    /// two different sets. if this is the case this means there are not yet connected and the wall is deleted and
    /// the sets are merged. if they are not distinct they are already connected and it would lead to loops thus
    /// the wall is not destroyed
    /// the algorithm terminates when there is only one set left which means that every cell is connected.
    /// </remarks>
    /// <returns> WaitForSeconds which sets the delay after each step of the algorithm. 
    /// </returns>
    private IEnumerator GenerateMazeKruskal()
    {
        var cellSets = new HashSet<Cell>[mazeWidth, mazeHeight];
        var walls = new List<Cell>();
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
                else if(x % 2 != 0 ^ y % 2 != 0 && (!(mazeWidth % 2 ==0 && x == mazeWidth-1) && !(mazeHeight % 2 ==0 && y == mazeHeight-1)))
                {
                    walls.Add((cells[x, y]));
                }
               
            }
        }
        while (setCount > 1)
        {
            var currWall = walls[Random.Range(0, walls.Count)];
            walls.Remove(currWall);
            HashSet<Cell> set1;
            HashSet<Cell> set2;
            if (currWall.x % 2 != 0 && currWall.y % 2 == 0)
            {
                set1 = cellSets[currWall.x - 1, currWall.y];
                set2 = cellSets[currWall.x + 1, currWall.y];
             
            }
            else
            {
                set1 = cellSets[currWall.x, currWall.y - 1];
                set2 = cellSets[currWall.x, currWall.y + 1];
            }
            if (set1.SetEquals(set2)) continue;
            currWall.SetWall(false);
            set1.UnionWith(set2);
            foreach (var cell in set1)
            {
                cellSets[cell.x, cell.y] = set1;
            }
            setCount--;
            if (!(speed > 0)) continue;
            yield return new WaitForSeconds(speed);
            //SetCursor(new Vector2Int(currCell.x, currCell.y));
            SetCursor(new Vector2Int(currWall.x, currWall.y));

        }
        OnMazeGenFinished?.Invoke();
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
    /// <summary>
    /// <c> GetNeighbors </c> retrieves the neighbors of a given cell
    /// </summary>
    /// <param name="cell"> The cell of which to retrieve the neighbors</param>
    /// <returns> A list of cells which are in bounds and yet not visited </returns>
    private List<Cell> GetNeighbors(Cell cell)
    {
        var neighbors = new List<Cell>();
        if (cell.x > 1 && !cells[cell.x - 2, cell.y].visited) neighbors.Add(cells[cell.x - 2, cell.y]);
        if (cell.y > 1 && !cells[cell.x, cell.y - 2].visited) neighbors.Add(cells[cell.x , cell.y- 2]);
        if (cell.x < mazeWidth - 2 && !cells[cell.x + 2, cell.y].visited) neighbors.Add(cells[cell.x + 2, cell.y]);
        if (cell.y < mazeHeight - 2 && !cells[cell.x, cell.y + 2].visited) neighbors.Add(cells[cell.x, cell.y + 2]);
        return neighbors;
    }
    /// <summary>
    /// <c> GetNeighbors </c> retrieves the neighboring walls of a given cell
    /// </summary>
    /// <param name="cell"> The cell of which to retrieve the neighbors</param>
    /// <returns> A list of walls which are in bounds and yet not visited </returns>
    private List<Cell> GetWalls(Cell cell)
    {
        var neighbors = new List<Cell>();
        if (cell.x > 0 && !cells[cell.x - 1, cell.y].visited) neighbors.Add(cells[cell.x - 1, cell.y]);
        if (cell.y > 0 && !cells[cell.x, cell.y - 1].visited) neighbors.Add(cells[cell.x , cell.y- 1]);
        if (cell.x < mazeWidth - 1 && !cells[cell.x + 1, cell.y].visited) neighbors.Add(cells[cell.x + 1, cell.y]);
        if (cell.y < mazeHeight - 1 && !cells[cell.x, cell.y + 1].visited) neighbors.Add(cells[cell.x, cell.y + 1]);
        return neighbors;
    }

    private void SetCursor(Vector2Int at)
    {
        Instantiate(cursor, 
                new Vector3(at.x, at.y, -5) + transform.position, 
                Quaternion.identity, 
                transform)
            ;
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
