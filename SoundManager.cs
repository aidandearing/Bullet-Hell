using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class SoundManager
    {
        public static Dictionary<string, SoundEffect> Sounds;
        public static Dictionary<string, Song> Songs;

        private static SoundManager instance { get; set; }
        public static float VolumeMaster { get; set; }
        public static float VolumeSFX { get; set; }
        public static float VolumeMusic { get; set; }
        public static float VolumeVoice { get; set; }

        public const float PitchModulation = 0.01f;

        private SoundManager()
        {
            Sounds = new Dictionary<string,SoundEffect>();
            Songs = new Dictionary<string, Song>();

            VolumeMaster = 1;
            VolumeMusic = 1;
            VolumeSFX = 1;
            VolumeVoice = 1;
        }

        public static SoundManager Instance()
        {
            if (instance == null)
            {
                instance = new SoundManager();
            }

            return instance;
        }

        public SoundEffectInstance PlaySFX(Sound sound)
        {
            Vector2 distance = Camera.Instance().Position - Camera.Instance().ViewportCenter - sound.Parent.Position;

            if (distance.Length() < sound.Range)
            {
                SoundEffect soundEffect = sound.Soundeffect;
                SoundEffectInstance soundI = soundEffect.CreateInstance();

                soundI.Volume = MathHelper.Clamp((sound.Range / distance.Length() * VolumeSFX), 0, VolumeMaster);
                Random random = new Random();
                float pitch = sound.Pitch + (float)((random.NextDouble() * 2 - 1) * PitchModulation);
                soundI.Pitch = pitch;
                soundI.Pan = -distance.X / sound.Range;
                soundI.IsLooped = sound.Looped;
                soundI.Play();

                return soundI;
            }

            return null;
        }

        public void PlayMusic(string name, float volume)
        {
            Song song = Songs[name];
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Volume = MathHelper.Clamp((volume * VolumeMusic), 0, VolumeMaster);
            MediaPlayer.Play(song);
        }

        public void PlayVoice(string name, Entity creator, float range, float volume)
        {
            //Distance Calculations
            Vector2 distance = Camera.Instance().Position - Camera.Instance().ViewportCenter - creator.Position;

            if (distance.Length() < range)
            {
                SoundEffect soundEffect = Sounds[name];
                SoundEffectInstance soundI = soundEffect.CreateInstance();

                soundI.Volume = MathHelper.Clamp((range / distance.Length() * VolumeVoice), 0, VolumeMaster);
                soundI.Pan = -distance.X / range;
                soundI.Play();
            }
        }

        public static void LoadContent(ContentManager Content)
        {
            //Sounds.Add("sound", Content.Load<SoundEffect>("Sounds/sound"));
            Sounds.Add("sound", Content.Load<SoundEffect>("Sounds/explosion1"));
            Sounds.Add("firecast", Content.Load<SoundEffect>("Sounds/Fireball_Impact"));

            Sounds.Add("consecrate", Content.Load<SoundEffect>("Sounds/Concencrate"));
            Sounds.Add("divinefury", Content.Load<SoundEffect>("Sounds/Divine_Furry_Cast"));
            Sounds.Add("godswrath", Content.Load<SoundEffect>("Sounds/Gods_Wrath_Cast"));
            Sounds.Add("heal", Content.Load<SoundEffect>("Sounds/Heal"));
            Sounds.Add("stronghold", Content.Load<SoundEffect>("Sounds/Stronghold_Cast"));
            Sounds.Add("weaponswing", Content.Load<SoundEffect>("Sounds/Weapon_Swing"));

            Sounds.Add("fireball_impact", Content.Load<SoundEffect>("Sounds/Fireball_Impact"));
            Sounds.Add("icecast", Content.Load<SoundEffect>("Sounds/BlizzardImpact1a"));
            Sounds.Add("ice_lance_impact", Content.Load<SoundEffect>("Sounds/BlizzardImpact1a"));
            Sounds.Add("frostbolt_impact", Content.Load<SoundEffect>("Sounds/BlizzardImpact1a"));
            Sounds.Add("ice_block", Content.Load<SoundEffect>("Sounds/BlizzardImpact1a"));
            Sounds.Add("rov", Content.Load<SoundEffect>("Sounds/Rain_of_Vengeance"));
            Sounds.Add("shadow_step", Content.Load<SoundEffect>("Sounds/ShadowStep"));
            Sounds.Add("bow_shot", Content.Load<SoundEffect>("Sounds/Bow_Shot"));
            Sounds.Add("blizzard", Content.Load<SoundEffect>("Sounds/BlizzardImpact1a"));
            //Songs.Add("song", Content.Load<Song>("Music/song"));
        }
    }
}
