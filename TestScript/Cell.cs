using Divine.Map.Components;
using Divine.Numerics;

namespace TestScript
{
    public sealed class Cell
    {
        public Cell(Vector3 position, MeshCellFlags flags, int x, int y)
        {
            Index = (y * 260) + x;
            Position = position;
            Flags = flags;
            X = x;
            Y = y;
        }

        public int Index;

        public Vector3 Position;

        public MeshCellFlags Flags;

        public int X;

        public int Y;
    }
}
