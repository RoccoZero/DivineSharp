using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

namespace WolfInfo
{
    internal sealed class ShowCourierItems
    {
        public readonly MenuSwitcher optEnabled;

        public readonly Hero localHero;
        public ShowCourierItems(Context context)
        {
            var rootShowCourierItems = context.RootMenu.CreateMenu("Show Courier Items");
            optEnabled = rootShowCourierItems.CreateSwitcher("Show Courier Items", false);

            RendererManager.Draw += RendererManager_Draw;
            localHero = EntityManager.LocalHero;
        }

        private void RendererManager_Draw()
        {
            if (!optEnabled.Value)
                return;

            foreach (var courier in EntityManager.GetEntities<Courier>().Where(
                                                                        x => x.IsValid
                                                                        && x.IsVisible
                                                                        && !x.IsAlly(localHero)))
            {
                var courierPos = RendererManager.WorldToScreen(courier.Position + new Vector3(0, 0, courier.HealthBarOffset));
                RendererManager.DrawText("Enemy Courier", courierPos, Color.White, 20);
            }
        }
    }
}
