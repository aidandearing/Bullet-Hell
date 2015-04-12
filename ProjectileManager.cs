using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ActionGame
{
    class ProjectileManager
    {
        private static ProjectileManager instance { get; set; }
        private List<Projectile> projectiles;
        private const int MAXPROJECTILES = 250;
        public static int CurrentProjectiles { get; private set; }

        private List<Projectile> deadprojectiles;

        private ProjectileManager()
        {
            projectiles = new List<Projectile>();
        }

        public static ProjectileManager Instance()
        {
            if (instance == null)
                instance = new ProjectileManager();
            return instance;
        }

        public void CreateProj(string key, float range, float speed, float angle, bool isOriented, float lifetime, Vector2 scale, Vector2 scaleend, bool scalecurved, float opacity, float opacityend, bool opacitycurved, Entity parent, List<StatusEffect> effects)
        {
            if (projectiles.Count < MAXPROJECTILES)
            {
                Projectile projectile = new Projectile(key, range, speed, angle, isOriented, lifetime, scale, scaleend, scalecurved, opacity, opacityend, opacitycurved, parent);
                projectile.effectManager.AddEffect(effects);
                projectiles.Add(projectile);
            }
        }

        public void CreateProj(Projectile projectile)
        {
            if (projectiles.Count < MAXPROJECTILES)
            {
                projectiles.Add(projectile);
            }
        }

        public void RemoveProj(Projectile projectile)
        {
            deadprojectiles.Add(projectile);
        }

        public void Update(GameTime gameTime)
        {
            deadprojectiles = new List<Projectile>();

            CurrentProjectiles = projectiles.Count();

            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }

            for (int i = 0; i < deadprojectiles.Count; i++)
            {
                for (int j = 0; j < projectiles.Count; j++)
                {
                    if (projectiles[j].ID == deadprojectiles[i].ID)
                    {
                        projectiles.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
