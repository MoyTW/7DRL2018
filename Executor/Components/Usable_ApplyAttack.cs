using Executor.Enums;
using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    class Usable_ApplyAttack : Component
    {
        public int BaseDamage { get; }
        public DamageType DamageType { get; }

        public Usable_ApplyAttack(int baseDamage, DamageType damageType)
        {
            this.BaseDamage = baseDamage;
            this.DamageType = damageType;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.USABLE);
        }

        private void HandleUseItem(GameEvent_UseItem ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                var attack = new GameEvent_ReceiveAttack(ev.Target, BodyPartLocation.TORSO, this.DamageType, this.BaseDamage);
                ev.Target.HandleEvent(attack);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_UseItem)
                this.HandleUseItem((GameEvent_UseItem)ev);

            return ev;
        }
    }
}
