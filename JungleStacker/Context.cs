using JungleStacker.Core;

namespace JungleStacker
{
    internal class Context
    {
        public Menu Menu;
        public Stacker JungleStacker;
        public bool InitDone;

        public Context()
        {
            Menu = new Menu();
            JungleStacker = new Stacker(this);
            InitDone = true;
        }

        internal void Dispose()
        {
            JungleStacker?.Dispose();
        }
    }
}