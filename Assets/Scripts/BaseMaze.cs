
using System;
using System.Collections;
using System.Collections.Generic;
using MyNamespace;
using UnityEngine;
namespace DefaultNamespace
{
    /// <summary>
    /// Base class which represents a maze. provided base-functionality for setting attributes. Each derives
    /// class has to implement the abstract methods to generate Mazes.
    /// </summary>
    public abstract class BaseMaze : MonoBehaviour
    {
        public static Action<Vector2Int> NewMazeEvent;
        public Action OnMazeGenFinished;
        [SerializeField] protected int cellSize;
        [SerializeField] protected int mazeWidth;
        [SerializeField] protected int mazeHeight;
        [SerializeField] protected float speed;
        [SerializeField] protected GameObject cursor;
        [SerializeField] protected GameObject wallPrefab;
        protected BaseCell[,] cells;
        protected BaseCell startCell;
        protected BaseCell endCell;
        //generates a grid determined by the width and height attribute to bused in the maze geneartion
        public abstract void GenerateGrid(bool wallsOnly);
        //generates a maze using depth first
        protected abstract IEnumerator GenerateMaze();
        //generates a maze using random prim algorithm
        protected abstract IEnumerator GenerateMazePrim();
        //generates a maze using random Kruskal algorithm
        protected abstract IEnumerator GenerateMazeKruskal();
        //returns all neighbors which are reachable(not a wall)
        protected abstract List<BaseCell> GetReachableNeighbors(BaseCell cell);
        //selects two random and distinct tiles to be start/finish of the maze and colors them accordingly 
        protected abstract void SetStartFinish();
        private void Start()
        {
            OnMazeGenFinished += SetStartFinish;
        }
        /// <summary>
        /// a public wrapper function which enables the controller to select grid options without further worrying
        /// about the individual implementations of each algorithm and the preparation needed to run it. 
        /// </summary>
        /// <param name="algorithm">An enum which represents any of the possible options for maze generation</param>
        public  void RunGeneration(MazeAlgorithm algorithm)
        {
            // stopping old maze generation if it is already running
            StopAllCoroutines();
            switch (algorithm)
            {
                case MazeAlgorithm.DepthFirst:
                    GenerateGrid(false);
                    StartCoroutine(GenerateMaze());
                    break;
                case MazeAlgorithm.Prim:
                    GenerateGrid(true);
                    StartCoroutine(GenerateMazePrim());
                    break;
                case MazeAlgorithm.Kruskal:
                    GenerateGrid(false);
                    StartCoroutine(GenerateMazeKruskal());
                    break;
            }

        
        }
        /// <summary>
        /// finds the path between the start and finish of the maze
        /// </summary>
        /// <remarks>
        /// the used algorithm is depth first as well. normally one would use A* or Dijkstra for pathfinding. However
        /// due to the nature of a perfect maze not containing any loops thus no alternative paths. A path between two
        /// points is always the shortest path. I used a dictionary to backtrack my path after I am finished.
        /// every node points to its predecessor. There is also n 
        /// </remarks>
        /// <returns>a list of cells which are in the path</returns>
        public List<BaseCell> Solve()
        {
            if (startCell == null || endCell == null) return new List<BaseCell>();
            var frontier = new Stack<BaseCell>();
            var breadCrumbs = new Dictionary<BaseCell, BaseCell>();
            var route = new List<BaseCell>();
            breadCrumbs[startCell] = null;
            frontier.Push(startCell);
            while (frontier.Count > 0)
            {
                var currentCell = frontier.Pop();
                if (currentCell == endCell) break;
                foreach (var neighbor in GetReachableNeighbors(currentCell) )
                {
                    if (breadCrumbs.ContainsKey(neighbor)) continue;
                    frontier.Push(neighbor);
                    breadCrumbs[neighbor] = currentCell;
                }

            
            }
            var cell = endCell;
            while (cell != null)
            {
                route.Add(cell);
                cell = breadCrumbs[cell];
            }
            return route;
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
}