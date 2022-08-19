using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// a class which represents all everything that belongs to a given cell e.g. position, if it is a wall, and
    /// if it has been visited by the algorithm so far. In addition every cell holds a reference to a <c> SpriteRenderer</c>
    /// to enable it to change color based on the wall state. saving the coordinates in grid space on cells enables
    /// easier determination of neighborhood relations
    /// </summary>
    public abstract class BaseCell
    {
        public int x { get; protected set;}
        public int y { get; protected set;}
        public float xPos { get; protected set;}
        public float yPos { get; protected set;}
        public bool visited = false;
        public bool isWall {get; protected set; }
        protected SpriteRenderer renderer;
        
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
        protected void DestroyCell(Vector2Int newMazeSize)
        {
            if (x < newMazeSize.x && y < newMazeSize.y) return;
            DestroyCell();
        }

        public void SetColor(Color color)
        {
            renderer.color = color;
        }
        public abstract void Reset();
    }
}