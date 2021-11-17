using Divine.Entity;
using Divine.Extensions;
using Divine.Game;
using Divine.Map;
using Divine.Map.Components;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

using System;
using System.Collections.Generic;
using System.Linq;

namespace TestScript
{
    public sealed class NavMeshPathfinding
    {

        public static RootMenu RootMenu { get; private set; }
        public static MenuSwitcher Enabled { get; private set; }

        const int CellCount = 120;
        public static Cell[,] MeshCells = new Cell[240, 240];

        private static List<Cell> path;

        static NavMeshPathfinding()
        {
            RootMenu = MenuManager.CreateRootMenu("TestScript");
            Enabled = RootMenu.CreateSwitcher("Enabled");
            Enabled.ValueChanged += Enabled_ValueChanged;

            for (var y = 0; y < CellCount * 2; ++y)
            {
                for (var x = 0; x < CellCount * 2; ++x)
                {
                    var cell = MapManager.GetMeshCell(x - CellCount, y - CellCount);
                    MeshCells[x, y] = new Cell(cell.Position, cell.Flags, cell.X, cell.Y);
                }
            }
        }

        private static void Enabled_ValueChanged(MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                RendererManager.Draw += RendererManager_Draw;
                //CameraManager.Pitch = CameraManager.DefaultPitch;
            }
            else
            {
                RendererManager.Draw -= RendererManager_Draw;
            }
        }

        public class PriorityQueue<T>
        {
            private List<Tuple<T, float>> elements = new List<Tuple<T, float>>();

            public int Count
            {
                get { return elements.Count; }
            }

            public void Enqueue(T item, float priority)
            {
                elements.Add(Tuple.Create(item, priority));
            }

            public T Dequeue()
            {
                int bestIndex = 0;

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Item2 < elements[bestIndex].Item2)
                    {
                        bestIndex = i;
                    }
                }

