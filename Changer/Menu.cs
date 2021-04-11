using Divine;
using Divine.Menu;
using Divine.Menu.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Changer
{
    internal sealed class Menu
    {
        public readonly RootMenu RootMenu;
        public readonly MenuSelector RiverType;
        public readonly MenuSelector TreeModel;
        public readonly string[] RiverTypes = new string[]
        {
            "Default",
            "Chrome",
            "Dry",
            "Slime",
            "Oil",
            "Electric",
            "Potion",
            "Blood"
        };
            
        public readonly string[] TreeModels = new string[]
        {
            "OFF",
            "models/props_stone/stone_column002a",
            "models/props_structures/pumpkin003",
            "models/props_gameplay/pumpkin_bucket",
            "models/particle/snowball",
            "models/effects/fountain_radiant_00",
            "models/props_magic/bad_magic_tower001"
        };

        public Menu()
        {
            RootMenu = MenuManager.CreateRootMenu("Changer");
            RiverType = RootMenu.CreateSelector(
                 "River type",
                 RiverTypes
            );
            TreeModel = RootMenu.CreateSelector(
                 "Tree model",
                 TreeModels
            );

        }
    }
}
