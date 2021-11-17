using Divine.Core.Managers.Mesh.Delegates;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Divine.Core.Managers.Mesh
{
    [FastRun]
    public static class MeshManager
    {
        private static readonly string FilePath = DivineDirectory.Data + "MeshManager.dat";

        private static bool IsActiveCells;

        private static bool IsActiveRefresher;

        public const int CellSize = 64;

        private static readonly Cell[,] Cells = new Cell[260, 260];

        static MeshManager()
        {
            if (File.Exists(FilePath))
            {
                ActivateCells();
            }
            else
            {
                DivineResource.OnUpdated += () =>
                {
                    ActivateCells();
                };
            }
        }

        private static void ActivateCells()
        {
            using (var fs = new FileStream(FilePath, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    for (var x = 0; x < 260; x++)
                    {
                        for (var y = 0; y < 260; y++)
                        {
                            Cells[x, y] = new Cell(new Vector3(br.ReadInt16(), br.ReadInt16(), br.ReadInt16()), (CellFlags)br.ReadUInt16(), x - 130, y - 130);
                        }
                    }
                }
            }

            IsActiveCells = true;
        }

        public static void RunRefresher()
        {
            if (IsActiveRefresher)
            {
                return;
            }

            Refresher();
            IsActiveRefresher = true;
        }

        private static void Refresher()
        {
            var trees = ObjectManager.GetEntities<Tree>().Where(x => x.Name == "ent_dota_tree").ToArray();
            var aliveTrees = new bool[trees.Length];

            UpdateManager.Subscribe(250, () =>
            {
                if (!UnitManager.Owner.IsValid)
                {
                    return;
                }

                for (var i = 0; i < trees.Length; i++)
                {
                    var tree = trees[i];
                    var alive = tree.IsAlive;

                    if (aliveTrees[i] == alive)
                    {
                        continue;
                    }

                    if (alive)
                    {
                        AddTree(tree.Position);
                    }
                    else
                    {
                        RemoveTree(tree.Position);
                    }

                    aliveTrees[i] = alive;
                }

                var night = Game.IsNight;
                var visions = GetVisions(UnitManager<CUnit, Ally>.Units);

                Task.Run(() =>
                {
                    RemoveVisibleFlags();
                    AddVisions(visions, night);
                });
            });
        }

        private static Vision[] GetVisions(IEnumerable<CUnit> units)
        {
            var arrayUnits = units.ToArray();
            var visions = new Vision[arrayUnits.Length];

            for (var i = 0; i < arrayUnits.Length; i++)
            {
                var unit = arrayUnits[i];
                visions[i] = new Vision(unit.Position, unit.DayVision, unit.NightVision, (unit.UnitState & UnitState.Flying) == UnitState.Flying);
            }

            return visions;
        }

        private static void AddVisions(Vision[] visions, bool night)
        {
            for (var i = 0; i < visions.Length; i++)
            {
                var vision = visions[i];
                var radius = night ? vision.NightVision : vision.DayVision;
                if (radius == 0)
                {
                    continue;
                }

                AddVisibleFlags(vision.Position, radius, vision.IsFlying);
            }
        }

        /*private static void OnDraw(EventArgs args)
        {
            var position = ObjectManager.LocalHero.Position;

            const int CellCount = 130;
            for (var i = -130; i < CellCount; ++i)
            {
                for (var j = -130; j < CellCount; ++j)
                {
                    Color c;
                    var cell = GetCell(i, j);
                    var flag = cell.CellFlags;
                    if (flag.HasFlag(CellFlags.Walkable))
                    {
                        c = flag.HasFlag(CellFlags.Tree) ? Color.Purple : Color.Green;
                        if (flag.HasFlag(CellFlags.GridFlagObstacle))
                        {
                            c = Color.Pink;
                        }
                    }
                    else if (flag.HasFlag(CellFlags.MovementBlocker))
                    {
                        c = Color.Green;
                    }
                    else
                    {
                        c = Color.Red;
                    }

                    if (flag.HasFlag(CellFlags.Visible))
                    {
                        c = Color.White;
                        DrawRectange(cell, c, 10);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                    
                    DrawRectange(cell, c);
                }
            }

            var mousePositionCell = GetCell(Game.MousePosition);
            if (mousePositionCell != null)
            {
                DrawRectange(mousePositionCell, Color.Aqua, 10);
            }
            
            DrawRectange(GetCell(position), Color.Yellow, 5);
        }

        private static void DrawRectange(Cell cell, Color color, int size = 1)
        {
            for (var i = 0; i < size; i++)
            {
                var a = cell.WorldPosition + new Vector3(4 + i, 4 + i, 0);
                var aEnd = a + new Vector3(0, CellSize - 8 - (i * 2), 0);
                var b = a + new Vector3(CellSize - 8 - (i * 2), 0, 0);
                var bEnd = b + new Vector3(0, CellSize - 8 - (i * 2), 0);

                if (!a.IsOnScreen() || !aEnd.IsOnScreen() || !b.IsOnScreen() || !bEnd.IsOnScreen())
                {
                    return;
                }

                Drawing.DrawLine(Drawing.WorldToScreen(a), Drawing.WorldToScreen(aEnd), color);
                Drawing.DrawLine(Drawing.WorldToScreen(aEnd), Drawing.WorldToScreen(bEnd), color);
                Drawing.DrawLine(Drawing.WorldToScreen(bEnd), Drawing.WorldToScreen(b), color);
                Drawing.DrawLine(Drawing.WorldToScreen(b), Drawing.WorldToScreen(a), color);
            }
        }*/

        public static Cell GetCell(int x, int y)
        {
            if (!IsActiveCells)
            {
                return null;
            }

            if (!IsActiveRefresher)
            {
                Refresher();
                IsActiveRefresher = true;
            }

            if (x >= 130 || x < -130 || y >= 130 || y < -130)
            {
                return null;
            }

            return Cells[x + 130, y + 130];
        }

        public static Cell GetCell(Vector3 position)
        {
            return GetCell((int)Math.Floor(position.X / CellSize), (int)Math.Floor(position.Y / CellSize));
        }

        private static bool TryGetCellPosition(Vector3 position, out int cellX, out int cellY)
        {
            cellX = (int)Math.Floor(position.X / CellSize);
            cellY = (int)Math.Floor(position.Y / CellSize);

            if (cellX >= 130 || cellX < -130 || cellY >= 130 || cellY < -130)
            {
                cellX = 0;
                cellY = 0;
                return false;
            }

            return true;
        }

        public static CellFlags GetCellFlags(Vector3 position)
        {
            var cell = GetCell(position);
            if (cell == null)
            {
                return CellFlags.None;
            }

            return cell.CellFlags;
        }

        private static void AddVisibleFlags(Vector3 position, uint radius, bool fullVision = false)
        {
            var cell = GetCell(position);
            if (cell == null)
            {
                return;
            }

            var worldHeight = cell.WorldHeight;
            var cellX = cell.GridX;
            var cellY = cell.GridY;

            var cellRadius = (int)Math.Floor((float)radius / CellSize);

            for (var x = cellX - cellRadius; x < cellRadius + cellX + 1; x++)
            {
                for (var y = cellY - cellRadius; y < cellRadius + cellY + 1; y++)
                {
                    if (Distance(cellX, cellY, x, y) != cellRadius)
                    {
                        continue;
                    }

                    Line(cellX, cellY, x, y, (lineX, lineY) =>
                    {
                        var otherCell = GetCell(lineX, lineY);
                        if (otherCell == null)
                        {
                            return false;
                        }

                        if (!fullVision)
                        {
                            if ((otherCell.CellFlags & CellFlags.Tree) == CellFlags.Tree && otherCell.WorldHeight > worldHeight - 2)
                            {
                                return false;
                            }

                            if (otherCell.WorldHeight > worldHeight)
                            {
                                return false;
                            }

                            if ((cell.CellFlags & CellFlags.FowRoshanBlocker) == CellFlags.FowRoshanBlocker)
                            {
                                if ((otherCell.CellFlags & CellFlags.FowRoshanBlocker) != CellFlags.FowRoshanBlocker)
                                {
                                    return false;
                                }
                            }
                            else if ((otherCell.CellFlags & CellFlags.FowRoshanBlocker) == CellFlags.FowRoshanBlocker)
                            {
                                return false;
                            }
                        }

                        otherCell.CellFlags |= CellFlags.Visible;
                        return true;
                    });
                }
            }
        }

        private static void RemoveVisibleFlags()
        {
            for (var x = 0; x < 260; x++)
            {
                for (var y = 0; y < 260; y++)
                {
                    var cell = Cells[x, y];
                    if ((cell.CellFlags & CellFlags.Visible) != CellFlags.Visible)
                    {
                        continue;
                    }

                    cell.CellFlags &= ~CellFlags.Visible;
                }
            }
        }

        public static void AddCellFlags(Vector3 position, CellFlags cellFlags, int radius)
        {
            if (!TryGetCellPosition(position, out var cellX, out var cellY))
            {
                return;
            }

            var cellRadius = (int)Math.Floor((float)radius / CellSize);

            for (var x = cellX - cellRadius; x < cellRadius + cellX + 1; x++)
            {
                for (var y = cellY - cellRadius; y < cellRadius + cellY + 1; y++)
                {
                    if (Distance(cellX, cellY, x, y) > cellRadius)
                    {
                        continue;
                    }

                    var cell = GetCell(x, y);
                    if (cell == null)
                    {
                        continue;
                    }

                    cell.CellFlags |= cellFlags;
                }
            }
        }

        public static void RemoveCellFlags(Vector3 position, CellFlags cellFlags, int radius)
        {
            if (!TryGetCellPosition(position, out var cellX, out var cellY))
            {
                return;
            }

            var cellRadius = (int)Math.Floor((float)radius / CellSize);

            for (var x = cellX - cellRadius; x < cellRadius + cellX + 1; x++)
            {
                for (var y = cellY - cellRadius; y < cellRadius + cellY + 1; y++)
                {
                    if (Distance(cellX, cellY, x, y) > cellRadius)
                    {
                        continue;
                    }

                    var cell = GetCell(x, y);
                    if (cell == null)
                    {
                        continue;
                    }

                    cell.CellFlags &= ~cellFlags;
                }
            }
        }

        private static void AddTree(Vector3 position)
        {
            if (!TryGetCellPosition(position, out var cellX, out var cellY))
            {
                return;
            }

            for (var x = cellX - 1; x < cellX + 1; x++)
            {
                for (var y = cellY - 1; y < cellY + 1; y++)
                {
                    var cell = GetCell(x, y);
                    if (cell == null)
                    {
                        continue;
                    }

                    cell.CellFlags |= CellFlags.Tree;
                }
            }
        }

        private static void RemoveTree(Vector3 position)
        {
            if (!TryGetCellPosition(position, out var cellX, out var cellY))
            {
                return;
            }

            for (var x = cellX - 1; x < cellX + 1; x++)
            {
                for (var y = cellY - 1; y < cellY + 1; y++)
                {
                    var cell = GetCell(x, y);
                    if (cell == null)
                    {
                        continue;
                    }

                    cell.CellFlags &= ~CellFlags.Tree;
                }
            }
        }

        private static void Rectangle(int startX, int startY, int endX, int endY, int startWidth, int endWidth, RectangleEventHandler handler)
        {
            if (startWidth == 0 && endWidth == 0)
            {
                Line(startX, startY, endX, endY, (lineX, lineY) =>
                {
                    handler.Invoke(lineX, lineY);
                    return true;
                });
            }
            else
            {
                var startPosition = new Vector2(startX, startY);
                var endPosition = new Vector2(endX, endY);

                var difference = startPosition - endPosition;
                var rotation = difference.Rotated(MathUtil.DegreesToRadians(90));
                rotation.Normalize();

                var leght = startWidth > endWidth ? startWidth : endWidth;

                for (var i = -leght; i < leght + 1; i++)
                {
                    var start = rotation * Math.Min(Math.Max(i, -startWidth), startWidth);
                    var end = rotation * Math.Min(Math.Max(i, -endWidth), endWidth);

                    var rightStartPosition = startPosition + start;
                    var rightEndPosition = endPosition + end;

                    Line((int)rightStartPosition.X, (int)rightStartPosition.Y, (int)rightEndPosition.X, (int)rightEndPosition.Y, (lineX, lineY) =>
                    {
                        handler.Invoke(lineX, lineY);
                        return true;
                    });
                }
            }
        }

        private static void Line(int startX, int startY, int endX, int endY, LineEventHandler handler)
        {
            var dx = Math.Abs(endX - startX);
            var dy = -Math.Abs(endY - startY);

            var sx = startX < endX ? 1 : -1;
            var sy = startY < endY ? 1 : -1;

            var err = dx + dy;

            while (true)
            {
                if (!handler.Invoke(startX, startY))
                {
                    break;
                }

                if (startX == endX && startY == endY)
                {
                    break;
                }

                var e2 = 2 * err;
                if (e2 > dy)
                {
                    err += dy;
                    startX += sx;
                }
                else if (e2 < dx)
                {
                    err += dx;
                    startY += sy;
                }
            }
        }

        private static int Distance(int x, int y, int otherX, int otherY)
        {
            x -= otherX;
            y -= otherY;

            return (int)Math.Sqrt((x * x) + (y * y));
        }
    }
}