namespace RegenFailSwitch
{
    internal sealed class Context
    {
        public readonly Menu Menu;
        public readonly RegenFailSwitch RegenFailSwitch;

        public Context()
        {
            Menu = new Menu();
            RegenFailSwitch = new RegenFailSwitch(this);
        }

        internal void Dispose()
        {
            RegenFailSwitch?.Dispose();
        }
    }
}