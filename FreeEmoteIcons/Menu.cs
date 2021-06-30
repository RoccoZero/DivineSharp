using Divine.Menu;
using Divine.Menu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeEmoteIcons
{
    internal sealed class Menu
    {
        public readonly RootMenu RootMenu;
        public readonly MenuSwitcher Enabled;

        public Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("FreeEmoteIcons");
            Enabled = RootMenu.CreateSwitcher("Enabled");
        }
    }
}
