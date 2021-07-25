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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

using TechiesMines.Enums;

namespace TechiesMines
{
    internal sealed class Context
    {
        private RootMenu RootMenu;
        private MenuSwitcher EnableMinePositions;
        private MenuSelector DisplayType;
        private Team MyTeam = Team.None;
        private Dictionary<Vector3, Mines> Positions;
        private Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        private const int IMG_SIZE = 26;
        private const int GAP_SIZE = 4;

        private readonly Dictionary<string, string> MineImages = new Dictionary<string, string>()
        {
            { Mines.LandMine.ToString(), "panorama/images/spellicons/techies_land_mines_png.vtex_c" },
            { Mines.StasisTrap.ToString(), "panorama/images/spellicons/techies_stasis_trap_png.vtex_c" },
            { Mines.RemoteMine.ToString(), "panorama/images/spellicons/techies_remote_mines_png.vtex_c" },
        };
        private Dictionary<Vector3, bool> Checked = new Dictionary<Vector3, bool>();
        private string[] DisplayTypes = {
            "Always",
            //"In Range",
            "Custom key"
        };
        private bool DisplayCondition;
        private bool IsKeyDown;
        private MenuSlider DisplayRange;
        private MenuHoldKey CustomKey;

        public Context()
        {
            RootMenu = MenuManager.CreateRootMenu("Techies Mine Positions");
            EnableMinePositions = RootMenu.CreateSwitcher("Enabled");
            DisplayType = RootMenu.CreateSelector("Display type", DisplayTypes);
            DisplayRange = RootMenu.CreateSlider("Display Range", 700, 300, 1500);
            CustomKey = RootMenu.CreateHoldKey("Custom Key");
            MyTeam = EntityManager.LocalHero.Team;
            Positions = (MyTeam == Team.Radiant) ? MinePositions.Radiant : MinePositions.Dire;

            RendererManager.LoadImageFromAssembly("TechiesMines.Rect.png", "TechiesMines.Resources.rect.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect2.png", "TechiesMines.Resources.rect2.png");
            RendererManager.LoadImageFromAssembly("TechiesMines.Rect3.png", "TechiesMines.Resources.rect3.png");

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

            RendererManager.Draw += RendererManager_Draw;
            InputManager.MouseMove += InputManager_MouseMove;
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
            Button.Button_Click += Button_Button_Click;
            DisplayType.ValueChanged += DisplayType_ValueChanged;
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

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseMove(e);
            }
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
                return;

            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseKeyUp(e);
            }
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
                return;

            foreach (var button in Buttons)
            {
                button.Value.InputManager_MouseKeyDown(e);
            }
        }

        private void RendererManager_Draw()
        {
            if (!EnableMinePositions.Value)
                return;

            switch (DisplayType.Value)
            {
                case "Always":
                    DisplayCondition = true;
                    break;
                case "In Range":
                    break;
                default:
                    DisplayCondition = IsKeyDown;
                    break;
            }

            if (!DisplayCondition)
                return;

            foreach (var val in Positions)
            {
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
                    rect.Height = (IMG_SIZE * 2) + (GAP_SIZE * 3);
                    textureKey = "TechiesMines.Rect2.png";
                }
                else if (minesInSpot == Mines.LandMine_StasisTrap_RemoteMine)
                {
                    rect.Height = (IMG_SIZE * 3) + (GAP_SIZE * 4);
                    textureKey = "TechiesMines.Rect3.png";
                }

                RendererManager.DrawImage(textureKey, rect);

                var yOffset = GAP_SIZE;
                var imgHoverSize = 1.5f;

                for (int i = 0; i < 3; i++)
                {
                    var imgRect = new RectangleF(screenPos.X + GAP_SIZE, screenPos.Y + yOffset, IMG_SIZE, IMG_SIZE);

                    if (InputManager.MousePosition.IsUnderRectangle(imgRect))
                    {
                        imgRect.X -= imgHoverSize;
                        imgRect.Y -= imgHoverSize;
                        imgRect.Width += imgHoverSize * 2;
                        imgRect.Height += imgHoverSize * 2;
                    }

                    if (i == 0 && (val.Value & Mines.LandMine) == Mines.LandMine)
                    {
                        if (!Buttons.TryGetValue($"({val.Key}){Mines.LandMine}", out var landMine))
                            Buttons.Add($"({val.Key}){Mines.LandMine}", new Button(imgRect, val.Key, Mines.LandMine));
                        landMine?.UpdateRect(imgRect);
                        landMine?.Draw();
                        yOffset += IMG_SIZE + GAP_SIZE;
                    }
                    else if (i == 1 && (val.Value & Mines.StasisTrap) == Mines.StasisTrap)
                    {
                        if (!Buttons.TryGetValue($"({val.Key}){Mines.StasisTrap}", out var stasisTrap))
                            Buttons.Add($"({val.Key}){Mines.StasisTrap}", new Button(imgRect, val.Key, Mines.StasisTrap));
                        stasisTrap?.UpdateRect(imgRect);
                        stasisTrap?.Draw();
                        yOffset += IMG_SIZE + GAP_SIZE;
                    }
                    else if (i == 2 && (val.Value & Mines.RemoteMine) == Mines.RemoteMine)
                    {
                        if (!Buttons.TryGetValue($"({val.Key}){Mines.RemoteMine}", out var remoteMine))
                            Buttons.Add($"({val.Key}){Mines.RemoteMine}", new Button(imgRect, val.Key, Mines.RemoteMine));
                        remoteMine?.UpdateRect(imgRect);
                        remoteMine?.Draw();
                    }
                }
            }
        }

        internal void Dispose()
        {

        }
    }
}
