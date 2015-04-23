using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ActionGame
{
    class ObjectManager
    {
        private const int COLLISIONZONE = WorldSpace.TILESIZE * 2;
        private const int COLLISIONZONES = 25;
        private const int COLLISIONZONES_WIDTH = 5;
        private const int COLLISIONZONES_HALFWIDTH = 2;
        /// <summary>
        /// Returns the total area that supports collisions, as a scalar value that represents the width and height of a square
        /// </summary>
        public const int COLLISIONBOUNDS = COLLISIONZONE * COLLISIONZONES_WIDTH;

        private List<StaticObject> objects;
        private List<StaticObject> deadObjects;
        public List<Player> players;
        private Rectangle[] collisionZones;
        private List<List<StaticObject>> collisionLists;

        private static ObjectManager instance { get; set; }
        private ObjectManager()
        {
            objects = new List<StaticObject>();
            deadObjects = new List<StaticObject>();
            players = new List<Player>();

            collisionZones = new Rectangle[COLLISIONZONES];
            collisionLists = new List<List<StaticObject>>();

            int index = 0;
            for (int x = 0; x < COLLISIONZONES_WIDTH; x++)
            {
                for (int y = 0; y < COLLISIONZONES_WIDTH; y++)
                {
                    collisionZones[index] = new Rectangle((x - COLLISIONZONES_HALFWIDTH) * COLLISIONZONE, (y - COLLISIONZONES_HALFWIDTH) * COLLISIONZONE, COLLISIONZONE, COLLISIONZONE);
                    index++;
                }
            }
        }

        public static ObjectManager Instance()
        {
            if (instance == null)
                instance = new ObjectManager();
            return instance;
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
            objects.Add(player);
        }

        public StaticObject AddObject(StaticObject create)
        {
            objects.Add(create);

            return create;
        }

        public void RemoveObject(StaticObject remove)
        {
            deadObjects.Add(remove);
        }

        public void Update(GameTime gameTime)
        {
            int index = 0;

            for (int x = 0; x < COLLISIONZONES_WIDTH; x++)
            {
                for (int y = 0; y < COLLISIONZONES_WIDTH; y++)
                {
                    collisionZones[index].X = (int)((x - COLLISIONZONES_HALFWIDTH) * COLLISIONZONE + Camera.Instance().Position.X);
                    collisionZones[index].Y = (int)((y - COLLISIONZONES_HALFWIDTH) * COLLISIONZONE + Camera.Instance().Position.Y);
                    index++;
                }
            }

            index = 0;
            for (int x = 0; x < COLLISIONZONES_WIDTH; x++)
            {
                for (int y = 0; y < COLLISIONZONES_WIDTH; y++)
                {
                    collisionLists.Add(new List<StaticObject>());
                    index++;
                }
            }

            List<StaticObject> collideChecks = new List<StaticObject>();
            collideChecks.AddRange(objects);
            collideChecks.AddRange(ProjectileManager.Instance().projectiles);

            foreach (StaticObject obj in collideChecks)
            {
                obj.ObjectManagerCollisionBoxIndexs = new List<int>();
                index = 0;

                for (int x = 0; x < COLLISIONZONES_WIDTH; x++)
                {
                    for (int y = 0; y < COLLISIONZONES_WIDTH; y++)
                    {
                        if (obj.isCollidable)
                        {
                            Rectangle collisionBounds = new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.CollisionRadius * 2, (int)obj.CollisionRadius * 2);
                            if (collisionZones[index].Intersects(collisionBounds))
                            {
                                obj.ObjectManagerCollisionBoxIndexs.Add(index);
                                collisionLists[index].Add(obj);
                            }
                        }
                        index++;
                    }
                }
            }

            foreach (StaticObject obj in objects)
            {
                if (obj is DynamicObject && !(obj is Entity))
                    ((DynamicObject)obj).Update(gameTime);
                if (obj is Player)
                    ((Player)obj).Update(gameTime);
            }

            ProjectileManager.Instance().Update(gameTime);

            for (int i = 0; i < deadObjects.Count; i++)
            {
                for (int j = 0; j < objects.Count; j++)
                {
                    if (objects[j].ID == deadObjects[i].ID)
                    {
                        objects.RemoveAt(j);
                        j--;
                    }
                }
            }


            collisionLists = new List<List<StaticObject>>();
            deadObjects = new List<StaticObject>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (StaticObject obj in objects)
            {
                if (obj is AnimatedObject)
                    ((AnimatedObject)obj).Draw(spriteBatch);
                else
                    obj.Draw(spriteBatch);
            }
        }

        public List<StaticObject> CheckCollisionReady(DynamicObject obj)
        {
            // Check the position of the object against a preconstructed fast pass rectangle system
            List<StaticObject> collidables = new List<StaticObject>();

            // Broad pass logic goes here, have fun
            foreach (int index in obj.ObjectManagerCollisionBoxIndexs)
            {
                if (collisionLists.Count > index)
                {
                    collidables.AddRange(collisionLists[index]);
                }
            }

            return collidables;
        }
    }
}
