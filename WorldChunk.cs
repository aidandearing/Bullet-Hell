using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class WorldChunk
    {
        List<StaticObject> scenes = new List<StaticObject>();
        List<StaticObject> deadScenes = new List<StaticObject>();

        ObjectManager objectManager = ObjectManager.Instance();

        public const float TERRAIN_SCALE = WorldSpace.TILESIZE / Scene.SPRITEDIMENSION;
        
        public WorldSpace.Biome Biome { get; set; }
        public Vector2 Position { get; set; }
        public string ID { get; set; }
        public WorldSpace.DungeonChunkState State { get; set; }
        public WorldSpace.WorldState WorldState { get; set; }

        public WorldChunk(WorldSpace.Biome biome, Vector2 position)
        {
            this.WorldState = WorldSpace.Instance().State;
            this.Biome = biome;
            this.Position = position;
            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            this.ID = id;
            CreateChunk();
        }

        public WorldChunk(WorldSpace.Biome biome, Vector2 position, WorldSpace.DungeonChunkState state)
        {
            this.WorldState = WorldSpace.Instance().State;
            this.Biome = biome;
            this.Position = position;
            this.State = state;
            Guid guid = Guid.NewGuid();
            string id = guid.ToString();
            this.ID = id;
            CreateDungeonChunk();
        }

        public void CreateChunk()
        {
            // Base Tile generation
            int texture = Game1.random.Next(1,9);

            for (int x = 0; x < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; x++)
            {
                for (int y = 0; y < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; y++)
                {
                    Scene scene = new Scene("textures_noisy1", Biome, 1, 0.0001f, true);
                    scene.Scale = new Vector2(8, 8);
                    int xS = (int)Math.Floor((double)(Position.X * WorldSpace.CHUNKSIZE + (x - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                    int yS = (int)Math.Floor((double)(Position.Y * WorldSpace.CHUNKSIZE + (y - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                    Vector2 position = new Vector2(xS, yS);
                    scene.Position = position;
                    scene.isCollideRectangular = true;
                    scene.isTerrain = true;
                    scene.CollisionRectangle = new Rectangle((int)scene.Position.X, (int)scene.Position.X, (int)(scene.Texture.Width * scene.Scale.X), (int)(scene.Texture.Height * scene.Scale.Y));

                    texture = (Game1.random.Next(1, 9) + texture * 2) / 3;

                    if (texture > 3 && texture <= 6)
                    {
                        texture -= 3;
                        scene.Texture = Scene.Textures["textures_noisy" + texture.ToString()];
                    }
                    else if (texture > 6)
                    {
                        texture -= 6;
                        scene.Texture = Scene.Textures["textures_stripped" + texture.ToString()];
                    }
                    else
                    {
                        scene.Texture = Scene.Textures["textures_smooth" + texture.ToString()];
                    }

                    scene.Start();
                    AddScene(scene);
                }
            }

            BeautifyChunk();
        }

        public void CreateDungeonChunk()
        {
            // Base Tile generation
            int texture = Game1.random.Next(1, 9);

            for (int x = 0; x < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; x++)
            {
                for (int y = 0; y < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; y++)
                {
                    List<Vector2> dungeonSceneExclusions = DungeonChunkSceneExclusions(State);
                    List<Vector2> dungeonSceneWalls = DungeonChunkSceneWalls(State);

                    Vector2 proposedPosition = new Vector2(x, y);

                    bool generate = true;
                    bool wall = false;

                    foreach(Vector2 exclusionPosition in dungeonSceneExclusions)
                    {
                        if (exclusionPosition == proposedPosition)
                        {
                            generate = false;
                        }
                    }

                    foreach(Vector2 wallPosition in dungeonSceneWalls)
                    {
                        if (wallPosition == proposedPosition)
                        {
                            wall = true;
                        }
                    }

                    if (generate)
                    {
                        Scene scene = new Scene("textures_noisy1", Biome, 0, 0.0001f, true);
                        scene.Scale = new Vector2(TERRAIN_SCALE, TERRAIN_SCALE);
                        int xS = (int)Math.Floor((double)(Position.X * WorldSpace.CHUNKSIZE + (x - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                        int yS = (int)Math.Floor((double)(Position.Y * WorldSpace.CHUNKSIZE + (y - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                        Vector2 position = new Vector2(xS, yS);
                        scene.Position = position;
                        scene.isCollideRectangular = true;
                        scene.CollisionRectangle = new Rectangle((int)scene.Position.X - 16, (int)scene.Position.Y - 16, (int)(scene.Texture.Width * scene.Scale.X) + 32, (int)(scene.Texture.Height * scene.Scale.Y) + 32);

                        texture = Game1.random.Next(1, 10);
                        if (texture == 1)
                        {
                            texture = Game1.random.Next(1, 5);
                        }
                        else
                            texture = 1;

                        scene.Texture = Scene.Textures["textures_ground_dungeon_tile" + texture.ToString()];

                        scene.Start();
                        AddScene(scene);
                    }
                    else
                    {
                        Scene scene = new Scene("DungeonWallGrey", Biome, 0.01f, 0.001f, false);
                        scene.Scale = new Vector2(TERRAIN_SCALE, TERRAIN_SCALE);
                        int xS = (int)Math.Floor((double)(Position.X * WorldSpace.CHUNKSIZE + (x - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                        int yS = (int)Math.Floor((double)(Position.Y * WorldSpace.CHUNKSIZE + (y - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
                        Vector2 position = new Vector2(xS, yS);
                        scene.Position = position;
                        scene.isCollidable = true;
                        scene.CollisionRadius = 256;
                        scene.isCollideRectangular = true;
                        scene.CollisionRectangle = new Rectangle((int)scene.Position.X, (int)scene.Position.Y, (int)scene.CollisionRadius, (int)scene.CollisionRadius);
                        scene.Start();
                        AddScene(scene);
                    }
                }
            }
        }

        public List<Vector2> DungeonChunkSceneExclusions(WorldSpace.DungeonChunkState state)
        {
            List<Vector2> positions = new List<Vector2>();
            positions = WorldSpace.dungeonChunkSceneExclusions[state];

            return positions;
        }

        public List<Vector2> DungeonChunkSceneWalls(WorldSpace.DungeonChunkState state)
        {
            List<Vector2> positions = new List<Vector2>();
            positions = WorldSpace.dungeonChunkSceneWalls[state];

            return positions;
        }

        public void BeautifyChunk()
        {
            // Beautifying Scene Generation
            for (int x = 0; x < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; x++)
            {
                for (int y = 0; y < WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE; y++)
                {
                    Vector2 position;
                    int num;

                    // Performing a greenery pass
                    // 1 in 100 chance of generating a tree/trees, with increasing chance as Humidity and Temperature increase
                    // Range 1 in 100 -> 1 in 33 from 
                    num = Game1.random.Next(1, 50) / MathHelper.Clamp((1 + (int)WorldSpace.CheckHumidity((int)Biome) + (1 - (int)WorldSpace.CheckTemperature((int)Biome))),1,6);
                    if (num == 1)
                    {
                        // 50/50 whether it is a lone plant or a cluster of them
                        if (Game1.random.NextDouble() > 0.5)
                        {
                            // Generate a lone plant
                            position = GeneratePosition(x, y, 128);
                            GenerateTree(position);
                        }
                        else if (Biome != WorldSpace.Biome.Shroom)
                        {
                            // Generate a cluster
                            num = Game1.random.Next(1, 3) * (1 + (int)WorldSpace.CheckHumidity((int)Biome) + (int)WorldSpace.CheckTemperature((int)Biome));
                            for (int i = 0; i < num; i++)
                            {
                                position = GeneratePosition(x, y, 256);
                                GenerateTree(position);
                            }
                        }
                    }

                    // Performing a detail pass
                    // 1 in 10 chance of populating details
                    if (Game1.random.Next(1, 10) == 1)
                    {
                        if (Game1.random.NextDouble() > 0.5)
                        {
                            position = GeneratePosition(x, y, 128);
                            GenerateDetail(position);
                        }
                        else
                        {
                            num = Game1.random.Next(2, 5);
                            for (int i = 0; i < num; i++)
                            {
                                position = GeneratePosition(x, y, 128);
                                GenerateDetail(position);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a tree at the position passed
        /// </summary>
        public void GenerateTree(Vector2 position)
        {
            Tree tree;
            tree = new Tree("trees_dead1", 4, Biome, 1f, 0.001f, true);
            int type = 0;

            // Based off the current biome
            switch (Biome)
            {
                case WorldSpace.Biome.Barren:
                    // Generate dead
                    tree.type = Tree.Type.dead;
                    break;
                case WorldSpace.Biome.Desert:
                    // Generate dead
                    tree.type = Tree.Type.dead;
                    break;
                case WorldSpace.Biome.Forest:
                    // Generate conifers deciduous and whimsical
                    type = Game1.random.Next(0, 2);
                    switch (type)
                    {
                        case 0:
                            tree.type = Tree.Type.coniferous;
                            break;
                        case 1:
                            tree.type = Tree.Type.deciduous;
                            break;
                        case 2:
                            tree.type = Tree.Type.whimsical;
                            break;
                        default:
                            tree.type = Tree.Type.deciduous;
                            break;
                    }
                    break;
                case WorldSpace.Biome.Plains:
                    // Generate deciduous
                    tree.type = Tree.Type.deciduous;
                    break;
                case WorldSpace.Biome.River:
                    // Generate deciduous small
                    tree.type = Tree.Type.deciduous;
                    break;
                case WorldSpace.Biome.Savannah:
                    // Generate dead and deciduous
                    type = Game1.random.Next(0, 1);
                    switch (type)
                    {
                        case 0:
                            tree.type = Tree.Type.dead;
                            break;
                        case 1:
                            tree.type = Tree.Type.deciduous;
                            break;
                        default:
                            tree.type = Tree.Type.deciduous;
                            break;
                    }
                    break;
                case WorldSpace.Biome.Shroom:
                    // Generate shroom
                    tree.type = Tree.Type.shroom;
                    break;
                case WorldSpace.Biome.Swamp:
                    // Generate deciduous and whimsical
                    type = Game1.random.Next(0, 1);
                    switch (type)
                    {
                        case 0:
                            tree.type = Tree.Type.deciduous;
                            break;
                        case 1:
                            tree.type = Tree.Type.whimsical;
                            break;
                        default:
                            tree.type = Tree.Type.deciduous;
                            break;
                    }
                    break;
                case WorldSpace.Biome.Toxic:
                    // Generate dead deciduous and shroom
                    type = Game1.random.Next(0, 2);
                    switch (type)
                    {
                        case 0:
                            tree.type = Tree.Type.dead;
                            break;
                        case 1:
                            tree.type = Tree.Type.deciduous;
                            break;
                        case 2:
                            tree.type = Tree.Type.shroom;
                            break;
                        default:
                            tree.type = Tree.Type.dead;
                            break;
                    }
                    break;
                case WorldSpace.Biome.Tundra:
                    // Generate conifers
                    tree.type = Tree.Type.coniferous;
                    break;
                case WorldSpace.Biome.Whimsy:
                    // Generate whimsical and shroom
                    type = Game1.random.Next(0, 2);
                    switch (type)
                    {
                        case 0:
                            tree.type = Tree.Type.whimsical;
                            break;
                        case 1:
                            tree.type = Tree.Type.shroom;
                            break;
                        default:
                            tree.type = Tree.Type.whimsical;
                            break;
                    }
                    break;
                default:
                    break;
            }

            tree.Position = position;
            tree.Depth = 0.6f + (float)(Game1.random.NextDouble() * 0.15);
            tree.Start();
            tree.isCollidable = true;
            tree.CollisionRadius = 64;
            tree.CollisionCenter = new Vector2(tree.CollisionRadius * tree.Scale.X, tree.CollisionRadius * tree.Scale.Y);
            AddScene(tree);
        }

        /// <summary>
        /// Generates biome specific detailings at the position passed
        /// </summary>
        public void GenerateDetail(Vector2 position)
        {
            // Based off the current biome
            switch (Biome)
            {
                case WorldSpace.Biome.Barren:
                    break;
                case WorldSpace.Biome.Desert:
                    break;
                case WorldSpace.Biome.Forest:
                    break;
                case WorldSpace.Biome.Plains:
                    break;
                case WorldSpace.Biome.River:
                    break;
                case WorldSpace.Biome.Savannah:
                    break;
                case WorldSpace.Biome.Shroom:
                    break;
                case WorldSpace.Biome.Swamp:
                    break;
                case WorldSpace.Biome.Toxic:
                    break;
                case WorldSpace.Biome.Tundra:
                    break;
                case WorldSpace.Biome.Whimsy:
                    break;
                default:
                    break;
            }
        }

        public Vector2 GeneratePosition(int x, int y, int variance)
        {
            int xS = (int)Math.Floor((double)(Position.X * WorldSpace.CHUNKSIZE + (x - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
            int yS = (int)Math.Floor((double)(Position.Y * WorldSpace.CHUNKSIZE + (y - WorldSpace.CHUNKSIZE / WorldSpace.TILESIZE / 2) * WorldSpace.TILESIZE));
            // Randomizing the position around the tile
            xS += Game1.random.Next(-variance, variance);
            yS += Game1.random.Next(-variance, variance);
            // Setting the position
            Vector2 position = new Vector2(xS, yS);
            return position;
        }

        /// <summary>
        /// Generates biome specific npcs at the position passed
        /// </summary>
        public void GenerateNPC(Vector2 position)
        {

        }

        public void Update()
        {
            deadScenes = new List<StaticObject>();

            Vector2 delta = Camera.Instance().Position / WorldSpace.CHUNKSIZE - Position;
            if (WorldSpace.Instance().deadChunk == null)
            {
                if (Math.Abs(delta.X) > WorldSpace.CHUNKRANGE * 2 || Math.Abs(delta.Y) > WorldSpace.CHUNKRANGE * 2)
                {
                    Dispose();
                }
                if (WorldSpace.Instance().State != WorldState)
                {
                   Dispose();
                }
            }
        }

        public void AddScene(Scene scene)
        {
            scenes.Add(objectManager.AddObject(scene));
        }

        public void AddScenes(List<Scene> scenes)
        {
            foreach (Scene scene in scenes)
            {
                scenes.Add((Scene)objectManager.AddObject(scene));
            }
        }

        public void RemoveScene(Scene scene)
        {
            deadScenes.Add(scene);
        }

        public void Dispose()
        {
            foreach (Scene scene in scenes)
            {
                if (scene is Tree)
                {
                    foreach (Scene dots in ((Tree)scene).scenes)
                        objectManager.RemoveObject(dots);
                }
                RemoveScene(scene);
            }

            foreach (Scene scene in deadScenes)
            {
                objectManager.RemoveObject(scene);
                scenes.Remove(scene);
            }

            WorldSpace.Instance().DisposeChunk(this);
        }
    }
}
