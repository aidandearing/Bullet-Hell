using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ActionGame
{
    class Entity : DynamicObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public int Health { get; set; }
        public bool isMoving { get; set; }
        public bool isAttacking { get; set; }
        public bool isCasting { get; set; }
        public bool isAlive { get; set; }
        public bool canMove { get; set; }
        public bool canAttack { get; set; }
        public bool canCast { get; set; }
        public bool canBeDamaged { get; set; }

        public StatusEffectManager effectManager;
        public AbilityManager abilityManager;

        public Entity(string key) : base(Textures[key])
        {
            this.effectManager = new StatusEffectManager(new List<StatusEffect>(),this);
            this.abilityManager = new AbilityManager(this);
        }

        public void ChangeAttr(string attr, float value)
        {

        }

        new public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Logic
            abilityManager.Update(gameTime);
        }
    }
}
