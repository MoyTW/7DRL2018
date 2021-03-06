﻿using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    // yes "inventoriable" is a REAL WORD go look it up on THE INTENETS
    class Component_Inventoriable : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }
}
