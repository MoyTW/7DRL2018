﻿using Executor.EntityBuilders;
using Executor.Enums;
using Executor.Components;

using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Dungeon
{
    public static class FloorBuilder
    {
        public struct Distribution
        {
            public readonly int EntityLevel, Min, Max;

            public Distribution(int entityLevel, int min, int max)
            {
                this.EntityLevel = entityLevel;
                this.Min = min;
                this.Max = max;
            }

        }

        public static readonly Dictionary<int, List<Distribution>> levelDefinitions = new Dictionary<int, List<Distribution>>()
        {
            {0, new List<Distribution>() { new Distribution(0, 3, 5) } },
            {1, new List<Distribution>() {
                new Distribution(0, 3, 5),
                new Distribution(1, 1, 3) } },
            {2, new List<Distribution>() {
                new Distribution(0, 1, 3),
                new Distribution(1, 3, 5),
            } },
            {3, new List<Distribution>() {
                new Distribution(0, 1, 3),
                new Distribution(1, 3, 5),
                new Distribution(2, 1, 3),
            } },
            {4, new List<Distribution>() {
                new Distribution(0, 1, 3),
                new Distribution(1, 3, 5),
                new Distribution(2, 3, 5),
            } },
            {5, new List<Distribution>() {
                new Distribution(1, 4, 8),
                new Distribution(2, 3, 5),
            } },
            {6, new List<Distribution>() {
                new Distribution(0, 3, 5),
                new Distribution(1, 4, 8),
                new Distribution(2, 3, 5),
            } },
            {7, new List<Distribution>() {
                new Distribution(1, 3, 5),
                new Distribution(2, 5, 9),
            } },
            {8, new List<Distribution>() {
                new Distribution(2, 8, 14),
            } },
            {9, new List<Distribution>() { new Distribution(0, 1, 1) } }
        };

        // TODO: Concurrent Dictionary
        private static ConcurrentDictionary<string, Tuple<IMap, PathFinder>> seedsToMaps =
            new ConcurrentDictionary<string, Tuple<IMap, PathFinder>>(Config.NumThreads(), Config.NumMaps());

        private static bool InScanRangeOfPlayer(Entity player, Entity aiEntity, Cell possiblePosition)
        {
            if (player != null)
            {
                var scanRange = aiEntity.TryGetAttribute(EntityAttributeType.SCAN_REQUIRED_RADIUS).Value;
                var playerPos = player.TryGetPosition();
                var dist = FloorState.DistanceBetweenPositions(playerPos.X, playerPos.Y, possiblePosition.X, possiblePosition.Y);
                return dist <= scanRange;
            }
            else
            {
                return false;
            }
        }

        public static FloorState BuildFloor(DungeonState dungeon, int width, int height, string mapID, int level)
        {
            var distributions = FloorBuilder.levelDefinitions[level];
            List<Entity> mapEntities = new List<Entity>();
            var placementRand = new DotNetRandom(Int32.Parse(mapID));
            int d = 0;
            foreach (var dist in distributions)
            {
                var numToAdd = placementRand.Next(dist.Min, dist.Max);
                for (int i = 0; i < numToAdd; i++)
                {
                    mapEntities.Add(EntityBuilder_Enemies.BuildRandomLevelledEntity(placementRand, d.ToString(), dist.EntityLevel));
                    d++;
                }
            }
            return FloorBuilder.BuildArena(dungeon, width, height, mapID, mapEntities, level);
        }

        public static FloorState BuildFloor(DungeonState dungeon, int width, int height, string mapID, int level, Entity player)
        {
            var distributions = FloorBuilder.levelDefinitions[level];
            List<Entity> mapEntities = new List<Entity>();

            mapEntities.Add(player);

            var placementRand = new DotNetRandom(Int32.Parse(mapID));
            int d = 0;
            foreach (var dist in distributions)
            {
                var numToAdd = placementRand.Next(dist.Min, dist.Max);
                for (int i = 0; i < numToAdd; i++)
                {
                    mapEntities.Add(EntityBuilder_Enemies.BuildRandomLevelledEntity(placementRand, d.ToString(), dist.EntityLevel));
                    d++;
                }
            }
            return FloorBuilder.BuildArena(dungeon, width, height, mapID, mapEntities, level);
        }

        private static FloorState BuildArena(DungeonState dungeon, int width, int height, string mapID, IEnumerable<Entity> entities, int level)
        {
            if (!seedsToMaps.ContainsKey(mapID))
            {
                var map = Map.Create(new RogueSharp.MapCreation.RandomRoomsMapCreationStrategy<Map>(width, height,
                    2000, 9, 5, new DotNetRandom(Int32.Parse(mapID))));
                var pathFinder = new PathFinder(map);
                seedsToMaps[mapID] = new Tuple<IMap, PathFinder>(map, pathFinder);
            }

            var mapEntities = new List<Entity>();
            foreach (var e in entities)
            {
                mapEntities.Add(e);
            }
            FloorState floorState = new FloorState(dungeon, mapEntities, mapID, seedsToMaps[mapID].Item1, seedsToMaps[mapID].Item2, level);

            var openCells = floorState.FloorMap.GetAllCells().Where(c => c.IsWalkable).ToList();
            var placementRand = new DotNetRandom(Int32.Parse(mapID));
            foreach (var e in mapEntities)
            {
                while (!e.HasComponentOfType<Component_Position>())
                {
                    var cell = openCells[placementRand.Next(openCells.Count - 1)];

                    Component_AI ai = e.GetComponentOfType<Component_AI>();
                    if (ai != null && !FloorBuilder.InScanRangeOfPlayer(floorState.Player, e, cell))
                    {
                        floorState.PlaceEntityNear(e, cell.X, cell.Y);
                        ai.DeterminePatrolPath(floorState, placementRand);
                    }
                    else if (ai == null)
                        floorState.PlaceEntityNear(e, cell.X, cell.Y);
                }
            }

            return floorState;
        }
    }
}
