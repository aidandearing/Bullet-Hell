using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Object
{
    public abstract class StaticObject
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public Color Colour { get; set; }
        public float Depth { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public StaticObject(Texture2D texture)
        {
            this.Texture = texture;
            this.Origin = new Vector2(texture.Width/2, texture.Height/2);
            this.Scale = 1;
            this.Colour = Color.White;
            this.SpriteEffect = SpriteEffects.None;
        }
        
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            Update(gameTime, graphicsDevice);
        }
        
        public  void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, null, Colour, Rotation, Origin, Scale, SpriteEffect, Depth);
            spriteBatch.End();
        }
    }
}

