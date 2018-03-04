using Executor.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.GameEvents
{
    class GameEvent_AddToInventory : GameEvent_Command
    {
        public Entity EntityToInventory { get; }

        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} equipped {1} on {2}", this.CommandEntity.Label, this.EntityToInventory.Label,
                    this.ExecutorEntity.Label);
            }
        }

        public GameEvent_AddToInventory(Entity carrier, Entity entityToInventory, int commandTick = 0)
            : base(commandTick, Config.ZERO, carrier, carrier)
        {
            if (!carrier.HasComponentOfType<Component_Inventory>())
                throw new ArgumentException("Can't slot to item without slots!");
            else if (!entityToInventory.HasComponentOfType<Component_Inventoriable>())
                throw new ArgumentException("Can't slot unslottable item!");

            this.EntityToInventory = entityToInventory;
        }
    }
}
