using System;
using System.Collections.Generic;

using GameInterfaces.Audio;
using GameInterfaces.Controller;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GameBase.MonoGameAudio
{
    class MonoGameAudioUnit : IAudioUnit
    {
        private readonly ContentManager Content;
        private readonly Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        private readonly Dictionary<string, Song> Songs = new Dictionary<string, Song>();

        private readonly List<MonoGameSoundEffect> Instances = new List<MonoGameSoundEffect>();
        private ISoundEffect ActiveSong = null;

        public ISoundEffect this[string name]
        {
            get
            {
                MonoGameSoundEffect instance = new MonoGameSoundEffect(this, Sounds[name].CreateInstance());
                instance.Name = name;
                Instances.Add(instance);
                return instance;
            }
        }

        public MonoGameAudioUnit(ContentManager content)
        {
            this.Content = content;
        }

        public void Load(string name)
        {
            Sounds[name] = Content.Load<SoundEffect>(name);
        }

        public void StopMusic()
        {
            ActiveSong?.Stop();
            ActiveSong = null;
        }

        public void SetMusic(ISoundEffect sound)
        {
            if (ActiveSong != null && sound.Name == ActiveSong.Name) return;
            ActiveSong?.Stop();
            ActiveSong = sound;
            sound.Play();
        }

        public void StopAll()
        {
            foreach(MonoGameSoundEffect sound in Instances)
            {
                sound.Stop();
            }
            Instances.Clear();
        }
    }
}
