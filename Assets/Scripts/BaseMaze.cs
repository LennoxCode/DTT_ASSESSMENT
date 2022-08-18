
using System;
using System.Collections;
using System.Collections.Generic;
using MyNamespace;
using UnityEngine;
namespace DefaultNamespace
{
    
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
        
        public abstract void RunGeneration(MazeAlgorithm algorithm);
        public abstract void GenerateGrid();
        protected abstract IEnumerator GenerateMaze();
        protected abstract IEnumerator GenerateMazePrim();
        protected abstract IEnumerator GenerateMazeKruskal();
        
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