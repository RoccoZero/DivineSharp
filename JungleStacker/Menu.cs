<<<<<<< Updated upstream
﻿using Divine.Input;
using Divine.Menu;
=======
﻿using Divine.Menu;
>>>>>>> Stashed changes
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
            AutoSelectUnits = RootMenu.CreateHoldKey("Auto Select Units for Stack", Key.None);
        }
    }
}
