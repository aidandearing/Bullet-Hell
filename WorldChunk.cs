using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class WorldChunk
    {
        List<Scene> scenes = new List<Scene>();
        
        public WorldSpace.Biome Biome { get; set; }
        public Vector2 Position { get; set; }
        public string ID { get; set; }

        public WorldChunk(WorldSpace.Biome biome, Vector2 position)
        {
            this.Biome = biome;
            this.Position = position;
            Guid guid = new Guid();
            string id = guid.ToString();
            this.ID = id;
            CreateChunk();
        }

        public void CreateChunk()
        {
            Console.WriteLine(this + ".CreateChunk: " + Position);

            // Base Tile generation
            Random random = new Random();
            for (int x = 0; x < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; x++)
            {
                for (int y = 0; y < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; y++)
                {
                    Scene scene = new Scene("noisy1", Biome, 1);
                    scene.Scale = new Vector2(8, 8);
                    int xS = (int)Math.Floor((double)(Position.X * WorldSpace.CHUNKSIZE + (x - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                    int yS = (int)Math.Floor((double)(Position.Y * WorldSpace.CHUNKSIZE + (y - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                    scene.Position = new Vector2(xS,yS);

                    int texture = random.Next(1, 6);

                    switch (Biome)
                    {
                        case WorldSpace.Biome.desert:
                            // Draw from the smooth and stripped textures
                            if (texture > 3)
                            {
                                texture -= 3;
                                scene.Texture = Scene.Textures["stripped" + texture.ToString()];
                            }
                            else
                            {
                                scene.Texture = Scene.Textures["smooth" + texture.ToString()];
                            }
                            break;
                        case WorldSpace.Biome.plains:
                            // Draw from the smooth and noisy textures
                            if (texture > 3)
                            {
                                texture -= 3;
                                scene.Texture = Scene.Textures["noisy" + texture.ToString()];
                            }
                            else
                            {
                                scene.Texture = Scene.Textures["smooth" + texture.ToString()];
                            }
                            break;
                        case WorldSpace.Biome.swamp:
                            // Draw from the stripped and noisy textures
                            if (texture > 3)
                            {
                                texture -= 3;
                                scene.Texture = Scene.Textures["stripped" + texture.ToString()];
                            }
                            else
                            {
                                scene.Texture = Scene.Textures["noisy" + texture.ToString()];
                            }
                            break;
                        case WorldSpace.Biome.tundra:
                            // Draw from the smooth and noisy textures
                            if (texture > 3)
                            {
                                texture -= 3;
                                scene.Texture = Scene.Textures["smooth" + texture.ToString()];
                            }
                            else
                            {
                                scene.Texture = Scene.Textures["noisy" + texture.ToString()];
                            }
                            break;
                        default:
                            if (texture > 3)
                            {
                                texture -= 3;
                                scene.Texture = Scene.Textures["noisy" + texture.ToString()];
                            }
                            else
                            {
                                scene.Texture = Scene.Textures["smooth" + texture.ToString()];
                            }
                            break;
                    }

                    scene.Update();
                    AddScene(scene);
                }
            }
        }

        public void Update()
        {
            Vector2 delta = WorldSpace.Instance().Position / WorldSpace.CHUNKSIZE - Position;
            if (Math.Abs(delta.X) > 2 || Math.Abs(delta.Y) > 2)
            {
                Dispose();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Scene scene in scenes)
            {
                //if (scene.Texture.Bounds.Intersects(Camera.Screen))
                //{
                    scene.Draw(spriteBatch);
                //}
            }
        }

        public void AddScene(Scene scene)
        {
            scenes.Add(scene);
        }

        public void AddScenes(List<Scene> scenes)
        {
            foreach (Scene scene in scenes)
            {
                this.scenes.Add(scene);
            }
        }

        public void RemoveScene(string id)
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].ID == id)
                {
                    scenes.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveScenes(List<string> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                for (int j = 0; j < scenes.Count; j++)
                {
                    if (ids[i] == scenes[j].ID)
                    {
                        scenes.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public void Dispose()
        {
            WorldSpace.Instance().DisposeChunk(this);
        }
    }
}
