using Divine.Service;

namespace FreeEmoteIcons
{
    public class Bootstrap : Bootstrapper
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
