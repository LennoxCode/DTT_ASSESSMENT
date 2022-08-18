using UnityEngine;

/// <summary>
/// a class which represents all everything that belongs to a given cell e.g. position, if it is a wall, and
/// if it has been visited by the algorithm so far. In addition every cell holds a reference to a <c> SpriteRenderer</c>
/// to enable it to change color based on the wall state. saving the coordinates in grid space on cells enables
/// easier determination of neighborhood relations
/// </summary>
public class Cell
{
    public int x { get; private set;}
    public int y { get; private set;}
    public int xPos { get; private set;}
    public int yPos { get; private set;}
    public bool visited = false;
    public bool isWall {get; private set; }
    private SpriteRenderer renderer;
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
    {
        Maze.NewMazeEvent += DestroyCell;
        x = row;
        y = col;
        xPos = row * cellSize;
        xPos = col * cellSize;
        if (row % 2 != 0 || col % 2 != 0) isWall = true;
        else isWall = false;
        
        renderer = Object.Instantiate(wallPrefab, 
            new Vector3(x, y, 0) + parent.position, 
            Quaternion.identity, 
            parent)
            .GetComponent<SpriteRenderer>();
        renderer.color = isWall ? Color.black : Color.white;
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
       
        x = row * cellSize ;
        y = col * cellSize;
   
       
        renderer = Object.Instantiate(wallPrefab, 
                new Vector3(x, y, 0) + parent.position, 
                Quaternion.identity, 
                parent)
            .GetComponent<SpriteRenderer>();
     
        SetWall(isWall);
        // renderer = new SpriteRenderer();
        //renderer.color = Color.black;
        //renderer.sprite = sprite;

    }
    /// <summary>
    /// sets the isWall variable and automatically changes appearance based on the new value
    /// </summary>
    /// <param name="isWall"> the parameter to set</param>
    public void SetWall(bool isWall)
    {
        this.isWall = isWall;
        renderer.color = isWall ? Color.black : Color.white;
    }
    /// <summary>
    /// Destroys a given cell
    /// </summary>
    public void DestroyCell()
    {
        Maze.NewMazeEvent -= DestroyCell;
        Object.Destroy(renderer.gameObject);
    }
    /// <summary>
    /// Destroys a given cell if it outside of the bound of the maze size. this is only called with an event
    /// </summary>
    /// <param name="newMazeSize"></param>
    private void DestroyCell(Vector2Int newMazeSize)
    {
        if (x < newMazeSize.x && y < newMazeSize.y) return;
        DestroyCell();
    }
    /// <summary>
    /// Resets a cell to the initial state so an alternating pattern is present again
    /// </summary>
    public void Reset()
    {
        if (x % 2 != 0 || y % 2 != 0) isWall = true;
        else isWall = false;
        SetWall(isWall);
        visited = false;
    }

}