                T bestItem = elements[bestIndex].Item1;
                elements.RemoveAt(bestIndex);
                return bestItem;
            }
            public void Clear()
            {
                elements.Clear();
            }
        }

        public static void RendererManager_Draw()
        {
            var position = EntityManager.LocalHero.Position;

            //foreach (var Cell in MeshCells)
            //{
            //    Color c;
            //    var flag = Cell.Value.Flags;
            //    if (flag.HasFlag(MeshCellFlags.Walkable))
            //    {
            //        c = flag.HasFlag(MeshCellFlags.Tree) ? Color.Purple : Color.Green;
            //        if (flag.HasFlag(MeshCellFlags.GridFlagObstacle))
            //        {
            //            c = Color.Orange;
            //        }
            //    }
            //    else if (flag.HasFlag(MeshCellFlags.MovementBlocker))
            //    {
            //        c = Color.Blue;
            //    }
            //    else
            //    {
            //        c = Color.Red;
            //    }

            //    DrawRectange(Cell.Value, c);
            //}

            //max index is 57599
            //var myCell = GetMeshCell(57599);
            //CameraManager.LookAtPosition = myCell.Position;
            //DrawRectange(myCell, Color.Black, 10);

            //AddNavMeshObstacle(GameManager.MousePosition, 70);

            var mousePositionCell = MapManager.GetMeshCell(GameManager.MousePosition);
            //Console.WriteLine(mousePositionCell.X + " " + mousePositionCell.Y + " " + (((mousePositionCell.Y + 120) * 240) + (mousePositionCell.X + 120)));
            DrawRectange(mousePositionCell, Color.Aqua, 10);

            DrawRectange(MapManager.GetMeshCell(position), Color.Yellow, 5);

            //Console.WriteLine($"Pos: {MapManager.GetMeshCell(GameManager.MousePosition).Position} " +
            //    $"X: {MapManager.GetMeshCell(GameManager.MousePosition).X} " +
            //    $"Y: {MapManager.GetMeshCell(GameManager.MousePosition).Y}");

            //if (!MultiSleeper<string>.Sleeping("pathfinder"))
            //{
            //    path = AThetaPathfinder.FindPath(EntityManager.LocalHero.Position, GameManager.MousePosition).ToList();
            //    MultiSleeper<string>.Sleep("pathfinder", 1000);
            //}
            //var needPatch = path.ToList();
            //foreach (var Cell in needPatch)
            //{
            //    DrawRectange(Cell, Color.Black, 10);
            //}
        }

        private static void DrawRectange(MeshCell cell, Color color, int size = 1)
        {
            var meshCellSize = MapManager.MeshCellSize;
            var position = cell.Position;
            if (position.Z < -5000)
            {
                return;
            }

            //for (var i = 0; i < size; i++)
            //{
            var a = position /*+ new Vector3(4, 4, 0)*/;
            var b = a + new Vector3(0, meshCellSize/* - 8*/, 0);
            var c = a + new Vector3(meshCellSize/* - 8*/, 0, 0);
            var d = c + new Vector3(0, meshCellSize/* - 8*/, 0);

            if (!a.IsOnScreen() || !b.IsOnScreen() || !c.IsOnScreen() || !d.IsOnScreen())
            {
                return;
            }

            var a2 = RendererManager.WorldToScreen(a);
            var b2 = RendererManager.WorldToScreen(b);
            var c2 = RendererManager.WorldToScreen(c);
            var d2 = RendererManager.WorldToScreen(d);

            RendererManager.DrawLine(a2, b2, color);
            RendererManager.DrawLine(b2, d2, color);
            RendererManager.DrawLine(d2, c2, color);
            RendererManager.DrawLine(c2, a2, color);
            //}
        }

        private static void DrawRectange(Cell cell, Color color, int size = 1)
        {
            var meshCellSize = MapManager.MeshCellSize;
            var position = cell.Position;
            if (position.Z < -5000)
            {
                return;
            }

            //for (var i = 0; i < size; i++)
            //{
            var a = position /*+ new Vector3(4, 4, 0)*/;
            var b = a + new Vector3(0, meshCellSize/* - 8*/, 0);
            var c = a + new Vector3(meshCellSize/* - 8*/, 0, 0);
            var d = c + new Vector3(0, meshCellSize/* - 8*/, 0);

            if (!a.IsOnScreen() || !b.IsOnScreen() || !c.IsOnScreen() || !d.IsOnScreen())
            {
                return;
            }

            var a2 = RendererManager.WorldToScreen(a);
            var b2 = RendererManager.WorldToScreen(b);
            var c2 = RendererManager.WorldToScreen(c);
            var d2 = RendererManager.WorldToScreen(d);

            RendererManager.DrawLine(a2, b2, color);
            RendererManager.DrawLine(b2, d2, color);
            RendererManager.DrawLine(d2, c2, color);
            RendererManager.DrawLine(c2, a2, color);
            //}
        }

        public static Cell GetMeshCell(int index)
        {
            int y = index / (CellCount * 2);
            int x = index - (y * (CellCount * 2));
            return MeshCells[x - CellCount, y - CellCount];
        }

        private static List<uint> AddNavMeshObstacle(Vector3 position, float radius)
        {
            var obstacleList = new List<uint>();

            var posCell = MapManager.GetMeshCell(position);
            var obstacles = new List<Cell>();
            var tempList = MeshCells.Cast<Cell>().ToList();
            var nearestCells = tempList.Where(x => x.Position.Distance(posCell.Position) <= radius);
            foreach (var item in nearestCells)
            {
                DrawRectange(item, Color.Red);
                //obstacleList.Add();
            }

            return obstacleList;
        }

        private void UpdateManager_IngameUpdate()
        {

        }

        internal void Dispose()
        {
            UpdateManager.IngameUpdate -= UpdateManager_IngameUpdate;
        }
    }
}
