using Divine.Menu;
using Divine.Menu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreepHPBars
{
    internal sealed class MenuInit
    {
        public readonly MenuSwitcher optEnabledAlly;
        public readonly MenuSwitcher optEnabledEnemy;
        public readonly MenuSlider DisplayRadius;
        public readonly MenuSlider TextSize;
        public readonly MenuSlider OffsetX;
        public readonly MenuSlider OffsetY;
        public readonly MenuSlider BarWidth;
        public readonly MenuSlider BarHeight;

        public MenuInit()
        {
            var rootMenu = MenuManager.CreateRootMenu("Ender_Wolf");
            var rootCreepHPBars = rootMenu.CreateMenu("Creep HP Bars", "Creep HP Bars");
            optEnabledAlly = rootCreepHPBars.CreateSwitcher("Ally Creeps");
            optEnabledEnemy = rootCreepHPBars.CreateSwitcher("Enemy Creeps");
            DisplayRadius = rootCreepHPBars.CreateSlider("Display Radius", 700, 500, 1500);
            TextSize = rootCreepHPBars.CreateSlider("Text Size", 20, 10, 50);
            BarWidth = rootCreepHPBars.CreateSlider("Bar Width", 100, 50, 300);
            BarHeight = rootCreepHPBars.CreateSlider("Bar Height", 10, 5, 50);
            OffsetX = rootCreepHPBars.CreateSlider("Offset X", 0, -200, 200);
            OffsetY = rootCreepHPBars.CreateSlider("Offset Y", 0, -50, 50);
        }
    }
}
