using Executor.AI;
using Executor.Enums;
using Executor.GameEvents;
using Executor.GameQueries;

using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Executor.Components
 {
    [Serializable()]
    class Component_AI : Component
    {
        private Guidebook activeBook;

        public bool Scanned { get; set; }
        public bool Alerted { get; set; }
        public bool OnReturnLeg { get; private set; }
        public GameQuery_Position PatrolStart { get; private set; }
        public GameQuery_Position PatrolEnd { get; private set; }

        public IEnumerable<ActionClause> ActionClauses { get { return this.activeBook.ActionClauses; } }

        public Component_AI(Guidebook activeBook)
        {
            this.activeBook = activeBook;
            this.Scanned = false;
            this.Alerted = false;
            this.OnReturnLeg = false;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private Cell PositionToCell(GameQuery_Position pos, FloorState floor)
        {
            return floor.FloorMap.GetCell(pos.X, pos.Y);
        }

        public class CellInfo
        {
            public List<Cell> AlertCells = new List<Cell>();
            public List<Cell> ScanCells = new List<Cell>();
        }

        public CellInfo AlertCells(FloorState floor)
        {
            var info = new CellInfo();
            var scanRadius = this.Parent.TryGetAttribute(EntityAttributeType.SCAN_REQUIRED_RADIUS).Value;
            var detectRadius = this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value;
            var myPosition = this.Parent.TryGetPosition();

            if (!this.Alerted)
                info.AlertCells = floor.CellsInRadius(myPosition.X, myPosition.Y, detectRadius);
            if (!this.Scanned)
                info.ScanCells = floor.CellsInRadius(myPosition.X, myPosition.Y, scanRadius);

            return info;
        }

        private CommandStub MoveEventForPath(GameQuery_Command commandQuery, Path path)
        {
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var nextCell = path.CurrentStep;

            if (path.CurrentStep != path.End)
            {
                path.StepForward();
            }

            return new CommandStub_MoveSingle(commandQuery.CommandEntity.EntityID, nextCell.X - commandPos.X,
                nextCell.Y - commandPos.Y);
        }

        public void DeterminePatrolPath(FloorState state, IRandom rand)
        {
            this.PatrolStart = this.Parent.TryGetPosition();
            var cells = state.WalkableCells();
            Cell cell = rand.RandomElement(cells);
            while (Config.MinPatrolDistance < FloorState.DistanceBetweenPositions(this.PatrolStart.X,
                this.PatrolStart.Y, cell.X, cell.Y))
            {
                cell = rand.RandomElement(cells);
            }
            var endPos = new GameQuery_Position();
            endPos.RegisterPosition(cell.X, cell.Y, false);
            this.PatrolEnd = endPos;
        }

        private void HandleQueryCommand(GameQuery_Command q)
        {
            if (this.Alerted)
            {
                this.activeBook.TryRegisterCommand(q);
            }
            else if (FloorState.DistanceBetweenEntities(this.Parent, q.FloorState.Player) <=
                    this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value)
            {
                q.FloorState.AlertAllAIs();
            }
            else
            {
                var myPos = this.Parent.TryGetPosition();
                if (myPos.X == this.PatrolStart.X && myPos.Y == this.PatrolStart.Y)
                    this.OnReturnLeg = false;
                else if (myPos.X == this.PatrolEnd.X && myPos.Y == this.PatrolEnd.Y)
                    this.OnReturnLeg = true;

                Cell myCell = q.FloorState.FloorMap.GetCell(myPos.X, myPos.Y);
                Path patrolPath;
                if (!this.OnReturnLeg)
                {
                    patrolPath = q.FloorState.ShortestPath(myCell, this.PositionToCell(this.PatrolEnd, q.FloorState));
                }
                else
                {
                    patrolPath = q.FloorState.ShortestPath(myCell, 
                        this.PositionToCell(this.PatrolStart, q.FloorState));
                }
                if (patrolPath != null)
                    q.RegisterCommand(this.MoveEventForPath(q, patrolPath));
                else
                    q.RegisterCommand(new CommandStub_Delay(this.Parent.EntityID, Config.ONE));
            }
        }

        private void HandleEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.DETECTION_RADIUS)
                q.RegisterBaseValue(Config.ZERO);
            else if (q.AttributeType == EntityAttributeType.SCAN_REQUIRED_RADIUS)
                q.RegisterBaseValue(this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value + 1);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);
            else if (q is GameQuery_EntityAttribute)
                this.HandleEntityAttribute((GameQuery_EntityAttribute)q);

            return q;

        }
    }
}
