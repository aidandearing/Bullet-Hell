using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class StatusEffect
    {
        public enum Status { changehealth, changehealthmax, changehealthregen, changespeed, noattack, noability, noheal, nodamage, lifetime, reflect };
        public enum StatusUpdate { update, move, cast, attack, change };

        public List<ParticleEmitter> Emitters;
        public List<LightSource> Lights;
        public List<Sound> Sounds;
        public List<SoundEffect> SoundEffects;

        public Status Name { get; set; }
        public string NameGame { get; set; }
        public string Description { get; set; }
        public StatusUpdate When { get; set; }
        public bool Hostile { get; set; }
        public Entity Parent { get; set; }
        public float Value { get; set; }
        public float Lifetime { get; set; }
        public float LifetimeMax { get; set; }
        public int Tick { get; set; }
        private float tickTimer { get; set; }
        private int ticksThisSecond { get; set; }
        public bool Over { get; set; }
        private bool First { get; set; }

        /// <summary>
        /// The constructor for effect, it accepts all the behaviour arguments for the effect
        /// </summary>
        /// <param name="name">The name of the effect: changehealth, changehealthmax, changehealthregen, changedamage, changelevel, changespeed, reflect, nodamage, noability, noattack, lifetime</param>
        /// <param name="gamename">The game name of the effect</param>
        /// <param name="description">The game description of the effect</param>
        /// <param name="value">The value of the effect: for all change effects except speed it acts as a standard value modifier, for speed -1 = no movement 0 = no change 1 = twice the speed, for everything else value is ignored</param>
        /// <param name="lifetime">The lifetime in seconds the effect will run for</param>
        /// <param name="caster">MAY NOT BE NECESSARY!!! Does the effect effect the caster or the parent</param>
        /// <param name="update">Does the effect run on Update() if not it runs in ChangeAttr()</param>
        /// <param name="move">Does it only run if the target is moving</param>
        /// <param name="cast">Does it only run if the target is casting</param>
        /// <param name="attack">Does it only run if the target is attacking</param>
        /// <param name="creator">The creator of the effect</param>
        public StatusEffect(Status name, string gamename, string description, float value, float lifetime, int tick, StatusUpdate when, bool ishostile, Entity parent)
        {
            this.Name = name;
            this.NameGame = gamename;
            this.Description = description;
            this.Value = value;
            this.Lifetime = lifetime;
            this.LifetimeMax = lifetime;
            this.Tick = tick;
            this.Parent = parent;
            this.Over = false;
            this.When = when;
            this.Hostile = ishostile;
            Emitters = new List<ParticleEmitter>();
            Lights = new List<LightSource>();
            Sounds = new List<Sound>();
            SoundEffects = new List<SoundEffect>();
            First = true;
        }

        public StatusEffect(StatusEffect effect)
        {
            this.Name = effect.Name;
            this.NameGame = effect.NameGame;
            this.Description = effect.Description;
            this.Value = effect.Value;
            this.Lifetime = effect.LifetimeMax;
            this.LifetimeMax = effect.LifetimeMax;
            this.Tick = effect.Tick;
            this.Parent = effect.Parent;
            this.Over = false;
            this.When = effect.When;
            this.Hostile = effect.Hostile;
            Emitters = new List<ParticleEmitter>();
            Lights = new List<LightSource>();
            First = true;
        }

        public void Begin()
        {
            foreach (ParticleEmitter emitter in Emitters)
            {
                emitter.Parent = this.Parent;
                ParticleManager.Instance().AddEmitter(emitter);
            }

            foreach (LightSource light in Lights)
            {
                light.Parent = this.Parent;
                LightManager.Instance().AddLight(light);
            }
        }

        // Normal Update Method
        public void Update(GameTime gameTime, Entity parent)
        {
            // Decrement iterations every time the effect updates
            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (First)
            {
                Begin();
                First = false;
            }

            Lifetime -= timeLapse;

            if (Tick > 0)
            {
                tickTimer += timeLapse;
                
                float tickTimerGoal = 1 / (float)Tick;

                if (tickTimer > tickTimerGoal && ticksThisSecond < Tick)
                {
                    this.Use(Parent, gameTime);
                    ticksThisSecond++;
                    tickTimer = 0;
                }
            }
            else
                this.Use(Parent, gameTime);

            if (Lifetime < 0)
            {
                this.Last(Parent);
                Over = true;
            }
        }

        public void Use(Entity target, GameTime gameTime)
        {
            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (this.Name == Status.changehealth)
                target.ChangeAttr(Entity.Attribute.health, this.Value);
            else if (this.Name == Status.changehealthmax)
                target.ChangeAttr(Entity.Attribute.healthmax, this.Value);
            else if (this.Name == Status.changehealthregen)
                target.ChangeAttr(Entity.Attribute.healthregen, this.Value);
            else if (this.Name == Status.changespeed)
            {
                // Change the position of the target by a multiplier on its velocity
                // Effects are updated after the parent is, and therefore can do things like this:
                target.Position += target.Velocity * Value * timeLapse;
            }
            else if (this.Name == Status.noability && target.canCast)
                target.canCast = false;
            else if (this.Name == Status.noattack && target.canAttack)
                target.canAttack = false;
            else if (this.Name == Status.nodamage && target.canBeDamaged)
                target.canBeDamaged = false;
            else if (this.Name == Status.noheal && target.canBeHealed)
                target.canBeHealed = false;
        }

        // If the effect is set to reflect incoming damage
        public void Use(Entity target, Projectile damageSource)
        {
            ProjectileManager projectileManager = ProjectileManager.Instance();

            // Might not work, vector2 is touchy with stuff like this
            Projectile projectile = new Projectile(damageSource);
            projectile.Velocity *= -1;
            projectile.Parent = target;

            // Add the projectile
            projectileManager.CreateProj(projectile);

            // Destroy the damageSource
            projectileManager.RemoveProj(damageSource);
        }

        public void Last(Entity target)
        {
            if (this.Name == Status.changehealthmax)
                target.ChangeAttr(Entity.Attribute.healthmax, Value);
            else if (this.Name == Status.changehealthregen)
                target.ChangeAttr(Entity.Attribute.healthregen, Value);
            else if (this.Name == Status.noability && !target.canCast)
                target.canCast = true;
            else if (this.Name == Status.noattack && !target.canAttack)
                target.canAttack = true;
            else if (this.Name == Status.nodamage && !target.canBeDamaged)
                target.canBeDamaged = true;
            else if (this.Name == Status.noheal && !target.canBeHealed)
                target.canBeHealed = true;
            else if (this.Name == Status.lifetime)
                target.isAlive = false;

            foreach(LightSource light in Lights)
            {
                LightManager.Instance().RemoveLight(light);
            }
        }
    }
}
