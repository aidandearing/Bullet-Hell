using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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
        float respawn = 0;
        const float RESPAWN = 10;

        KeyListener input;
        PlayerIndex player;

        public Player(string key, PlayerIndex player) : base(key)
        {
            input = new KeyListener(player);
            Depth = 0.5f;
            isCollidable = true;
            Health = 500;
            HealthMax = 500;
            HealthRegen = 1;
        }

        new public void ChangeAttr(Entity.Attribute attr, float value)
        {
            switch (attr)
            {
                case Entity.Attribute.health:
                    if (canBeDamaged && value < 0 && Health >= 0)
                    {
                        Health += value;
                        stat_damage_taken += value;
                    }
                    else if (canBeHealed && value > 0 && Health <= HealthMax)
                    {
                        Health += value;
                        stat_health_healed += value;
                    }
                    break;
                case Entity.Attribute.healthmax:
                    HealthMax += value;
                    break;
                case Entity.Attribute.healthregen:
                    HealthRegen += value;
                    break;
                case Entity.Attribute.experience:
                    Experience += (int)value;
                    break;
                default:
                    break;
            }
        }

        new public void Update(GameTime gameTime)
        {
            GamePadState gamePad = input.Update();

            float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            actionCooldown -= timelapse;

            if (isAlive)
            {
                if (gamePad.IsConnected)
                {
                    if (actionCooldown <= 0)
                    {
                        if (gamePad.Triggers.Right > 0.5f)
                        {
                            UseAbility(0);
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                        else if (gamePad.Triggers.Left > 0.5f)
                        {
                            if (UseAbility(1))
                                stat_skills_used++;
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                        else if (gamePad.Buttons.A == ButtonState.Pressed)
                        {
                            if (UseAbility(2))
                                stat_skills_used++;
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                        else if (gamePad.Buttons.X == ButtonState.Pressed)
                        {
                            if (UseAbility(3))
                                stat_skills_used++;
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                        else if (gamePad.Buttons.Y == ButtonState.Pressed)
                        {
                            if (UseAbility(4))
                                stat_skills_used++;
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                        else if (gamePad.Buttons.B == ButtonState.Pressed)
                        {
                            if (UseAbility(5))
                                stat_skills_used++;
                            actionCooldown = ACTIONCOOLDOWN;
                        }
                    }

                    float x = gamePad.ThumbSticks.Left.X;
                    float y = -gamePad.ThumbSticks.Left.Y;

                    Velocity = new Vector2(Speed * x, Speed * y);

                    if (Velocity.Length() == 0 && gamePad.ThumbSticks.Right.Length() > 0)
                        Rotation = (float)Math.Atan2((double)-gamePad.ThumbSticks.Right.Y, (double)gamePad.ThumbSticks.Right.X);
                    else if (Velocity.Length() > 0)
                        Rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                        Velocity = new Vector2(Velocity.X, -Speed);
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        Velocity = new Vector2(Speed, Velocity.Y);
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        Velocity = new Vector2(Velocity.X, Speed);
                    else if (!Keyboard.GetState().IsKeyDown(Keys.Up))
                        Velocity = new Vector2(Velocity.X, 0);
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        Velocity = new Vector2(-Speed, Velocity.Y);
                    else if (!Keyboard.GetState().IsKeyDown(Keys.Right))
                        Velocity = new Vector2(0, Velocity.Y);

                    if (PreviousPosition != Position)
                        Rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);

                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                        UseAbility(0);
                }
            }

            if (Health <= 0)
            {
                isAlive = false;
            }

            if (isAlive)
                respawn = 0;
            else
            {
                respawn += timelapse;
                if (respawn > RESPAWN)
                    isAlive = true;
            }

            base.Update(gameTime);
        }
    }
}
