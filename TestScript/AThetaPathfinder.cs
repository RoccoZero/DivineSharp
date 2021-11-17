using Divine.Extensions;
using Divine.Map;
using Divine.Map.Components;
using Divine.Numerics;

using System;
using System.Collections.Generic;

using static TestScript.NavMeshPathfinding;

namespace TestScript
{
    static class AThetaPathfinder
    {
        private static Dictionary<int, Vector2> CameFrom = new Dictionary<int, Vector2>();
        private static Dictionary<Vector2, float> Cost = new Dictionary<Vector2, float>();
        private static PriorityQueue<Vector2> Queue = new PriorityQueue<Vector2>();
        const int CellCount = 120;
        private static int MaxIter = (CellCount * 2) * 100;

        static AThetaPathfinder()
        {
            for (var y = 0; y < CellCount * 2; ++y)
            {
                for (var x = 0; x < CellCount * 2; ++x)
                {
                    var cell = MapManager.GetMeshCell(x - CellCount, y - CellCount);
                    MeshCells[x, y] = new Cell(cell.Position, cell.Flags, cell.X, cell.Y);
                }
            }

        }

        static public float Heuristic(Vector2 a, Vector2 b)
        {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static IEnumerable<Cell> FindPath(Vector3 start, Vector3 end)
        {
            Vector2[] neighbours = {
                new Vector2( -1, 0 ),
                new Vector2( -1, 1 ),
                new Vector2( 0, 1 ),
                new Vector2( 1, 1 ),
                new Vector2( 1, 0 ),
                new Vector2( 1, -1 ),
                new Vector2( 0, -1 ),
                new Vector2( -1, -1 )
            };

            MeshCell startCell = MapManager.GetMeshCell(start);
            MeshCell endCell = MapManager.GetMeshCell(end);
            Vector2 startCellIndexPos = new Vector2(startCell.X + CellCount, startCell.Y + CellCount);
            Vector2 endCellIndexPos = new Vector2(endCell.X + CellCount, endCell.Y + CellCount);

            //var Queue1 = new PriorityQueue

            CameFrom.Clear();
            Cost.Clear();
            Queue.Clear();
            Queue.Enqueue(startCellIndexPos, 0);
            Cost.Add(startCellIndexPos, 0);

            int startIndex = (((int)startCellIndexPos.Y) * (CellCount * 2)) + ((int)startCellIndexPos.X);
            //DrawRectange(MeshCells[(int)startCellIndexPos.X, (int)startCellIndexPos.Y], Color.Black, 10);
            CameFrom[startIndex] = startCellIndexPos;

            Vector2 bestPos = startCellIndexPos;
            float nearest = Heuristic(startCellIndexPos, endCellIndexPos);
            //Console.WriteLine(nearest);

            int iter = 0;

            while (Queue.Count > 0)
            {
                Vector2 curPos = Queue.Dequeue();
                int curIndex = (((int)curPos.Y) * (CellCount * 2)) + ((int)curPos.X);
                iter++;

                if (curPos == endCellIndexPos)
                {
                    bestPos = curPos;
                    break;
                }

                if (iter >= MaxIter)
                    break;

                foreach (var n in neighbours)
                {
                    Vector2 nextPos = new Vector2(curPos.X + n.X, curPos.Y + n.Y);
                    int nextIndex = (((int)nextPos.Y + CellCount) * (CellCount * 2)) + ((int)nextPos.X + CellCount);
                    var flag = GetMeshCell(nextIndex).Flags;

                    if (flag.HasFlag(MeshCellFlags.Walkable))
                    {
                        if (flag.HasFlag(MeshCellFlags.Tree))
                            continue;
                        if (flag.HasFlag(MeshCellFlags.GridFlagObstacle))
                            continue;
                    }
                    else if (flag.HasFlag(MeshCellFlags.MovementBlocker))
                        continue;
                    else
                        continue;

                    if (curPos.X != nextPos.X || curPos.Y != nextPos.Y)
                    {
                        int tempIndex1 = (((int)curPos.Y + CellCount) * (CellCount * 2)) + ((int)nextPos.X + CellCount);
                        var tempFlag1 = GetMeshCell(tempIndex1).Flags;
                        int tempIndex2 = (((int)nextPos.Y + CellCount) * (CellCount * 2)) + ((int)curPos.X + CellCount);
                        var tempFlag2 = GetMeshCell(tempIndex2).Flags;
                        if (tempFlag1.HasFlag(MeshCellFlags.Walkable)
                            && tempFlag2.HasFlag(MeshCellFlags.Walkable))
                        {
                            if (tempFlag1.HasFlag(MeshCellFlags.Tree)
                                && tempFlag2.HasFlag(MeshCellFlags.Tree))
                                continue;
                            if (tempFlag1.HasFlag(MeshCellFlags.GridFlagObstacle)
                                && tempFlag2.HasFlag(MeshCellFlags.GridFlagObstacle))
                                continue;
                        }
                        else if (tempFlag1.HasFlag(MeshCellFlags.MovementBlocker)
                            && tempFlag2.HasFlag(MeshCellFlags.MovementBlocker))
                            continue;
                        else
                            continue;
                    }

                    float newCost = Cost[curPos] + (float)((nextPos.X - curPos.X == 0 || nextPos.Y - curPos.Y == 0) ? 1 : Math.Sqrt(2))/*Heuristic(curPos, nextPos)*/;
                    if (!Cost.ContainsKey(nextPos) || newCost < Cost[nextPos])
                    {
                        Cost[nextPos] = newCost;
                        float dist = nextPos.Distance(endCellIndexPos)/*Heuristic(nextPos, endCellIndexPos)*/;
                        if (dist < nearest)
                        {
                            nearest = dist;
                            bestPos = nextPos;
                        }

                        float priority = newCost + dist;
                        Queue.Enqueue(nextPos, priority);
                        CameFrom[nextIndex] = curPos;
                    }
                }
            }

            List<Cell> path = new List<Cell>();
            while (bestPos != startCellIndexPos)
            {
                int bestIndex = (((int)bestPos.Y) * (CellCount * 2)) + ((int)bestPos.X);
                var bestCell = GetMeshCell(bestIndex);
                path.Add(bestCell);
                bestPos = CameFrom[bestIndex];
            }
            path.Add(GetMeshCell(startIndex));
            return path;
        }
    }
}
