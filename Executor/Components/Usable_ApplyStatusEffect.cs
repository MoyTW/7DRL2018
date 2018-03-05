using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    class Usable_ApplyStatusEffect : Component
    {
        public StatusEffect EffectToApply { get; }

        public Usable_ApplyStatusEffect(StatusEffect effectToApply)
        {
            this.EffectToApply = effectToApply;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.USABLE);
        }

        private void HandleUseItem(GameEvent_UseItem ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                var applyEvent = new GameEvent_ReceiveStatusEffect(ev.CommandTick, ev.APCost, ev.Target, ev.Item,
                    this.EffectToApply);
                ev.Target.HandleEvent(applyEvent);
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
