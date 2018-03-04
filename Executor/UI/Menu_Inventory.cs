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

        public Menu_Inventory(IDisplay parent, ArenaState arena, int centerX, int centerY)
        {
            this.parent = parent;
            this.arena = arena;
            this.centerX = centerX;
            this.centerY = centerY;

            this.inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
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
