using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
using NavSystem;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable NotAccessedField.Local

namespace GroundSystem
{
    public sealed class GroundPathFinder
    {
        private readonly GroundArea area;
        private readonly GroundCellChecker cellChecker;

        private readonly Dictionary<GroundCell, CellNode> openCellList;
        private readonly Dictionary<Vector3, CornerNode> openCornerList;
        private readonly HashSet<GroundCell> closedCellList;
        private readonly Dictionary<Vector3, CornerNode> closedCornerList;

        private GroundCell startCell;
        private CellNode closestCell;
        private Vector3 closestPoint;

        private Vector3 startPoint;
        private Vector3 targetPoint;

        //Caches:
        private readonly List<Vector3> resultPathCache = new();
        private bool pathPartialCache;
        private Vector3 startPointCache;
        private Vector3 endPointCache;

        public GroundPathFinder(GroundArea area)
        {
            this.area = area;
            this.cellChecker = new GroundCellChecker();

            this.openCellList = new Dictionary<GroundCell, CellNode>();
            this.openCornerList = new Dictionary<Vector3, CornerNode>();
            this.closedCellList = new HashSet<GroundCell>();
            this.closedCornerList = new Dictionary<Vector3, CornerNode>();
        }

        public int CalculatePath(Vector3 startPoint, Vector3 endPoint, Vector3[] bufferPath)
        {
            startPoint.y = 0;
            endPoint.y = 0;

            if (this.EqualsPoints(startPoint, endPoint))
            {
                return 0;
            }

            startPoint = this.area.SamplePoint(startPoint);

            var startCell = GroundCell.Floor(startPoint);
            var endCell = GroundCell.Floor(endPoint);
            Debug.Log($"stasrtcell {startCell.x}, {startCell.y}");
            Debug.Log($"end cell {endCell.x}, {endCell.y}");
            if (startCell.Equals(endCell))
            {
                bufferPath[0] = startPoint;
                bufferPath[1] = endPoint;
                return 2;
            }

            //Проверяем с предыдущими вычислениями
            if (this.EqualsPoints(startPoint, this.startPointCache) &&
                this.EqualsPoints(endPoint, this.endPointCache))
            {
                return this.ReturnPath(bufferPath);
            }

            this.startCell = startCell;
            this.closestCell = null;

            this.startPoint = startPoint;
            this.startPointCache = startPoint;

            //Проверяем с предыдущими вычислениями
            if (this.EqualsPoints(endPoint, this.endPointCache) && this.pathPartialCache)
            {
                this.targetPoint = this.closestPoint;
                this.pathPartialCache = true;
            }
            else
            {
                this.targetPoint = endPoint;
                this.pathPartialCache = false;
            }

            this.endPointCache = endPoint;

            return this.CalculatePathInternal(bufferPath);
        }

        private int CalculatePathInternal(Vector3[] bufferPath)
        {
            this.openCornerList.Clear();
            this.closedCornerList.Clear();
            this.openCellList.Clear();
            this.closedCellList.Clear();

            var cornerNode = new CornerNode(
                point: this.startPoint,
                baseNode: null,
                distanceToStart: 0,
                distanceToEnd: this.HeuristicDistance(this.startPoint, this.targetPoint)
            );

            var cellNode = new CellNode(
                cell: this.startCell,
                baseNode: null,
                distanceToStart: 0,
                distanceToEnd: this.HeuristicDistance(this.startCell.GetCenter(), this.targetPoint)
            );

            while (true)
            {
                this.closedCornerList.Add(cornerNode.point, cornerNode);

                if (this.area.TraceLine(cornerNode.point, this.targetPoint))
                {
                    return this.CreateFullPath(cornerNode, bufferPath);
                }

                while (!this.SelectNextCorner(out cornerNode))
                {
                    this.UpdateClosestCell(cellNode);
                    this.VisitCell(cellNode);

                    if (this.SelectNextCell(out cellNode))
                    {
                        this.openCellList.Remove(cellNode.cell);
                    }
                    else
                    {
                        return this.CreatePartialPath(bufferPath);
                    }
                }

                this.openCornerList.Remove(cornerNode.point);
            }
        }


        private void VisitCell(CellNode cellNode)
        {
            this.closedCellList.Add(cellNode.cell);

            var neighbours = this.area.GetNeighboors(cellNode.cell);
            for (int i = 0, count = neighbours.Count; i < count; i++)
            {
                this.VisitCellNeighbour(neighbours[i], cellNode);
            }

            var corners = this.area.GetCorners(cellNode.cell);
            for (int i = 0, count = corners.Count; i < count; i++)
            {
                this.VisitCorner(corners[i]);
            }
        }

        private void VisitCorner(Vector3 corner)
        {
            if (this.closedCornerList.ContainsKey(corner))
            {
                return;
            }

            var cornerNode = this.CreateCornerNode(corner);
            if (cornerNode.baseNode != null)
            {
                this.openCornerList[corner] = cornerNode;
            }
        }

