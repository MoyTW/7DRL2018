using Executor.GameEvents;

using System;

namespace Executor.GameQueries
{
    public class GameQuery_Command : GameQuery
    {
        public Entity CommandEntity { get; }
        public FloorState FloorState { get; }

        public CommandStub Command { get; private set; }

        public GameQuery_Command(Entity commandEntity, FloorState floorState)
        {
            this.CommandEntity = commandEntity;
            this.FloorState = floorState;
        }

        public void RegisterCommand(CommandStub stub)
        {
            if (this.Command != null)
                throw new InvalidOperationException("Can't double-register commands!");

            this.Command = stub;
            this.Completed = true;
        }
    }
}
