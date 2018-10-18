using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Sounds
{
    [PersistentObject]
    class Sound
    {
        [PersistentProperty]
        public string Name = "";
        [PersistentProperty]
        public float Volume = 1;
        [PersistentProperty]
        public float Pitch = 0;
        [PersistentProperty]
        public float Pan = 0;
        [PersistentProperty]
        public bool Looping = false;

        [PersistentProperty]
        public double Range = 256;

        public Sound()
        {
        }

        public Sound(string name, float volume = 1, float pitch = 0, float pan = 0, bool looping = false)
        {
            Name = name;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            Looping = looping;
        }
    }
}