        private CornerNode CreateCornerNode(Vector3 point)
        {
            float minDistanceToStart = float.MaxValue;
            CornerNode baseNode = null;

            foreach (var otherNode in this.closedCornerList.Values)
            {
                var otherPoint = otherNode.point;
                if (!this.area.TraceLine(otherPoint, point))
                {
                    continue;
                }

                var distanceToStart = otherNode.distanceToStart + this.HeuristicDistance(point, otherPoint);
                if (minDistanceToStart > distanceToStart)
                {
                    minDistanceToStart = distanceToStart;
                    baseNode = otherNode;
                }
            }

            var distanceToEnd = this.HeuristicDistance(point, this.targetPoint);
            return new CornerNode(
                point,
                baseNode,
                minDistanceToStart,
                distanceToEnd
            );
        }

        private void VisitCellNeighbour(GroundCell neighbour, CellNode baseNode)
        {
            if (this.closedCellList.Contains(neighbour))
            {
                return;
            }

            if (!this.cellChecker.IsAvailable(neighbour))
            {
                this.closedCellList.Add(neighbour);
                return;
            }

            var neighbourDistance = Vector3.Distance(neighbour.GetCenter(), baseNode.cell.GetCenter());
            var fullDistance = baseNode.distanceToStart + neighbourDistance;

            var alreadyExists = this.openCellList.TryGetValue(neighbour, out var node);
            if (alreadyExists)
            {
                if (node.distanceToStart > fullDistance)
                {
                    node.baseNode = baseNode;
                    node.distanceToStart = fullDistance;
                }
            }
            else
            {
                node = new CellNode(
                    cell: neighbour,
                    baseNode: baseNode,
                    distanceToStart: fullDistance,
                    distanceToEnd: this.HeuristicDistance(neighbour.GetCenter(), this.targetPoint)
                );

                this.openCellList.Add(neighbour, node);
            }
        }

        private void UpdateClosestCell(CellNode node)
        {
            if (this.closestCell == null)
            {
                this.closestCell = node;
            }
            else if (this.closestCell.distanceToEnd > node.distanceToEnd)
            {
                this.closestCell = node;
            }
        }

        private int CreatePartialPath(Vector3[] bufferPath)
        {
            this.pathPartialCache = true;
            this.resultPathCache.Clear();

            this.closestPoint = this.closestCell.cell.ProjectionPoint(this.targetPoint);
            var node = this.CreateCornerNode(this.closestPoint);

            while (node != null)
            {
                this.resultPathCache.Add(node.point);
                node = node.baseNode;
            }

            this.resultPathCache.Reverse();

            return this.ReturnPath(bufferPath);
        }

        private int CreateFullPath(CornerNode node, Vector3[] bufferPath)
        {
            this.resultPathCache.Clear();
            this.resultPathCache.Add(this.targetPoint);

            while (node != null)
            {
                this.resultPathCache.Add(node.point);
                node = node.baseNode;
            }

            this.resultPathCache.Reverse();

            return this.ReturnPath(bufferPath);
        }

        private int ReturnPath(Vector3[] bufferPath)
        {
            var count = this.resultPathCache.Count;
            for (int i = 0; i < count; i++)
            {
                bufferPath[i] = this.resultPathCache[i];
            }

            return count;
        }

        private bool SelectNextCorner(out CornerNode result)
        {
            result = null;
            float minWeight = float.MaxValue;

            foreach (var node in this.openCornerList.Values)
            {
                var weight = node.distanceToStart + node.distanceToEnd;
                if (minWeight > weight)
                {
                    result = node;
                    minWeight = weight;
                }
            }

            return result != null;
        }

        private bool SelectNextCell(out CellNode result)
        {
            result = null;
            float minWeight = float.MaxValue;

            foreach (var node in this.openCellList.Values)
            {
                var weight = node.distanceToStart + node.distanceToEnd;

                if (minWeight > weight)
                {
                    result = node;
                    minWeight = weight;
                }
            }

            return result != null;
        }

        private float HeuristicDistance(Vector3 c1, Vector3 c2)
        {
            return Vector3.Distance(c1, c2);
        }

        private bool EqualsPoints(Vector3 p1, Vector3 p2)
        {
            return (p1 - p2).sqrMagnitude < 0.01f;
        }

        private sealed class CellNode
        {
            public GroundCell cell;
            public CellNode baseNode;
            public float distanceToStart;
            public float distanceToEnd;

            public CellNode(GroundCell cell, CellNode baseNode, float distanceToStart, float distanceToEnd)
            {
                this.cell = cell;
                this.baseNode = baseNode;
                this.distanceToStart = distanceToStart;
                this.distanceToEnd = distanceToEnd;
            }
        }

        private sealed class CornerNode
        {
            public Vector3 point;
            public CornerNode baseNode;
            public float distanceToStart;
            public float distanceToEnd;

            public CornerNode(Vector3 point, CornerNode baseNode, float distanceToStart, float distanceToEnd)
            {
                this.point = point;
                this.baseNode = baseNode;
                this.distanceToStart = distanceToStart;
                this.distanceToEnd = distanceToEnd;
            }
        }
    }
}