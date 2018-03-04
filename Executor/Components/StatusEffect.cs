using Executor.GameEvents;

using System;

namespace Executor.Components
{
    [Serializable()]
    // TODO: Should have a IHandlesEvents or something
    abstract public class StatusEffect : Component
    {
        private bool isTimed;
        private int duration;
        public bool Expired { get { return this.isTimed && this.duration <= 0; } }

        abstract public string EffectLabel { get; }

        public StatusEffect(int duration)
        {
            if (duration > -1)
            {
                this.isTimed = true;
                this.duration = duration;
            }
            else
            {
                this.isTimed = false;
            }
        }

        abstract protected void _HandleEndTurn(GameEvent_EndTurn ev);

        private void HandleEndTurn(GameEvent_EndTurn ev)
        {
            this._HandleEndTurn(ev);

            if (this.isTimed)
            {
                this.duration--;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_EndTurn)
                this.HandleEndTurn((GameEvent_EndTurn)ev);

            return ev;
        }
    }
}

