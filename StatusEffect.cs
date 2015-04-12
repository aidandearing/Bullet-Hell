using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using System.Collections.Generic;

namespace ActionGame
{
    class StatusEffect
    {
        public string Name { get; set; }
        public string NameGame { get; set; }
        public string Description { get; set; }
        public bool Caster { get; set; }                // Used to dictate whether the effect targets the creater/castor or the parent, if they are one and the same, no biggy
        public bool onUpdate { get; set; }              // Used by the EffectManager to see whether it should update on parent.Update or on parent.ChangeAttr()
        public bool onMove { get; set; }                // Will only run in .Update() and while the parent is moving
        public bool onCast { get; set; }                // Will only run in .Update() and while the parent is casting
        public bool onAttack { get; set; }              // Will only run in .Update() and while the parent is attacking
        public bool Hostile { get; set; }
        public float Curve { get; set; }
        public object Creator { get; set; }
        public float Value { get; set; }
        public float Lifetime { get; set; }
        public float LifetimeCurve { get; set; }
        public bool Over { get; set; }

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
        public StatusEffect(string name, string gamename, string description, float value, float lifetime, bool caster, bool update, bool move, bool cast, bool attack, float curve, bool ishostile, float lifetimecurve, Entity creator)
        {
            this.Name = name.ToLower();
            this.NameGame = gamename;
            this.Description = description;
            this.Value = value;
            this.Lifetime = lifetime;
            this.Caster = caster;
            this.Creator = creator;
            this.Over = false;
            this.onAttack = attack;
            this.onCast = cast;
            this.onMove = move;
            this.Curve = curve;
            this.Hostile = ishostile;
            this.LifetimeCurve = lifetimecurve;

            // reflecting damage can only occur on ChangeAttr as only then will the effect have a damageSource
            if (Name != "reflect" || Name != "nodamage")
                // onAttack onCast and onMove can only occur within .Update()
                if (onAttack || onCast || onMove)
                    this.onUpdate = true;
                else
                    this.onUpdate = update;
            else // Reflect is on and so the effect should update within the Entity.ChangeAttr()
                this.onUpdate = false;
        }

        // Normal Update Method
        public void Update(GameTime gameTime, Entity parent)
        {
            // Decrement iterations every time the effect updates
            Lifetime -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            // If it is less than zero flag the effect for removal
            if (Lifetime < 0)
            {
                if (Caster)
                    this.Last((Entity)Creator);
                else
                    this.Last(parent);

                Over = true;
            }
            else // Otherwise run all the update logic
            {
                // Check to see who the effect is targeting
                // But why? Should not it just be created inside the targets EffectManager, and therefore assume its target is its parent?
                if (Caster)
                {
                    this.Use((Entity)Creator);
                }
                else // It must be targeting the parent of the effect, and not the creator
                {
                    this.Use(parent);
                }
            }
        }

        // Used for the reflect projectile effect
        public void Update(GameTime gameTime, Entity parent, Projectile damageSource)
        {
            Lifetime -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            // If it is less than zero flag the effect for removal
            if (Lifetime < 0)
            {
                if (Caster)
                    this.Last((Entity)Creator);
                else
                    this.Last(parent);

                Over = true;
            }
            else // Otherwise run all the update logic
                this.Use(parent, damageSource);
        }

        public void Use(Entity target)
        {
            if (this.Name == "changehealth")
                target.ChangeAttr("health", this.Value);
            if (this.Name == "changehealthmax")
                target.ChangeAttr("healthmax", this.Value);
            if (this.Name == "changehealthregen")
                target.ChangeAttr("healthregen", this.Value);
            if (this.Name == "changedamage")
                target.ChangeAttr("damage", this.Value);
            if (this.Name == "changelevel")
                target.ChangeAttr("level", this.Value);
            else if (this.Name == "changespeed")
            {
                // Change the position of the target by a multiplier on its velocity
                // Effects are updated after the parent is, and therefore can do things like this:
                target.Position += target.Velocity * Value;
            }
            else if (this.Name == "noability" && target.canCast)
                target.canCast = false;
            else if (this.Name == "noattack" && target.canAttack)
                target.canAttack = false;
            else if (this.Name == "nodamage" && target.canBeDamaged)
                target.canBeDamaged = false;
        }

        // If the effect is set to reflect incoming damage
        public void Use(Entity target, Projectile damageSource)
        {
            ProjectileManager projectileManager = ProjectileManager.Instance();

            // Might not work, vector2 is touchy with stuff like this
            Projectile projectile = damageSource;
            projectile.Velocity = damageSource.Velocity * -1;
            projectile.Parent = target;

            // Add the projectile
            projectileManager.CreateProj(projectile);

            // Destroy the damageSource
            projectileManager.RemoveProj(damageSource);
        }

        public void Last(Entity target)
        {
            if (this.Name == "noability" && !target.canCast)
                target.canCast = true;
            else if (this.Name == "noattack" && !target.canAttack)
                target.canAttack = true;
            else if (this.Name == "nodamage" && !target.canBeDamaged)
                target.canBeDamaged = true;

            if (this.Name == "lifetime")
                target.isAlive = false;
        }
    }
}
