using Divine.Numerics;
using Divine.Renderer;

using FreeEmoteIcons.Window;

namespace FreeEmoteIcons
{
    internal class Context
    {
        private readonly Vector2 ScreenSize;
        public readonly Menu Menu;
        public readonly MainWindow Window;

        public Context()
        {
            ScreenSize = RendererManager.ScreenSize;
            Menu = new Menu();
            Window = new MainWindow(this, ScreenSize.X / 1.35f, ScreenSize.Y / 1.8f, 330, 235);
        }

        internal void Dispose()
        {

        }
    }
}