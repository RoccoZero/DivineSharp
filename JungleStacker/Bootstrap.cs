using Divine.Service;

namespace JungleStacker
{
    internal sealed class Bootstrap : Bootstrapper
    {
        public Context context;

        protected override void OnActivate()
        {
            context = new Context();
        }

        protected override void OnDeactivate()
        {
            context?.Dispose();
        }
    }
}
