using Executor.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.GameEvents
{
    class GameEvent_UseItem : GameEvent_Command
    {
        public Entity User { get { return this.CommandEntity; } }
        public Entity Item { get { return this.ExecutorEntity; } }
        public Entity Target { get; }

        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return String.Format("{0} used {1} on {2}!", this.User.Label, this.Item.Label, this.Target.Label);
            }
        }

        public GameEvent_UseItem(int commandTick, int APCost, Entity user, Entity item, Entity target) :
            base(commandTick, APCost, user, item)
        {
            if (!item.HasComponentOfType<Component_Usable>())
                throw new ArgumentException("Can't build attack event - weapon has no Weapon component!");

            this.Target = target;
        }
    }
}
