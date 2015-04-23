using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ActionGame
{
    class WorldSpace
    {
        #region Lists
        private List<WorldChunk> chunks;
        public WorldChunk deadChunk;
        public static Dictionary<DungeonChunkState, List<Vector2>> dungeonChunkSceneExclusions;
        public static Dictionary<DungeonChunkState, List<Vector2>> dungeonChunkSceneWalls;
        #endregion

        #region Enums and Constants
        /// <summary>
        /// The state of the world space:
        /// World: Generating the open world space the players are free to wander through, killing and enjoying
        /// Dungeon: Generates the closed algorithmic world space that forces the players through a specific, randomized system of logic
        /// </summary>
        public enum WorldState { world, dungeon }
        /// <summary>
        /// The enumerator that has all the possible biomes.
        /// </summary>
        public enum Biome { Plains, Tundra, Forest, River, Swamp, Whimsy, Shroom, Savannah, Desert, Barren, Toxic }
        /// <summary>
        /// The temperature at any given biome
        /// </summary>
        public enum Temperature { Cold, Moderate, Warm, Hot }
        /// <summary>
        /// The humidity at any given biome
        /// </summary>
        public enum Humidity { Dry, Moderate, Moist, Wet }
        /// <summary>
        /// The current direction
        /// </summary>
        public enum Direction { north, northeast, east, southeast, south, southwest, west, northwest }
        /// <summary>
        /// The current dungeon state, dictates what generates next
        /// </summary>
        public enum DungeonChunkState { straightN, straightE, cornerNE, cornerNW, cornerSE, cornerSW, tboneNEW, tboneENS, tboneSEW, tboneWNS, cross, startN, startE, startS, startW, endN, endE, endS, endW, roomA, roomB, roomC, roomD }
        /// <summary>
        /// The constant integer that represents the number of biomes in the game.
        /// </summary>
        public const int BIOMES = 11;
        /// <summary>
        /// The constant int that represents the chance of calling BiomeChange
        /// </summary>
        public const int BIOME_CHANCEOFCHANGE = 10;
        /// <summary>
        /// The maximum difference in temperature and humidity allowed between the current biome and the desired next biome inorder for the change to validate
        /// </summary>
        public const int BIOME_MAXDIFFERENCE = 2;
        /// <summary>
        /// The number of steps it takes to transition between biomes
        /// Functions like this
        /// Actual biomesteps scale with the total distance between the start and goal biomes
        /// 64 acts as the maximum number of steps that can be taken if the 2 biomes are on 2 seperate corners, but not diagonal.
        /// </summary>
        public const int BIOME_STEPS = 128;
        /// <summary>
        /// The size of each tile of ground, also 1m²
        /// </summary>
        public const int TILESIZE = 256;
        /// <summary>
        /// The size of each chunk
        /// </summary>
        public const int CHUNKSIZE = 2048;
        /// <summary>
        /// The distance in chunks at which chunks will if further than trigger self disposal
        /// </summary>
        public const int CHUNKRANGE = 2;
        /// <summary>
        /// The minimum number of dungeon chunks that will be generated before the dungeon ends
        /// </summary>
        public const int DUNGEON_LENGTHMIN = 50;
        /// <summary>
        /// The maximum number of dungeon chunks that will be generated before the dungeon ends
        /// </summary>
        public const int DUNGEON_LENGTHMAX = 1000;
        #endregion

        bool firstRun = true;

        #region Biome Vectors & ColourMap
        public static Texture2D BIOME_COLOURMAP;
        private static Vector2 BIOME_VECTOR_PLAINS = new Vector2(0.5f, 0.5f);
        private static Vector2 BIOME_VECTOR_DESERT = new Vector2(0, 0);
        private static Vector2 BIOME_VECTOR_TUNDRA = new Vector2(1f, 0);
        private static Vector2 BIOME_VECTOR_SWAMP = new Vector2(1f, 1f);
        private static Vector2 BIOME_VECTOR_SHROOM = new Vector2(0, 1f);
        private static Vector2 BIOME_VECTOR_SAVANNAH = new Vector2(0.25f,0.25f);
        private static Vector2 BIOME_VECTOR_FOREST = new Vector2(0.75f, 0.25f);
        private static Vector2 BIOME_VECTOR_RIVER = new Vector2(0.75f, 0.75f);
        private static Vector2 BIOME_VECTOR_WHIMSY = new Vector2(0.25f, 0.75f);
        private static Vector2 BIOME_VECTOR_BARREN = new Vector2(0.5f, 0);
        private static Vector2 BIOME_VECTOR_TOXIC = new Vector2(0.5f,0.25f);
        public static Vector2 BIOME_VECTOR_COLOUR = new Vector2(0.5f, 0.5f);
        public static Vector2 BIOME_VECTOR_GOAL = BIOME_VECTOR_PLAINS;
        public static Vector2 BIOME_VECTOR_START = BIOME_VECTOR_PLAINS;
        public static Vector2 BIOME_POSITION_START = Vector2.Zero;
        public static Color BIOME_COLOUR = new Color(0, 120, 25);
        #endregion

        #region Variables
        public Vector2 Position { get; private set; }
        /// <summary>
        /// The current world state the world is in
        /// </summary>
        public WorldState State { get; private set; }
        /// <summary>
        /// The current dungeonstate the world is in
        /// </summary>
        public DungeonChunkState DungeonState { get; set; }
        /// <summary>
        /// The current biome the world is in, this dictates what new scenes textures will generate as, and what new props and npcs are generated.
        /// </summary>
        public Biome biome { get; private set; }
        /// <summary>
        /// The temperature of the current biome
        /// </summary>
        public Temperature temperature { get; private set; }
        /// <summary>
        /// The humidity of the current biome
        /// </summary>
        public Humidity humidity { get; private set; }
        /// <summary>
        /// The direction the players are heading
        /// </summary>
        public Direction direction { get; set; }
        public int DungeonLength { get; set; }
        public int DungeonTravelled { get; set; }

        private bool StateSwitch { get; set; }
        private float StateSwitchTimer { get; set; }
        private const float STATESWITCHTIMER = 1; 
        #endregion

        private static WorldSpace instance { get; set; }

        private WorldSpace()
        {
            chunks = new List<WorldChunk>();
            deadChunk = null;
            dungeonChunkSceneExclusions = new Dictionary<DungeonChunkState, List<Vector2>>();
            dungeonChunkSceneWalls = new Dictionary<DungeonChunkState, List<Vector2>>();
        }

        public static WorldSpace Instance()
        {
            if (instance == null)
                instance = new WorldSpace();
            return instance;
        }

        public void Update(GameTime gameTime)
        {
            deadChunk = null;

            if (firstRun)
            {
                temperature = CheckTemperature((int)biome);
                humidity = CheckHumidity((int)biome);
                firstRun = false;
                SwitchStates();
                DungeonLength = Game1.random.Next(75, 150);
            }

            StateSwitchTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (StateSwitchTimer > STATESWITCHTIMER && StateSwitch)
            {
                StateSwitch = false;
                StateSwitchTimer = 0;
            }

            if (!StateSwitch)
            {
                switch (State)
                {
                    case WorldState.world:
                        UpdateWorld(gameTime);
                        break;
                    case WorldState.dungeon:
                        UpdateDungeon(gameTime);
                        break;
                    default:
                        UpdateWorld(gameTime);
                        break;
                }
            }
            else
            {
                foreach (WorldChunk chunk in chunks)
                    chunk.Dispose();
            }

            if (deadChunk != null)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].ID == deadChunk.ID)
                    {
                        chunks.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void UpdateWorld(GameTime gameTime)
        {
            bool biomeTransitioning = true;

            Vector2 position = Camera.Instance().Position;

            Vector2 CameraPosition = new Vector2((int)(Math.Round((double)(position.X / CHUNKSIZE))), (int)(Math.Round((double)(position.Y / CHUNKSIZE))));
            Vector2 WorldPosition = new Vector2((int)(Math.Round((double)(Position.X / CHUNKSIZE))), (int)(Math.Round((double)(Position.Y / CHUNKSIZE))));

            #region Chunk Generation
            // Iterate through a grid centered around the camera's absolute chunk position
            bool chunksGenerate = false;

            for (int x = -CHUNKRANGE; x <= CHUNKRANGE; x++)
            {
                for (int y = -CHUNKRANGE; y <= CHUNKRANGE; y++)
                {
                    bool generate = true;

                    Vector2 newChunkPosition = CameraPosition + new Vector2(x, y);
                    // Check every chunk already existant against the proposed new chunk
                    foreach (WorldChunk chunk in chunks)
                    {
                        // If the original chunks position is the same as the proposed new chunks position prevent generation
                        if (chunk.Position == newChunkPosition)
                        {
                            generate = false;
                        }
                    }

                    if (generate)
                    {
                        CreateChunk(newChunkPosition);
                        Position = CameraPosition;
                        chunksGenerate = true;
                    }
                }
            }
            #endregion

            #region Biome Logic
            if (chunksGenerate)
            {
                #region Biome Colour Shift Logic
                // Get the delta between the start and the goal
                Vector2 BiomeColourDelta = BIOME_VECTOR_GOAL - BIOME_VECTOR_START;
                // Get the current length away from the start position of the transition
                float BiomeSteps = BIOME_STEPS * (BIOME_STEPS * (BiomeColourDelta.Length() / BIOME_STEPS));
                float BiomeColourProgress = MathHelper.Clamp((float)Math.Abs((BIOME_POSITION_START.Length() - Position.Length()) / BiomeSteps), 0, 1);

                // Set the current colour position to the position along the line between the start and end spots, lerped by the progress towards the end
                BIOME_VECTOR_COLOUR.X = MathHelper.Lerp(BIOME_VECTOR_START.X, BIOME_VECTOR_GOAL.X, BiomeColourProgress);
                BIOME_VECTOR_COLOUR.X = (float)Math.Round((double)(BIOME_VECTOR_COLOUR.X * 1000)) / 1000;
                BIOME_VECTOR_COLOUR.Y = MathHelper.Lerp(BIOME_VECTOR_START.Y, BIOME_VECTOR_GOAL.Y, BiomeColourProgress);
                BIOME_VECTOR_COLOUR.Y = (float)Math.Round((double)(BIOME_VECTOR_COLOUR.Y * 1000)) / 1000;
                #endregion

                #region Biome Colour Logic
                // If the progress towards the new biome is complete allow biome transitioning
                if (BiomeColourProgress == 1)
                {
                    BIOME_VECTOR_COLOUR = BIOME_VECTOR_GOAL;
                    biomeTransitioning = false;
                }

                Rectangle sample = new Rectangle((int)MathHelper.Clamp(BIOME_VECTOR_COLOUR.X * (BIOME_COLOURMAP.Width), 0, BIOME_COLOURMAP.Width - 1), (int)MathHelper.Clamp(BIOME_VECTOR_COLOUR.Y * (BIOME_COLOURMAP.Height), 0, BIOME_COLOURMAP.Height - 1), 1, 1);
                Color[] sampleColour = new Color[1];
                BIOME_COLOURMAP.GetData<Color>(0, sample, sampleColour, 0, 1);
                BIOME_COLOUR = sampleColour[0];
                #endregion

                #region Biome Change Logic
                int rng = Game1.random.Next(1, BIOME_CHANCEOFCHANGE);

                // The biome will only change if it rolls a 1 against the chance of changing, and the worldspace isn't currently transitioning biomes
                if (rng == 1 && !biomeTransitioning)
                {
                    rng = Game1.random.Next(0, BIOMES);
                    bool notValidated = true;
                    while (notValidated)
                    {
                        // If the current biome and the new biome would be the same biome
                        int deltaCheck = Math.Abs((int)temperature - (int)CheckTemperature(rng)) + Math.Abs((int)humidity - (int)CheckHumidity(rng));
                        if (biome == (Biome)rng || deltaCheck > BIOME_MAXDIFFERENCE)
                        {
                            rng = Game1.random.Next(0, BIOMES);
                        }
                        else
                        {
                            notValidated = false;
                        }
                    }
                    BiomeChange(rng);
                }
                #endregion
            }
            #endregion

            foreach (WorldChunk chunk in chunks)
            {
                chunk.Update();
            }
        }

        public void UpdateDungeon(GameTime gameTime)
        {
            Vector2 position = Camera.Instance().Position;

            Vector2 CameraPosition = new Vector2((int)(Math.Round((double)(position.X / CHUNKSIZE))), (int)(Math.Round((double)(position.Y / CHUNKSIZE))));
            Vector2 WorldPosition = new Vector2((int)(Math.Round((double)(Position.X / CHUNKSIZE))), (int)(Math.Round((double)(Position.Y / CHUNKSIZE))));

            if (DungeonTravelled > DungeonLength)
            {
                foreach (WorldChunk chunk in chunks)
                {
                    if ((chunk.State == DungeonChunkState.endE || chunk.State == DungeonChunkState.endN || chunk.State == DungeonChunkState.endS || chunk.State == DungeonChunkState.endW) && chunk.Position == CameraPosition)
                        SwitchStates();
                }
            }

            if (chunks.Count < 1)
            {
                CreateChunk(Position, (DungeonChunkState)Game1.random.Next(11, 14));
            }

            #region Chunk Generation
            // Iterate through a grid centered around the camera's absolute chunk position

            List<Direction> newChunkDirections = new List<Direction>();

            Vector2 previousChunkPosition = new Vector2();

            for (int x = -CHUNKRANGE; x <= CHUNKRANGE; x++)
            {
                for (int y = -CHUNKRANGE; y <= CHUNKRANGE; y++)
                {
                    if (x == 0 || y == 0)
                    {
                        Vector2 newChunkPosition = CameraPosition + new Vector2(x, y);

                        // Check every chunk already existant against the proposed new chunk
                        foreach (WorldChunk chunk in chunks)
                        {
                            // If the original chunks position is the same as the proposed new chunks position prevent generation
                            if (chunk.Position == newChunkPosition)
                            {
                                newChunkDirections = DungeonCheckDirection(chunk.State);
                                previousChunkPosition = chunk.Position;
                            }
                        }

                        foreach (Direction newDirection in newChunkDirections)
                        {
                            Vector2 newPos = previousChunkPosition + DungeonGetChunkPosition(newDirection);
                            List<DungeonChunkState> possibleStates = DungeonGetChunk(newDirection);

                            // Ideally the WorldSpace will get the state of all surrounding chunks and depending on their state build a chunk that enables the continued generation of the dungeon

                            bool generate = true;

                            foreach (WorldChunk chunk in chunks)
                            {
                                if (chunk.Position == newPos)
                                {
                                    generate = false;
                                }
                            }

                            if (generate)
                            {
                                CreateChunk(newPos, possibleStates[Game1.random.Next(0, possibleStates.Count - 1)]);
                                DungeonTravelled++;
                            }
                        }
                    }
                } 
            }
            #endregion

            foreach (WorldChunk chunk in chunks)
            {
                chunk.Update();
            }
        }

        public void BiomeChange(int biome)
        {
            if (biome < BIOMES)
            {
                this.biome = (Biome)biome;

                switch(this.biome)
                {
                    case Biome.Desert:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_DESERT;
                        temperature = Temperature.Hot;
                        humidity = Humidity.Dry;
                        break;
                    case Biome.Plains:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_PLAINS;
                        temperature = Temperature.Moderate;
                        humidity = Humidity.Moderate;
                        break;
                    case Biome.Shroom:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_SHROOM;
                        temperature = Temperature.Warm;
                        humidity = Humidity.Moist;
                        break;
                    case Biome.Swamp:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_SWAMP;
                        temperature = Temperature.Warm;
                        humidity = Humidity.Wet;
                        break;
                    case Biome.Tundra:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_TUNDRA;
                        temperature = Temperature.Cold;
                        humidity = Humidity.Dry;
                        break;
                    case Biome.Forest:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_FOREST;
                        temperature = Temperature.Moderate;
                        humidity = Humidity.Moderate;
                        break;
                    case Biome.River:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_RIVER;
                        temperature = Temperature.Moderate;
                        humidity = Humidity.Wet;
                        break;
                    case Biome.Savannah:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_SAVANNAH;
                        temperature = Temperature.Hot;
                        humidity = Humidity.Dry;
                        break;
                    case Biome.Whimsy:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_WHIMSY;
                        temperature = Temperature.Warm;
                        humidity = Humidity.Moderate;
                        break;
                    case Biome.Barren:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_BARREN;
                        temperature = Temperature.Moderate;
                        humidity = Humidity.Dry;
                        break;
                    case Biome.Toxic:
                        BIOME_VECTOR_COLOUR = BIOME_VECTOR_GOAL;
                        temperature = Temperature.Warm;
                        humidity = Humidity.Dry;
                        break;
                    default:
                        BIOME_VECTOR_GOAL = BIOME_VECTOR_PLAINS;
                        temperature = Temperature.Moderate;
                        humidity = Humidity.Moderate;
                        this.biome = Biome.Plains;
                        break;
                }
            }
            // Save the amount of step the biome vector should take
            BIOME_VECTOR_START = BIOME_VECTOR_COLOUR;
            BIOME_POSITION_START = Position;
        }

        public static Temperature CheckTemperature(int biome)
        {
            switch ((Biome)biome)
            {
                case Biome.Desert:
                    return Temperature.Hot;
                case Biome.Plains:
                    return Temperature.Moderate;
                case Biome.Shroom:
                    return Temperature.Warm;
                case Biome.Swamp:
                    return Temperature.Warm;
                case Biome.Tundra:
                    return Temperature.Cold;
                case Biome.Forest:
                    return Temperature.Moderate;
                case Biome.River:
                    return Temperature.Moderate;
                case Biome.Savannah:
                    return Temperature.Hot;
                case Biome.Whimsy:
                    return Temperature.Warm;
                case Biome.Barren:
                    return Temperature.Moderate;
                case Biome.Toxic:
                    return Temperature.Warm;
                default:
                    return Temperature.Moderate;
            }
        }

        public static Humidity CheckHumidity(int biome)
        {
            switch ((Biome)biome)
            {
                case Biome.Desert:
                    return Humidity.Dry;
                case Biome.Plains:
                    return Humidity.Moderate;
                case Biome.Shroom:
                    return Humidity.Moist;
                case Biome.Swamp:
                    return Humidity.Wet;
                case Biome.Tundra:
                    return Humidity.Dry;
                case Biome.Forest:
                    return Humidity.Moderate;
                case Biome.River:
                    return Humidity.Wet;
                case Biome.Savannah:
                    return Humidity.Dry;
                case Biome.Whimsy:
                    return Humidity.Moderate;
                case Biome.Barren:
                    return Humidity.Dry;
                case Biome.Toxic:
                    return Humidity.Dry;
                default:
                    return Humidity.Moderate;
            }
        }

        public static List<Direction> DungeonCheckDirection(DungeonChunkState chunk)
        {
            List<Direction> possibleDirections = new List<Direction>();

            switch (chunk)
            {
                case DungeonChunkState.cornerNE:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    break;
                case DungeonChunkState.cornerNW:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.cornerSE:
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    break;
                case DungeonChunkState.cornerSW:
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.cross:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.startN:
                    possibleDirections.Add(Direction.south);
                    break;
                case DungeonChunkState.endN:
                    possibleDirections.Add(Direction.south);
                    break;
                case DungeonChunkState.startE:
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.endE:
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.startS:
                    possibleDirections.Add(Direction.north);
                    break;
                case DungeonChunkState.endS:
                    possibleDirections.Add(Direction.north);
                    break;
                case DungeonChunkState.startW:
                    possibleDirections.Add(Direction.east);
                    break;
                case DungeonChunkState.endW:
                    possibleDirections.Add(Direction.east);
                    break;
                case DungeonChunkState.straightE:
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.straightN:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.south);
                    break;
                case DungeonChunkState.tboneENS:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    break;
                case DungeonChunkState.tboneNEW:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.tboneSEW:
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.tboneWNS:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.roomA:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.roomB:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.roomC:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                case DungeonChunkState.roomD:
                    possibleDirections.Add(Direction.north);
                    possibleDirections.Add(Direction.east);
                    possibleDirections.Add(Direction.south);
                    possibleDirections.Add(Direction.west);
                    break;
                default:
                    break;
            }

            return possibleDirections;
        }

        public static List<DungeonChunkState> DungeonGetChunk(Direction direction)
        {
            List<DungeonChunkState> possibleDungeonChunks = new List<DungeonChunkState>();

            if (WorldSpace.Instance().DungeonTravelled > WorldSpace.Instance().DungeonLength)
            {
                switch (direction)
                {
                    case Direction.east:
                        possibleDungeonChunks.Add(DungeonChunkState.endE);
                        break;
                    case Direction.north:
                        possibleDungeonChunks.Add(DungeonChunkState.endN);
                        break;
                    case Direction.south:
                        possibleDungeonChunks.Add(DungeonChunkState.endS);
                        break;
                    case Direction.west:
                        possibleDungeonChunks.Add(DungeonChunkState.endW);
                        break;
                }
            }

            if (WorldSpace.Instance().DungeonTravelled % 2 == 0)
            {
                switch (direction)
                {
                    case Direction.north:
                        possibleDungeonChunks.Add(DungeonChunkState.cornerSE);
                        possibleDungeonChunks.Add(DungeonChunkState.cornerSW);
                        possibleDungeonChunks.Add(DungeonChunkState.straightN);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneENS);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneSEW);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneWNS);
                        break;
                    case Direction.east:
                        possibleDungeonChunks.Add(DungeonChunkState.cornerNW);
                        possibleDungeonChunks.Add(DungeonChunkState.cornerSW);
                        possibleDungeonChunks.Add(DungeonChunkState.straightE);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneNEW);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneSEW);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneWNS);
                        break;
                    case Direction.south:
                        possibleDungeonChunks.Add(DungeonChunkState.cornerNE);
                        possibleDungeonChunks.Add(DungeonChunkState.cornerNW);
                        possibleDungeonChunks.Add(DungeonChunkState.straightN);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneENS);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneNEW);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneWNS);
                        break;
                    case Direction.west:
                        possibleDungeonChunks.Add(DungeonChunkState.cornerNE);
                        possibleDungeonChunks.Add(DungeonChunkState.cornerSE);

                        possibleDungeonChunks.Add(DungeonChunkState.straightE);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneENS);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneNEW);
                        possibleDungeonChunks.Add(DungeonChunkState.tboneSEW);
                        break;
                    default:
                        break;
                }
            }
            if (WorldSpace.Instance().DungeonTravelled % 5 == 0)
            {
                possibleDungeonChunks.Add(DungeonChunkState.cross);
                possibleDungeonChunks.Add(DungeonChunkState.roomB);
                possibleDungeonChunks.Add(DungeonChunkState.roomC);
                possibleDungeonChunks.Add(DungeonChunkState.roomD);
            }
            possibleDungeonChunks.Add(DungeonChunkState.roomA);

            return possibleDungeonChunks;
        }

        public static Vector2 DungeonGetChunkPosition(Direction direction)
        {
            switch (direction)
            {
                case Direction.north:
                    return new Vector2(0, -1);
                case Direction.east:
                    return new Vector2(1, 0);
                case Direction.south:
                    return new Vector2(0, 1);
                case Direction.west:
                    return new Vector2(-1, 0);
                default:
                    return Vector2.Zero;
            }
        }

        public Vector2 GetBiomeGoal(Biome biome)
        {
            switch (biome)
            {
                case Biome.Desert:
                    return BIOME_VECTOR_DESERT;
                case Biome.Plains:
                    return BIOME_VECTOR_PLAINS;
                case Biome.Shroom:
                    return BIOME_VECTOR_SHROOM;
                case Biome.Swamp:
                    return BIOME_VECTOR_SWAMP;
                case Biome.Tundra:
                    return BIOME_VECTOR_TUNDRA;
                case Biome.Forest:
                    return BIOME_VECTOR_FOREST;
                case Biome.River:
                    return BIOME_VECTOR_RIVER;
                case Biome.Savannah:
                    return BIOME_VECTOR_SAVANNAH;
                case Biome.Whimsy:
                    return BIOME_VECTOR_WHIMSY;
                case Biome.Barren:
                    return BIOME_VECTOR_BARREN;
                case Biome.Toxic:
                    return BIOME_VECTOR_TOXIC;
                default:
                    return BIOME_VECTOR_PLAINS;
            }
        }

        public Biome CheckBiome(Vector2 position)
        {
            float distance = 2;
            Biome biome = Biome.Plains;
            for (int i = 0; i < BIOMES; i++)
            {
                Vector2 delta = GetBiomeGoal((Biome)i) - position;
                if (delta.Length() < distance)
                {
                    distance = delta.Length();
                    biome = (Biome)i;
                }
            }
            return biome;
        }

        public void CreateChunk(Vector2 position)
        {
            WorldChunk chunk = new WorldChunk(CheckBiome(BIOME_VECTOR_COLOUR), position);
            chunks.Add(chunk);
        }

        public void CreateChunk(Vector2 position, DungeonChunkState state)
        {
            WorldChunk chunk = new WorldChunk(CheckBiome(BIOME_VECTOR_COLOUR), position, state);
            chunks.Add(chunk);
        }

        public void DisposeChunk(WorldChunk chunk)
        {
            deadChunk = chunk;
        }

        public static float PixelsToMetres(float value)
        {
            return value / TILESIZE / 2;
        }

        public static float MetresToPixels(float value)
        {
            return value * TILESIZE * 2;
        }

        public void CheckPositionWithinBounds(StaticObject obj)
        {
            if (chunks.Count < 1)
                return;
            
            Vector2 objChunkPosition = obj.Position / CHUNKSIZE;
            objChunkPosition = new Vector2(objChunkPosition.X, objChunkPosition.Y);

            int count = 0;
            WorldChunk nearest = chunks[0];

            foreach (WorldChunk chunk in chunks)
            {
                if ((chunk.Position - objChunkPosition).Length() < (nearest.Position - objChunkPosition).Length())
                    nearest = chunk;
                if (new Vector2 ((float)Math.Round((double)objChunkPosition.X), (float)Math.Round((double)objChunkPosition.Y)) != chunk.Position)
                {
                    count++;
                }
            }

            // If every chunk incremented the counter then every chunk found the obj to be outside them, and the obj should be moved back within the nearest chunk
            if (count == chunks.Count)
                obj.Position = nearest.Position * CHUNKSIZE;
        }

        public void SwitchStates()
        {
            if (State == WorldState.world)
            {
                DungeonLength = Game1.random.Next(DUNGEON_LENGTHMIN, DUNGEON_LENGTHMAX);

                State = WorldState.dungeon;
            }
            else
            {
                State = WorldState.world;
            }

            StateSwitchTimer = 0;
            StateSwitch = true;
            DungeonTravelled = 0;
        }

        public static void LoadContent(ContentManager Content)
        {
            BIOME_COLOURMAP = Content.Load<Texture2D>("Colourmaps/biome");

            Rectangle sample = new Rectangle(0, 0, 1, 1);
            Color[] sampleColour = new Color[1];
            List<Vector2> exclusions = new List<Vector2>();
            List<Vector2> walls = new List<Vector2>();

            Texture2D render;

            for (int i = 0; i < Enum.GetNames(typeof(DungeonChunkState)).Length; i++)
            {
                render = Content.Load<Texture2D>("Scenes/Maps/" + ((DungeonChunkState)i).ToString());
                for (int x = 0; x < render.Width; x++)
                {
                    for (int y = 0; y < render.Height; y++)
                    {
                        sample.X = x;
                        sample.Y = y;
                        render.GetData<Color>(0, sample, sampleColour, 0, 1);
                        if (sampleColour[0].R == 0 && sampleColour[0].G == 0 && sampleColour[0].B == 0)
                        {
                            exclusions.Add(new Vector2(sample.X, sample.Y));
                        }
                        else if (sampleColour[0].R == 255 && sampleColour[0].G == 0 && sampleColour[0].B == 0)
                        {
                            walls.Add(new Vector2(sample.X, sample.Y));
                        }
                    }
                }
                dungeonChunkSceneExclusions.Add((DungeonChunkState)i, exclusions);
                dungeonChunkSceneWalls.Add((DungeonChunkState)i, walls);
                exclusions = new List<Vector2>();
                walls = new List<Vector2>();
            }
        }
    }
}
