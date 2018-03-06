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

    }
}
