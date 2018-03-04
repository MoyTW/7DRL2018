using Executor.Components;

using RogueSharp;
using System;
using System.Collections.Generic;

namespace Executor.GameEvents
{
    class GameEvent_ReceiveAttack : GameEvent
    {
        private GameEvent_PrepareAttack preparedAttack;

        private Entity Weapon { get { return this.preparedAttack.ExecutorEntity; } }
        private List<Tuple<int, String>> modifiers = new List<Tuple<int, String>>();

        public Entity Attacker { get { return this.preparedAttack.CommandEntity; } }
        public Entity Target { get { return this.preparedAttack.Target; } }
        public BodyPartLocation SubTarget { get { return this.preparedAttack.SubTarget; } }

        public DamageType DamageType { get { return this.Weapon.GetComponentOfType<Component_Weapon>().DamageType; } }
        // TODO: The below is super janky.
        // What's happening is this:
        // The "PrepareAttack" is only a "I attack X with Y weapon". This means that all the resolution of modifiers never touches the PrepareAttack event. So !?!?
        // then we get waaaay down into the ReceiveAttack and run back and try and sum up all the modifiers with this Query against DAMAGE
        // What *should* happen is that the PrepareAttack is modified as it passes through the attacker and then "checkpoints" before flipping over to the defender.
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
            this.BaseIncomingDamage = this.Attacker.TryGetAttribute(EntityAttributeType.DAMAGE, this.preparedAttack.ExecutorEntity).Value;
        }

        public void ModifyIncomingDamage(int modifier, String explanation)
        {
            this.modifiers.Add(new Tuple<int, String>(modifier, explanation));
        }

        public void RegisterAttackResults(string logMessage)
        {
            this.Completed = true;
            this.preparedAttack.RegisterResult(logMessage);
        }
    }
}
