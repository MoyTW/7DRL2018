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
    public class FloorState
    {
        private List<Entity> mapEntities;
        
        // TODO: lol at exposing literally everything
        public int Level { get; }
        public Entity Player { get { return this.dungeon.Player; } }
        public string MapID { get; }
        public IMap FloorMap { get; }
        private PathFinder FloorPathFinder { get; }

        private DungeonState dungeon;
        public int CurrentTick { get { return this.dungeon.CurrentTick; } }
        
        public bool HasAIPresent
        {
            get
            {
                return this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>())
                    .Any(e => !e.TryGetDestroyed());
            }
        }

        public bool IsWalkableAndOpen(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.FloorMap.Width || y >= this.FloorMap.Height)
                return false;

            foreach (var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.BlocksMovement && position.X == x && position.Y == y)
                    return false;
            }
            return this.FloorMap.IsWalkable(x, y);
        }

        public bool InBounds(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= this.FloorMap.Width || y >= this.FloorMap.Height);
        }

        public List<Cell> CellsInRadius(int centerX, int centerY, int radius, bool requiresWalkable=true)
        {
            List<Cell> acc = new List<Cell>();
            for (int x = centerX - radius; x <= centerX + radius; ++x)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (this.InBounds(x, y) &&
                        DistanceBetweenPositions(x, y, centerX, centerY) <= radius
                        && (!requiresWalkable || this.FloorMap.IsWalkable(x, y)))
                    {
                        acc.Add(this.FloorMap.GetCell(x, y));
                    }
                }
            }
            return acc;
        }

        public Path ShortestPath(Cell source, Cell destination)
        {
            try
            {
                return this.FloorPathFinder.ShortestPath(source, destination);
            } catch (ArgumentOutOfRangeException ex)
            {
                Log.ErrorLine(ex.ToString());
                return null;
            }
        }

        public static int DistanceBetweenEntities(Entity a, Entity b)
        {
            var aPos = a.TryGetPosition();
            var bPos = b.TryGetPosition();

            if (aPos.Z != bPos.Z)
            {
                return Int16.MaxValue;
            }

            return FloorState.DistanceBetweenPositions(aPos.X, aPos.Y, bPos.X, bPos.Y);
        }

        public static int DistanceBetweenPositions(int x0, int y0, int x1, int y1)
        {
            return (int)Math.Floor(Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1)));
        }

        public IList<Entity> InspectMapEntities()
        {
            return this.mapEntities.AsReadOnly();
        }

        public Entity EntityAtPosition(int x, int y)
        {
            foreach (var en in mapEntities)
            {
                if (!en.TryGetDestroyed())
                {
                    var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                    if (position != null && position.X == x && position.Y == y)
                        return en;
                }
            }
            return null;
        }

        public FloorState(DungeonState dungeon, IEnumerable<Entity> mapEntities, string mapID, IMap arenaMap, PathFinder arenaPathFinder, int level)
        {
            this.dungeon = dungeon;
            this.mapEntities = new List<Entity>();
            foreach (Entity e in mapEntities)
            {
                this.mapEntities.Add(e);
            }

            this.MapID = mapID;
            this.FloorMap = arenaMap;
            this.FloorPathFinder = arenaPathFinder;

            this.Level = level;
        }

        /*public FloorState DeepCopy()
        {
            List<Entity> copyList = new List<Entity>();
            foreach (var e in this.mapEntities)
            {
                copyList.Add(e.DeepCopy());
            }
            return new FloorState(copyList, this.MapID, this.FloorMap.Clone(), this.FloorPathFinder, this.Level);
        }*/

        // Only call this if you're using the arena as a copy!
        public void RemoveAllAIEntities()
        {
            for (int i = this.mapEntities.Count - 1; i >= 0; i--)
            {
                if (this.mapEntities[i].HasComponentOfType<Component_AI>())
                    this.mapEntities.RemoveAt(i);
            }
        }

        #region State Changes

        public void AlertAllAIs()
        {
            foreach (var entity in this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>()))
            {
                entity.GetComponentOfType<Component_AI>().Alerted = true;
            }
            //this.FloorLog.Add("Your cloak was disrupted! All enemies are now on ALERT!");
        }



        public Tuple<int, int> EmptyCellNear(int x, int y)
        {
            int round = 0;
            while (round < 10)
            {
                var firstEmptyNear = this.DiamondFirstEmpty(x, y, round);
                if (firstEmptyNear != null)
                    return firstEmptyNear;
                else
                    round++;
            }
            return null;
        }

        private Tuple<int, int> DiamondFirstEmpty(int x, int y, int round)
        {
            int runningX = x;
            int runningY = y - round;
            int thisRound = round;

            while (thisRound > 0)
            {
                runningX--;
                runningY++;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 0)
            {
                runningX++;
                runningY++;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 0)
            {
                runningX++;
                runningY--;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 1)
            {
                runningX--;
                runningY--;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            return null;
        }

        public IEnumerable<Cell> WalkableCells()
        {
            return this.FloorMap.GetAllCells().Where(c => c.IsWalkable).ToList();
        }

        public bool PlaceEntityNear(Entity en, int x, int y)
        {
            var emptyCell = this.EmptyCellNear(x, y);
            if (emptyCell == null)
            {
                return false;
            }
            else
            {
                if (!this.mapEntities.Contains(en))
                    this.mapEntities.Add(en);
                en.AddComponent(new Component_Position(emptyCell.Item1, emptyCell.Item2, this.Level, true));
                return true;
            }
        }

        #endregion

        // TODO: REMOVE?
        public Entity ResolveEID(string eid)
        {
            return this.mapEntities.Where(e => e.EntityID == eid).First();
        }
    }
}
