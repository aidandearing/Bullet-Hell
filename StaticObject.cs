using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    public abstract class StaticObject
    {
        public Texture2D Texture { get; set; }
        public Vector2 TextureOrigin { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Color Colour { get; set; }
        public float Depth { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public string ID { get; set; }
        public float Opacity { get; set; }
        public bool isCollidable { get; set; }
        public bool isTerrain { get; set; }
        public bool isCollideRectangular { get; set; }
        public Rectangle CollisionRectangle { get; set; }
        public int CollisionRadius { get; set; }
        public Vector2 CollisionCenter { get; set; }
        public List<int> ObjectManagerCollisionBoxIndexs = new List<int>();

        public static Dictionary<string, Texture2D> DEBUG { get; set; }

        public StaticObject(Texture2D texture)
        {
            this.Texture = texture;
            this.TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            this.Scale = new Vector2(2,2);
            this.Colour = Color.White;
            this.Opacity = 1;
            this.SpriteEffect = SpriteEffects.None;
            Guid id = Guid.NewGuid();
            this.ID = id.ToString();
            this.isCollidable = false;
            if (this.isCollideRectangular)
            {
                this.CollisionRectangle = Texture.Bounds;
            }
            else
            {
                this.CollisionRadius = Texture.Width / 2;
                this.CollisionCenter = new Vector2(CollisionRadius * Scale.X, CollisionRadius * Scale.Y);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
            if (bounds.Intersects(Camera.Screen))
            {
                spriteBatch.Draw(Texture, Position, null, Colour * Opacity, Rotation, TextureOrigin, Scale, SpriteEffect, Depth);
            }
            //spriteBatch.Draw(DEBUG_circle, Position, null, Color.White, 1.0f, new Vector2(16,16), CollisionRadius / 16, SpriteEffects.None, 1f);
        }

        public static void LoadContent(ContentManager Content)
        {
            // NAMEOFCLASS.Textures.Add("name", Content.Load<Texture2D>("reference"));
            DEBUG = new Dictionary<string, Texture2D>();
            StaticObject.DEBUG.Add("circle", Content.Load<Texture2D>("Debug/Circle"));
            StaticObject.DEBUG.Add("triangle", Content.Load<Texture2D>("Debug/Triangle"));
        }
    }
}
