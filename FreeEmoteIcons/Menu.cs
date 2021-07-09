using Divine.Menu;
using Divine.Menu.Items;

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