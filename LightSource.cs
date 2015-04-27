using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace ActionGame
{
    class LightSource
    {
        static Dictionary<Light,Texture2D> Textures = new Dictionary<Light,Texture2D>();
        
        public enum Light { circle, circlesharp };
        public const int LIGHTDIMENSION = 64;

        public Vector2 Position { get; set; }
        public Color Colour { get; set; }
        public float Intensity { get; set; }
        public float Range { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 TextureOrigin { get; set; }
        public StaticObject Parent { get; set; }
        private Vector2 Scale { get; set; }

        public LightSource(Light key, float intensity, float range, Color colour, StaticObject parent)
        {
            this.Texture = Textures[key];
            this.TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            if (parent != null)
                this.Position = parent.Position;
            this.Parent = parent;
            this.Colour = colour;
            this.Intensity = intensity;
            this.Range = range;
        }

        public LightSource(LightSource light)
        {
            this.Texture = light.Texture;
            this.TextureOrigin = light.TextureOrigin;
            if (light.Parent != null)
                this.Position = light.Parent.Position;
            this.Parent = light.Parent;
            this.Colour = light.Colour;
            this.Intensity = light.Intensity;
            this.Range = light.Range;
        }

        public void Update()
        {
            if (this.Parent != null)
                Position = Parent.Position;

            Scale = Vector2.One * (WorldSpace.TILESIZE / 64) * Range;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Colour * Intensity, 0, TextureOrigin, Scale, SpriteEffects.None, 1);
        }

        public static void LoadContent(ContentManager Content)
        {
            Textures.Add(Light.circle, Content.Load<Texture2D>("Textures/Lights/circle"));
            Textures.Add(Light.circlesharp, Content.Load<Texture2D>("Textures/Lights/circlesharp"));
        }
    }
}
