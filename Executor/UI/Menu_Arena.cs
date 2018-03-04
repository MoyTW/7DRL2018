using Executor.Components;
using Executor.GameEvents;

using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.UI
{
    class Menu_Arena : IDisplay
    {
        private readonly Menu_Main parent;
        private readonly ArenaState arena;
        private readonly Menu_Inventory inventoryMenu;
        private readonly Menu_Examine examineMenu;

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


        public bool MatchEnded { get { return this.arena.PlayerLost || this.arena.PlayerWon; } }

        public Menu_Arena(Menu_Main parent, ArenaState arena)
        {
            this.parent = parent;
            this.arena = arena;
            this.inventoryMenu = new Menu_Inventory(this, arena, this.parent.Width / 2, this.parent.Height / 2);
            this.examineMenu = new Menu_Examine(this, arena);

            arenaConsole = new RLConsole(Menu_Arena.arenaConsoleWidth, Menu_Arena.arenaConsoleHeight);
            this.infoConsole = new RLConsole(Menu_Arena.infoConsoleWidth, Menu_Arena.infoConsoleHeight);
            status1Console = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
            this.logConsole = new RLConsole(Menu_Arena.statusWidth, Menu_Arena.statusHeight);
        }

        #region IDisplay Fns

        public IDisplay OnRootConsoleUpdate(RLConsole console, RLKeyPress keyPress)
        {
            // Drawing sets
            this.arenaConsole.SetBackColor(0, 0, Menu_Arena.arenaConsoleWidth, Menu_Arena.arenaConsoleHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.infoConsole.SetBackColor(0, 0, Menu_Arena.infoConsoleWidth, Menu_Arena.infoConsoleHeight, RLColor.Black);

            this.status1Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightBlue);
            this.logConsole.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.Black);

            // Logic
            if (this.arena.PlayerLost)
                return new Menu_Death(this.parent, this.arena.Level);
            else if (this.arena.PlayerWon)
            {
                return new Menu_NextLevel(this.parent, this.arena.Player, this.arena.Level + 1);
            }

            if (!this.arena.ShouldWaitForPlayerInput)
            {
                this.arena.TryFindAndExecuteNextCommand();
                return this;
            }   
            else if (keyPress != null)
                return this.HandleKeyPressed(keyPress);
            else
            {
                this.arena.TryFindAndExecuteNextCommand();
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
            this.arenaConsole.SetBackColor(0, 0, Menu_Arena.arenaConsoleWidth, Menu_Arena.arenaConsoleHeight, RLColor.Black);
            this.arenaConsole.Print(1, 1, "Arena", RLColor.White);

            this.infoConsole.SetBackColor(0, 0, Menu_Arena.infoConsoleWidth, Menu_Arena.infoConsoleHeight, RLColor.Black);

            this.status1Console.SetBackColor(0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, RLColor.LightBlue);

            this.DrawArena(this.arenaConsole);
            RLConsole.Blit(this.arenaConsole, 0, 0, Menu_Arena.arenaConsoleWidth, Menu_Arena.arenaConsoleHeight, console, 0, 0);

            this.DrawInfo(this.infoConsole);
            RLConsole.Blit(this.infoConsole, 0, 0, Menu_Arena.infoConsoleWidth, Menu_Arena.infoConsoleHeight, console, 0, Menu_Arena.arenaConsoleHeight);

            Drawer_Mech.DrawMechStatus(this.examineMenu.ExaminedEntity, this.status1Console);
            RLConsole.Blit(this.status1Console, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                Menu_Arena.arenaConsoleWidth, 0);

            this.DrawLog(this.logConsole);
            RLConsole.Blit(this.logConsole, 0, 0, Menu_Arena.statusWidth, Menu_Arena.statusHeight, console,
                Menu_Arena.arenaConsoleWidth, Menu_Arena.statusHeight);
        }

        #endregion

        private void TryPlayerMove(int dx, int dy)
        {
            var stub = new CommandStub_MoveSingleOrPrepareAttack(this.arena.Player.EntityID, dx, dy);
            this.arena.ResolveStub(stub);
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
                    var stub = new CommandStub_Delay(this.arena.Player.EntityID, 1);
                    this.arena.ResolveStub(stub);
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
            return this.arena.InspectMapEntities()
                .Select(e => new Tuple<Entity,int>(e, e.TryGetTicksToLive(this.arena.CurrentTick)))
                .OrderBy(t => t.Item2);
        }

        public void DrawLog(RLConsole console)
        {
            var log = this.arena.ArenaLog;
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
            var position = arena.Player.TryGetPosition();
            arena.ArenaMap.ComputeFov(position.X, position.Y, 50, true);

            foreach (var cell in arena.ArenaMap.GetAllCells())
            {
                // When a Cell is in the field-of-view set it to a brighter color
                if (cell.IsInFov)
                {
                    arena.ArenaMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
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

            List<RogueSharp.Cell> alertCells = new List<RogueSharp.Cell>();
            List<RogueSharp.Cell> scanCells = new List<RogueSharp.Cell>();
            foreach (var e in arena.InspectMapEntities().Where(e => e != arena.Player))
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
                        var infoCells = componentAI.AlertCells(this.arena);
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

            // Draw commands
            foreach (var command in arena.ExecutedCommands)
            {
                if (command is GameEvent_PrepareAttack)
                {
                    var cmd = (GameEvent_PrepareAttack)command;
                    var attackerPos = cmd.CommandEntity.TryGetPosition();
                    var targetPos = cmd.Target.TryGetPosition();
                    var lineCells = this.arena.ArenaMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X,
                                    targetPos.Y);
                    foreach (var cell in lineCells)
                    {
                        console.SetBackColor(cell.X, cell.Y, RLColor.LightRed);
                    }
                }
            }
            arena.ClearExecutedCommands();
        }
    }

    #endregion
}
