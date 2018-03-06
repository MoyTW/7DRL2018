﻿using Executor.Components;
using Executor.Dungeon;

using RLNET;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_Inventory : IFloorDisplay
    {
        private readonly IFloorDisplay parent;
        private readonly Menu_Target targetMenu;
        private RLConsole inventoryConsole;

        public const int inventoryWidth = 60;
        public const int inventoryHeight = 45;
        public readonly int centerX, centerY;

        public FloorState Floor { get { return this.parent.Floor; } }
        public Entity SelectedItem { get; private set; }
        public Entity SelectedTarget { get; private set; }

        public Menu_Inventory(IFloorDisplay parent, Menu_Target targetMenu, int centerX, int centerY)
        {
            this.parent = parent;
            this.targetMenu = targetMenu;
            this.centerX = centerX;
            this.centerY = centerY;

            this.inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);
        }

        public void Reset()
        {
            this.SelectedItem = null;
            this.SelectedTarget = null;
            this.targetMenu.Reset();
        }

        private Entity TryGetEntityInIndex(int idx)
        {
            var inventory = this.Floor.Player.GetComponentOfType<Component_Inventory>();
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
                    var range = this.SelectedItem.GetComponentOfType<Component_Usable>().TargetRange;
                    this.targetMenu.Start(range);
                    return this.targetMenu;
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
            else if (this.targetMenu.Targeted)
            {
                this.SelectedTarget = this.targetMenu.TargetedEntity;
                return this.parent;
            }
            else
                return this;
        }

        private void DrawInventoryMenu(RLConsole console)
        {
            int currentX = 4;
            int currentY = 4;

            var inventory = this.Floor.Player.GetComponentOfType<Component_Inventory>();

            int idx = 0;
            foreach (var entity in inventory.InventoriedEntities)
            {
                RLKey useKey = (RLKey)(idx + 83);
                console.Print(currentX, currentY, useKey.ToString() + ": " + entity.Label, RLColor.Black);
                currentY += 2;

                if (currentY > Menu_Inventory.inventoryHeight - 4)
                {
                    currentX = Menu_Inventory.inventoryWidth / 2;
                    currentY = 4;
                }

                idx += 1;
            }
        }

        public void Blit(RLConsole console)
        {
            this.parent.Blit(console);

            //if (!this.targetMenu.Targeting)
            //{
                this.inventoryConsole.SetBackColor(0, 0, inventoryWidth, inventoryHeight, RLColor.White);

                this.DrawInventoryMenu(this.inventoryConsole);
                RLConsole.Blit(this.inventoryConsole, 0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, console,
                    this.centerX - Menu_Inventory.inventoryWidth / 2, this.centerY - Menu_Inventory.inventoryHeight / 2);
            //}
        }
    }
}
