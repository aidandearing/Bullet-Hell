using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    class Scene : StaticObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public const int SPRITEDIMENSION = 32;

        public float isBiomeEffected { get; set; }
        public float BiomeColourVariance { get; set; }
        public WorldSpace.Biome Biome { get; set; }
        public bool isFlippable { get; set; }

        public Scene(string key, WorldSpace.Biome biome, float biomeEffect, float biomeColourVariance, bool flippable) :base(Textures[key])
        {
            this.isBiomeEffected = biomeEffect;
            this.BiomeColourVariance = biomeColourVariance;
            this.Biome = biome;
            this.isFlippable = flippable;
        }

        new public static void LoadContent(ContentManager Content)
        {
            // Ground
            Scene.Textures.Add("textures_smooth1", Content.Load<Texture2D>("Textures/smooth1"));
            Scene.Textures.Add("textures_smooth2", Content.Load<Texture2D>("Textures/smooth2"));
            Scene.Textures.Add("textures_smooth3", Content.Load<Texture2D>("Textures/smooth3"));
            Scene.Textures.Add("textures_stripped1", Content.Load<Texture2D>("Textures/stripped1"));
            Scene.Textures.Add("textures_stripped2", Content.Load<Texture2D>("Textures/stripped2"));
            Scene.Textures.Add("textures_stripped3", Content.Load<Texture2D>("Textures/stripped3"));
            Scene.Textures.Add("textures_noisy1", Content.Load<Texture2D>("Textures/noisy1"));
            Scene.Textures.Add("textures_noisy2", Content.Load<Texture2D>("Textures/noisy2"));
            Scene.Textures.Add("textures_noisy3", Content.Load<Texture2D>("Textures/noisy3"));

            // Greenery
            // Conifers
            for (int i = 1; i <= 4; i++)
            {
                Scene.Textures.Add("trees_coniferous" + i.ToString(), Content.Load<Texture2D>("Scenes/Trees/Coniferous/conifers_0" + i.ToString()));
            }
            // Deciduous
            for (int i = 1; i <= 8; i++)
            {
                Scene.Textures.Add("trees_deciduous" + i.ToString(), Content.Load<Texture2D>("Scenes/Trees/Deciduous/deciduous_0" + i.ToString()));
            }
            // Dead
            for (int i = 1; i <= 8; i++)
            {
                Scene.Textures.Add("trees_dead" + i.ToString(), Content.Load<Texture2D>("Scenes/Trees/Dead/dead_0" + i.ToString()));
            }
            // Whimsical
            for (int i = 1; i <= 8; i++)
            {
                Scene.Textures.Add("trees_whimsical" + i.ToString(), Content.Load<Texture2D>("Scenes/Trees/Whimsical/whimsical_0" + i.ToString()));
            }
            // Shroom
            Scene.Textures.Add("trees_shroom", Content.Load<Texture2D>("Scenes/Trees/Shroom/shroom_01"));
            for (int i = 1; i <= 7; i++)
            {
                Scene.Textures.Add("trees_shroom_dots_0" + i.ToString(), Content.Load<Texture2D>("Scenes/Trees/Shroom/shroom_dots_0" + i.ToString()));
            }

            // Details
            // Stones
            //Scene.Textures.Add("conifers", Content.Load<Texture2D>("Scenes/Details/stones"));
            // Pebbles
            //Scene.Textures.Add("conifers", Content.Load<Texture2D>("Scenes/Details/pebbles"));
            // Rocks
            Scene.Textures.Add("details_rocks", Content.Load<Texture2D>("Scenes/Details/rocks"));
            // Runes
            //Scene.Textures.Add("conifers", Content.Load<Texture2D>("Scenes/Details/runes"));
        }

        /// <summary>
        /// Should only be called once right after creation, or anytime a variable is changed in a scene. Calling it continuously will be taxing and inefficient
        /// </summary>
        public void Start()
        {
            if (isBiomeEffected > 0)
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;
                r = (byte)MathHelper.Lerp(Color.White.R, WorldSpace.BIOME_COLOUR.R, isBiomeEffected);
                g = (byte)MathHelper.Lerp(Color.White.G, WorldSpace.BIOME_COLOUR.G, isBiomeEffected);
                b = (byte)MathHelper.Lerp(Color.White.B, WorldSpace.BIOME_COLOUR.B, isBiomeEffected);
                Colour = new Color(r, g, b) * Opacity;
            }

            if (BiomeColourVariance > 0)
            {
                float rng;
                int variance;

                rng = (float)(Game1.random.Next(0,100) * BiomeColourVariance);
                variance = (int)((float)Colour.R * rng);
                int r = Colour.R + variance;

                rng = (float)(Game1.random.Next(0, 100) * BiomeColourVariance);
                variance = (int)((float)Colour.G * rng);
                int g = Colour.G + variance;

                rng = (float)(Game1.random.Next(0, 100) * BiomeColourVariance);
                variance = (int)((float)Colour.B * rng);
                int b = Colour.B + variance;

                Colour = new Color(r, g, b) * Opacity;
            }

            if (isFlippable)
            {
                int rng = Game1.random.Next(0, 3);
                switch (rng)
                {
                    case 0:
                        SpriteEffect = SpriteEffects.None;
                        break;
                    case 1:
                        SpriteEffect = SpriteEffects.None;
                        break;
                    case 2:
                        SpriteEffect = SpriteEffects.FlipHorizontally;
                        break;
                    case 3:
                        SpriteEffect = SpriteEffects.FlipVertically;
                        break;
                    default:
                        SpriteEffect = SpriteEffects.None;
                        break;
                }
            }
        }
    }
}
