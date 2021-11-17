using Divine.Service;
using Divine.Update;

namespace Overwolf
{
    public class Bootstrap : Bootstrapper
    {
        private Context context;

        protected override void OnPreActivate()
        {
            UpdateManager.BeginInvoke(() =>
            {
                context = new Context();
            });
        }

        protected override void OnDeactivate()
        {
            context?.Dispose();
        }
    }
}