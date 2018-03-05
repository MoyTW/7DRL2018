using Executor.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.EntityBuilders
{
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
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.SLASHING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.PIERCING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.CRUSHING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Component_Usable());
        }
    }
}
