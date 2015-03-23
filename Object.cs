using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Object
{
    public abstract class StaticObject
    {
        protected Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        protected Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        
        protected float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        protected float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        protected Color colour;
        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        protected float depth;
        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        protected SpriteEffects spriteEffect;
        public SpriteEffects SpriteEffect
        {
            get { return spriteEffect;}
            set { spriteEffect = value; }
        }
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

