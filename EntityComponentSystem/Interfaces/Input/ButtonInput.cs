using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInterfaces.Input;

namespace EntityComponentSystem.Interfaces.Input
{
    public class ButtonInput
    {
        public int FramesPressed = 0;
        public bool Consumed { get; protected set; }
        public bool FrameConsumed { get; protected set; }

        public StateLookup Lookup = null;
        protected int Timeframe;
        protected int Delay;
        protected int Period;

        public bool Pressed => FramesPressed > 0;
        private bool Effective => Pressed && FramesPressed < 5;

        private bool EffectiveRepeating => (!Consumed && Effective) || (FramesPressed > Delay && (FramesPressed-Delay) % Period == 0);

        public ButtonInput() : this(null) { }

        public ButtonInput(StateLookup lookup) : this(lookup, 5, 15, 3) { }

        public ButtonInput(StateLookup lookup, int timeframe, int delay, int period)
        {
            Lookup = lookup;
            Timeframe = timeframe;
            Delay = delay;
            Period = period;
        }

        public bool Consume()
        {
            bool returnValue = !FrameConsumed && !Consumed && Effective;
            if(returnValue)
            {
                Consumed = true;
                FrameConsumed = true;
            }
            return returnValue;
        }

        public bool ConsumeRepeating()
        {
            bool returnValue = !FrameConsumed && EffectiveRepeating;
            if(returnValue)
            {
                Consumed = true;
                FrameConsumed = true;
            }
            return returnValue;
        }

        public void RegisterState(ButtonState state)
        {
            FrameConsumed = false;
            if (state.IsPressed()) FramesPressed++;
            else
            {
                FramesPressed = 0;
                Consumed = false;
            }
        }

        public void RegisterState()
        {
            if (Lookup == null) throw new InvalidOperationException("Cannot implicitly register state when the lookup delegate is null");
            RegisterState(Lookup());
        }

        public delegate ButtonState StateLookup();
    }
}
