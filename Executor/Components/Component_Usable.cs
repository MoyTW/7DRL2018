using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    class Component_Usable : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleUseItem(GameEvent_UseItem ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                Log.Info("USING");

                ev.Completed = true;
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
