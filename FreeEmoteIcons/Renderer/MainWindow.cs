using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Divine.Game;
using Divine.GameConsole;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Renderer.ValveTexture;

using FreeEmoteIcons.Button;
using FreeEmoteIcons.Enums;
using FreeEmoteIcons.VPK;

namespace FreeEmoteIcons.Window
{
    internal sealed class MainWindow
    {
        private bool MousePressed;

        private Vector2 clickPosition;
        private Vector2 moveStartPosition;
        public Vector2 Position = new Vector2(0, 0);
        public Vector2 Size = new Vector2(0, 0);
        private readonly Context Context;
        private int MinScroll;
        private int MaxScroll;
        public float HeaderHeight = 25;
        bool Show = false;
        public Vector2 globalPos = new Vector2(0, 0);
        private Dictionary<string, ImageButton> imgButtons = new() { };
        public int Scroll;
        private Dictionary<string, Emoticon> Emoticons = new();
        private bool EnterPressed;
        private bool LeftShiftPressed;
        private object say;

        private readonly CancellationTokenSource Source = new();

        public MainWindow(Context context, float x, float y, float width, float height)
        {
            MinScroll = 0;
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);

            Context = context;

            RendererManager.LoadImageFromAssembly("FreeEmoteIcons.Background", "FreeEmoteIcons.Resources.bckgnd.png");
            RendererManager.LoadImageFromAssembly("FreeEmoteIcons.Ellipse", "FreeEmoteIcons.Resources.ellipse.png");

            var emoticonKV = KeyValue.CreateFromGameFile("scripts/emoticons.txt");
            foreach (var kv in emoticonKV.SubKeys)
            {
                var value = int.Parse(kv.Name);
                var kv1 = emoticonKV.GetSubKey($"{kv.Name}/image_name");
                if (kv1 == null || value >= 10000)
                    continue;
                var kv1Str = kv1.GetString();

                // Missing file
                if (kv1Str == "marci_whistle.png")
                    continue;

                var kv2 = emoticonKV.GetSubKey($"{kv.Name}/ms_per_frame");
                if (kv2 == null)
                    continue;
                var kv2Str = kv2.GetString();
                var fileName = kv1Str[0..^4] + "_png.vtex_c";

                //if (kv1Str != "wink.png")
                //    continue;

                //Console.WriteLine($"{fileName}, {kv1Str}, {value}, {kv2Str}");
                Emoticons[fileName] = new Emoticon(fileName, kv1Str, value, int.Parse(kv2Str));

            }

            var vpkBrowser = new VpkBrowser();
            HashSet<string> needExtensions = new()
            {
                "txt"
            };

            HashSet<string> needFolders = new()
            {
                "scripts/emoticons"
            };

            var entries = vpkBrowser.ReadFiles(needExtensions, needFolders);
            foreach (var entry in entries)
            {
                emoticonKV = KeyValue.CreateFromGameFile(entry.Key);

                foreach (var kv in emoticonKV.SubKeys)
                {
                    var value = int.Parse(kv.Name);
                    var kv1 = emoticonKV.GetSubKey($"{kv.Name}/image_name");
                    if (kv1 == null || value >= 10000)
                        continue;
                    var kv1Str = kv1.GetString();

                    var kv2 = emoticonKV.GetSubKey($"{kv.Name}/ms_per_frame");
                    if (kv2 == null)
                        continue;
                    var kv2Str = kv2.GetString();
                    var fileName = kv1Str[0..^4] + "_png.vtex_c";

                    //Console.WriteLine($"{fileName}, {kv1Str}, {value}, {kv2Str}");
                    Emoticons[fileName] = new Emoticon(fileName, kv1Str, value, int.Parse(kv2Str));
                }
            }
            MaxScroll = -(40 * ((int)Math.Ceiling(Emoticons.Count / 8f) - 5));

            var emoticons = new List<(Emoticon, BinaryReader)>();

            foreach (var emoticon in Emoticons.Values)
            {
                //Console.WriteLine("panorama/images/emoticons/" + emoticon.FileName);
                var stream = GameManager.OpenGameFile("panorama/images/emoticons/" + emoticon.FileName);
                emoticons.Add((emoticon, new BinaryReader(stream)));
            }

            var token = Source.Token;

            Task.Run(() =>
            {
                foreach (var (emoticon, reader) in emoticons)
                {
                    //Console.WriteLine("panorama/images/emoticons/" + emoticon.FileName);
                    var valveTextureBlock = new Texture(reader);
                    if (valveTextureBlock.Data == null)
                    {
                        //Console.WriteLine(item);
                        //Emoticons.Remove(fileName);
                        continue;
                    }

                    var bitmap = valveTextureBlock.Data.Bitmap;
                    var frameCount = bitmap.Width / 32;
                    var imgName = emoticon.ImgName;
                    emoticon.FrameCount = frameCount;

                    for (int i = 0; i < frameCount; i++)
                    {
                        var editedBitmap = bitmap.Rect(i * 32, 0, 32, 32);
                        //Console.WriteLine(new RectangleF(i * 32, 0, 32, 32));
                        //Console.WriteLine("Edited: " + editedBitmap.Width + " " + editedBitmap.Height);
                        var memoryStream = new MemoryStream();
                        editedBitmap.Save(memoryStream, ImageFormat.Png);
                        var subStr = imgName[0..^4];

                        lock (this)
                        {
                            token.ThrowIfCancellationRequested();
                            RendererManager.LoadImage($"FreeEmoteIcons.{subStr}_{i}.png", memoryStream);
                        }
                    }

                    emoticon.Loaded = true;
                }
            },
            token);

