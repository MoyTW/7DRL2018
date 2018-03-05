using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace Executor.Components
{
    [Serializable()]
    /* A very primitive inventory with only an item limit, no weight or stacking or anything */
    class Component_Inventory : Component
    {
        private int maxItems;

        public List<Entity> InventoriedEntities { get; }
        public int MaxItems { get { return this.maxItems; } }
        public int NumItemsInventoried { get { return this.InventoriedEntities.Count; } }
        public int RemainingInventory { get { return this.maxItems - this.NumItemsInventoried; } }

        public Component_Inventory(int maxItems=26)  // alphabet letters
        {
            this.maxItems = maxItems;
            this.InventoriedEntities = new List<Entity>();
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        public Entity FindByEid(string entityID)
        {
            return this.InventoriedEntities.Where(e => e.EntityID == entityID).FirstOrDefault();
        }

        private void HandleAddToInventory(GameEvent_AddToInventory ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.InventoriedEntities.Contains(ev.EntityToInventory))
                    throw new ArgumentException("Cannot inventory already inventoried item " + ev.EntityToInventory.ToString());

                if (this.RemainingInventory > 0)
                {
                    this.InventoriedEntities.Add(ev.EntityToInventory);
                    ev.Completed = true;
                }
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_AddToInventory)
                this.HandleAddToInventory((GameEvent_AddToInventory)ev);

            return ev;
        }

        private void HandleQuerySubEntities(GameQuery_SubEntities q)
        {
            foreach (var inventoried in this.InventoriedEntities)
            {
                if (q.MatchesSelectors(inventoried))
                    q.RegisterEntity(inventoried);
                inventoried.HandleQuery(q);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);

            return q;
        }
    }
}
