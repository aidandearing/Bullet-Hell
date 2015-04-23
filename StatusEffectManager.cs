using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ActionGame
{
    class StatusEffectManager
    {
        public List<StatusEffect> effects;
        public DynamicObject Parent { get; set; }
        public bool hasReflect { get; set; }

        public StatusEffectManager(List<StatusEffect> effects, DynamicObject parent)
        {
            this.effects = effects;
            this.Parent = parent;
        }

        public void AddEffect(StatusEffect effect)
        {
            effect.Parent = (Entity)(this.Parent);
            this.effects.Add(effect);
        }

        public void AddEffect(List<StatusEffect> effects)
        {
            foreach (StatusEffect effect in effects)
            {
                this.effects.Add(effect);
            }
        }

        public void Update(GameTime gameTime, Entity parent, Projectile damageSource)
        {
            List<StatusEffect> deadEffects = new List<StatusEffect>();
            hasReflect = false;

            foreach (StatusEffect effect in effects)
            {
                // If the effect is onUpdate and the EffectManager.Update() are being called inside the update method, then the effect should update
                if (effect.When == StatusEffect.StatusUpdate.update)
                    effect.Update(gameTime, parent);
                else if (effect.When == StatusEffect.StatusUpdate.move && parent.isMoving)
                    effect.Update(gameTime, parent);
                else if (effect.When == StatusEffect.StatusUpdate.cast && parent.isCasting)
                    effect.Update(gameTime, parent);
                else if (effect.When == StatusEffect.StatusUpdate.attack && parent.isAttacking)
                    effect.Update(gameTime, parent);
                else if (effect.When == StatusEffect.StatusUpdate.change && parent.isChangingAttr)
                    effect.Update(gameTime,parent);

                
                if (effect.Over)
                    deadEffects.Add(effect);
            }
            foreach(StatusEffect effect in deadEffects)
            {
                effects.Remove(effect);
            }
        }
    }
}
