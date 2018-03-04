using Executor.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.EntityBuilders
{
    /*
    private static Entity BuildWeapon(string label, AttachmentSize size, int maxRange, int damage,
            int refireTicks, DamageType type)
    {
        return new Entity(label: label, typeLabel: "Weapon")
            .AddComponent(new Component_Attachable(size))
            .AddComponent(new Component_Weapon(size, Config.ZERO, maxRange, damage, refireTicks, type))
            .AddComponent(new Component_Attacker());
    }*/
    public static class EntityBuilder_Inventoriables
    {
        public static Entity BuildSuicidePotion()
        {
            return new Entity(label: "Suicide pill")
                .AddComponent(new Component_Inventoriable())
                .AddComponent(new Component_Usable());
            // AddComponent(new Effect_SelfDeath)
        }

        public static Entity BuildIronskinPotion()
        {
            return new Entity(label: "Ironskin potion")
                .AddComponent(new Component_Inventoriable())
                .AddComponent(new Component_Usable());
            // AddComponent(new Effect_AddBuff())
        }
    }
}
