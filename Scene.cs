using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ActionGame
{
    class Scene : StaticObject
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public float isBiomeEffected { get; set; }
        public WorldSpace.Biome Biome { get; set; }

        public Scene(string key, WorldSpace.Biome biome, float biomeEffect) :base(Textures[key])
        {
            this.isBiomeEffected = biomeEffect;
            this.Biome = biome;
        }

        new public static void LoadContent(ContentManager Content)
        {
            Scene.Textures.Add("smooth1", Content.Load<Texture2D>("Textures/smooth1"));
            Scene.Textures.Add("smooth2", Content.Load<Texture2D>("Textures/smooth2"));
            Scene.Textures.Add("smooth3", Content.Load<Texture2D>("Textures/smooth3"));
            Scene.Textures.Add("stripped1", Content.Load<Texture2D>("Textures/stripped1"));
            Scene.Textures.Add("stripped2", Content.Load<Texture2D>("Textures/stripped2"));
            Scene.Textures.Add("stripped3", Content.Load<Texture2D>("Textures/stripped3"));
            Scene.Textures.Add("noisy1", Content.Load<Texture2D>("Textures/noisy1"));
            Scene.Textures.Add("noisy2", Content.Load<Texture2D>("Textures/noisy2"));
            Scene.Textures.Add("noisy3", Content.Load<Texture2D>("Textures/noisy3"));
        }

        /// <summary>
        /// Should only be called once right after creation, or anytime a variable is changed in a scene. Calling it continuously will be taxing and inefficient
        /// </summary>
        public void Update()
        {
            if (isBiomeEffected > 0)
            {
                byte r = 0;
                byte g = 0;
                byte b = 0;
                switch (Biome)
                {
                    case WorldSpace.Biome.plains:
                        r = (byte)MathHelper.Lerp(Color.White.R, WorldSpace.BIOMES_COLOUR_PLAINS.R, isBiomeEffected);
                        g = (byte)MathHelper.Lerp(Color.White.G, WorldSpace.BIOMES_COLOUR_PLAINS.G, isBiomeEffected);
                        b = (byte)MathHelper.Lerp(Color.White.B, WorldSpace.BIOMES_COLOUR_PLAINS.B, isBiomeEffected);
                        Colour = new Color(r, g, b) * Opacity;
                        break;
                    case WorldSpace.Biome.desert:
                        r = (byte)MathHelper.Lerp(Color.White.R, WorldSpace.BIOMES_COLOUR_DESERT.R, isBiomeEffected);
                        g = (byte)MathHelper.Lerp(Color.White.G, WorldSpace.BIOMES_COLOUR_DESERT.G, isBiomeEffected);
                        b = (byte)MathHelper.Lerp(Color.White.B, WorldSpace.BIOMES_COLOUR_DESERT.B, isBiomeEffected);
                        Colour = new Color(r, g, b) * Opacity;
                        break;
                    case WorldSpace.Biome.tundra:
                        r = (byte)MathHelper.Lerp(Color.White.R, WorldSpace.BIOMES_COLOUR_TUNDRA.R, isBiomeEffected);
                        g = (byte)MathHelper.Lerp(Color.White.G, WorldSpace.BIOMES_COLOUR_TUNDRA.G, isBiomeEffected);
                        b = (byte)MathHelper.Lerp(Color.White.B, WorldSpace.BIOMES_COLOUR_TUNDRA.B, isBiomeEffected);
                        Colour = new Color(r, g, b) * Opacity;
                        break;
                    case WorldSpace.Biome.swamp:
                        r = (byte)MathHelper.Lerp(Color.White.R, WorldSpace.BIOMES_COLOUR_SWAMP.R, isBiomeEffected);
                        g = (byte)MathHelper.Lerp(Color.White.G, WorldSpace.BIOMES_COLOUR_SWAMP.G, isBiomeEffected);
                        b = (byte)MathHelper.Lerp(Color.White.B, WorldSpace.BIOMES_COLOUR_SWAMP.B, isBiomeEffected);
                        Colour = new Color(r, g, b) * Opacity;
                        break;
                    default:
                        Colour = Color.White * Opacity;
                        break;
                }
            }
        }
    }
}
