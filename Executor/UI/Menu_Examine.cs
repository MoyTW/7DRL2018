using Executor.Dungeon;

using RLNET;
using System;

namespace Executor.UI
{
    class Menu_Examine : IDisplay
    {
        private readonly IDisplay parent;
        private FloorState floor;
        private int entityIndex;

        public bool Examining { get; private set; }

        // http://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        private static int modSO(int x, int m) {
            return (x%m + m)%m;
        }

        public Entity ExaminedEntity {
            get
            {
                var mapEntities = floor.InspectMapEntities();
                return mapEntities[Menu_Examine.modSO(this.entityIndex, mapEntities.Count)]; 
            }
        }

        public Menu_Examine(IDisplay parent, FloorState floor)
        {
            this.parent = parent;
            this.floor = floor;
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
            this.entityIndex = floor.InspectMapEntities().IndexOf(floor.Player);
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

