using DefaultNamespace;
using UnityEngine;

/// <summary>
/// represents a cell in a rectangular grid. the main difference to the base cell is how it is determined if the
/// wall is a cell or not and how to world position is calculated from the grid position.
/// </summary>
public class Cell : BaseCell
{
    /// <summary>
    /// creates a new cell in using the given parameters. It is automatically decided if the cell is a wall or not
    /// to create an alternating pattern of wall and cell.
    /// </summary>
    /// <param name="row"> the row in grid space of the cell</param>
    /// <param name="col"> the column in grid space of the cell</param>
    /// <param name="cellSize">the physical size of the cell</param>
    /// <param name="wallPrefab">prefab for the visual cell</param>
    /// <param name="parent"> parent transform to instantiate as child of maze</param>
    public Cell(int row, int col, int cellSize, GameObject wallPrefab, Transform parent) 
        : this(row, col, cellSize, wallPrefab, parent, (row % 2 != 0 || col % 2 != 0))
    {
    }
    /// <summary>
    /// creates a new cell in using the given parameters. It is automatically decided if the cell is a wall or not
    /// to create an alternating pattern of wall and cell.
    /// </summary>
    /// <param name="row"> the row in grid space of the cell</param>
    /// <param name="col"> the column in grid space of the cell</param>
    /// <param name="cellSize">the physical size of the cell</param>
    /// <param name="wallPrefab">prefab for the visual cell</param>
    /// <param name="parent"> parent transform to instantiate as child of maze</param>
    /// <param name="isWall"> if the cell is a wall or not</param>

    public Cell(int row, int col, int cellSize, GameObject wallPrefab, Transform parent, bool isWall)
    {
        var parentPos = parent.position;
        x = row;
        y = col;
        xPos = row * cellSize + parentPos.x;
        yPos = col * cellSize + parentPos.y;

        Maze.NewMazeEvent += DestroyCell;
        renderer = Object.Instantiate(wallPrefab, 
                new Vector3(xPos, yPos, 0), 
                Quaternion.identity, 
                parent)
            .GetComponent<SpriteRenderer>();
        SetWall(isWall); 
    }
    /// <summary>
    /// Resets a cell to the initial state so an alternating pattern is present again
    /// </summary>
    public override void Reset()
    {
        if (x % 2 != 0 || y % 2 != 0) isWall = true;
        else isWall = false;
        SetWall(isWall);
        visited = false;
    }

}
