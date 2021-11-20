using Divine.Menu;
using Divine.Menu.Items;

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
        }

        public void Dispose()
        {
            CreepHPBars.Dispose();
        }
    }
}