            InputManager.MouseMove += InputManager_MouseMove;
            InputManager.MouseWheel += InputManager_MouseWheel;
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
            InputManager.WindowProc += InputManager_WindowProc;
            RendererManager.Draw += RendererManager_Draw;
            ImageButton.Button_Click += ImageButton_Button_Click;
        }

        private void InputManager_WindowProc(WindowProcEventArgs e)
        {
            //Console.WriteLine($"e.Msg: {(WindowsMessage)e.Msg} e.LParam: {e.LParam} e.WParam: {(VirtualKeys)e.WParam}");
            if (((WindowsMessage)e.Msg) != WindowsMessage.WM_KEYUP && ((WindowsMessage)e.Msg) != WindowsMessage.WM_KEYDOWN)
                return;

            if (((VirtualKeys)e.WParam) != VirtualKeys.RETURN && ((VirtualKeys)e.WParam) != VirtualKeys.SHIFT)
                return;

            switch ((WindowsMessage)e.Msg)
            {
                case WindowsMessage.WM_KEYDOWN:
                switch ((VirtualKeys)e.WParam)
                {
                    case VirtualKeys.RETURN:
                    EnterPressed = true;
                    break;
                    case VirtualKeys.SHIFT:
                    LeftShiftPressed = true;
                    break;
                }
                break;
                case WindowsMessage.WM_KEYUP:
                switch ((VirtualKeys)e.WParam)
                {
                    case VirtualKeys.RETURN:
                    EnterPressed = false;
                    break;
                    case VirtualKeys.SHIFT:
                    LeftShiftPressed = false;
                    break;
                }
                break;
            }
        }

        private void InputManager_MouseWheel(MouseWheelEventArgs e)
        {
            if (!Show)
                return;
            Scroll += e.Up ? 32 : -32;
            if (Scroll > MinScroll)
                Scroll = MinScroll;
            else if (Scroll <= MaxScroll)
                Scroll = MaxScroll;
            //Console.WriteLine($"Scroll {Scroll} Max {MaxScroll}");
        }

        private void ImageButton_Button_Click(ImageButton sender)
        {

            GameConsoleManager.ExecuteCommand($"{say} {char.ConvertFromUtf32(0xE000 + sender.Emoticon.Value)}");
        }

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (!Show)
                return;

            foreach (var button in imgButtons)
            {
                button.Value.InputManager_MouseMove(e);
            }
            if (!MousePressed)
                return;

            Vector2 wndOffset = e.Position - clickPosition;
            Position = moveStartPosition + wndOffset;
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (!Show)
                return;

            foreach (var button in imgButtons)
            {
                button.Value.InputManager_MouseKeyUp(e);
            }
            if (e.MouseKey != MouseKey.Left)
                return;

            MousePressed = false;
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (!Show)
                return;

            foreach (var button in imgButtons)
            {
                button.Value.InputManager_MouseKeyDown(e);
            }
            if (e.MouseKey != MouseKey.Left || !new RectangleF(Position.X, Position.Y, Size.X, HeaderHeight).Contains(e.Position))
                return;

            MousePressed = true;
            clickPosition = e.Position;
            moveStartPosition = Position;
        }

        private void RendererManager_Draw()
        {
            if (!Show && EnterPressed && !LeftShiftPressed)
            {
                Show = true;
                say = "say_team";
            }
            else if (!Show && EnterPressed && LeftShiftPressed)
            {
                Show = true;
                say = "say";
            }

            if (Show && !GameManager.IsChatOpen)
            {
                say = string.Empty;
                Show = false;
            }

            if (!Context.Menu.Enabled.Value || !Show)
                return;

            globalPos = new Vector2(Position.X + 10, Position.Y + HeaderHeight + 10);

            RendererManager.DrawImage("FreeEmoteIcons.Background", new RectangleF(Position.X, Position.Y, Size.X, Size.Y)); ;

            var buttonSize = new Vector2(30, 30);

            int posY = 1;
            int posX = 1;

            foreach (var value in Emoticons)
            {
                //if (posY * 10 + posX >= Emoticons.Count)
                //    break;

                if (!Emoticons.TryGetValue(value.Key, out var emote))
                    continue;

                if (!emote.Loaded)
                    continue;

                var emoteNum = value.Value.Value;
                if (!imgButtons.TryGetValue($"FreeEmoteIcons.Button_{emoteNum}", out var imgButton))
                {
                    //Console.WriteLine(emote.FileName);
                    imgButton = new ImageButton(this, emote, buttonSize);
                    imgButtons[$"FreeEmoteIcons.Button_{emoteNum}"] = imgButton;
                }
                imgButton.Draw();

                globalPos.X += buttonSize.X + 10;
                posX++;
                if (posX > 8)
                {
                    globalPos.X = Position.X + 10;
                    globalPos.Y += buttonSize.Y + 10;
                    posX = 1;
                    posY++;
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                Source.Cancel();
            }
        }
    }
}
