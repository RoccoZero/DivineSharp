using System;
using System.Collections.Generic;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;

using O9K.Core.Entities.Abilities.Base;

using TechiesMines.Enums;

namespace TechiesMines
{
    public static class Panel
    {
        public static Lane lane;

        private static readonly int _optionsCount = Enum.GetNames(typeof(Lane)).Length;

        private static readonly Dictionary<Lane, Vector4> vector4PosClick = new();

        public static Vector2 SizePanel;

        public static string unitName { get; set; }
        public static bool pushComboStatus { get; set; }
        public static int size { get; set; }

        private static List<AbilityId> CloneItems { get; } =
            new()
            {
                AbilityId.item_hand_of_midas,
                AbilityId.item_tpscroll
            };

        private static Dictionary<AbilityId, Ability9> CloneItemsAbilities { get; } = new();

        public static void OnMouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            for (int i = 0; i < _optionsCount; i++)
            {
                if (e.Position.IsUnderRectangle(new RectangleF(vector4PosClick[(Lane)i].X - 5, vector4PosClick[(Lane)i].Y - 5, vector4PosClick[(Lane)i].Z, vector4PosClick[(Lane)i].W)))
                {
                    lane = (Lane)i;
                }
            }
        }

        public static void ButtonDrawOn()
        {
            float scaling = RendererManager.Scaling;
            int optionsCount = _optionsCount;
            var positionX = 400;
            var positionY = 300;
            var sizeMenu = (size * scaling);
            var indent = (sizeMenu * 0.2f);
            var halfIndent = indent * 0.5f;
            var menuWidth = (sizeMenu * optionsCount) + (indent * 2);
            var menuHeight = (sizeMenu * 7.76f) + (indent * 2);

            //Draw main rect without identst
            var mainRect = new RectangleF(
                positionX - indent,
                positionY - indent,
                menuWidth + (indent * 2),
                menuHeight + (indent * 2));
            RendererManager.DrawFilledRectangle(mainRect, Color.Black);

            //Draw rect with identst
            var rect = new RectangleF(
                positionX,
                positionY,
                menuWidth,
                menuHeight);
            RendererManager.DrawFilledRectangle(rect, Color.Gray);

            vector4PosClick.Clear();

            var inRectPosX = rect.X;
            var inRectPosY = rect.Y;
            var rectHalfWidth = (rect.Width * 0.5f);

            //Draw Divine logo
            var imgSize = sizeMenu;
            var imgHalfSize = (imgSize * 0.5f);
            var divineLogoRect = new RectangleF(inRectPosX + rectHalfWidth - imgHalfSize, inRectPosY, imgSize, imgSize);
            RendererManager.DrawRectangle(divineLogoRect, Color.Gold);
            inRectPosY += imgSize + indent;
            inRectPosX = rect.X;

            //Draw text: "FARM STATUS"
            var farmStatusTextWidth = rect.Width;
            var farmStatusTextHeight = imgHalfSize;
            var farmStatusTextRect = new RectangleF(inRectPosX, inRectPosY, farmStatusTextWidth, farmStatusTextHeight);
            var farmStatusTextSize = sizeMenu * 0.4f;
            RendererManager.DrawRectangle(farmStatusTextRect, Color.Indigo);
            RendererManager.DrawText("FARM STATUS", farmStatusTextRect, Color.Indigo, FontFlags.Center | FontFlags.VerticalCenter, farmStatusTextSize);
            inRectPosY += farmStatusTextHeight + halfIndent;
            inRectPosX = rect.X;

            //Draw farm status
            var farmStatusHeight = imgHalfSize * 1.5f;
            var farmStatusRect = new RectangleF(inRectPosX, inRectPosY, rect.Width, farmStatusHeight);
            var farmStatusSize = sizeMenu * 0.7f;
            RendererManager.DrawRectangle(farmStatusRect, Color.Green);
            RendererManager.DrawText("ACTIVATED", farmStatusRect, Color.Green, FontFlags.Center | FontFlags.VerticalCenter, farmStatusSize);
            inRectPosY += farmStatusHeight + indent;
            inRectPosX = rect.X;

            //Draw text: "LINE SELECT FARM"
            var lineTextWidth = rect.Width;
            var lineTextHeight = imgHalfSize;
            var lineTextRect = new RectangleF(inRectPosX, inRectPosY, lineTextWidth, lineTextHeight);
            var lineTextSize = farmStatusTextSize;
            RendererManager.DrawRectangle(lineTextRect, Color.Chartreuse);
            RendererManager.DrawText("LINE SELECT FARM", lineTextRect, Color.Chartreuse, FontFlags.Center | FontFlags.VerticalCenter, lineTextSize);
            inRectPosY += lineTextHeight + halfIndent;
            inRectPosX = rect.X;

            //Draw modes for clone farm
            float buttonFontSize = sizeMenu * 0.35f;
            var buttonWidth = imgSize;
            var buttonHeight = imgHalfSize;

            for (int i = 0; i < optionsCount; i++)
            {
                vector4PosClick.Add((Lane)i, new Vector4(inRectPosX, inRectPosY, buttonWidth, buttonHeight));

                var rectBorderImage = new RectangleF(
                    inRectPosX,
                    inRectPosY,
                    buttonWidth,
                    buttonHeight);

                if ((Lane)i == lane)
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Green);

                    RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.Green, FontFlags.Center | FontFlags.VerticalCenter, buttonFontSize);
                }
                else
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Red);

                    RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.White, FontFlags.Center | FontFlags.VerticalCenter, buttonFontSize);
                }

                inRectPosX += buttonWidth + (indent * 0.68f);
            }
            inRectPosX = rect.X;
            inRectPosY += buttonHeight + indent;

            //Draw text: "ARC TARGET COMBO"
            var targetTextWidth = rect.Width;
            var targetTextHeight = imgHalfSize;
            var targetTextRect = new RectangleF(inRectPosX, inRectPosY, targetTextWidth, targetTextHeight);
            var targetTextSize = farmStatusTextSize;
            RendererManager.DrawRectangle(targetTextRect, Color.BlueViolet);
            RendererManager.DrawText("ARC TARGET COMBO", targetTextRect, Color.BlueViolet, FontFlags.Center | FontFlags.VerticalCenter, targetTextSize);
            inRectPosY += targetTextHeight + halfIndent;
            inRectPosX = rect.X;

            //Draw target image
            var targetImgWidth = imgSize * 2;
            var targetImgHeight = imgSize;
            var targetImgRect = new RectangleF(inRectPosX + rectHalfWidth - imgSize, inRectPosY, targetImgWidth, targetImgHeight);
            RendererManager.DrawRectangle(targetImgRect, Color.Cyan);
            inRectPosY += targetImgHeight + indent;
            inRectPosX = rect.X;

            //Draw text: "CLONE COOLDOWN ITEMS"
            var cloneCDsTextWidth = rect.Width;
            var cloneCDsTextHeight = imgHalfSize;
            var cloneCDsTextRect = new RectangleF(inRectPosX, inRectPosY, cloneCDsTextWidth, cloneCDsTextHeight);
            var cloneCDsTextSize = farmStatusTextSize;
            RendererManager.DrawRectangle(cloneCDsTextRect, Color.DarkKhaki);
            RendererManager.DrawText("CLONE COOLDOWN ITEMS", cloneCDsTextRect, Color.DarkKhaki, FontFlags.Center | FontFlags.VerticalCenter, cloneCDsTextSize);
            inRectPosY += cloneCDsTextHeight + indent;
            inRectPosX = rect.X;

            //Draw clone mids and boots
            var itemImgWidth = rect.Width * 0.48f;
            var itemImgHeight = imgSize;
            var itemIndent = rect.Width * 0.04f;

            //Draw text: "MIDAS"
            var itemMidasTextWidth = rect.Width;
            var itemMidasTextHeight = imgHalfSize;
            var itemMidasTextRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemMidasTextHeight);
            var itemMidasTextSize = buttonFontSize;
            RendererManager.DrawRectangle(itemMidasTextRect, Color.MediumBlue);
            RendererManager.DrawText("MIDAS", itemMidasTextRect, Color.MediumBlue, FontFlags.Center | FontFlags.VerticalCenter, itemMidasTextSize);
            inRectPosX += itemImgWidth + itemIndent;

            //Draw text: "BOOTS"
            var itemBootsTextWidth = rect.Width;
            var itemBootsTextHeight = imgHalfSize;
            var itemBootsTextRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemBootsTextHeight);
            var itemBootsTextSize = buttonFontSize;
            RendererManager.DrawRectangle(itemBootsTextRect, Color.MediumBlue);
            RendererManager.DrawText("BOOTS", itemBootsTextRect, Color.MediumBlue, FontFlags.Center | FontFlags.VerticalCenter, itemBootsTextSize);
            inRectPosY += itemBootsTextHeight + halfIndent;
            inRectPosX = rect.X;

            //Draw midas image
            var itemMidasRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemImgHeight);
            RendererManager.DrawRectangle(itemMidasRect, Color.Maroon);
            inRectPosX += itemImgWidth + itemIndent;

            //Draw boots image
            var itemBootsRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemImgHeight);
            RendererManager.DrawRectangle(itemBootsRect, Color.Maroon);
        }
    }
}
