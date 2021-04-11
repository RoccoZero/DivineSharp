using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divine.Menu.Items;
using Divine.Menu.EventArgs;
using Divine;
using Divine.SDK.Extensions;

namespace RegenFailSwitch
{
    internal sealed class RegenFailSwitch
    {

        private readonly Dictionary<AbilityId, (string, string)> RegenItems = new()
        {
            { AbilityId.item_tango, ("modifier_tango_heal", "health") },
            { AbilityId.item_flask, ("modifier_flask_healing", "health") },
            { AbilityId.item_clarity, ("modifier_clarity_potion", "mana") },
            { AbilityId.item_urn_of_shadows, ("modifier_item_urn_heal", "health") },
            { AbilityId.item_spirit_vessel, ("modifier_item_spirit_vessel_heal", "health") },
            { AbilityId.item_bottle, ("modifier_bottle_regeneration", "both") },
            { AbilityId.item_magic_stick, (string.Empty, "both") },
            { AbilityId.item_magic_wand, (string.Empty, "both") },
            { AbilityId.item_holy_locket, (string.Empty, "both") },
            { AbilityId.item_enchanted_mango, (string.Empty, "mana") },
            { AbilityId.item_faerie_fire, (string.Empty, "health") }
        };
        private readonly MenuItemToggler Items;

        public RegenFailSwitch(Context context)
        {
            Items = context.Menu.Items;
            context.Menu.Enabled.ValueChanged += Enabled_ValueChanged;
        }

        private void Enabled_ValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                OrderManager.OrderAdding += OrderManager_OrderAdding;
            }
            else
            {
                OrderManager.OrderAdding -= OrderManager_OrderAdding;
            }
        }

        private void OrderManager_OrderAdding(OrderAddingEventArgs e)
        {
            if (e.IsCustom || 
                (e.Order.Type != OrderType.CastTarget && e.Order.Type != OrderType.Cast) || 
                !RegenItems.TryGetValue(e.Order.Ability.Id, out var modifier))
            {
                return;
            }

            if (!Items[e.Order.Ability.Id])
            {
                return;
            }

            switch (e.Order.Type)
            {
                case OrderType.CastTarget:
                    e.Process = CheckRegen((Unit)e.Order.Target, modifier.Item1, modifier.Item2);
                    break;
                case OrderType.Cast:
                    e.Process = CheckRegen(e.Order.Units.FirstOrDefault(), modifier.Item1, modifier.Item2);
                    break;
            }
        }

        internal bool CheckRegen(Unit unit, string modifier, string type)
        {
            return type switch
            {
                "health" when unit.Health == unit.MaximumHealth || unit.HasModifier(modifier) => false,
                "mana" when unit.Mana == unit.MaximumMana || unit.HasModifier(modifier) => false,
                "both" when (unit.Health == unit.MaximumHealth && unit.Mana == unit.MaximumMana) || unit.HasModifier(modifier) => false,
                {} => true
            };
        }

        internal void Dispose()
        {
            
        }
    }
}
