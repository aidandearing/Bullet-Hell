using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ActionGame
{
    class Entity : AnimatedObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public Vector2 PreviousPosition { get; protected set; }
        public float Health { get; protected set; }
        public float HealthMax { get; protected set; }
        public float HealthRegen { get; protected set; }
        public int Level { get; protected set; }
        public int Experience { get; protected set; }
        public int ExperienceMax { get; protected set; }
        public float Speed { get; protected set; }
        public bool canBeHealed { get; set; }
        public bool canBeDamaged { get; set; }
        public bool isMoving { get; protected set; }
        public bool isAttacking { get; protected set; }
        public bool isCasting { get; protected set; }
        public bool isAlive { get; set; }
        public bool canMove { get; set; }
        public bool canAttack { get; set; }
        public bool canCast { get; set; }
        public FactionManager.Faction Faction { get; set; }

        public StatusEffectManager effectManager;
        public AbilityManager abilityManager;

        public Entity(string key) : base(Textures[key])
        {
            this.effectManager = new StatusEffectManager(new List<StatusEffect>(), this);
            this.abilityManager = new AbilityManager(this);
            this.canAttack = true;
            this.canBeDamaged = true;
            this.canBeHealed = true;
            this.canCast = true;
            this.canMove = true;
            //The size in pixels per tile.
            Speed = WorldSpace.MetresToPixels(1);
        }

        new public static void LoadContent(ContentManager Content)
        {
            Textures.Add("paladin", Content.Load<Texture2D>("Entities/Sprites/paladinTemplate"));
            Textures.Add("archer", Content.Load<Texture2D>("Entities/Sprites/archerTemplate"));
            Textures.Add("knight", Content.Load<Texture2D>("Entities/Sprites/knightTemplate"));
            Textures.Add("ninja", Content.Load<Texture2D>("Entities/Sprites/ninjaTemplate"));
            Textures.Add("wizard", Content.Load<Texture2D>("Entities/Sprites/wizardTemplate"));
        }

        public void ChangeAttr(string attr, float value)
        {
            attr.ToLower();

            switch (attr)
            {
                case "health":
                    if (canBeDamaged && value < 0)
                        Health += value;
                    else if (canBeHealed && value > 0)
                        Health += value;
                    break;
                case "healthmax":
                    HealthMax += value;
                    break;
                case "healthregen":
                    HealthRegen += value;
                    break;
                case "experience":
                    Experience += (int)value;
                    break;
                default:
                    break;
            }
        }

        new public void Update(GameTime gameTime)
        {
            float timeLapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            ChangeAttr("health", HealthRegen * timeLapse);

            PreviousPosition = Position;
            isAttacking = false;
            isCasting = false;

            base.Update(gameTime);

            // Update Logic
            abilityManager.Update(gameTime);

            effectManager.Update(gameTime, true, this, null);

            if (PreviousPosition != Position)
                isMoving = true;
            else
                isMoving = false;
            
            if (isMoving)
            {
                Animation = "walking";
                Vector2 delta = PreviousPosition - Position;
                Animations[Animation].TimeBetweenFrames = (int)(960 / delta.Length());
            }
            else
            {
                Animation = "idle";
            }
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
    }
}
