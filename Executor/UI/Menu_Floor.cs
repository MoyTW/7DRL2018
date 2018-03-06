using Executor.Components;
using Executor.Dungeon;
using Executor.GameEvents;

using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.UI
{
    class Menu_Floor : IFloorDisplay
    {
        private readonly Menu_Main parent;
        private readonly DungeonState dungeon;
        private readonly Menu_Examine examineMenu;
        private readonly Menu_Target targetMenu;
        private readonly Menu_Inventory inventoryMenu;

        private const int arenaConsoleWidth = 70;
        private const int arenaConsoleHeight = 70;
        private RLConsole arenaConsole;

        private const int infoConsoleWidth = 70;
        private const int infoConsoleHeight = 20;
        private RLConsole infoConsole;

        public const int statusWidth = 60;
        public const int statusHeight = 45;
        private RLConsole status1Console;
        private RLConsole logConsole;

        public FloorState Floor { get { return this.dungeon.PlayerFloor; } }

        public bool MatchEnded { get { return this.dungeon.PlayerLost || this.dungeon.PlayerWon; } }

        public Menu_Floor(Menu_Main parent, DungeonState dungeon)
        {
            this.parent = parent;
            this.dungeon = dungeon;
            this.examineMenu = new Menu_Examine(this);
            this.targetMenu = new Menu_Target(this);
            this.inventoryMenu = new Menu_Inventory(this, targetMenu, this.parent.Width / 2, this.parent.Height / 2);
            this.targetMenu.Init(this.inventoryMenu);

            arenaConsole = new RLConsole(Menu_Floor.arenaConsoleWidth, Menu_Floor.arenaConsoleHeight);
            this.infoConsole = new RLConsole(Menu_Floor.infoConsoleWidth, Menu_Floor.infoConsoleHeight);
            status1Console = new RLConsole(Menu_Floor.statusWidth, Menu_Floor.statusHeight);
            this.logConsole = new RLConsole(Menu_Floor.statusWidth, Menu_Floor.statusHeight);
        }

        #region IDisplay Fns

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            // Drawing sets
            this.arenaConsole.SetBackColor(0, 0, Menu_Floor.arenaConsoleWidth, Menu_Floor.arenaConsoleHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.infoConsole.SetBackColor(0, 0, Menu_Floor.infoConsoleWidth, Menu_Floor.infoConsoleHeight, RLColor.Black);

            this.status1Console.SetBackColor(0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, RLColor.LightBlue);
            this.logConsole.SetBackColor(0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, RLColor.Black);

            // Logic
            if (this.dungeon.PlayerLost)
                return new Menu_Death(this.parent, this.Floor.Level);
            else if (this.dungeon.PlayerWon)
            {
                return new Menu_NextLevel(this.parent, this, this.Floor.Level + 1);
            }

            if (!this.Floor.ShouldWaitForPlayerInput)
            {
                this.Floor.TryFindAndExecuteNextCommand();
                return this;
            }
            // TODO: logic here is !?!?
            else if (this.inventoryMenu.SelectedItem != null && this.targetMenu.Targeted && this.targetMenu.TargetedEntity != null)
            {
                var selected = this.inventoryMenu.SelectedItem;
                var target = this.targetMenu.TargetedEntity;
                this.inventoryMenu.Reset();

                var stub = new CommandStub_UseItem(this.Floor.Player.EntityID, selected.EntityID, target.EntityID);
                this.Floor.ResolveStub(stub);

                return this;
            }
            else if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
            {
                this.Floor.TryFindAndExecuteNextCommand();
                return this;
            }
        }

        private void DrawInfo(RLConsole console)
        {
            console.Print(1, 1, "Controls Info", RLColor.White);

            int normalX = 1;
            int normalY = 3;
            console.Print(normalX, normalY, "Normal Mode", RLColor.White);
            console.Print(normalX, normalY + 1, "+ Move: NumPad, HJKLYUBN, Arrows", RLColor.White);
            console.Print(normalX, normalY + 2, "+ Delay: Space", RLColor.White);
            console.Print(normalX, normalY + 5, "+ Examine: E -> Left/Right", RLColor.White);
            console.Print(normalX, normalY + 6, "+ Main Menu: Esc", RLColor.White);
        }

        public void Blit(RLConsole console)
        {
            this.arenaConsole.SetBackColor(0, 0, Menu_Floor.arenaConsoleWidth, Menu_Floor.arenaConsoleHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.infoConsole.SetBackColor(0, 0, Menu_Floor.infoConsoleWidth, Menu_Floor.infoConsoleHeight, RLColor.Black);

            this.status1Console.SetBackColor(0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, RLColor.LightBlue);

            this.DrawArena(this.arenaConsole);
            RLConsole.Blit(this.arenaConsole, 0, 0, Menu_Floor.arenaConsoleWidth, Menu_Floor.arenaConsoleHeight, console, 0, 0);

            this.DrawInfo(this.infoConsole);
            RLConsole.Blit(this.infoConsole, 0, 0, Menu_Floor.infoConsoleWidth, Menu_Floor.infoConsoleHeight, console, 0, Menu_Floor.arenaConsoleHeight);

            if (this.targetMenu.Targeting && this.targetMenu.TargetedEntity != null)
                Drawer_Mech.DrawMechStatus(this.targetMenu.TargetedEntity, this.status1Console);
            else
                Drawer_Mech.DrawMechStatus(this.examineMenu.ExaminedEntity, this.status1Console);
            RLConsole.Blit(this.status1Console, 0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, console,
                Menu_Floor.arenaConsoleWidth, 0);

            this.DrawLog(this.logConsole);
            RLConsole.Blit(this.logConsole, 0, 0, Menu_Floor.statusWidth, Menu_Floor.statusHeight, console,
                Menu_Floor.arenaConsoleWidth, Menu_Floor.statusHeight);
        }

        #endregion

        private void TryPlayerMove(int dx, int dy)
        {
            var stub = new CommandStub_MoveSingleOrPrepareAttack(this.Floor.Player.EntityID, dx, dy);
            this.Floor.ResolveStub(stub);
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.I:
                    return this.inventoryMenu;
                case RLKey.Escape:
                    return this.parent;
                case RLKey.E:
                    this.examineMenu.Start();
                    return this.examineMenu;
                case RLKey.Space:
                    var stub = new CommandStub_Delay(this.Floor.Player.EntityID, 1);
                    this.Floor.ResolveStub(stub);
                    break;
                case RLKey.Keypad1:
                case RLKey.B:
                    this.TryPlayerMove(-1, 1);
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                    this.TryPlayerMove(0, 1);
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.TryPlayerMove(1, 1);
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.TryPlayerMove(-1, 0);
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.TryPlayerMove(1, 0);
                    break;
                case RLKey.Keypad7:
                case RLKey.Y:
                    this.TryPlayerMove(-1, -1);
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.TryPlayerMove(0, -1);
                    break;
                case RLKey.Keypad9:
                case RLKey.U:
                    this.TryPlayerMove(1, -1);
                    break;
                default:
                    break;
            }
            return this;
        }

        #region Drawing

        private IEnumerable<Tuple<Entity,int>> ArenaTimeTrackers()
        {
            return this.Floor.InspectMapEntities()
                .Select(e => new Tuple<Entity,int>(e, e.TryGetTicksToLive(this.Floor.CurrentTick)))
                .OrderBy(t => t.Item2);
        }

        public void DrawLog(RLConsole console)
        {
            var log = this.Floor.FloorLog;
            int i = log.Count - 1;
            for (int line = console.Height - 1; line > 0; line--)
            {
                if (i >= 0)
                {
                    console.Print(0, line, log[i] + "                                               ", RLColor.White);
                    i--;
                }
            }
        }

        private static RLColor FadeColor(RLColor color, float f)
        {
            return new RLColor(color.r * f, color.g * f, color.b * f);
        }

        public void DrawArena(RLConsole console)
        {
            // Use RogueSharp to calculate the current field-of-view for the player
            var position = Floor.Player.TryGetPosition();
            Floor.FloorMap.ComputeFov(position.X, position.Y, 50, true);

            foreach (var cell in Floor.FloorMap.GetAllCells())
            {
                // When a Cell is in the field-of-view set it to a brighter color
                if (cell.IsInFov)
                {
                    Floor.FloorMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.LightGray, null, '#');
                    }
                }
                // If the Cell is not in the field-of-view but has been explored set it darker
                else
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, new RLColor(30, 30, 30), null, '.');
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, RLColor.Gray, null, '#');
                    }
                }
            }

            // Draw enemies, alert + scan radii
            List<RogueSharp.Cell> alertCells = new List<RogueSharp.Cell>();
            List<RogueSharp.Cell> scanCells = new List<RogueSharp.Cell>();
            foreach (var e in Floor.InspectMapEntities().Where(e => e != Floor.Player))
            {
                var entityPosition = e.TryGetPosition();
                if (e.TryGetDestroyed())
                    console.Set(entityPosition.X, entityPosition.Y, RLColor.Gray, null, 'D');
                else
                {
                    console.Set(entityPosition.X, entityPosition.Y, RLColor.Red, null, 'E');

                    var componentAI = e.GetComponentOfType<Component_AI>();
                    if (componentAI != null)
                    {
                        var infoCells = componentAI.AlertCells(this.Floor);
                        scanCells.AddRange(infoCells.ScanCells);
                        alertCells.AddRange(infoCells.AlertCells);
                    }
                }
            }
            foreach (var cell in scanCells)
            {
                console.SetBackColor(cell.X, cell.Y, RLColor.LightBlue);
            }
            foreach (var cell in alertCells)
            {
                console.SetBackColor(cell.X, cell.Y, RLColor.LightRed);
            }

            // Draw player
            console.Set(position.X, position.Y, RLColor.Green, null, '@');

            // Highlight examined
            if (this.examineMenu.Examining)
            {
                var examinedPostion = this.examineMenu.ExaminedEntity.TryGetPosition();
                console.SetBackColor(examinedPostion.X, examinedPostion.Y, RLColor.Yellow);
            }

            // Highlight targeting
            if (this.targetMenu.Targeting)
            {
                var playerPosition = Floor.Player.TryGetPosition();
                // TODO: Artemis is crying
                var cellsInRange = Floor.CellsInRadius(playerPosition.X, playerPosition.Y, 
                    this.inventoryMenu.SelectedItem.GetComponentOfType<Component_Usable>().TargetRange);
                foreach (RogueSharp.Cell cell in cellsInRange) {
                    if (cell.IsInFov && cell.IsWalkable)
                    {
                        console.SetBackColor(cell.X, cell.Y, RLColor.LightGreen);
                    }
                }

                console.SetBackColor(this.targetMenu.X, this.targetMenu.Y, RLColor.Green);
            }

            // Draw commands
            foreach (var command in Floor.ExecutedCommands)
            {
                if (command is GameEvent_PrepareAttack)
                {
                    var cmd = (GameEvent_PrepareAttack)command;
                    var attackerPos = cmd.CommandEntity.TryGetPosition();
                    var targetPos = cmd.Target.TryGetPosition();
                    var lineCells = this.Floor.FloorMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X,
                                    targetPos.Y);
                    foreach (var cell in lineCells)
                    {
                        console.SetBackColor(cell.X, cell.Y, RLColor.LightRed);
                    }
                }
            }
            Floor.ClearExecutedCommands();
        }
    }

    #endregion
}
