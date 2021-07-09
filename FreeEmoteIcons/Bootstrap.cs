using Divine;
using Divine.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
