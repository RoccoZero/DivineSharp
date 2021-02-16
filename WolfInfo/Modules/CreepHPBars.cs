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
        public readonly MenuSlider OffsetX;
        public readonly MenuSlider OffsetY;
        public readonly MenuSlider BarWidth;
        public readonly MenuSlider BarHeight;

        private readonly Hero localHero = EntityManager.LocalHero;

        public CreepHPBars(Context context)
        {
            var rootCreepHPBars = context.RootMenu.CreateMenu("Creep HP Bars", "Creep HP Bars");
            optEnabledAlly = rootCreepHPBars.CreateSwitcher("Ally Creeps");
            optEnabledEnemy = rootCreepHPBars.CreateSwitcher("Enemy Creeps");
            DisplayRadius = rootCreepHPBars.CreateSlider("Display Radius", 700, 500, 1500);
            TextSize = rootCreepHPBars.CreateSlider("Text Size", 20, 10, 50);
            BarWidth = rootCreepHPBars.CreateSlider("Bar Width", 100, 50, 300);
            BarHeight = rootCreepHPBars.CreateSlider("Bar Height", 10, 5, 50);
            OffsetX = rootCreepHPBars.CreateSlider("Offset X", 0, -200, 200);
            OffsetY = rootCreepHPBars.CreateSlider("Offset Y", 0, -50, 50);

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
                else if (optEnabledAlly.Value)
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
            var textPos = new Vector2(unitScreenPos.X + OffsetX.Value - (textSize.X * 0.5f) - 2, unitScreenPos.Y + OffsetY.Value + (BarHeight.Value * 0.5f));

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
            DrawTextCentered(text, new Vector2(position.X + scaling, position.Y + scaling), new Color(0, 0, 0, (int)color.A), fontFamilyName, fontSize);
            DrawTextCentered(text, new Vector2(position.X + scaling, position.Y - scaling), new Color(0, 0, 0, (int)color.A), fontFamilyName, fontSize);
            DrawTextCentered(text, new Vector2(position.X - scaling, position.Y - scaling), new Color(0, 0, 0, (int)color.A), fontFamilyName, fontSize);
            DrawTextCentered(text, new Vector2(position.X - scaling, position.Y + scaling), new Color(0, 0, 0, (int)color.A), fontFamilyName, fontSize);
            DrawTextCentered(text, position, color, fontFamilyName, fontSize);
        }

        public static void DrawTextCentered(string text, Vector2 position, Color color, string fontFamilyName, float fontSize)
        {
            var textSize = RendererManager.MeasureText(text, fontFamilyName, fontSize);
            RendererManager.DrawText(text, new Vector2(position.X - (textSize.X * 0.5f), position.Y - (textSize.Y * 0.5f)), color, fontFamilyName, fontSize);
        }
    }
}
