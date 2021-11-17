using System;

using SharpDX;

namespace Divine.Core.Managers.Mesh
{
    public sealed class Cell
    {
        public Cell(Vector3 worldPosition, CellFlags cellFlags, int gridX, int gridY)
        {
            WorldPosition = worldPosition;
            WorldHeight = (int)Math.Floor(worldPosition.Z / 128);
            CellFlags = cellFlags;
            GridX = gridX;
            GridY = gridY;
        }

        public Vector3 WorldPosition;

        public int WorldHeight;

        public CellFlags CellFlags;

        public int GridX;

        public int GridY;
    }
}