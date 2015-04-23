using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    abstract class AnimatedObject : DynamicObject
    {
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        public string Animation { get; set; }

        public AnimatedObject(Texture2D texture)
            : base(texture)
        {
            this.Animation = "idle";
        }

        new public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Animations[Animation].Update(gameTime);
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            Animations[Animation].Draw(spriteBatch, this);
        }

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation.Name, animation);
        }
    }
}
