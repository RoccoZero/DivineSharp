using Divine.Menu;
using Divine.Menu.Items;

namespace Changer
{
    internal sealed class Menu
    {
        public readonly RootMenu RootMenu;
        public readonly MenuSelector RiverType;
        public readonly MenuSelector TreeModel;
        public readonly MenuSlider TreeModelScale;
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
            "models/props_diretide/pumpkin_head",
            "models/props_gameplay/pumpkin_bucket",
            "models/particle/snowball",
            "models/effects/fountain_radiant_00",
            "models/props_magic/bad_magic_tower001",
            "models/props/stoneforest/stoneforest01",
            "models/props_debris/barrel002",
            "models/props_frostivus/frostivus_snowman",
            "models/props_generic/sphere",
            "models/props_structures/crystal003",
            //"models/vr_env/dendi_head_model"
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
            TreeModelScale = RootMenu.CreateSlider("Tree model scale (%)", 100, 1, 500);

        }
    }
}
