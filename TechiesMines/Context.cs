using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Renderer.ValveTexture;

using TechiesMines.Enums;

namespace TechiesMines
{
    internal sealed class Context
    {
        private RootMenu RootMenu;
        private MenuSwitcher EnableMinePositions;
        private MenuSelector DisplayStyle;
        private MenuSelector DisplayType;
        private MenuSlider DisplayRange;
        private MenuHoldKey CustomKey;
        private MenuSlider TestSize;
        private Dictionary<Vector3, Mines> Positions;

        private Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        private Tuple<Vector3, bool> MouseWheelHovered = null;
        private Dictionary<Vector3, int> MouseWheelSelected = new Dictionary<Vector3, int>();
        private const int IMG_SIZE = 26;
        private const int GAP_SIZE = 4;
        private Team MyTeam = Team.None;
        private bool DisplayCondition;
        private bool IsKeyDown;

        private readonly Dictionary<string, string> MineImages = new Dictionary<string, string>()
        {
            { nameof(Mines.LandMine), "panorama/images/spellicons/techies_land_mines_png.vtex_c" },
            { nameof(Mines.StasisTrap), "panorama/images/spellicons/techies_stasis_trap_png.vtex_c" },
            { nameof(Mines.RemoteMine), "panorama/images/spellicons/techies_remote_mines_png.vtex_c" },
        };
        private readonly List<Mines> MainMines = new List<Mines>
        {
            Mines.LandMine,
            Mines.StasisTrap,
            Mines.RemoteMine
        };
        private string[] DisplayStyles = {
            "Vertical",
            "Horizontal",
            //"Mouse Wheel"
        };
        private string[] DisplayTypes = {
            "Always",
            "In Range",
            "Custom key"
        };

        public Context()
        {
            RootMenu = MenuManager.CreateRootMenu("Techies Mine Positions");
            EnableMinePositions = RootMenu.CreateSwitcher("Enabled");
            DisplayStyle = RootMenu.CreateSelector("Display Style", DisplayStyles);
            DisplayType = RootMenu.CreateSelector("Display Type", DisplayTypes);
            DisplayRange = RootMenu.CreateSlider("Display Range", 1500, 700, 5000);
            CustomKey = RootMenu.CreateHoldKey("Custom Key");
            MyTeam = EntityManager.LocalHero.Team;
            Positions = (MyTeam == Team.Radiant) ? MinePositions.Radiant : MinePositions.Dire;
            //TestSize = RootMenu.CreateSlider("Test Size", 100, 50, 300);

            RendererManager.LoadImageFromAssembly("TechiesMines.Rect.png", "TechiesMines.Resources.rect.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect2.png", "TechiesMines.Resources.rect2.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect3.png", "TechiesMines.Resources.rect3.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect4.png", "TechiesMines.Resources.rect4.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect5.png", "TechiesMines.Resources.rect5.png");

            foreach (var Mine in MineImages)
            {
                var gameFileStream = GameManager.OpenGameFile(Mine.Value);
                var binaryReader = new BinaryReader(gameFileStream);
                var valveTextureBlock = new Texture(binaryReader);
                if (valveTextureBlock.Data == null)
                    continue;
                var bitmap = valveTextureBlock.Data.Bitmap;
                var editedBitmap = bitmap.Round(0.5f);
                var memoryStream = new MemoryStream();
                editedBitmap.Save(memoryStream, ImageFormat.Png);
                //Console.WriteLine($"TechiesMines.{Mine.Key}.png");
                RendererManager.LoadImage($"TechiesMines.{Mine.Key}.png", memoryStream);
            }

            foreach (var spot in Positions)
            {
                MouseWheelSelected[spot.Key] = 1;
            }

            RendererManager.Draw += RendererManager_Draw;
            InputManager.MouseMove += InputManager_MouseMove;
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyDown += Panel.OnMouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
            InputManager.MouseWheel += InputManager_MouseWheel;
            Button.Button_Click += Button_Button_Click;
            DisplayType.ValueChanged += DisplayType_ValueChanged;
            //TestSize.ValueChanged += TestSize_ValueChanged;
        }

        private void TestSize_ValueChanged(MenuSlider slider, Divine.Menu.EventArgs.SliderEventArgs e)
        {
            Panel.size = e.NewValue;
        }

        private void Button_Button_Click(Button sender)
        {
            if ((sender.Mine & Mines.LandMine) == Mines.LandMine)
                EntityManager.LocalHero.Spellbook.Spell1.Cast(sender.PlantPos, true);
            else if ((sender.Mine & Mines.StasisTrap) == Mines.StasisTrap)
                EntityManager.LocalHero.Spellbook.Spell2.Cast(sender.PlantPos, true);
            else
                EntityManager.LocalHero.Spellbook.Spell6.Cast(sender.PlantPos, true);
        }

        private void DisplayType_ValueChanged(MenuSelector selector, Divine.Menu.EventArgs.SelectorEventArgs e)
        {
            switch (e.NewValue)
            {
                case "Always":
                    DisplayRange.IsHidden = true;
                    CustomKey.IsHidden = true;
                    if (CustomKey != null)
                        CustomKey.ValueChanged -= CustomKey_ValueChanged;
                    break;
                case "In Range":
                    DisplayRange.IsHidden = false;
                    CustomKey.IsHidden = true;
                    if (CustomKey != null)
                        CustomKey.ValueChanged -= CustomKey_ValueChanged;
                    break;
                case "Custom key":
                    CustomKey.ValueChanged += CustomKey_ValueChanged;
                    DisplayRange.IsHidden = true;
                    CustomKey.IsHidden = false;
                    break;
            }
        }

