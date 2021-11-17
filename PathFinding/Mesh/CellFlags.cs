using System;

namespace Divine.Core.Managers.Mesh
{
    [Flags]
    public enum CellFlags
    {
        None = 0,

        Walkable = 1 << 0,

        Tree = 1 << 1,

        MovementBlocker = 1 << 2,

        InteractionBlocker = 1 << 4,

        GridFlagObstacle = 1 << 7,

        FowRoshanBlocker = 1 << 8,

        Visible = 1 << 9
    }
}