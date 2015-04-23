using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    class Tree : Scene
    {
        private float ScaleFloat { get; set; }
        private Texture2D TextureDetail { get; set; }
        public Type type { get; set; }
        public List<Scene> scenes;

        public enum Type { coniferous, deciduous, dead, shroom, whimsical };

        public Tree(string key, float scale, WorldSpace.Biome biome, float biomeEffect, float biomeColourVariance, bool isFlippable) :base(key, biome, biomeEffect, biomeColourVariance, isFlippable)
        {
            this.ScaleFloat = scale;
            scenes = new List<Scene>();
        }

        new public void Start()
        {
            float scale = 2 * (ScaleFloat + (float)Game1.random.NextDouble());
            this.Scale = new Vector2(scale, scale);

            base.Start();

            int r = 0;
            int g = 0;
            int b = 0;
            int y = 0;

            int num = Game1.random.Next(1, 4);

            switch (type)
            {
                case Type.coniferous:
                    r = MathHelper.Clamp((int)(Colour.R * 0.75f),0,25);
                    g = MathHelper.Clamp(Colour.G,100,150);
                    b = MathHelper.Clamp(Colour.B,75,100);
                    Scale *= 1f + (float)Game1.random.NextDouble() * (4 - (int)WorldSpace.CheckTemperature((int)Biome)); 
                    Texture = Scene.Textures["trees_coniferous" + num.ToString()];
                    break;
                case Type.dead:
                    r = MathHelper.Clamp(Colour.R,25,100);
                    g = MathHelper.Clamp((int)(Colour.R / 1.5f),25,100);
                    b = MathHelper.Clamp(Colour.R / 3,0,100);
                    Scale *= 1f + (float)Game1.random.NextDouble();
                    Texture = Scene.Textures["trees_dead" + num.ToString()];
                    break;
                case Type.deciduous:
                    r = MathHelper.Clamp(Colour.R / 2,0,255);
                    g = MathHelper.Clamp(Colour.G, 100, 255);
                    b = MathHelper.Clamp(Colour.B / 5,0,255);
                    Scale *= 1f + (float)Game1.random.NextDouble() * ((4 - (int)WorldSpace.CheckTemperature((int)Biome)) / 2); 
                    Texture = Scene.Textures["trees_deciduous" + num.ToString()];
                    break;
                case Type.shroom:
                    r = MathHelper.Clamp(255 - Colour.R,0,255);
                    g = MathHelper.Clamp(255 - Colour.G,0,255);
                    b = MathHelper.Clamp(255 - Colour.B,0,255);
                    y = (int)(r * 0.3f + g * 0.6f + b * 0.1f);
                    r = (int)(-(g * 0.6f + b * 0.1 - 255) / 0.3f);
                    g = (int)(-(r * 0.3f + b * 0.1 - 255) / 0.6f);
                    b = (int)(-(g * 0.6f + r * 0.3 - 255) / 0.1f);
                    Texture = Scene.Textures["trees_shroom"];
                    Scale *= 0.25f + (float)Game1.random.NextDouble() * ((int)WorldSpace.CheckTemperature((int)Biome) + (int)WorldSpace.CheckHumidity((int)Biome)); 
                    num = Game1.random.Next(1, 7);
                    Scene dots = new Scene("trees_shroom_dots_0" + num.ToString(), Biome, (float)(0.75f + Game1.random.NextDouble() * 0.25f), 0.01f, true);
                    dots.Scale = Scale;
                    dots.Colour = new Color((255 - r),(255 - g),(255 - b));
                    dots.Position = Position;
                    dots.Depth = Depth + 0.01f;
                    scenes.Add(dots);
                    ObjectManager.Instance().AddObject(dots);
                    break;
                case Type.whimsical:
                    r = MathHelper.Clamp((int)(Colour.R * 2),100,255);
                    g = MathHelper.Clamp(Colour.G,0,255);
                    b = MathHelper.Clamp(Colour.B / 5,0,100);
                    Scale *= 1f + (float)Game1.random.NextDouble() * ((4 - (int)WorldSpace.CheckTemperature((int)Biome)) / 2); 
                    Texture = Scene.Textures["trees_whimsical" + num.ToString()];
                    break;
                default:
                    break;
            }

            Colour = new Color(r, g, b);
        }
    }
}
