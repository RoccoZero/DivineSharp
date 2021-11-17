using SharpDX;

namespace Divine.Core.Managers.Mesh
{
    internal struct Vision
    {
        public Vision(Vector3 position, uint dayVision, uint nightVision, bool flying)
        {
            Position = position;
            DayVision = dayVision;
            NightVision = nightVision;
            IsFlying = flying;
        }

        public readonly Vector3 Position;

        public readonly uint DayVision;

        public readonly uint NightVision;

        public readonly bool IsFlying;
    }
}