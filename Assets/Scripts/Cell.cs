using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int x { get; private set;}

    public int y { get; private set;}

    public bool visited = false;

    private int row;

    private int col;

    private bool isWall;
    private SpriteRenderer renderer;
    public Cell(int row, int col, int cellSize, GameObject wallPrefab)
    {
       
        x = row * cellSize;
        y = col * cellSize;
        this.row = row;
        this.col = col;
        if (row % 2 != 0 || col % 2 != 0) isWall = true;
        else isWall = false;
        renderer = GameObject.Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<SpriteRenderer>();
        renderer.color = isWall ? Color.black : Color.white;

        // renderer = new SpriteRenderer();
        //renderer.color = Color.black;
        //renderer.sprite = sprite;

    }

    public void SetWall(bool isWall)
    {
        this.isWall = isWall;
        renderer.color = isWall ? Color.black : Color.white;
    }
}
