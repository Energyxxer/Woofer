using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Audio
{
    public interface ISoundEffect
    {
        bool Looping { get; set; }
        float Volume { get; set; }
        float Pitch { get; set; }
        float Pan { get; set; }
        string Name { get; set; }

        void Play();
        void PlayAsMusic();
        void Pause();
        void Resume();
        void Stop();
    }
}
