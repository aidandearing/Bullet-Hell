using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class Projectile : DynamicObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        private List<Entity> exclusionList;
        public List<ParticleEmitter> Emitters;
        public List<StatusEffect> Effects;
        public List<LightSource> Lights;
        public List<SoundEffect> SoundEffects;
        public List<Sound> Sounds;

        public Vector2 PositionOrigin { get; set; }
        public Entity Parent { get; set; }
        public object Target { get; set; }
        public float Range { get; set; }
        public float RangeMax { get; set; }
        public float Lifetime { get; set; }
        private float LifetimeMax { get; set; }
        public Vector2 ScaleEnd { get; set; }
        public bool ScaleCurved { get; set; }
        public float OpacityEnd { get; set; }
        public bool OpacityCurved { get; set; }
        public bool diesOnImpact { get; set; }
        private bool isOriented { get; set; }
        private float Angle { get; set; }
        private float Speed { get; set; }

        private bool first = true;

        /// <summary>
        /// The Constructor for a projectile, this should never be used directly anywhere in code, save inside ProjectileManager
        /// </summary>
        /// <param name="key">The name of the projectile texture in the dictionary</param>
        /// <param name="range">The range in pixels of the projectile</param>
        /// <param name="speed">The speed in pixels/second of the projectile</param>
        /// <param name="angle">The direction the projectile is launched</param>
        /// <param name="lifetime">The lifetime in seconds the projectile stays alive for</param>
        /// <param name="parent">The creator of the projectile</param>
        /// <param name="scale">The scale of the projectile at start</param>
        /// <param name="scaleend">The scale of the projectile at end</param>
        public Projectile(string key, float range, float speed, float angle, bool isOriented, float lifetime, Vector2 scale, Vector2 scaleend, bool scalecurved, float opacity, float opacityend, bool opacitycurved, Entity parent) : base(Textures[key])
        {
            this.Range = WorldSpace.MetresToPixels(range);
            this.RangeMax = this.Range;
            this.Lifetime = lifetime;
            this.LifetimeMax = this.Lifetime;
            this.Parent = parent;
            this.isOriented = isOriented;
            this.Angle = angle;
            this.Speed = WorldSpace.MetresToPixels(speed);
            float scaleFloat = WorldSpace.TILESIZE / this.Texture.Width * scale.X;
            this.Scale = Vector2.One * scaleFloat;
            float scaleFloatEnd = WorldSpace.TILESIZE / this.Texture.Width * scaleend.X;
            this.ScaleEnd = Vector2.One * scaleFloatEnd;
            this.ScaleCurved = scalecurved;
            this.Opacity = opacity;
            this.OpacityEnd = opacityend;
            this.OpacityCurved = opacitycurved;
            this.Depth = 0.5f;
            this.isCollidable = true;
            exclusionList = new List<Entity>();
            Effects = new List<StatusEffect>();
            Emitters = new List<ParticleEmitter>();
            Lights = new List<LightSource>();
            Sounds = new List<Sound>();
            SoundEffects = new List<SoundEffect>();
            if (Parent != null)
                Begin();
        }

        public Projectile(Projectile projectile) : base(projectile.Texture)
        {
            this.Angle = projectile.Angle;
            this.Colour = projectile.Colour;
            this.diesOnImpact = projectile.diesOnImpact;
            this.ID = projectile.ID;
            this.isOriented = projectile.isOriented;
            this.Lifetime = projectile.LifetimeMax;
            this.LifetimeMax = projectile.LifetimeMax;
            this.Opacity = projectile.Opacity;
            this.OpacityEnd = projectile.OpacityEnd;
            this.OpacityCurved = projectile.OpacityCurved;
            this.TextureOrigin = projectile.TextureOrigin;
            this.Parent = projectile.Parent;
            this.Position = projectile.Position;
            this.PositionOrigin = projectile.PositionOrigin;
            this.Range = projectile.RangeMax;
            this.RangeMax = projectile.RangeMax;
            this.Rotation = projectile.Rotation;
            this.Scale = projectile.Scale;
            this.ScaleEnd = projectile.ScaleEnd;
            this.ScaleCurved = projectile.ScaleCurved;
            this.Speed = projectile.Speed;
            this.SpriteEffect = projectile.SpriteEffect;
            this.Target = projectile.Target;
            this.Texture = projectile.Texture;
            this.Velocity = projectile.Velocity;
            this.Depth = 0.5f;
            this.isCollidable = true;
            exclusionList = new List<Entity>();
            Effects = new List<StatusEffect>();
            Lights = new List<LightSource>();
            if (Parent != null)
                Begin();
        }

        public void Begin()
        {
            float x, y;

            if (isOriented)
                Angle += Parent.Rotation;

            if (Speed > 0)
            {
                x = Speed * (float)Math.Cos(Angle);
                y = Speed * (float)Math.Sin(Angle);
                this.Velocity = new Vector2(x, y);
            }
            else
            {
                x = Range * (float)Math.Cos(Angle);
                y = Range * (float)Math.Sin(Angle);
                this.Rotation = (float)Math.Atan2(Parent.Position.Y - y, Parent.Position.X - x);
                this.Position = Parent.Position + new Vector2(x, y);
            }

            foreach (ParticleEmitter emitter in Emitters)
            {
                ParticleManager.Instance().AddEmitter(emitter);
            }

            foreach (LightSource light in Lights)
            {
                LightManager.Instance().AddLight(light);
            }
        }

        new public void Update(GameTime gameTime)
        {
            if (first)
            {
                Begin();
                first = false;
            }

            Velocity = new Vector2(Speed * (float)Math.Cos(Angle), Speed * (float)Math.Sin(Angle));
            Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);

            base.Update(gameTime);

            // Update Logic
            float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float percentLife = 0;

            Range = Range - Velocity.Length() * timelapse;
            Lifetime -= timelapse;

            if (LifetimeMax > 0)
                percentLife = Lifetime / LifetimeMax;
            else
                percentLife = Range / RangeMax;

            if (ScaleCurved)
                Scale = Vector2.SmoothStep(ScaleEnd, Scale, percentLife);
            else
                Scale = Vector2.Lerp(ScaleEnd, Scale, percentLife);

            if (OpacityCurved)
                Opacity = MathHelper.SmoothStep(OpacityEnd, Opacity, percentLife);
            else
                Opacity = MathHelper.Lerp(OpacityEnd, Opacity, percentLife);

            if (Speed > 0)
                diesOnImpact = true;
            else
                diesOnImpact = false;

            if (Range < 0 || (Lifetime < 0 && LifetimeMax > 0))
                End();

            if (Target != null)
            {
                if (Target is Vector2)
                {
                    Vector2 target = (Vector2)Target;
                    Vector2 direction = target - this.Position;
                    float angle = (float)Math.Atan2(direction.Y, direction.X);
                    float x = this.Velocity.Length() * (float)Math.Cos(angle);
                    float y = this.Velocity.Length() * (float)Math.Sin(angle);
                    Velocity = new Vector2(x, y);
                }
                if (Target is DynamicObject || Target is Entity)    // Once prop is done it needs to be here too
                {
                    DynamicObject target = (DynamicObject)Target;
                    Vector2 direction = target.Position - this.Position;
                    float angle = (float)Math.Atan2(direction.Y,direction.X);
                    float x = this.Velocity.Length() * (float)Math.Cos(angle);
                    float y = this.Velocity.Length() * (float)Math.Sin(angle);
                    Velocity = new Vector2(x, y);
                }
                else
                    return;
            }

            this.CollisionRadius = (int)this.Scale.X * Texture.Width / 2;

            List<StaticObject> collisionList = ObjectManager.Instance().CheckCollisionReady(this);
            foreach (StaticObject obj in collisionList)
            {
                if (obj != this)
                    Collide(gameTime, obj);
            }
        }

        new public void Collide(GameTime gameTime, StaticObject target)
        {
            Vector2 delta = Position - target.Position;

            if (target.isCollideRectangular)
            {
                CollisionRectangle = new Rectangle((int)Math.Floor((double)Position.X), (int)Math.Floor((double)Position.Y), CollisionRectangle.Width, CollisionRectangle.Height);
                target.CollisionRectangle = new Rectangle((int)Math.Floor((double)target.Position.X), (int)Math.Floor((double)target.Position.Y), target.CollisionRectangle.Width, target.CollisionRectangle.Height);

                if (CollisionRectangle.Intersects(target.CollisionRectangle))
                {
                    if (target is Entity)
                    { }
                    else if (target is StaticObject)
                        End();
                }
            }
            else
            {
                if (delta.Length() <= CollisionRadius + target.CollisionRadius)
                {
                    if (target is Entity && !exclusionList.Contains((Entity)target))
                    {
                        Entity entity = (Entity)target;

                        // Get the hostility between the parent of this projectile and the colliding entity
                        FactionManager.Hostility hostility = FactionManager.GetHostility(Parent.Faction, entity.Faction);

                        // Check the hostility
                        switch (hostility)
                        {
                            case FactionManager.Hostility.Friendly:
                                foreach (StatusEffect effect in Effects)
                                {
                                    if (!effect.Hostile)
                                        entity.effectManager.AddEffect(effect);
                                }
                                exclusionList.Add(entity);
                                break;
                            default:
                                foreach (StatusEffect effect in Effects)
                                {
                                    if (effect.Hostile)
                                        entity.effectManager.AddEffect(effect);
                                }
                                exclusionList.Add(entity);
                                break;
                        }
                    }
                    else if (target is StaticObject && diesOnImpact && !(target is DynamicObject || target is Projectile))
                        End();
                }
            }
        }

        new public static void LoadContent(ContentManager Content)
        {
            Projectile.Textures.Add("circlesharp", Content.Load<Texture2D>("Textures/Lights/circlesharp"));

            Projectile.Textures.Add("gravitybolt", Content.Load<Texture2D>("Projectiles/gravitybolt"));

            Projectile.Textures.Add("consecratedGround", Content.Load<Texture2D>("Projectiles/consecratedGround"));
            Projectile.Textures.Add("paladinHammer", Content.Load<Texture2D>("Projectiles/pallyHammer"));
            Projectile.Textures.Add("lightning", Content.Load<Texture2D>("Projectiles/pallyLightning"));
            Projectile.Textures.Add("smallLightning", Content.Load<Texture2D>("Projectiles/smallLightning"));
            Projectile.Textures.Add("slash", Content.Load<Texture2D>("Projectiles/slash"));
            Projectile.Textures.Add("stronghold", Content.Load<Texture2D>("Projectiles/stronghold"));

            Projectile.Textures.Add("arrow", Content.Load<Texture2D>("Projectiles/arrow_purple"));
            Projectile.Textures.Add("xplarrow", Content.Load<Texture2D>("Projectiles/arrow_orange"));
            Projectile.Textures.Add("iceLance", Content.Load<Texture2D>("Projectiles/iceLance"));
            Projectile.Textures.Add("iceBlock", Content.Load<Texture2D>("Projectiles/iceBlock"));
            Projectile.Textures.Add("fireBall", Content.Load<Texture2D>("Projectiles/fireBall"));
            Projectile.Textures.Add("bola", Content.Load<Texture2D>("Projectiles/bola"));

        }

        public void End()
        {
            ProjectileManager projectileManager = ProjectileManager.Instance();
            // Dying logic goes here
            foreach (LightSource light in Lights)
            {
                LightManager.Instance().RemoveLight(light);
            }

            foreach (ParticleEmitter emitter in Emitters)
            {
                ParticleManager.Instance().RemoveEmitter(emitter);
            }

            // This goes at the end
            projectileManager.RemoveProj(this);
        }
    }
}
