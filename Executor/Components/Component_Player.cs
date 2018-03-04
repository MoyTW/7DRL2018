using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
 {
    [Serializable()]
    class Component_Player : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }
}
