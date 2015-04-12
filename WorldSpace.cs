using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class WorldSpace
    {
        private List<WorldChunk> chunks;
        private List<WorldChunk> deadChunks;

        public enum WorldState { world, dungeon }
        /// <summary>
        /// The enumerator that has all the possible biomes.
        /// </summary>
        public enum Biome { plains, desert, tundra, swamp }
        public enum Direction { north, east, south, west }
        public enum DungeonChunkState { straightN, straightE, cornerNE, cornerNW, cornerSE, cornerSW, tboneNEW, tboneENS, tboneSEW, tboneWNS, cross, startN, startE, startS, startW, endN, endE, endS, endW }
        /// <summary>
        /// The constant integer that represents the number of biomes in the game.
        /// </summary>
        public const int BIOMES = 4;
        /// <summary>
        /// The constant float that represents the number of seconds that must pass in game before a BiomeChange can occur.
        /// </summary>
        public const float BIOMES_TIMEBETWEENCHANGES = 20;
        /// <summary>
        /// The constant int that represents the chance of calling BiomeChange
        /// </summary>
        public const int BIOMES_CHANCEOFCHANGE = 100;
        public const int TILESIZE = 256;
        public const int CHUNKSIZE = 4096;

        bool firstRun = true;
        private Vector2 lastChunk = new Vector2();

        public static Color BIOMES_COLOUR_PLAINS = new Color(0f, 0.4f, 0.1f);
        public static Color BIOMES_COLOUR_DESERT = new Color(1f, 1f, 0.9f);
        public static Color BIOMES_COLOUR_TUNDRA = new Color(0.9f, 0.9f, 1f);
        public static Color BIOMES_COLOUR_SWAMP = new Color(0f, 0.3f, 0.1f);

        private static WorldSpace instance { get; set; }
        public Vector2 Position { get; set; }
        public WorldState State { get; set; }
        public DungeonChunkState DungeonState { get; set; }
        /// <summary>
        /// The current biome the world is in, this dictates what new scenes textures will generate as, and what new props and npcs are generated.
        /// </summary>
        public Biome biome { get; set; }
        public Direction direction { get; set; }
        private float timeBiomeChange { get; set; }          // The number of seconds that must ellapse before a BiomeChange can occur

        private WorldSpace()
        {
            chunks = new List<WorldChunk>();
            deadChunks = new List<WorldChunk>();
        }

        public static WorldSpace Instance()
        {
            if (instance == null)
                instance = new WorldSpace();
            return instance;
        }

        public void Update(GameTime gameTime)
        {
            if (firstRun)
            {
                CreateChunk(Vector2.Zero);
                firstRun = false;
            }

            deadChunks = new List<WorldChunk>();
            timeBiomeChange += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Vector2 position = Camera.Instance().Position;

            Vector2 CameraPosition = new Vector2((int)(Math.Floor((double)(position.X / CHUNKSIZE))), (int)(Math.Floor((double)(position.Y / CHUNKSIZE))));
            Vector2 WorldPosition = new Vector2((int)(Math.Floor((double)(Position.X / CHUNKSIZE))), (int)(Math.Floor((double)(Position.Y / CHUNKSIZE))));

            Vector2 delta = position - Position;
            delta /= CHUNKSIZE / 2;
            //Console.WriteLine("W Pos:" + WorldPosition);
            //Console.WriteLine("C Pos:" + CameraPosition);
            //5Console.WriteLine("D Pos:" + delta);

            if (delta.X > 1 || delta.X < -1 || delta.Y > 1 || delta.Y < -1)
            {
                if (delta.X > 1)
                    direction = Direction.east;
                if (delta.X < -1)
                    direction = Direction.west;
                if (delta.Y > 1)
                    direction = Direction.south;
                if (delta.Y < -1)
                    direction = Direction.north;


                bool generate = true;
                foreach (WorldChunk chunk in chunks)
                {
                    if (chunk.Position == CameraPosition)
                        generate = false;
                }
                if (generate)
                {
                    Console.WriteLine("Camera: " + CameraPosition);
                    Console.WriteLine("World: " + WorldPosition);
                    Position = position;
                    CreateChunk(CameraPosition);
                }
            }

            #region Timers
            if (timeBiomeChange >= BIOMES_TIMEBETWEENCHANGES)
                timeBiomeChange = 0;
            #endregion

            foreach (WorldChunk chunk in chunks)
            {
                chunk.Update();
            }

            foreach (WorldChunk chunk in deadChunks)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].ID == chunk.ID)
                    {
                        Console.WriteLine("Disposing chunk at: " + chunk.Position);
                        chunks.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (WorldChunk chunk in chunks)
            {
                chunk.Draw(spriteBatch);
            }
        }

        public void BiomeChange(int biome)
        {
            if (biome < BIOMES)
                this.biome = (Biome)biome;
        }

        public void CreateChunk(Vector2 position)
        {
            WorldChunk chunk = new WorldChunk(biome, position);
            chunks.Add(chunk);
        }

        public void DisposeChunk(WorldChunk chunk)
        {
            deadChunks.Add(chunk);
        }
    }
}
