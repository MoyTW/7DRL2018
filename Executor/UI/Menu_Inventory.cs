using Executor.Components;

using RLNET;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_Inventory : IDisplay
    {
        private readonly IDisplay parent;
        private ArenaState arena;
        private RLConsole inventoryConsole;

        public const int inventoryWidth = 60;
        public const int inventoryHeight = 45;
        public readonly int centerX, centerY;

        public Entity SelectedItem { get; private set; }

        public Menu_Inventory(IDisplay parent, ArenaState arena, int centerX, int centerY)
        {
            this.parent = parent;
            this.arena = arena;
            this.centerX = centerX;
            this.centerY = centerY;

            this.inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);
        }

        public void Reset()
        {
            this.SelectedItem = null;
        }

        private Entity TryGetEntityInIndex(int idx)
        {
            var inventory = this.arena.Player.GetComponentOfType<Component_Inventory>();
            if (idx < inventory.NumItemsInventoried)
            {
                return inventory.InventoriedEntities[idx];
            } else
            {
                return null;
            }
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.A:
                case RLKey.B:
                case RLKey.C:
                case RLKey.D:
                case RLKey.E:
                case RLKey.F:
                case RLKey.G:
                case RLKey.H:
                case RLKey.I:
                case RLKey.J:
                case RLKey.K:
                case RLKey.L:
                case RLKey.M:
                case RLKey.N:
                case RLKey.O:
                case RLKey.P:
                case RLKey.Q:
                case RLKey.R:
                case RLKey.S:
                case RLKey.T:
                case RLKey.U:
                case RLKey.V:
                case RLKey.W:
                case RLKey.X:
                case RLKey.Y:
                case RLKey.Z:
                    this.SelectedItem = TryGetEntityInIndex((int)keyPress.Key - 83);
                    return this.parent;
                case RLKey.Escape:
                    return this.parent;
                default:
                    break;
            }
            return this;
        }

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
                return this;
        }

        private void DrawInventoryMenu(RLConsole console)
        {
            int currentX = 4;
            int currentY = 4;

            var inventory = this.arena.Player.GetComponentOfType<Component_Inventory>();

            foreach (var entity in inventory.InventoriedEntities)
            {
                console.Print(currentX, currentY, entity.Label, RLColor.Black);
                currentY += 2;

                if (currentY > Menu_Inventory.inventoryHeight - 4)
                {
                    currentX += 20;
                    currentY = 4;
                }
            }
        }

        public void Blit(RLConsole console)
        {
            this.parent.Blit(console);

            this.inventoryConsole.SetBackColor(0, 0, inventoryWidth, inventoryHeight, RLColor.White);

            this.DrawInventoryMenu(this.inventoryConsole);
            RLConsole.Blit(this.inventoryConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                this.centerX - Menu_Inventory.inventoryWidth / 2, this.centerY - Menu_Inventory.inventoryHeight / 2);
        }
    }
}
