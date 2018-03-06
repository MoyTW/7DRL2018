using Executor.Components;
using Executor.Enums;
using Executor.Dungeon;
using Executor.GameQueries;

using RogueSharp;
using System;
using System.Linq;

namespace Executor.GameEvents
{
    public class CommandStub_PrepareDirectionalAttack : CommandStub
    {
        public string AttackerEID { get; }
        public int x { get; }
        public int y { get; }
        public BodyPartLocation SubTarget { get; }

        public CommandStub_PrepareDirectionalAttack(string attackerEID, int x, int y, BodyPartLocation subTarget)
            : base (attackerEID)
        {
            this.AttackerEID = attackerEID;
            this.x = x;
            this.y = y;
            this.SubTarget = subTarget;
        }

        public override GameEvent_Command ReifyStub(FloorState floor)
        {
            var attackerEntity = floor.ResolveEID(this.AttackerEID);
            var equippedWeapon = attackerEntity.GetComponentOfType<Component_Skeleton>()
                .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                .FirstOrDefault();

            // TODO: Don't require exact square
            var attackerPosition = attackerEntity.TryGetPosition();
            var targetEntity = floor.EntityAtPosition(attackerPosition.X + this.x, attackerPosition.Y + this.y);
            if (targetEntity != null)
            {
                return new GameEvent_PrepareAttack(floor.CurrentTick, Config.ONE, attackerEntity, targetEntity,
                    equippedWeapon, floor.FloorMap, this.SubTarget);
            }
            else
                return new GameEvent_Delay(floor.CurrentTick, Config.ONE, attackerEntity);
        }
    }

    public class CommandStub_PrepareTargetedAttack : CommandStub
    {
        public string AttackerEID { get; }
        public string TargetLabel { get; }
        public string TargetEID { get; }
        public BodyPartLocation SubTarget { get; }

        public CommandStub_PrepareTargetedAttack(string attackerEID, string targetEID, string targetLabel,
            BodyPartLocation subTarget) : base(attackerEID)
        {
            this.AttackerEID = attackerEID;
            this.TargetLabel = targetLabel;
            this.TargetEID = targetEID;
            this.SubTarget = subTarget;
        }

        public override GameEvent_Command ReifyStub(FloorState floor)
        {
            // TODO: Equipped items are *not* "Whatever is held in right right arm"
            var attackerEntity = floor.ResolveEID(this.AttackerEID);
            var equippedWeapon = attackerEntity.GetComponentOfType<Component_Skeleton>()
                    .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                    .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                    .FirstOrDefault();

            return new GameEvent_PrepareAttack(floor.CurrentTick, Config.ONE, attackerEntity,
                floor.ResolveEID(this.TargetEID), equippedWeapon, floor.FloorMap, this.SubTarget);
        }

        public override string ToString()
        {
            return string.Format("Attack {0}'s {1}", this.TargetLabel, SubTarget);
        }
    }

    public class GameEvent_PrepareAttack : GameEvent_Command
    {
        private string resultMessage;

        // Attack info
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; private set; }
        public IMap GameMap { get; }

        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return String.Format("{0} -> {1} ({2}): {3}", this.CommandEntity.Label, this.Target.Label,
                    this.SubTarget, this.resultMessage);
            }
        }

        public override bool Completed
        {
            set
            {
                throw new InvalidOperationException("Need to return results");
            }
        }

        public GameEvent_PrepareAttack(int commandTick, int APCost, Entity attacker, Entity target, Entity weapon,
            IMap gameMap, BodyPartLocation subTarget) : base(commandTick, APCost, attacker, weapon)
        {
            if (!weapon.HasComponentOfType<Component_Weapon>())
                throw new ArgumentException("Can't build attack event - weapon has no Weapon component!");

            this.Target = target;
            this.SubTarget = subTarget;
            this.GameMap = gameMap;
        }

        public void RegisterResult(string resultMessage)
        {
            this.resultMessage = resultMessage;
            base.Completed = true;
        }
    }
}
