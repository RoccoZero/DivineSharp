using Divine;
using Divine.Menu;
using Divine.Menu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegenFailSwitch
{
    internal sealed class Menu
    {
        public readonly RootMenu RootMenu;
        public readonly MenuSwitcher Enabled;
        public readonly MenuItemToggler Items;

        public Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("RegenFailSwitch");
            Enabled = RootMenu.CreateSwitcher("Enable RegenFailSwitch");
            Items =  RootMenu.CreateItemToggler("Items", new() {
                { AbilityId.item_tango, true },
                { AbilityId.item_flask, true },
                { AbilityId.item_clarity, true },
                { AbilityId.item_urn_of_shadows, true },
                { AbilityId.item_spirit_vessel, true },
                { AbilityId.item_bottle, true },
                { AbilityId.item_magic_stick, true },
                { AbilityId.item_magic_wand, true },
                { AbilityId.item_holy_locket, true },
                { AbilityId.item_enchanted_mango, true },
                { AbilityId.item_faerie_fire, true }
            }, false, true);
        }

    }
}
