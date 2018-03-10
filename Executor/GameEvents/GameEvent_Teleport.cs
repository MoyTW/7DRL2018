using Executor.Enums;
using Executor.Dungeon;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.GameEvents
{
    public class CommandStub_Teleport : CommandStub
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public FloorState DestinationFloor { get; }

        public CommandStub_Teleport(string teleporterID, int x, int y, int z, FloorState destinationFloor) : base(teleporterID)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.DestinationFloor = destinationFloor;
        }

        public override GameEvent_Command ReifyStub(FloorState floor)
        {
            return new GameEvent_Teleport(floor.CurrentTick, Config.ONE, floor.ResolveEID(this.CommandEID), this.X,
                this.Y, this.Z, this.DestinationFloor);
        }

        public override string ToString()
        {
            return string.Format("Move {0}, {1}", X, Y);
        }
    }

    public class GameEvent_Teleport : GameEvent_Command
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public FloorState DestinationFloor { get; }

        public override bool ShouldLog { get { return false; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} moved [{1}, {2}]", this.CommandEntity.Label, this.X, this.Y);
            }
        }

        public GameEvent_Teleport(int commandTick, int APCost, Entity mover, int x, int y, int z, FloorState destinationFloor)
            : base(commandTick, APCost, mover)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.DestinationFloor = destinationFloor;
        }
    }
}