        private void CustomKey_ValueChanged(MenuHoldKey holdKey, Divine.Menu.EventArgs.HoldKeyEventArgs e)
        {
            IsKeyDown = e.Value;
        }

        private void InputManager_MouseWheel(MouseWheelEventArgs e)
        {
            if (DisplayStyle.Value != "Mouse Wheel" || MouseWheelHovered == null)
                return;

            if (!MouseWheelSelected.TryGetValue(MouseWheelHovered.Item1, out var value))
                MouseWheelSelected.Add(MouseWheelHovered.Item1, 1);

            int tempVal;
            if (e.Up)
            {
                if ((value << 1) <= 4)
                    tempVal = (value << 1);
                else
                    tempVal = value;

                //if (!Buttons.TryGetValue($"({MouseWheelHovered.Item1}){(Mines)tempVal}", out var buttonn))
                //    tempVal <<= 1;
            }
            else
            {
                if ((value >> 1) >= 1)
                    tempVal = (value >> 1);
                else
                    tempVal = value;

                //if (!Buttons.TryGetValue($"({MouseWheelHovered.Item1}){(Mines)tempVal}", out var buttonn))
                //    tempVal >>= 1;
            }
            Console.WriteLine(Buttons.TryGetValue($"({MouseWheelHovered.Item1}){(Mines)tempVal}", out var buttonn));

            MouseWheelSelected[MouseWheelHovered.Item1] = tempVal;
            MouseWheelHovered = null;
        }

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (DisplayStyle.Value == "Mouse Wheel" && MouseWheelHovered == null)
                return;

            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseMove(e);
            }
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (DisplayStyle.Value == "Mouse Wheel" && MouseWheelHovered == null)
                return;

            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseKeyUp(e);
            }
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (DisplayStyle.Value == "Mouse Wheel" && MouseWheelHovered == null)
                return;

            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseKeyDown(e);
            }
        }

        private void RendererManager_Draw()
        {
            //Panel.ButtonDrawOn();
            if (!EnableMinePositions.Value)
                return;

            DisplayCondition = true;
            if (DisplayType.Value == "Custom key")
                DisplayCondition = IsKeyDown;

            if (!DisplayCondition)
                return;

            foreach (var val in Positions)
            {
                if (DisplayType.Value == "In Range" && EntityManager.LocalHero.Distance2D(val.Key) > DisplayRange.Value)
                    continue;

                var screenPos = RendererManager.WorldToScreen(val.Key);
                if (screenPos.IsZero)
                    continue;

                var rect = new RectangleF(screenPos.X, screenPos.Y, IMG_SIZE + (GAP_SIZE * 2), IMG_SIZE + (GAP_SIZE * 2));
                var textureKey = "TechiesMines.Rect.png";
                var minesInSpot = val.Value & Mines.LandMine_StasisTrap_RemoteMine;

                if (minesInSpot == Mines.LandMine_RemoteMine
                    || minesInSpot == Mines.LandMine_StasisTrap
                    || minesInSpot == Mines.StasisTrap_RemoteMine)
                {
                    if (DisplayStyle.Value == "Vertical")
                    {
                        rect.Height = (IMG_SIZE * 2) + (GAP_SIZE * 3);
                        textureKey = "TechiesMines.Rect2.png";
                    }
                    else if (DisplayStyle.Value == "Horizontal")
                    {
                        rect.Width = (IMG_SIZE * 2) + (GAP_SIZE * 3);
                        textureKey = "TechiesMines.Rect4.png";
                    }
                }
                else if (minesInSpot == Mines.LandMine_StasisTrap_RemoteMine)
                {
                    if (DisplayStyle.Value == "Vertical")
                    {
                        rect.Height = (IMG_SIZE * 3) + (GAP_SIZE * 4);
                        textureKey = "TechiesMines.Rect3.png";
                    }
                    else if (DisplayStyle.Value == "Horizontal")
                    {
                        rect.Width = (IMG_SIZE * 3) + (GAP_SIZE * 4);
                        textureKey = "TechiesMines.Rect5.png";
                    }
                }

                RendererManager.DrawImage(textureKey, rect);

                var xOffset = GAP_SIZE;
                var yOffset = GAP_SIZE;
                var imgHoverSize = 1.5f;

                foreach (var mine in MainMines)
                {
                    if ((val.Value & mine) != mine)
                        continue;

                    if (DisplayStyle.Value == "Mouse Wheel" && ((Mines)MouseWheelSelected[val.Key]) != mine)
                        continue;

                    var imgRect = new RectangleF(screenPos.X + xOffset, screenPos.Y + yOffset, IMG_SIZE, IMG_SIZE);

                    if (InputManager.MousePosition.IsUnderRectangle(imgRect))
                    {
                        if (MouseWheelHovered == null)
                            MouseWheelHovered = Tuple.Create(val.Key, true);
                        imgRect.X -= imgHoverSize;
                        imgRect.Y -= imgHoverSize;
                        imgRect.Width += imgHoverSize * 2;
                        imgRect.Height += imgHoverSize * 2;
                    }

                    if (!Buttons.TryGetValue($"({val.Key}){mine}", out var buttonMine))
                        Buttons.Add($"({val.Key}){mine}", new Button(imgRect, val.Key, mine));

                    buttonMine?.UpdateRect(imgRect);
                    buttonMine?.Draw();

                    if (DisplayStyle.Value == "Vertical")
                        yOffset += IMG_SIZE + GAP_SIZE;
                    else if (DisplayStyle.Value == "Horizontal")
                        xOffset += IMG_SIZE + GAP_SIZE;
                }
            }
        }

        internal void Dispose()
        {

        }
    }
}
