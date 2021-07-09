using Divine.Service;

namespace RegenFailSwitch
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
