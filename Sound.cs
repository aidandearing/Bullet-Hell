using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class Sound
    {
        public SoundEffect Soundeffect { get; set; }
        public StaticObject Parent { get; set; }
        public float Volume { get; set; }
        public float Range { get; set; }
        public float Pitch { get; set; }
        public bool Looped { get; set; }

        public Sound(string key, float volume, float range, float pitch, bool looped, StaticObject parent)
        {
            this.Soundeffect = SoundManager.Sounds[key];
            this.Volume = volume;
            this.Range = range;
            this.Pitch = pitch;
            this.Looped = looped;
            this.Parent = parent;
        }

        public Sound(Sound sound)
        {
            this.Soundeffect = sound.Soundeffect;
            this.Volume = sound.Volume;
            this.Range = sound.Range;
            this.Pitch = sound.Pitch;
            this.Looped = sound.Looped;
            this.Parent = sound.Parent;
        }
    }
}
