using Executor.Enums;
using Executor.Components;

using RogueSharp;
using System;
using System.Collections.Generic;

namespace Executor.GameEvents
{
    class GameEvent_ReceiveAttack : GameEvent
    {
        private GameEvent_PrepareAttack preparedAttack;
        private List<Tuple<int, String>> modifiers = new List<Tuple<int, String>>();

        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; }
        public DamageType DamageType { get; }
        public int BaseIncomingDamage { get; }

        public int IncomingDamage {
            get {
                var baseDmg = this.BaseIncomingDamage;
                foreach(var entry in this.modifiers)
                {
                    baseDmg += entry.Item1;
                }
                return baseDmg;
            }
        }

        public GameEvent_ReceiveAttack(GameEvent_PrepareAttack preparedAttack)
        {
            this.preparedAttack = preparedAttack;

            this.Target = preparedAttack.Target;
            this.SubTarget = preparedAttack.SubTarget;
            this.DamageType = preparedAttack.ExecutorEntity.GetComponentOfType<Component_Weapon>().DamageType;
            // TODO: The below is super janky.
            // What's happening is this:
            // The "PrepareAttack" is only a "I attack X with Y weapon". This means that all the resolution of modifiers never touches the PrepareAttack event. So !?!?
            // then we get waaaay down into the ReceiveAttack and run back and try and sum up all the modifiers with this Query against DAMAGE
            // What *should* happen is that the PrepareAttack is modified as it passes through the attacker and then "checkpoints" before flipping over to the defender.
            this.BaseIncomingDamage = preparedAttack.CommandEntity.TryGetAttribute(EntityAttributeType.DAMAGE, preparedAttack.ExecutorEntity).Value;
        }

        public GameEvent_ReceiveAttack(Entity target, BodyPartLocation subTarget, DamageType damageType, int damage)
        {
            this.Target = target;
            this.SubTarget = subTarget;
            this.DamageType = damageType;
            this.BaseIncomingDamage = damage;
        }

        public void ModifyIncomingDamage(int modifier, String explanation)
        {
            this.modifiers.Add(new Tuple<int, String>(modifier, explanation));
        }

        public void RegisterAttackResults(string logMessage)
        {
            this.Completed = true;
            if (this.preparedAttack != null)
            {
                this.preparedAttack.RegisterResult(logMessage);
            }
        }
    }
}
