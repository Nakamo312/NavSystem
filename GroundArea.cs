using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace NavSystem
{
    public class GroundArea
    {
        private bool[,] matrix;
        private Vector2Int size;

        public Vector2Int Size 
        {
            set
            {
                size = value;
            }
            
            get => size; 
        }
        public GroundArea(bool[,] matrix)
        {
            Setup(matrix);

        }
        public void Setup(bool[,] _matrix)
        {
            Size = new Vector2Int(_matrix.GetLength(0), _matrix.GetLength(1));
            matrix = new bool[size.x, size.y];

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    matrix[i,j] = _matrix[i,j];
                }
            }
        }
        public bool HasCell(int x, int y)
        {
            if (x < 0 || x >= Size.x
                || y < 0 || y >= Size.y)
            {
                return false;
            }

            return matrix[Size.y - y -1, x];
        }
        public bool SetCell(int x, int y, bool value)
        {
            if (x < 0 || x >= Size.x
               || y < 0 || y >= Size.y)
            {
                return false;
            }
            matrix[x,y] = value;
            return true;
        }
        public bool HasCell(Vector2Int coordCell)
        {
            if (coordCell.x < 0 || coordCell.x >= Size.x
                || coordCell.y < 0 || coordCell.y >= Size.y)
            {
                return false;
            }

            return matrix[coordCell.x, coordCell.y];
        }
        public List<GroundCell> GetNeighboors(GroundCell cell)
        {
            return GetNeighboors(cell.x, cell.y);
        }
        public List<GroundCell> GetNeighboors(int x, int y)
        {
            List<GroundCell> result = new List<GroundCell>();
            if (this.HasCell(x - 1, y - 1)) result.Add(new GroundCell(x - 1, y - 1));
            if (this.HasCell(x - 1, y)) result.Add(new GroundCell(x - 1, y));
            if (this.HasCell(x - 1, y + 1)) result.Add(new GroundCell(x - 1, y + 1));
            if (this.HasCell(x, y - 1)) result.Add(new GroundCell(x, y - 1));
            if (this.HasCell(x, y + 1)) result.Add(new GroundCell(x, y + 1));
            if (this.HasCell(x + 1, y - 1)) result.Add(new GroundCell(x + 1, y - 1));
            if (this.HasCell(x + 1, y)) result.Add(new GroundCell(x + 1, y));
            if (this.HasCell(x + 1, y + 1)) result.Add(new GroundCell(x + 1, y + 1));
            return result;
        }
        public bool TryGetCell(Vector3 point,  out GroundCell cell)
        {
            return TryGetCell(point.x, point.z, out cell);
        }
        public bool TryGetCell(float x, float y, out GroundCell cell)
        {
            x = Mathf.Max(x, 0);
            y = Mathf.Max(y, 0);

            int gridX = (int) x;
            int gridY = (int) y;

            if (HasCell(gridX, gridY))
            {
                cell = new GroundCell(gridX, gridY);
                return true;
            }

            var dx = x - gridX;
            var dy = y - gridY;
            if (dx == 0 && dy > 0)
            {
                if(HasCell(gridX - 1, gridY))
                {
                    cell = new GroundCell(gridX - 1, gridY);
                    return true;
                }
                cell = default;
                return false;
            }

            if (dx > 0 && dy == 0)
            {
                if (HasCell(gridX, gridY - 1))
                {
                    cell = new GroundCell(gridX, gridY - 1);
                    return true;
                }
                cell = default;
                return false;
            }
            if (dx == 0 && dy == 0)
            {
                if (HasCell(gridX - 1, gridY))
                {
                    cell = new GroundCell(gridX - 1, gridY);
                    return true;
                }
                if (HasCell(gridX - 1, gridY - 1))
                {
                    cell = new GroundCell(gridX - 1, gridY - 1);
                    return true;
                }
                if (HasCell(gridX, gridY - 1))
                {
                    cell = new GroundCell(gridX, gridY - 1);
                    return true;
                }
                cell = default;
                return false;
            }
            cell = default;
            return false;
        }
        public GroundCell SampleCell(Vector3 point)
        {
            if (TryGetCell(point, out var cell))
            {
                return cell;
            }

            int x = (int)point.x;
            int y = (int)point.z;

            int level = 1;

            while (level <= Size.x && level <= Size.y)
            {
                var startX = Mathf.Max(x - level, 0);
                var startY = Mathf.Max(y - level, 0);

                var endX = x + level;
                var endY = y + level;

                var minDistance = float.MaxValue;
                GroundCell result = default;
                for (int dx = startX; dx <= endX; dx++)
                {
                    if (HasCell(dx, startY))
                    {
                        Vector3 vec = point - new Vector3(dx, 0, startY);
                        float distance = vec.sqrMagnitude;
                        if (distance < minDistance)
                        {
                            result = new GroundCell(dx, startY);
                            minDistance = distance;
                        }
                    }
                }
                for (int dx = startX; dx <= endX; dx++)
                {
                    if (HasCell(dx, endY))
                    {
                        Vector3 vec = point - new Vector3(dx, 0, endY);
                        float distance = vec.sqrMagnitude;
                        if (distance < minDistance)
                        {
                            result = new GroundCell(dx, endY);
                            minDistance = distance;
                        }
                    }
                }
                for (int dy = startY; dy <= endY; dy++)
                {
                    if (HasCell(startX, dy))
                    {
                        Vector3 vec = point - new Vector3(startX, 0, dy);
                        float distance = vec.sqrMagnitude;
                        if (distance < minDistance)
                        {
                            result = new GroundCell(startX, dy);
                            minDistance = distance;
                        }
                    }
                }
                for (int dy = startY; dy <= endY; dy++)
                {
                    if (HasCell(endX, dy))
                    {
                        Vector3 vec = point - new Vector3(endX, 0, dy);
                        float distance = vec.sqrMagnitude;
                        if (distance < minDistance)
                        {
                            result = new GroundCell(endX, dy);
                            minDistance = distance;
                        }
                    }
                }
                if (minDistance < float.MaxValue)
                {
                    return result;
                }
                level++;
            }
            throw new Exception("Closest cell is not found");
        }
        public Vector3 SamplePoint(Vector3 point)
        {
           GroundCell cell = SampleCell(point);
            if (cell.ContainsPoint(point))
            {
                return point;
            }
            return cell.ProjectionPoint(point);
        }
        public List<Vector3> GetCorners(int x, int z)
        {
            List<Vector3> result = new List<Vector3>();

            if (!HasCell(x, z))
            {
                return result;
            }

            bool top = HasCell(x, z + 1);
            bool bottom = HasCell(x, z-1);
            bool right = HasCell(x + 1, z);
            bool left = HasCell(x - 1, z);

            bool topright = HasCell(x +1, z + 1);
            bool topleft = HasCell(x - 1, z + 1);
            bool bottomright = HasCell(x + 1, z - 1);
            bool bottomleft = HasCell(x -1, z - 1);

            if (topleft && (!left || !top) || !topleft && top && left)
            {
                result.Add(new Vector3(x, 0, z + 1));
            }
            if (topright && (!right || !top) || !topright && top && right)
            {
                result.Add(new Vector3(x + 1, 0, z + 1));
            }
            if (bottomleft && (!left || !bottom) || !bottomleft && bottom && left)
            {
                result.Add(new Vector3(x, 0, z));
            }
            if (bottomright && (!right || !bottom) || !bottomright && bottom && right)
            {
                result.Add(new Vector3(x + 1, 0, z));
            }
            return result;
        }
        public List<Vector3> GetCorners(GroundCell cell)
        {
            return GetCorners(cell.x, cell.y);
        }

        public bool TryFloorCell(Vector3 point, out GroundCell cell)
        {
            int x = Mathf.Max(Mathf.FloorToInt(point.x), 0);
            int y = Mathf.Max(Mathf.FloorToInt(point.z), 0);
            cell = new GroundCell(x, y);
            return HasCell(x, y);

        }
        public bool TraceLine(Vector3 start, Vector3 end)
        {
            float dx = end.x - start.x;
            float dy = end.z - start.z;
            float maxDistance = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

            int steps = (int)(maxDistance * 2) + 1;

            float stepx = dx / steps;
            float stepy = dy / steps;

            float x = start.x;
            float y = start.z;

            for (int i = 0; i < steps; i++)
            {
                if (!TryGetCell(x,y, out GroundCell cell))
                {
                    return false;
                }
                if (cell.ContainsPoint(end))
                {
                    return true;
                }
                x += stepx;
                y += stepy;
            }
            return false;
        }
    }

}
