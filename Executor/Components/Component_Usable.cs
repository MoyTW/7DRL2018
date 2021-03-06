﻿using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    class Component_Usable : Component
    {
        public int TargetRange { get; }

        public Component_Usable(int targetRange)
        {
            this.TargetRange = targetRange;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.USABLE);
        }

        private void HandleUseItem(GameEvent_UseItem ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
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
