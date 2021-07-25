using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;
using TechiesMines.Enums;

namespace TechiesMines
{
    internal sealed class Button
    {

        public delegate void ButtonEventHandler(Button sender);
        public static event ButtonEventHandler Button_Click;
        private event ButtonEventHandler Click;
        private bool IsKeyDown;
        public bool IsHovered;
        private RectangleF ButtonRect;
        public readonly Mines Mine;
        public readonly Vector3 PlantPos;

        public Button(Vector2 position, Vector2 size, Vector3 plantPos, Mines mine)
        {
            Click += Button_Click;
            ButtonRect = new RectangleF(position.X, position.Y, size.X, size.Y);
            PlantPos = plantPos;
            Mine = mine;
        }
        public Button(RectangleF buttonRect, Vector3 plantPos, Mines mine)
        {
            Click += Button_Click;
            ButtonRect = buttonRect;
            PlantPos = plantPos;
            Mine = mine;
        }

        public void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (!IsHovered && e.Position.IsUnderRectangle(new RectangleF(ButtonRect.X, ButtonRect.Y, ButtonRect.Width, ButtonRect.Height)))
            {
                IsHovered = true;
                //Hover?.Invoke(this);
            }
            else if (IsHovered && !e.Position.IsUnderRectangle(new RectangleF(ButtonRect.X, ButtonRect.Y, ButtonRect.Width, ButtonRect.Height)))
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

            if (e.Position.IsUnderRectangle(new RectangleF(ButtonRect.X, ButtonRect.Y, ButtonRect.Width, ButtonRect.Height)))
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

            if (e.Position.IsUnderRectangle(new RectangleF(ButtonRect.X, ButtonRect.Y, ButtonRect.Width, ButtonRect.Height)) && IsKeyDown)
            {
                //Console.WriteLine($"{TextureKey} clicked!");
                Click?.Invoke(this);
                e.Process = false;
            }
            IsKeyDown = false;
        }

        public void UpdateRect(RectangleF rect)
        {
            ButtonRect = rect;
        }

        public void Draw()
        {
            //Position = RendererManager.WorldToScreen(PlantPos);

            RendererManager.DrawImage(
            $"TechiesMines.{Mine}.png",
            ButtonRect);

        }
    }
}