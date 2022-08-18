using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class Cell
{
    public int x { get; private set;}
    public int y { get; private set;}
    
    public bool visited = false;
    public bool isWall {get; private set; }
    private SpriteRenderer renderer;
    public Cell(int row, int col, int cellSize, GameObject wallPrefab, Transform parent)
    {
        Maze.NewMazeEvent += DestroyCell;  
        x = row * cellSize;
        y = col * cellSize;
   
        if (row % 2 != 0 || col % 2 != 0) isWall = true;
        else isWall = false;
        
        renderer = GameObject.Instantiate(wallPrefab, 
            new Vector3(x, y, 0) + parent.position, 
            Quaternion.identity, 
            parent)
            .GetComponent<SpriteRenderer>();
        renderer.color = isWall ? Color.black : Color.white;
    }
    public Cell(int row, int col, int cellSize, GameObject wallPrefab, Transform parent, bool isWall)
    {
       
        x = row * cellSize ;
        y = col * cellSize;
   
       
        renderer = GameObject.Instantiate(wallPrefab, 
                new Vector3(x, y, 0) + parent.position, 
                Quaternion.identity, 
                parent)
            .GetComponent<SpriteRenderer>();
     
        SetWall(isWall);
        // renderer = new SpriteRenderer();
        //renderer.color = Color.black;
        //renderer.sprite = sprite;

    }
    public void SetWall(bool isWall)
    {
        this.isWall = isWall;
        renderer.color = isWall ? Color.black : Color.white;
    }
    
    public void DestroyCell()
    {
        GameObject.Destroy(renderer.gameObject);
        
    }

    private void DestroyCell(Vector2Int newMazeSize)
    {
        if (x < newMazeSize.x && y < newMazeSize.y) return;
        Maze.NewMazeEvent -= DestroyCell;
        GameObject.Destroy(renderer.gameObject);
        
    }
    public void Reset()
    {
        if (x % 2 != 0 || y % 2 != 0) isWall = true;
        else isWall = false;
        SetWall(isWall);
        visited = false;
    }

}
