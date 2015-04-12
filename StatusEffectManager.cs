using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using System.Collections.Generic;

namespace ActionGame
{
    class StatusEffectManager
    {
        public List<StatusEffect> effects;
        public object Parent { get; set; }

        public StatusEffectManager(List<StatusEffect> effects, object Parent)
        {
            this.effects = effects;
        }

        public void AddEffect(StatusEffect effect)
        {
            effect.Creator = this.Parent;
            this.effects.Add(effect);
        }

        public void AddEffect(List<StatusEffect> effects)
        {
            foreach (StatusEffect effect in effects)
                this.effects.Add(effect);
        }

        public void Update(GameTime gameTime, bool onUpdate, Entity parent, Projectile damageSource)
        {
            List<StatusEffect> deadEffects = new List<StatusEffect>();

            foreach (StatusEffect effect in effects)
            {
                // If the effect is onUpdate and the EffectManager.Update() are being called inside the update method, then the effect should update
                if (effect.onUpdate && onUpdate)
                {
                    if (effect.onMove && parent.isMoving)
                        effect.Update(gameTime, parent);
                    else if (effect.onCast && parent.isCasting)
                        effect.Update(gameTime, parent);
                    else if (effect.onAttack && parent.isAttacking)
                        effect.Update(gameTime, parent);
                    else
                        effect.Update(gameTime,parent);
                }
                else if (!effect.onUpdate && !onUpdate)
                {
                    // If the effect is reflect then it uses a special update
                    if (effect.Name == "reflect" && damageSource != null)
                        effect.Update(gameTime, parent, damageSource);
                    else // If not it doesn't
                        effect.Update(gameTime, parent);
                }
                
                if (effect.Over)
                    deadEffects.Add(effect);
            }
            foreach(StatusEffect effect in deadEffects)
            {
                effects.RemoveAt(effects.IndexOf(effect));
            }
        }
    }
}
