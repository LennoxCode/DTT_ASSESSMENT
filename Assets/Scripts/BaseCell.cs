using UnityEngine;

namespace DefaultNamespace
{
    public abstract class BaseCell
    {
        public int x { get; protected set;}
        public int y { get; protected set;}
        public int xPos { get; protected set;}
        public int yPos { get; protected set;}
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

        public abstract void Reset();
    }
}