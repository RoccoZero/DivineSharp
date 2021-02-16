using System;
using System.Linq;

using Divine;
using Divine.Menu.Items;
using Divine.SDK.Extensions;

using SharpDX;

namespace WolfInfo
{
    internal sealed class CreepHPBars
    {
        public readonly MenuSwitcher optEnabledAlly;
        public readonly MenuSwitcher optEnabledEnemy;

        public readonly MenuSlider DisplayRadius;
        public readonly MenuSlider TextSize;
        public readonly MenuSlider BarWidth;
        public readonly MenuSlider BarHeight;
        public readonly MenuSlider OffsetX;
        public readonly MenuSlider OffsetY;
        public Unit[] creeps;

        private readonly Hero localHero = EntityManager.LocalHero;

        public CreepHPBars(Context context)
        {
            var rootCreepHPBars = context.RootMenu.CreateMenu("Creep HP Bars", "Creep HP Bars");
            optEnabledAlly = rootCreepHPBars.CreateSwitcher("Ally Creeps", false);
            optEnabledEnemy = rootCreepHPBars.CreateSwitcher("Enemy Creeps", false);
            var rootCreepHPBarsSettings = rootCreepHPBars.CreateMenu("Settings");
            DisplayRadius = rootCreepHPBarsSettings.CreateSlider("Display Radius", 700, 500, 1500);
            TextSize = rootCreepHPBarsSettings.CreateSlider("Text Size", 20, 10, 50);
            BarWidth = rootCreepHPBarsSettings.CreateSlider("Bar Width", 100, 50, 300);
            BarHeight = rootCreepHPBarsSettings.CreateSlider("Bar Height", 10, 5, 50);
            OffsetX = rootCreepHPBarsSettings.CreateSlider("Offset X", 0, -200, 200);
            OffsetY = rootCreepHPBarsSettings.CreateSlider("Offset Y", 0, -50, 50);

            RendererManager.Draw += RendererManager_Draw;
        }

        public void Dispose()
        {
            //TODO
        }

        private void RendererManager_Draw()
        {
            if (!optEnabledAlly.Value && !optEnabledEnemy.Value)
                return;

            foreach (var unit in EntityManager.GetEntities<Creep>().Where(
                                x => x.IsValid 
                                && x.IsSpawned 
                                && (x.ClassId == ClassId.CDOTA_BaseNPC_Creep 
                                || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane
                                || x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege)
                                && !x.IsDormant 
                                && x.IsVisible 
                                && x.IsAlive 
                                && x.Distance2D(localHero) <= DisplayRadius.Value))
            {
                if (!unit.IsAlly(localHero) && optEnabledEnemy.Value)
                {
                    DrawCreepHP(unit);
                }
                else if (unit.IsAlly(localHero) && optEnabledAlly.Value)
                {
                    DrawCreepHP(unit);
                }
            }
        }
        
       

