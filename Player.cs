using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class Player : Entity
    {
        public float stat_health_healed { get; set; }
        public float stat_damage_dealt { get; set; }
        public float stat_damage_taken { get; set; }
        public float stat_healing_done { get; set; }
        public float stat_distance_travelled { get; set; }
        public int stat_skills_used { get; set; }
        public int stat_enemies_killed { get; set; }
        float actionCooldown = 0;
        const float ACTIONCOOLDOWN = 0.2f;

        KeyListener input;
        
        public Player(string key, uint index) : base(key)
        {
            input = new KeyListener(index);
        }

        new public void ChangeAttr(string attr, float value, Projectile damageSource)
        {
            attr.ToLower();
            
            switch (attr)
            {
                case "health":
                    if (canBeDamaged && value < 0)
                    {
                        Health += value;
                        stat_damage_taken += value;
                    }
                    else if (canBeHealed && value > 0)
                    {
                        Health += value;
                        stat_health_healed += value;
                    }
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
            GamePadState gamePad = input.Update();

            if (actionCooldown == 0)
            {
                if (gamePad.IsButtonDown(Buttons.RightTrigger))
                {
                    UseAbility(0);
                    actionCooldown = ACTIONCOOLDOWN;
                }
                else if (gamePad.IsButtonDown(Buttons.LeftTrigger))
                {
                    if (UseAbility(1))
                        stat_skills_used++;
                    actionCooldown = ACTIONCOOLDOWN;
                }
                else if (gamePad.IsButtonDown(Buttons.A))
                {
                    if (UseAbility(2))
                        stat_skills_used++;
                    actionCooldown = ACTIONCOOLDOWN;
                }
                else if (gamePad.IsButtonDown(Buttons.X))
                {
                    if (UseAbility(3))
                        stat_skills_used++;
                    actionCooldown = ACTIONCOOLDOWN;
                }
                else if (gamePad.IsButtonDown(Buttons.Y))
                {
                    if (UseAbility(4))
                        stat_skills_used++;
                    actionCooldown = ACTIONCOOLDOWN;
                }
                else if (gamePad.IsButtonDown(Buttons.B))
                {
                    if (UseAbility(5))
                        stat_skills_used++;
                    actionCooldown = ACTIONCOOLDOWN;
                }
            }
            float x = gamePad.ThumbSticks.Left.X;
            float y = gamePad.ThumbSticks.Left.Y;

            Velocity = new Vector2(Speed * (float)Math.Acos((double)x), Speed * (float)Math.Asin((double)y));
            if (Velocity.Length() == 0)
                Rotation = (float)Math.Atan2((double)gamePad.ThumbSticks.Right.Y, (double)gamePad.ThumbSticks.Right.X);
            else
                Rotation = (float)Math.Atan2((double)-Velocity.Y, (double)Velocity.X);

            base.Update(gameTime);
        }
    }
}
