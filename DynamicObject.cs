using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    abstract class DynamicObject : StaticObject
    {
        public Vector2 Velocity { get; set; }

        public DynamicObject(Texture2D texture)
            : base(texture)
        {
        }

        public void Update(GameTime gameTime)
        {
            // Update code goes here
            float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Position += Velocity * timelapse;

            Rotation = MathHelper.WrapAngle(Rotation);

            if (!(this is Projectile || this is Entity))
            {
                List<StaticObject> collisionList = ObjectManager.Instance().CheckCollisionReady(this);
                foreach (StaticObject obj in collisionList)
                {
                    if (obj != this)
                        Collide(gameTime, obj);
                }
            }
            // Update its animation here
        }

        public void Collide(GameTime gameTime, StaticObject obj)
        {
            float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (obj is Projectile)
            { }
            else
            {
                if (!obj.isCollideRectangular)
                {
                    Vector2 delta = obj.Position - Position;

                    if (delta.Length() <= CollisionRadius + obj.CollisionRadius)
                    {
                        Position -= 2 * Velocity * timelapse;
                    }
                }
                else
                {
                    CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, CollisionRectangle.Width, CollisionRectangle.Height);
                    obj.CollisionRectangle = new Rectangle((int)obj.Position.X - obj.CollisionRectangle.Width / 2, (int)obj.Position.Y - obj.CollisionRectangle.Width / 2, obj.CollisionRectangle.Width, obj.CollisionRectangle.Height);

                    if (CollisionRectangle.Intersects(obj.CollisionRectangle))
                    {
                        Position -= 2 * Velocity * timelapse;
                    }
                }
            }
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
