using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Object
{
    abstract class Entity : DynamicObject
    {
        Dictionary<string, uint> attr = new Dictionary<string, uint>();
        protected List<Projectile> projectiles = new List<Projectile>();

        protected int Health { get; set; }
        protected int HealthMax { get; set; }
        protected int HealthRegen { get; set; }
        protected bool isAlive { get; set; }
        protected int Damage { get; set; }
        protected int Level { get; set; }
        
        public Entity(Texture2D texture)
            : base(texture)
        {
            this.isAlive = true;
            attr.Add("health", 0);
            attr.Add("healthMax", 1);
            attr.Add("healthRegen", 2);
            attr.Add("damage", 3);
            attr.Add("level", 4);
        }
        public void ChangeAttr(string attr, int change)
        {
            switch (this.attr[attr])
            {
                case 0:
                    if (Health + change < 0)
                    {
                        this.isAlive = false;
                    }
                    else
                    {
                        Health += change;
                    }
                    break;
                case 1:
                    HealthMax += change;
                    break;
                case 2:
                    HealthRegen += change;
                    break;
                case 3:
                    Damage += change;
                    break;
                case 4:
                    Level += change;
                    if (Level < 1)
                        Level = 1;
                    break;
                default:
                    break;
            }
        }
        public abstract void Attack(List<Entity> targets);
    }
}
