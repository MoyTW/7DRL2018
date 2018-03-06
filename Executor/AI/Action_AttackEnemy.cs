using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Linq;

namespace Executor.AI
{
    [Serializable()]
    class Action_AttackEnemy : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            var equipped = commandQuery.CommandEntity.TryGetSubEntities(SubEntitiesSelector.EQUIPPED);
            return equipped.Any(e => e.MatchesSelector(SubEntitiesSelector.WEAPON));
        }

        public override CommandStub GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target = commandQuery.FloorState.Player;    
            return new CommandStub_PrepareTargetedAttack(commandQuery.CommandEntity.EntityID, target.EntityID,
                target.Label, BodyPartLocation.TORSO);
        }
    }
}

