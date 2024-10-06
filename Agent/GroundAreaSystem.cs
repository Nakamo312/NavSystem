using GroundSystem;
using NavSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;


namespace Sample
{
    public class GroundAreaSystem : MonoBehaviour
    {
        private NavSystem.GroundArea groundArea;
        public static GroundPathFinder pathFinder;

        [SerializeField]
        private BuildingGrid _buildings;

        public int sizeX;
        public int sizeY;

        private void Start()
        {
            sizeX = _buildings.GridSize.x;
            sizeY = _buildings.GridSize.y;

            bool[,] matrix = new bool[this.sizeY, this.sizeX];


            for(int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (_buildings.buildings[i, j] == null) matrix[i, j] = true;
                    else matrix[i, j] = false;
                }
            }
            groundArea = new GroundArea(matrix);
            pathFinder = new GroundPathFinder(groundArea);
            Debug.Log($"Карта создана: {groundArea}");

            _buildings.placeBuild += OnPlaceBuild;
        }

        public void OnPlaceBuild(List<Vector2Int> cells)
        {
            foreach (var cell in cells)
            {
                groundArea.SetCell(cell.x, cell.y, false);
                Debug.Log($"в координатах {cell.x} {cell.y}: false");
            }
        }
    }

}