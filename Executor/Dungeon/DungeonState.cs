using Executor.Components;
using Executor.Dungeon;
using Executor.Enums;
using Executor.GameEvents;
using Executor.GameQueries;

using RogueSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Executor.Dungeon
{
    public class DungeonState
    {
        private List<FloorState> floors;

        private int currentTick;
        private List<GameEvent_Command> executedCommands = new List<GameEvent_Command>();

        public int CurrentTick { get { return this.currentTick; } }
        public Entity Player { get; }
        public int PlayerLevel { get; }
        public FloorState PlayerFloor { get { return this.floors[0]; } }
        public int Wave { get { throw new NotImplementedException(); } }

        private IEnumerable<Entity> DungeonEntities { get { return this.floors.SelectMany(floor => floor.InspectMapEntities()); } }

        public List<String> DungeonLog { get; }

        public bool ShouldWaitForPlayerInput
        {
            get
            {
                return !this.Player.HasComponentOfType<Component_AI>() && this.nextCommandEntity == this.Player;
            }
        }

        // Turn state
        private Entity nextCommandEntity;

        public Entity NextCommandEntity { get { return this.nextCommandEntity; } }
        public IEnumerable<GameEvent_Command> ExecutedCommands { get { return this.executedCommands; } }

        public void ClearExecutedCommands() { this.executedCommands.Clear(); }


        public DungeonState(Entity player)
        {
            this.currentTick = 0;

            this.floors = new List<FloorState>();
            this.Player = player;

            this.DungeonLog = new List<String>();
        }

        public void AddFloor(FloorState floor)
        {
            this.floors.Add(floor);
            if (this.floors[floor.Level] != floor)
            {
                throw new NotImplementedException("can't have multiple of same floor");
            }
        }

        // TODO: LOL
        public void FinalizeConstruction()
        {
            ForwardToNextAction();
        }

        public bool PlayerWon
        {
            get
            {
                return !floors.Any(floor => floor.HasAIPresent);
            }
        }
        public bool PlayerLost
        {
            get
            {
                return this.Player.TryGetDestroyed();
            }
        }

        private void ForwardToNextAction()
        {
            int leastTTL = 9999;
            Entity next = null;

            foreach (var entity in this.DungeonEntities.Where(e => !e.TryGetDestroyed()))
            {
                var nextTTL = entity.HandleQuery(new GameQuery_TicksToLive(this.CurrentTick)).TicksToLive;
                if (nextTTL < leastTTL)
                {
                    leastTTL = nextTTL;
                    next = entity;
                }
            }

            this.nextCommandEntity = next;
            this.currentTick += leastTTL;
        }

        public void TryFindAndExecuteNextCommand()
        {
            // If it's the player's turn we must wait on input!
            if (this.ShouldWaitForPlayerInput)
                return;

            var nextCommandEntityFloor = this.nextCommandEntity.TryGetPosition().Z;
            var queryCommand = this.nextCommandEntity.HandleQuery(
                new GameQuery_Command(this.nextCommandEntity, this.floors[nextCommandEntityFloor]));
            if (!queryCommand.Completed)
            {
                Log.ErrorLine("Failed to register AI command for " + this.nextCommandEntity);
                var remainingAP = this.nextCommandEntity.TryGetAttribute(EntityAttributeType.CURRENT_AP).Value;
                queryCommand.RegisterCommand(new CommandStub_Delay(this.nextCommandEntity.EntityID, remainingAP));
            }
            this.ResolveStub(queryCommand.Command);
        }

        public Entity ResolveEID(string eid)
        {
            return this.DungeonEntities.Where(e => e.EntityID == eid).First();
        }

        // TODO: Whoops, I designed the stubs badly. I should swap the resolution function to the stub classes.
        public void ResolveStub(CommandStub stub)
        {
            var cmdPos = this.ResolveEID(stub.CommandEID).GetComponentOfType<Component_Position>();

            var gameEvent = stub.ReifyStub(this.floors[cmdPos.Z]); // TODO: LOL you should resolve against the floor it's on!

            if (gameEvent != null && gameEvent.CommandEntity == this.nextCommandEntity)
            {
                gameEvent.CommandEntity.HandleEvent(gameEvent);
                if (gameEvent.ShouldLog)
                    this.DungeonLog.Add(gameEvent.LogMessage);
                this.executedCommands.Add(gameEvent);
            }
            else if (gameEvent != null)
            {
                Log.ErrorLine("Can't resolve stub " + stub + " against entity " + gameEvent.CommandEntity +
                    " as next Entity is " + this.nextCommandEntity);
            }
            else
                throw new NullReferenceException("Stub " + stub + " reified to null; instead, return a delay!");

            // Hacky
            if (this.nextCommandEntity == this.Player)
            {
                foreach (var ai in this.DungeonEntities.Where(e => e.HasComponentOfType<Component_AI>()))
                {
                    if (!ai.GetComponentOfType<Component_AI>().Scanned &&
                        FloorState.DistanceBetweenEntities(this.Player, ai) <=
                        ai.TryGetAttribute(EntityAttributeType.SCAN_REQUIRED_RADIUS).Value)
                    {
                        ai.GetComponentOfType<Component_AI>().Scanned = true;
                        this.DungeonLog.Add("Scanned " + ai.Label + "!");
                    }
                }
            }
            this.ForwardToNextAction();
        }

    }
}
