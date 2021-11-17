using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;
using System;

namespace Overwolf.Renderer
{
    internal sealed class Toggler
    {
        private RectangleF tooglerRect;
        public event Action<bool> ValueChanged;
        private bool value;
        private bool IsKeyDown;
        private readonly string TextureKey;
        private readonly float Scaling;

        public Toggler(string textureKey, float scaling = 1f)
        {
            tooglerRect = new RectangleF();
            Scaling = scaling;
            TextureKey = textureKey;
            RendererManager.LoadImage(textureKey);
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
            Value = value;

        }

        public bool Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(tooglerRect))
            {
                e.Process = false;
                IsKeyDown = true;
            }
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(tooglerRect) && IsKeyDown)
            {
                value = !value;
                ValueChanged?.Invoke(value);
                e.Process = false;
            }
            IsKeyDown = false;
        }

        public void SetRectangle(RectangleF rect)
        {
            tooglerRect = rect;
        }

        public void Draw()
        {
            //RendererManager.DrawFilledRectangle(tooglerRect, new Color(0, 0, 0, 127));
            var scaleOffset = (tooglerRect.Width - (tooglerRect.Width * Scaling)) * 0.5f;
            RendererManager.DrawImage(TextureKey, new RectangleF(tooglerRect.X + scaleOffset, tooglerRect.Y + scaleOffset, tooglerRect.Width * Scaling, tooglerRect.Height * Scaling));
        }
    }
}