        private void DrawCreepHP(Unit unit)
        {
            var unitPos = unit.Position + new Vector3(0, 0, unit.HealthBarOffset);
            var unitScreenPos = RendererManager.WorldToScreen(unitPos);
            var barHPWidth = (int)(BarWidth.Value * unit.HealthPercent());
            var barHitHPWidth = (int)(BarWidth.Value * (localHero.GetAttackDamage(unit, true) / unit.MaximumHealth));
            var hitCount = (int)Math.Ceiling(unit.Health / localHero.GetAttackDamage(unit, true));

            if (barHitHPWidth > barHPWidth)
            {
                barHitHPWidth = barHPWidth;
            }
            var barHitHPXOffset = (barHPWidth - barHitHPWidth);

            Color Color1;
            Color Color2;
            Color Color3;
            if (!unit.IsAlly(localHero))
            {
                Color1 = new Color(230, 50, 50);
                Color2 = new Color(180, 3, 8);
                Color3 = Color.Red;
            }
            else
            {
                Color1 = new Color(50, 230, 50);
                Color2 = new Color(3, 180, 8);
                Color3 = Color.Green;
            }
            RendererManager.DrawFilledRectangle(
                new RectangleF(
                    unitScreenPos.X + OffsetX.Value - 1,
                    unitScreenPos.Y + OffsetY.Value - 1,
                    BarWidth.Value + 2,
                    BarHeight.Value + 2),
                Color.Black);
            RendererManager.DrawFilledRectangle(
                new RectangleF(
                    unitScreenPos.X + OffsetX.Value,
                    unitScreenPos.Y + OffsetY.Value,
                    barHPWidth,
                    BarHeight.Value),
                Color1);
            RendererManager.DrawFilledRectangle(
                new RectangleF(
                    unitScreenPos.X + OffsetX.Value + barHitHPXOffset - 1,
                    unitScreenPos.Y + OffsetY.Value,
                    barHitHPWidth + 2,
                    BarHeight.Value),
                Color.Black);
            RendererManager.DrawFilledRectangle(
                new RectangleF(
                    unitScreenPos.X + OffsetX.Value + barHitHPXOffset,
                    unitScreenPos.Y + OffsetY.Value,
                    barHitHPWidth,
                    BarHeight.Value),
                Color2);
            var textSize = RendererManager.MeasureText(hitCount.ToString(), "Tahoma", TextSize.Value);
            var textPos = new Vector2(unitScreenPos.X + OffsetX.Value - (float)(textSize.X * 0.5f) - 2, unitScreenPos.Y + OffsetY.Value + (float)(BarHeight.Value * 0.5f));

            DrawBlackText(
                hitCount.ToString(),
                textPos,
                Color3,
                "Tahoma",
                TextSize.Value);
        }

        public static void DrawBlackText(string text, Vector2 position, Color color, string fontFamilyName, float fontSize)
        {
            var scaling = (fontSize * 0.04f);
            RendererManager.DrawText(
                text,
                new RectangleF(
                    position.X + scaling - (ushort.MaxValue / 2),
                    position.Y + scaling - (ushort.MaxValue / 2),
                    ushort.MaxValue,
                    ushort.MaxValue),
                new Color(0, 0, 0, (int)color.A),
                fontFamilyName,
                FontFlags.Center | FontFlags.VerticalCenter,
                fontSize);
            RendererManager.DrawText(
                text,
                new RectangleF(
                    position.X + scaling - (ushort.MaxValue / 2),
                    position.Y - scaling - (ushort.MaxValue / 2),
                    ushort.MaxValue,
                    ushort.MaxValue),
                new Color(0, 0, 0, (int)color.A),
                fontFamilyName,
                FontFlags.Center | FontFlags.VerticalCenter,
                fontSize);
            RendererManager.DrawText(
                text,
                new RectangleF(
                    position.X - scaling - (ushort.MaxValue / 2),
                    position.Y - scaling - (ushort.MaxValue / 2),
                    ushort.MaxValue,
                    ushort.MaxValue),
                new Color(0, 0, 0, (int)color.A),
                fontFamilyName,
                FontFlags.Center | FontFlags.VerticalCenter,
                fontSize);
            RendererManager.DrawText(
                text,
                new RectangleF(
                    position.X - scaling - (ushort.MaxValue / 2),
                    position.Y + scaling - (ushort.MaxValue / 2),
                    ushort.MaxValue,
                    ushort.MaxValue),
                new Color(0, 0, 0, (int)color.A),
                fontFamilyName,
                FontFlags.Center | FontFlags.VerticalCenter,
                fontSize);
            RendererManager.DrawText(
                text,
                new RectangleF(
                    position.X - (ushort.MaxValue / 2),
                    position.Y - (ushort.MaxValue / 2),
                    ushort.MaxValue,
                    ushort.MaxValue),
                color,
                fontFamilyName,
                FontFlags.Center | FontFlags.VerticalCenter,
                fontSize);
        }
    }
}
