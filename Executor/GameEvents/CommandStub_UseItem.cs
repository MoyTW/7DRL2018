using Executor.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.GameEvents
{
    class CommandStub_UseItem : CommandStub
    {
        public string UserEID { get; }
        public string ItemEID { get; }
        public string TargetEID { get; }

        public CommandStub_UseItem(string userEID, string itemEID, string targetEID) : base(userEID)
        {
            this.UserEID = userEID;
            this.ItemEID = itemEID;
            this.TargetEID = targetEID;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            var user = arena.ResolveEID(this.UserEID);
            var item = user.GetComponentOfType<Component_Inventory>().FindByEid(this.ItemEID);

            return new GameEvent_UseItem(arena.CurrentTick, Config.ONE, user, item, arena.ResolveEID(this.TargetEID));
        }

        public override string ToString()
        {
            return string.Format("{0} uses item {1} on {2}!", this.UserEID, this.ItemEID, this.TargetEID);
        }
    }
}
