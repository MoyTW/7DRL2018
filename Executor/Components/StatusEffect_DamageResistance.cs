using Executor.Enums;
using Executor.GameEvents;
using Executor.GameQueries;

using System;
using System.Collections.Immutable;

namespace Executor.Components
{
    [Serializable()]
    public class StatusEffect_DamageResistance : StatusEffect
    {
        public DamageType DamageType { get; }
        public ResistStrength Strength { get; }
        public override string EffectLabel
        {
            get
            {
                return "Blocking";
            }
        }

        public StatusEffect_DamageResistance(int duration, DamageType damageType, ResistStrength strength) : base(duration)
        {
            this.DamageType = damageType;
            this.Strength = strength;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleReceiveAttack(GameEvent_ReceiveAttack ev)
        {
            if (this.DamageType == ev.DamageType)
            {
                var incomingDamage = ev.IncomingDamage;

                if (this.Strength == ResistStrength.IMMUNE)
                {
                    ev.RegisterAttackResults(String.Format("IMMUNE TO " + this.DamageType));
                }
                else if (this.Strength == ResistStrength.MAJOR)
                {
                    ev.ModifyIncomingDamage((int)(-.5 * incomingDamage), "MAJOR RESIST FROM " + this.DamageType);
                }
                else if (this.Strength == ResistStrength.MINOR)
                {
                    ev.ModifyIncomingDamage((int)(-.25 * incomingDamage), "MINOR RESIST FROM " + this.DamageType);
                }
            }
        }

        protected override void _HandleEndTurn(GameEvent_EndTurn ev) { }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            base._HandleEvent(ev);
            if (ev is GameEvent_ReceiveAttack)
                this.HandleReceiveAttack((GameEvent_ReceiveAttack)ev);

            return ev;
        }
    }
}
