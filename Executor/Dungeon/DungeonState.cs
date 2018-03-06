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

        public Entity Player { get; }
        public int PlayerLevel { get; }
        public FloorState PlayerFloor { get { return this.floors[0]; } }
        public int Wave { get { throw new NotImplementedException(); } }

        public DungeonState(List<FloorState> floors, Entity player)
        {
            this.floors = floors;
            this.Player = player;
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
    }
}
