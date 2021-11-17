using Divine.Menu;
using Divine.Menu.Items;
using Divine.Order;

namespace WolfInfo
{
    internal sealed class Context
    {
        public readonly RootMenu RootMenu;
        public readonly CreepHPBars CreepHPBars;

        public Context()
        {
            RootMenu = MenuManager.CreateRootMenu("WolfInfo");
            CreepHPBars = new CreepHPBars(this);
            OrderManager.
        }

        public void Dispose()
        {
            CreepHPBars.Dispose();
        }
    }
}