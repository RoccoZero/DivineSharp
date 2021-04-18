using Divine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
