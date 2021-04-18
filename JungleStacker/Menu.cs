using Divine.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divine.Menu.Items;

namespace JungleStacker
{
    internal sealed class Menu
    {
        public MenuSwitcher Enabled;
        public MenuHoldKey AutoSelectUnits;

        public Menu()
        {
            var RootMenu = MenuManager.CreateRootMenu("Jungle Stacker");
            Enabled = RootMenu.CreateSwitcher("Enabled");
            AutoSelectUnits = RootMenu.CreateHoldKey("Auto Select Units for Stack", System.Windows.Input.Key.None);
        }
    }
}
