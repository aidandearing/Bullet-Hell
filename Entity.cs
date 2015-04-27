using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    class Entity : AnimatedObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public enum Attribute { health, healthmax, healthregen, experience };

        public Vector2 PreviousPosition { get; protected set; }
        public float Health { get; protected set; }
        public float HealthMax { get; protected set; }
        public float HealthRegen { get; protected set; }
        public int Level { get; protected set; }
        public int Experience { get; protected set; }
        public int ExperienceMax { get; protected set; }
        public float Speed { get; set; }
        public bool canBeHealed { get; set; }
        public bool canBeDamaged { get; set; }
        public bool isMoving { get; protected set; }
        public bool isAttacking { get; protected set; }
        public bool isCasting { get; protected set; }
        public bool isAlive { get; set; }
        public bool isChangingAttr { get; set; }
        public bool canMove { get; set; }
        public bool canAttack { get; set; }
        public bool canCast { get; set; }
        public FactionManager.Faction Faction { get; set; }

        public StatusEffectManager effectManager;
        public AbilityManager abilityManager;

        public Entity(string key)
            : base(Textures[key])
        {
            this.effectManager = new StatusEffectManager(new List<StatusEffect>(), this);
            this.abilityManager = new AbilityManager(this);
            this.canAttack = true;
            this.canBeDamaged = true;
            this.canBeHealed = true;
            this.canCast = true;
            this.canMove = true;
            this.isAlive = true;
            //The size in pixels per tile.
            Speed = WorldSpace.MetresToPixels(2);
        }

        public void ChangeAttr(Attribute attr, float value)
        {
            isChangingAttr = true;

            switch (attr)
            {
                case Attribute.health:
                    if (canBeDamaged && value < 0)
                        Health += value;
                    else if (canBeHealed && value > 0)
                        Health += value;
                    break;
                case Attribute.healthmax:
                    HealthMax += value;
                    break;
                case Attribute.healthregen:
                    HealthRegen += value;
                    break;
                case Attribute.experience:
                    Experience += (int)value;
                    break;
                default:
                    break;
            }
        }

        new public void Update(GameTime gameTime)
        {
            isChangingAttr = false;

            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (this is Player)
                ((Player)this).ChangeAttr(Attribute.health, HealthRegen * timeLapse);
            else
                ChangeAttr(Attribute.health, HealthRegen * timeLapse);
            

            PreviousPosition = Position;
            isAttacking = false;
            isCasting = false;

            base.Update(gameTime);

            // Update Logic
            abilityManager.Update(gameTime);

            effectManager.Update(gameTime, this, null);

            if (PreviousPosition != Position)
                isMoving = true;
            else
                isMoving = false;

            if (isAlive)
            {
                if (isMoving)
                {
                    Animation = "walking";
                    Vector2 delta = PreviousPosition - Position;
                    // 960 / 60 = 16 | 256 / 16 = 16, the sprite is 16x16, therefore 960 is the perfect value to divide the delta length by
                    Animations[Animation].TimeBetweenFrames = (int)(960 / delta.Length());
                }
                else
                {
                    Animation = "idle";
                }

                List<StaticObject> collisionList = ObjectManager.Instance().CheckCollisionReady(this);
                foreach (StaticObject obj in collisionList)
                {
                    if (obj != this)
                        Collide(gameTime, obj);
                }
            }
            else
                Animation = "dead";

            WorldSpace.Instance().CheckPositionWithinBounds(this);
        }

        public bool UseAbility(uint index)
        {
            if (canAttack && index == 0)
            {
                isAttacking = abilityManager.UseAbility(index);
                return true;
            }
            else if (canCast && index != 0)
            {
                isCasting = abilityManager.UseAbility(index);
                return true;
            }
            return false;
        }

        new public void Collide(GameTime gameTime, StaticObject obj)
        {
            if (obj is Projectile)
            { }
            else
            {
                if (!obj.isCollideRectangular)
                {
                    Vector2 delta = obj.Position - Position;

                    if (delta.Length() <= CollisionRadius + obj.CollisionRadius)
                    {
                        Position = PreviousPosition;
                    }
                }
                else
                {
                    CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, CollisionRectangle.Width, CollisionRectangle.Height);
                    obj.CollisionRectangle = new Rectangle((int)obj.Position.X - obj.CollisionRectangle.Width / 2, (int)obj.Position.Y - obj.CollisionRectangle.Height / 2, obj.CollisionRectangle.Width, obj.CollisionRectangle.Height);

                    if (CollisionRectangle.Intersects(obj.CollisionRectangle))
                    {
                        Position = PreviousPosition;
                    }
                }
            }
        }

        new public static void LoadContent(ContentManager Content)
        {
            Textures.Add("paladin", Content.Load<Texture2D>("Entities/Sprites/paladinTemplate"));
            Textures.Add("archer", Content.Load<Texture2D>("Entities/Sprites/archerTemplate"));
            Textures.Add("knight", Content.Load<Texture2D>("Entities/Sprites/knightTemplate"));
            Textures.Add("ninja", Content.Load<Texture2D>("Entities/Sprites/ninjaTemplate"));
            Textures.Add("wizard", Content.Load<Texture2D>("Entities/Sprites/wizardTemplate"));
            Textures.Add("hoody", Content.Load<Texture2D>("Entities/Sprites/hoodyGuy"));
        }
    }
}
