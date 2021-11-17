using Divine.Service;

namespace TestScript
{
    public class Bootstrap : Bootstrapper
    {
        private NavMeshPathfinding MeshPathFinder;

        protected override void OnActivate()
        {
            MeshPathFinder = new NavMeshPathfinding();
        }

        protected override void OnDeactivate()
        {
            MeshPathFinder?.Dispose();
        }
    }
}
