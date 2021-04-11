using Divine;
using Divine.Menu.Items;
using System;
using Divine.Menu.EventArgs;

namespace Changer
{
    internal class Context
    {
        private readonly Menu Menu;

        public Context()
        {
            Menu = new Menu();

            //GameManager.RiverType = (RiverType)Array.IndexOf<string>(Menu.RiverTypes, Menu.RiverType.Value);

            //foreach (var tree in EntityManager.GetEntities<Tree>())
            //{
            //    tree.SetModel(Menu.TreeModel.Value + ".vmdl");
            //}

            Menu.RiverType.ValueChanged += RiverType_ValueChanged;
            Menu.TreeModel.ValueChanged += TreeModel_ValueChanged;
        }

        private void TreeModel_ValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            if (e.NewValue != "OFF" && e.NewValue != "")
            {
                //Console.WriteLine(e.NewValue + ".vmdl");
                foreach (var tree in EntityManager.GetEntities<Tree>())
                {
                    tree.SetModel(e.NewValue + ".vmdl");
                }
            }
        }

        private void RiverType_ValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            GameManager.RiverType = (RiverType)Array.IndexOf<string>(Menu.RiverTypes, e.NewValue);
        }

        internal void Dispose()
        {
            
        }
    }
}