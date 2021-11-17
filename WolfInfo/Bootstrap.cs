using Divine.Service;

namespace WolfInfo
{
    internal sealed class Bootstrap : Bootstrapper
    {
        public Context Context;

        protected override void OnActivate()
        {
            Context = new Context();
        }

        protected override void OnDeactivate()
        {
            Context.Dispose();
        }
    }
}