using Executor.Dungeon;

using RLNET;
using System;

namespace Executor.UI
{
    class Menu_Examine : IFloorDisplay
    {
        private readonly IFloorDisplay parent;
        private int entityIndex;

        public FloorState Floor { get { return this.parent.Floor; } }
        public bool Examining { get; private set; }

        // http://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        private static int modSO(int x, int m) {
            return (x%m + m)%m;
        }

        public Entity ExaminedEntity {
            get
            {
                var mapEntities = this.Floor.InspectMapEntities();
                return mapEntities[Menu_Examine.modSO(this.entityIndex, mapEntities.Count)]; 
            }
        }

        public Menu_Examine(IFloorDisplay parent)
        {
            this.parent = parent;
            this.Reset();
        }

        public void Start()
        {
            this.Reset();
            this.Examining = true;
        }

        public void Reset()
        {
            this.Examining = false;
            this.entityIndex = this.Floor.InspectMapEntities().IndexOf(this.Floor.Player);
        }

        private IDisplay HandleKeyPressed(RLKeyPress keyPress)
        {
            if (keyPress == null)
                throw new InvalidOperationException("Called HandleKeyPressed with null, don't do this!");

            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    this.Reset();
                    return this.parent;
                case RLKey.Keypad4:
                case RLKey.H:
                case RLKey.Left:
                    this.entityIndex--;
                    break;
                case RLKey.Keypad6:
                case RLKey.Right:
                case RLKey.L:
                    this.entityIndex++;
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
            this.parent.Blit(console);
        }
    }
}

