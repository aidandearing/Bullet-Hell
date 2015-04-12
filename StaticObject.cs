using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ActionGame
{
    public abstract class StaticObject
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Color Colour { get; set; }
        public float Depth { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public string ID { get; set; }
        public float Opacity { get; set; }

        public StaticObject(Texture2D texture)
        {
            this.Texture = texture;
            this.Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            this.Scale = new Vector2(1,1);
            this.Colour = Color.White;
            this.Opacity = 1;
            this.SpriteEffect = SpriteEffects.None;
            Guid id = Guid.NewGuid();
            this.ID = id.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Colour, Rotation, Origin, Scale, SpriteEffect, Depth);
        }

        public static void LoadContent(ContentManager Content)
        {
            // NAMEOFCLASS.Textures.Add("name", Content.Load<Texture2D>("reference"));
        }
    }
}
