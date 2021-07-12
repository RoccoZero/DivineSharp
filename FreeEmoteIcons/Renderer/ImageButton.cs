using Divine.Extensions;
using Divine.Helpers;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;
using FreeEmoteIcons.Window;

namespace FreeEmoteIcons.Button
{
    internal sealed class ImageButton
    {
        private MainWindow Window;
        public readonly Emoticon Emoticon;

        public delegate void ButtonEventHandler(ImageButton sender);
        public static event ButtonEventHandler Button_Click;
        //public static event ButtonEventHandler Button_Hover;
        private event ButtonEventHandler Click;
        //private event ButtonEventHandler Hover;
        private bool IsKeyDown;
        public bool IsHovered;
        private Vector2 Position;
        private Vector2 Size;
        private int CurrentFrame;
        private Sleeper FrameSleeper;
        private static int Count;
        public readonly int Index;
        private int BonusSize;

        public ImageButton(MainWindow window, Emoticon emoticon, Vector2 size)
        {
            Index = Count++;
            FrameSleeper = new Sleeper();
            Click += Button_Click;
            //Hover += Button_Hover;
            Size = size;
            Window = window;
            Emoticon = emoticon;

        }

        public void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (!IsHovered && e.Position.IsUnderRectangle(new RectangleF(Position.X, Position.Y, Size.X, Size.Y)))
            {
                IsHovered = true;
                //Hover?.Invoke(this);
            }
            else if (IsHovered && !e.Position.IsUnderRectangle(new RectangleF(Position.X, Position.Y, Size.X, Size.Y)))
            {
                IsHovered = false;
                //Hover?.Invoke(this);
            }
        }

        public void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(new RectangleF(
                Window.Position.X,
                Window.Position.Y + Window.HeaderHeight,
                Window.Size.X,
                Window.Size.Y - Window.HeaderHeight))
                && e.Position.IsUnderRectangle(new RectangleF(Position.X, Position.Y, Size.X, Size.Y)))
            {
                e.Process = false;
                IsKeyDown = true;
            }
        }

        public void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(new RectangleF(
                Window.Position.X,
                Window.Position.Y + Window.HeaderHeight,
                Window.Size.X,
                Window.Size.Y - Window.HeaderHeight))
                && e.Position.IsUnderRectangle(new RectangleF(Position.X, Position.Y, Size.X, Size.Y)) && IsKeyDown)
            {
                //Console.WriteLine($"{TextureKey} clicked!");
                Click?.Invoke(this);
                e.Process = false;
            }
            IsKeyDown = false;
        }

        public void UpdateFrame(int frame)
        {
            CurrentFrame = frame;
        }


        public void Draw()
        {
            Position = new Vector2(Window.globalPos.X, Window.globalPos.Y + Window.Scroll);

            var subStr = Emoticon.ImgName.Substring(0, Emoticon.ImgName.Length - 4);
            RendererManager.DrawImage(
                "FreeEmoteIcons.Ellipse",
                new RectangleF(Window.Position.X, Window.Position.Y + Window.HeaderHeight, Window.Size.X, Window.Size.Y - Window.HeaderHeight),
                new RectangleF(Position.X - 3 - BonusSize, Position.Y - 3 - BonusSize, Size.X + 6 + (BonusSize * 2), Size.Y + 6 + (BonusSize * 2)));
            RendererManager.DrawImage(
            $"FreeEmoteIcons.{subStr}_{CurrentFrame}.png",
            new RectangleF(Window.Position.X, Window.Position.Y + Window.HeaderHeight, Window.Size.X, Window.Size.Y - Window.HeaderHeight),
            new RectangleF(Position.X - BonusSize, Position.Y - BonusSize, Size.X + (BonusSize * 2), Size.Y + (BonusSize * 2)));

            Animate();
        }

        private void Animate()
        {
            if (IsHovered)
            {
                if (!FrameSleeper.Sleeping)
                {
                    CurrentFrame++;

                    if (BonusSize < 3)
                        BonusSize++;

                    if (CurrentFrame >= Emoticon.FrameCount)
                    {
                        CurrentFrame = 0;
                    }

                    UpdateFrame(CurrentFrame);
                    FrameSleeper.Sleep(Emoticon.MsPerFrame);
                }
            } 
            else
            {
                if (BonusSize > 0)
                    BonusSize--;

                if (CurrentFrame != 0)
                    CurrentFrame = 0;
            }
        }
    }
}