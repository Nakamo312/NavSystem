using System.Collections;
using System;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

namespace NavSystem
{
    public readonly struct GroundCell 
    {
        public readonly int x;
        public readonly int y;

        public GroundCell(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return ContainsPoint(point.x, point.z); 
        }

        public bool ContainsPoint(float x, float y)
        {
            return x >= this.x && x <= this.x + 1 && y >= this.y && y <= this.y + 1;
        }
        
        public Vector3 GetBottomLeft()
        {
            return new Vector3(x, 0, y);
        }
        public Vector3 GetBottomRight()
        {
            return new Vector3(x + 1, 0, y);
        }
        public Vector3 GetTopLeft()
        {
            return new Vector3(x, 0, y + 1);
        }
        public Vector3 GetTopRight()
        {
            return new Vector3(x + 1, 0, y + 1);
        }
        public Vector3 GetCenter()
        {
            return new Vector3(x + 0.5f, 0, y + 0.5f);
        }

        public bool Equals(GroundCell other)
        {
            return x == other.x && y == other.y ;
        }

        public Vector3 ProjectionPoint(Vector3 point)
        {
            Vector3 projection = Vector3.zero;
            Vector3[] vertices =  
            {
                this.GetBottomLeft(),
                this.GetBottomRight(),
                this.GetTopLeft(),
                this.GetTopLeft(),
            };

            for (int i = 0; i < 4; i++)
            {
                Vector3 distance = point - vertices[i];
                Vector3 edge = vertices[(i + 1) % 4] - vertices[i];
                float projectionLength = Vector3.Dot(distance, edge) / edge.sqrMagnitude;
                Vector3 projectedPoint = vertices[i] + Mathf.Clamp01(projectionLength) * edge;

                if (i == 0 || Vector3.Distance(point, projectedPoint) < Vector3.Distance(point, projection))
                {
                    projection = projectedPoint;
                }
            }
            return projection;
        }

        public static GroundCell Floor(Vector3 point)
        {
            int x = Mathf.Max(Mathf.FloorToInt(point.x), 0);
            int y = Mathf.Max(Mathf.FloorToInt(point.z), 0);
            return new GroundCell(x, y);
        }
        //public override string ToString()
        //{
        //    return $"({x}, {y})";
        //}
    }
}