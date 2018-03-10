using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    class Component_Portal : Component
    {
        public int XDest { get; private set; }
        public int YDest { get; private set; }
        public int ZDest { get; private set; }

        public Component_Portal(int xDest, int yDest, int zDest)
        {
            this.XDest = xDest;
            this.YDest = yDest;
            this.ZDest = zDest;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }


        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            // Portals can't teleport themselves ever
            if (ev.CommandEntity == this.Parent) return;

            var portalPos = this.Parent.TryGetPosition();
            var commandPos = ev.CommandEntity.TryGetPosition();
            if (portalPos != null && commandPos != null && 
                portalPos.X == commandPos.X && portalPos.Y == commandPos.Y && portalPos.Z == commandPos.Z)
            {
                throw new NotImplementedException();
            }
        }
    }
}
