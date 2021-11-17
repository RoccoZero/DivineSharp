using System;
using Overwolf.Core;
using Overwolf.Renderer;

namespace Overwolf
{
    internal sealed class Context : IDisposable
    {
        private CoreMain Main;
        private RendererMain RendererMain;
        public Menu Menu { get; }

        public Context()
        {
            Menu = new Menu();
            Main = new CoreMain(this);
            RendererMain = new RendererMain(this);
        }


        public void Dispose()
        {
            Main?.Dispose();
        }
    }
}