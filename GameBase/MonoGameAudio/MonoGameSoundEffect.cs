using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInterfaces.Audio;
using Microsoft.Xna.Framework.Audio;

namespace GameBase.MonoGameAudio
{
    class MonoGameSoundEffect : ISoundEffect
    {
        public bool Looping
        {
            get => Sound.IsLooped;
            set => Sound.IsLooped = value;
        }
        public float Pan
        {
            get => Sound.Pan;
            set => Sound.Pan = value;
        }
        public float Volume
        {
            get => Sound.Volume;
            set => Sound.Volume = value;
        }
        public float Pitch
        {
            get => Sound.Pitch;
            set => Sound.Pitch = value;
        }
        public string Name { get; set; }

        private readonly MonoGameAudioUnit Unit;
        private readonly SoundEffectInstance Sound;

        public MonoGameSoundEffect(MonoGameAudioUnit unit, SoundEffectInstance sound)
        {
            this.Unit = unit;
            this.Sound = sound;
        }

        public void Play()
        {
            Sound.Play();
        }

        public void PlayAsMusic()
        {
            Unit.SetMusic(this);
        }

        public void Pause() => Sound.Pause();
        public void Resume() => Sound.Resume();
        public void Stop() => Sound.Stop();
    }
}
