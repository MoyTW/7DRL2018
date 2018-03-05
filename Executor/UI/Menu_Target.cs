using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.UI
{
    class Menu_Target : IDisplay
    {
        private IDisplay confirmParent;
        private IDisplay cancelParent;
        private ArenaState arena;

        public int Range { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Targeting { get; private set; }
        public bool Targeted { get; private set; }

        public Entity TargetedEntity {
            get
            {
                return arena.EntityAtPosition(this.X, this.Y);
            }
        }

        public Menu_Target(IDisplay confirmParent, ArenaState arena)
        {
            this.confirmParent = confirmParent;
            this.arena = arena;
            this.Reset();
        }
        // TODO: FFFF
        public void Init(IDisplay cancelParent)
        {
            this.cancelParent = cancelParent;
        }

        public void Start(int range)
        {
            this.Reset();
            this.Targeting = true;
            this.Range = range;
        }

        public void Reset()
        {
            this.Targeting = false;
            this.Targeted = false;
            var position = arena.Player.TryGetPosition();
            this.X = position.X;
            this.Y = position.Y;
        }

        private void Complete()
        {
            this.Targeting = false;
            this.Targeted = true;
        }

        private void MoveCursor(int dx, int dy)
        {
            var cell = this.arena.ArenaMap.GetCell(this.X + dx, this.Y + dy);
            var playerPos = this.arena.Player.TryGetPosition();

            if (ArenaState.DistanceBetweenPositions(playerPos.X, playerPos.Y, cell.X, cell.Y) <= this.Range &&
                cell.IsInFov && cell.IsWalkable)
            {
                this.X += dx;
                this.Y += dy;
            }
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    this.Reset();
                    return this.cancelParent;
                case RLKey.KeypadEnter:
                case RLKey.Enter:
                    if (this.TargetedEntity != null)
                    {
                        this.Complete();
                        return this.confirmParent;
                    }
                    else
                        return this;
                case RLKey.Keypad1:
                case RLKey.B:
                    this.MoveCursor(-1, 1);
                    break;
                case RLKey.Keypad2:
                case RLKey.Down:
                case RLKey.J:
                    this.MoveCursor(0, 1);
                    break;
                case RLKey.Keypad3:
                case RLKey.N:
                    this.MoveCursor(1, 1);
                    break;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.MoveCursor(-1, 0);
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.MoveCursor(1, 0);
                    break;
                case RLKey.Keypad7:
                case RLKey.Y:
                    this.MoveCursor(-1, -1);
                    break;
                case RLKey.Keypad8:
                case RLKey.Up:
                case RLKey.K:
                    this.MoveCursor(0, -1);
                    break;
                case RLKey.Keypad9:
                case RLKey.U:
                    this.MoveCursor(1, -1);
                    break;
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

        public void Blit(RLConsole console)
        {
            this.confirmParent.Blit(console);
        }
    }
}
