using Executor.Components;
using Executor.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.EntityBuilders
{
    public static class EntityBuilder_Inventoriables
    {
        // TODO: Component_Usable always have to come after Usable_... effects, because it completes the event and 
        // thereby stops the further propagation of the event and it's all just terrible. I an program best.
        public static Entity BuildSuicidePotion()
        {
            return new Entity(label: "Suicide pill")
                .AddComponent(new Component_Inventoriable())
                .AddComponent(new Usable_ApplyAttack(9999, DamageType.DARK))
                .AddComponent(new Component_Usable(3));
        }

        public static Entity BuildIronskinPotion()
        {
            return new Entity(label: "Ironskin potion")
                .AddComponent(new Component_Inventoriable())
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.SLASHING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.PIERCING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Usable_ApplyStatusEffect(new StatusEffect_DamageResistance(3, DamageType.CRUSHING, Enums.ResistStrength.MAJOR)))
                .AddComponent(new Component_Usable(5));
        }
    }
}
