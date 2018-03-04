using Executor.GameQueries;

using System;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Executor.Components
{
    [Serializable()]
    /* A very primitive inventory with only an item limit, no weight or stacking or anything */
    class Component_Inventory : Component
    {
        private int maxItems;
        private List<Entity> inventoryEntities;

        public int MaxItems { get { return this.maxItems; } }
        public int ItemsInventoried { get { return this.inventoryEntities.Count; } }
        public int RemainingInventory { get { return this.maxItems - this.ItemsInventoried; } }

        public Component_Inventory(int maxItems=26)  // alphabet letters
        {
            this.maxItems = maxItems;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }
}
