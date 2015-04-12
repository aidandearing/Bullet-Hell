using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            // Update its animation here
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
