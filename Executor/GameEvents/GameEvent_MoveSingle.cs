using Executor.Enums;

namespace Executor.GameEvents
{
    public enum XDirection
    {
        LEFT = -1,
        RIGHT = 1,
        NONE = 0
    }

    public enum YDirection
    {
        UP = -1,
        DOWN = 1,
        NONE = 0
    }

    public class CommandStub_MoveSingle : CommandStub
    {
        public XDirection X { get; }
        public YDirection Y { get; }

        public CommandStub_MoveSingle(string moverEID, int x, int y) : base(moverEID)
        {
            this.X = (XDirection)x;
            this.Y = (YDirection)y;
        }

        public override GameEvent_Command ReifyStub(FloorState floor)
        {
            return new GameEvent_MoveSingle(floor.CurrentTick, Config.ONE, floor.ResolveEID(this.CommandEID), this.X,
                this.Y, floor);
        }

        public override string ToString()
        {
            return string.Format("Move {0}, {1}", X, Y);
        }
    }

    public class CommandStub_MoveSingleOrPrepareAttack : CommandStub
    {
        public XDirection X { get; }
        public YDirection Y { get; }

        public CommandStub_MoveSingleOrPrepareAttack(string moverEID, int x, int y) : base(moverEID)
        {
            this.X = (XDirection)x;
            this.Y = (YDirection)y;
        }

        public override GameEvent_Command ReifyStub(FloorState floor)
        {
            var commandEntity = floor.ResolveEID(this.CommandEID);
            var commandPosition = commandEntity.TryGetPosition();
            var entityAtTargetPosition = floor.EntityAtPosition(commandPosition.X + (int)this.X,
                commandPosition.Y + (int)this.Y);

            if (entityAtTargetPosition != null)
            {
                // TODO: Don't always bump attack to torso
                return new CommandStub_PrepareTargetedAttack(this.CommandEID, entityAtTargetPosition.EntityID, 
                    entityAtTargetPosition.Label, BodyPartLocation.TORSO)
                    .ReifyStub(floor);
            }
            else
            {
                return new GameEvent_MoveSingle(floor.CurrentTick, Config.ONE, commandEntity, this.X, this.Y, floor);
            }
        }

        public override string ToString()
        {
            return string.Format("Move or attack {0}, {1}", X, Y);
        }
    }

    public class GameEvent_MoveSingle : GameEvent_Command
    {
        public XDirection X { get; }
        public YDirection Y { get; }
        public FloorState CurrentFloor { get; }

        public override bool ShouldLog { get { return false; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} moved [{1}, {2}]", this.CommandEntity.Label, this.X, this.Y);
            }
        }

        public GameEvent_MoveSingle(int commandTick, int APCost, Entity mover, XDirection x, YDirection y, FloorState currentFloor)
            : base(commandTick, APCost, mover)
        {
            this.X = x;
            this.Y = y;
            this.CurrentFloor = currentFloor;
        }
    }
}
