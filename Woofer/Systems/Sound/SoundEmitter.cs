using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Sounds
{
    [Component("sound_emitter")]
    class SoundEmitter : Component
    {
        [PersistentProperty]
        public List<Sound> Sounds;
        [PersistentProperty]
        public bool Music = false;

        public SoundEmitter() : this(new Sound[0])
        {
        }

        public SoundEmitter(params Sound[] sounds) => Sounds = sounds.ToList();
    }
}
