using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using WooferGame.Systems.Pulse;

namespace WooferGame.Systems.Player.Feedback
{
    [ComponentSystem("player_feedback", ProcessingCycles.Update),
        Watching(typeof(PlayerComponent)),
        Listening(typeof(PulseEvent))]
    class PlayerFeedbackSystem : ComponentSystem
    {
        private double VibrationAmount;

        public override void EventFired(object sender, Event evt)
        {
            if(evt is PulseEvent pe)
            {
                PlayerComponent player = WatchedComponents.FirstOrDefault() as PlayerComponent;
                if (player == null) return;
                double distance = (pe.Source - player.Owner.Components.Get<Spatial>().Position).Magnitude;
                if (distance < 0.1) distance = 0.1;
                if (distance > 64 && pe.Sender.Owner != player.Owner) return;

                VibrationAmount += Math.Pow(pe.Strength/256, 2)*256 / distance;
                if (VibrationAmount < 0.1) VibrationAmount = 0;
            }
        }
        public override void Update()
        {
            Woofer.Controller.InputManager.ActiveInputMap.SetVibration((float)(VibrationAmount/2));
            VibrationAmount = Math.Max(VibrationAmount/8, 0);
            if (VibrationAmount <= 1e-8) VibrationAmount = 0;
        }
    }
}
