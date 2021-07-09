using Divine.Service;

namespace Changer
{
    public class BootStrap : Bootstrapper
    {
        private Context context;

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