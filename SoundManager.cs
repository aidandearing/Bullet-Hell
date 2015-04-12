using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SoundManager
{
    class SoundManager
    {
        private static SoundManager instance { get; set; }
        public static float PitchModulation { get; set; }
        public static float VolumeMaster { get; set; }
        public static float VolumeSFX { get; set; }
        public static float VolumeMusic { get; set; }
        public static float VolumeVoice { get; set; }

        //Sound dictionary
        public static Dictionary<string, SoundEffect> Sounds;

        private SoundManager(Dictionary<string, SoundEffect> sounds)
        {
            Sounds = sounds;
        }

        public static SoundManager Instance(Dictionary<string, SoundEffect> sounds)
        {
            if (instance == null)
            {
                instance = new SoundManager(sounds);
            }

            return instance;
        }

        public void PlaySFX(string name, DynamicObject creator, float pitch, float range, float volume)
        {
            //Distance Calculations
            Vector2 distance = Camera.Center - creator.Position;

            if (distance.Length() < range)
            {
                SoundEffect soundEffect = Sounds[name];
                SoundEffectInstance soundI = soundEffect.CreateInstance();

                //volume
                soundI.Volume = MathHelper.Clamp((range / distance.Length() * VolumeSFX), 0, VolumeMaster);

                //pitch modulation
                Random random = new Random();
                pitch += (float)((random.NextDouble() * 2 - 1) * PitchModulation);

                //pan
                soundI.Pan = -distance.X / range;

                soundI.Play();
            }

        }

        public void PlayMusic(string name, float volume)
        {
            SoundEffect soundEffect = Sounds[name];
            SoundEffectInstance soundI = soundEffect.CreateInstance();

            soundI.Volume = MathHelper.Clamp((volume * VolumeMusic), 0, VolumeMaster);
            soundI.Play();
        }

        public void PlayVoice(string name, Entity creator, float range, float volume)
        {
            //Distance Calculations
            Vector2 distance = Camera.Center - creator.Position;

            if (distance.Length() < range)
            {
                SoundEffect soundEffect = Sounds[name];
                SoundEffectInstance soundI = soundEffect.CreateInstance();

                //volume
                soundI.Volume = MathHelper.Clamp((range / distance.Length() * VolumeVoice), 0, VolumeMaster);

                //pan
                soundI.Pan = -distance.X / range;

                soundI.Play();
            }
        }
    }
}
