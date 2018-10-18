using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using GameInterfaces.Audio;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.Sounds
{
    [ComponentSystem("sound_system", ProcessingCycles.Update),
        Watching(typeof(SoundEmitter)),
        Listening(typeof(ActivationEvent))]
    class SoundSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event evt)
        {
            if(evt is ActivationEvent ae && ae.Affected.Components.Get<SoundEmitter>() is SoundEmitter em)
            {
                Spatial sp = ae.Affected.Components.Get<Spatial>();
                foreach(Sound sound in em.Sounds)
                {
                    double volumeMultiplier = 1;
                    double pan = 0;

                    if(sp != null && sound.Range != 0)
                    {
                        volumeMultiplier = 1 - ((sp.Position - Owner.CurrentViewport.Location).Magnitude / sound.Range);
                        if (volumeMultiplier <= 0) continue;
                        pan = Math.Atan(sp.Position.X - Owner.CurrentViewport.Location.X) / (2*Math.PI);
                    }

                    ISoundEffect soundEffect = Woofer.Controller.AudioUnit[sound.Name];
                    soundEffect.Volume = sound.Volume * (float)volumeMultiplier;
                    soundEffect.Pitch = sound.Pitch;
                    soundEffect.Pan = (float) pan;
                    soundEffect.Looping = sound.Looping;
                    if (em.Music)
                        soundEffect.PlayAsMusic();
                    else
                        soundEffect.Play();
                }
            } else if(evt is ActivationEvent ae1 && ae1.Affected.HasComponent<StopMusic>())
            {
                Owner.Controller.AudioUnit.StopMusic();
            }
        }
    }
}
